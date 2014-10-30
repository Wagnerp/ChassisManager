// Copyright © Microsoft Open Technologies, Inc.
// All Rights Reserved
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at 
// http://www.apache.org/licenses/LICENSE-2.0 

// THIS CODE IS PROVIDED ON AN *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR
// CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 
// See the Apache 2 License for the specific language governing permissions and limitations under the License. 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.CompilerServices;
using Microsoft.GFS.WCS.Contracts;

namespace ChassisValidation
{
    public class CmRestTest
    {
        private readonly AcSocketPowerCommands acSocketPowerCmnds;
        private readonly NextBootCommands nextBootCmnds;
        private readonly PowerCommands powerCmnds;
        private readonly Dictionary<int, IChassisManager> restChannelContexts = new Dictionary<int, IChassisManager>();

        private readonly AssetManagementCommands assetManagementCommands;
        private readonly BladeCommands bladeCommands;
        private readonly ChassisCommands chassisCommands;
        private readonly LogCommands logCommands;
        private readonly SerialConsoleCommands serialCommands;
        private readonly UserManagementCommands usrMgmntCmnds;
        private IChassisManager channel;

        /// <summary>
        ///     Initializes a new instance of CmRestTest.
        /// </summary>
        /// <param name="cmUrl">The URL of the chassis manager, including port information.</param>
        /// <param name="username">The username used to send web requests.</param>
        /// <param name="password">The password corresponding to the username.</param>
        /// <param name="loggers">Loggers for the test. If it is null, default loggers will be used.</param>
        public CmRestTest(string cmUrl, string username, string password, string testDomainUser, string testDomainName,
            IEnumerable<ILogger> loggers = null)
        {
            if (string.IsNullOrWhiteSpace(cmUrl))
            {
                throw new ArgumentNullException("cmUrl");
            }
            var uri = new Uri(cmUrl);
            NetworkCredential cre = !string.IsNullOrWhiteSpace(username)
                                    ? new NetworkCredential(username, password)
                                    : null;

            var testEnvironmentSetting = new Dictionary<string, string>();
            testEnvironmentSetting.Add("CMURL", cmUrl);
            testEnvironmentSetting.Add("AdminUserName", username);
            testEnvironmentSetting.Add("DefaultPassword", password);
            testEnvironmentSetting.Add("LabDomainTestUser", testDomainUser);
            testEnvironmentSetting.Add("LabDomainName", testDomainName);

            // Logging still occur on the Main Channel
            this.Initialize(uri, cre, loggers);

            // Test channel collection
            this.InitializeRestChannelCollection(uri, username, password);

            if (this.channel != null)
            {
                this.bladeCommands = new BladeCommands(this.channel);
                this.serialCommands = new SerialConsoleCommands(this.channel);
            }

            if (this.channel != null && this.restChannelContexts != null)
            {
                this.chassisCommands = new ChassisCommands(this.channel, this.restChannelContexts);
                this.usrMgmntCmnds = new UserManagementCommands(this.channel, this.restChannelContexts, testEnvironmentSetting);
                this.powerCmnds = new PowerCommands(this.channel, this.restChannelContexts, testEnvironmentSetting);
                this.acSocketPowerCmnds = new AcSocketPowerCommands(this.channel, this.restChannelContexts);
                this.nextBootCmnds = new NextBootCommands(this.channel, this.restChannelContexts);
                this.bladeCommands = new BladeCommands(this.channel, this.restChannelContexts);
                this.assetManagementCommands = new AssetManagementCommands(this.channel, this.restChannelContexts, testEnvironmentSetting);
                this.logCommands = new LogCommands(this.channel, this.restChannelContexts, testEnvironmentSetting);
            }
            else
            {
                throw new Exception("Channel not created upon Initialization");
            }
        }

        /// <summary>
        ///     Initializes a new instance of CmRestTest.
        /// </summary>
        /// <param name="cmUrl">The URL of the chassis manager, including port information.</param>
        /// <param name="credential">The client credential used to send web requests.</param>
        /// <param name="loggers">Loggers for the test. If it is null, default loggers will be used.</param>
        public CmRestTest(Uri cmUrl, NetworkCredential credential, IEnumerable<ILogger> loggers = null)
        {
            if (cmUrl == null)
            {
                throw new ArgumentNullException("cmUrl");
            }
            this.Initialize(cmUrl, credential, loggers);
        }

        /// <summary>
        ///     Basic functional validation test for NextBoot Command Basic Validation Test.
        ///     Validate Next Boot command for Chassis Manager.
        /// </summary>
        /// <returns></returns>
        public bool NextBootCommandBasicFunctionalTest()
        {
            if (this.nextBootCmnds != null)
            {
                return this.nextBootCmnds.NextBootCommandBasicValidationTest();
            }
            throw new NullReferenceException("CmRestTest.NextBootCmnds");
        }

        /// <summary>
        ///     Test Command: SetNextBoot, GetNextBoot. The test case verifies:
        ///     The command returns a success completion code on server blades;
        ///     The command works with all blade types (except unknown type);
        ///     When set to non-persistent, the blade will lose its next boot value after restart.
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        public bool SetGetNextBootTest()
        {
            if (this.nextBootCmnds != null)
            {
                return this.nextBootCmnds.SetGetNextBootTest();
            }
            throw new NullReferenceException("CmRestTest.bladeCommands");
        }

        /// <summary>
        /// Basic functional validation test for ACSocket Power Command.
        /// </summary>
        /// <returns></returns>
        public bool ACPowerSocketCommandBasicFunctionalTest()
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        {
            if (this.powerCmnds != null)
            {
                return this.acSocketPowerCmnds.AcSocketPowerCommandBasicValidationTest();
            }
            throw new NullReferenceException("CmRestTest.ACSocketPowerCmnds");
        }

        /// <summary>
        /// Test Command: GetChassisManagerAssetInfo 
        /// The test case verifies:
        /// The command returns a success completion code;
        /// Correct FRU Information is displayed
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        public bool GetChassisManagerAssetInfoTest()
        {
            if (this.assetManagementCommands != null)
            {
                return this.assetManagementCommands.GetChassisManagerAssetInfoTest();
            }
            else
            {
                throw new NullReferenceException("CmRestTest.assetManagementCommands");
            }
        }

        /// <summary>
        /// Test Command: GetPdbAssetInfo 
        /// The test case verifies:
        /// The command returns a success completion code;
        /// Correct FRU Information is displayed
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        public bool GetPdbAssetInfoTest()
        {
            if (this.assetManagementCommands != null)
            {
                return this.assetManagementCommands.GetPdbAssetInfoTest();
            }
            else
            {
                throw new NullReferenceException("CmRestTest.assetManagementCommands");
            }
        }

        /// Test Command: GetBladeAssetInfo 
        /// The test case verifies:
        /// The command returns a success completion code;
        /// Correct FRU Information is displayed
        ///     Basic functional validation test for Power Command.
        /// </summary>
        public bool GetBladeAssetInfoTest()
        {
            if (this.assetManagementCommands != null)
            {
                return this.assetManagementCommands.GetBladeAssetInfoTest();
            }
            else
            {
                throw new NullReferenceException("CmRestTest.assetManagementCommands");
            }
        }

        /// <summary>
        /// Basic functional validation test for Power Command.
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        public bool PowerCommandBasicFunctionalTest()
        {
            if (this.powerCmnds != null)
            {
                return this.powerCmnds.PowerCommandBasicValidationTest();
            }
            throw new NullReferenceException("CmRestTest.PowerCmnds");       
        }

        /// <summary>
        ///     Test Commands: GetPowerState, SetPowerOn,SetPowerOff. The test case verifies:
        ///     The command returns completion code success;
        ///     The command returns correct power state when power is on;
        ///     The command returns correct power state when power is off.
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        public bool SetGetPowerStateTest()
        {
            if (this.powerCmnds != null)
            {
                return this.powerCmnds.SetGetPowerStateTest();
            }
            throw new NullReferenceException("CmRestTest.bladeCommands");
        }

        /// <summary>
        ///     Test Command: GetAllPowerState. The test case verifies:
        ///     The command returns completion code success;
        ///     The command returns correct power state when power is on;
        ///     The command returns correct power state when power is off.
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>        
        public bool SetGetAllPowerStateTest()
        {
            if (this.powerCmnds != null)
            {
                return this.powerCmnds.SetGetAllPowerStateTest();
            }
            throw new NullReferenceException("CmRestTest.bladeCommands");
        }

        /// <summary>
        /// Basic functional validation test for User management.
        /// </summary>
        /// <returns></returns>
        public bool UserManagementBasicFunctionalTest()
        {
            if (this.usrMgmntCmnds != null)
            {
                return this.usrMgmntCmnds.UserMangementBasicValidationTest();
            }
            throw new NullReferenceException("CmRestTest.usrMgmntCmnds");
        }

        /// <summary>
        /// Test Command: SetChassisManagerAssetInfo 
        /// The test case verifies:
        /// The command returns a success completion code;
        /// Correct FRU Information is changed 
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        public bool SetChassisManagerAssetInfoTest()
        {
            if (this.assetManagementCommands != null)
            {
                return this.assetManagementCommands.SetChassisManagerAssetInfoTest();
            }
            else
            {
                throw new NullReferenceException("CmRestTest.assetManagementCommands");
            }
        }

        /// <summary>
        /// Test Command: SetPdbAssetInfo 
        /// The test case verifies:
        /// The command returns a success completion code;
        /// Correct FRU Information is changed 
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        public bool SetPdbAssetInfoTest()
        {
            if (this.assetManagementCommands != null)
            {
                return this.assetManagementCommands.SetPdbAssetInfoTest();
            }
            else
            {
                throw new NullReferenceException("CmRestTest.assetManagementCommands");
            }
        }

        /// <summary>
        /// Test Command: SetBladeAssetInfo 
        /// The test case verifies:
        /// The command returns a success completion code;
        /// Correct FRU Information is changed 
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        public bool SetBladeAssetInfoTest()
        {
            if (this.assetManagementCommands != null)
            {
                return this.assetManagementCommands.SetBladeAssetInfoTest();
            }
            else
            {
                throw new NullReferenceException("CmRestTest.assetManagementCommands");
            }
        }

        /// <summary>
        ///     Test Command: GetAllBladesInfo. The test case verifies:
        ///     The command completion code is a success;
        ///     The command returns all blades info in the chassis (server, jbod and Empty);
        ///     The command returns failure for power-off blades, and returns success for
        ///     blade-off blades.
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        public bool GetAllBladesInfoTest()
        {
            if (this.bladeCommands != null)
            {
                return this.bladeCommands.GetAllBladesInfoTest();
            }
            throw new NullReferenceException("CmRestTest.bladeCommands");
        }

        /// <summary>
        ///     Test Command: GetBladeInfo. The test case verifies:
        ///     The command completion code is a success for server and JBOD;
        ///     The command returns TimeOut for Empty Slots);
        ///     The command returns Devicepowered off for power-off blades, and returns success for
        ///     blade-off blades.
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        public bool GetBladeInfoTest()
        {
            if (this.bladeCommands != null)
            {
                return this.bladeCommands.GetBladeInfoTest();
            }
            throw new NullReferenceException("CmRestTest.bladeCommands");
        }

        /// <summary>
        ///     Test Command: GetBladeHealth and it takes bladeId all the parameters as true. The test case verifies:
        ///     The command returns completion code success;
        ///     The blade shell is in healthy state;
        ///     The command returns full health information for server ;
        ///     The command returns full health information for jbod ;
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        public bool GetBladeHealthTest()
        {
            if (this.bladeCommands != null)
            {
                return this.bladeCommands.GetBladeHealthTest();
            }
            throw new NullReferenceException("CmRestTest.bladeCommands");
        }

        /// <summary>
        ///     Test Command: GetBladeState; SetBladeOn; SetBladeOff. The test case verifies:
        ///     The command returns completion code success on server blades;
        ///     The command returns CommandNotValidForBlade on JBOD blades;
        ///     The command returns Failure on empty blades;
        ///     The command returns correct blade state when a blade is on;
        ///     The command returns correct blade state when a blade is off.
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>  
        public bool SetGetBladeStateTest()
        {
            if (this.bladeCommands != null)
            {
                return this.bladeCommands.SetGetBladeStateTest();
            }
            throw new NullReferenceException("CmRestTest.bladeCommands");
        }

        /// <summary>
        ///     Test Command: GetAllBladesState. The test case verifies:
        ///     The command returns completion code success on server blades;
        ///     The command returns CommandNotValidForBlade on JBOD blades;
        ///     The command returns correct blade state when a blade is on;
        ///     The command returns correct blade state when a blade is off.
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>        
        public bool SetGetAllBladesStateTest()
        {
            if (this.bladeCommands != null)
            {
                return this.bladeCommands.SetGetAllBladesStateTest();
            }
            throw new NullReferenceException("CmRestTest.bladeCommands");
        }

        /// <summary>
        ///     Test Command: SetAllBladesDefaultPowerStateOff, SetAllBladesDefaultPowerStateOn,
        ///     and GetAllBladesDefaultPowerState. The test case verifies:
        ///     The commands return completion code success;
        ///     When default power state is set to off, all blade states stay off when power is back on;
        ///     When default power state is set to on, all blade states are on when power is back on;
        ///     The default power state value stays unchanged even after the blade is restarted.
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        public bool SetGetAllBladesDefaultPowerStateTest()
        {
            if (this.bladeCommands != null)
            {
                return this.bladeCommands.SetGetAllBladesDefaultPowerStateTest();
            }
            throw new NullReferenceException("CmRestTest.bladeCommands");
        }

        /// <summary>
        ///     Test Command: SetBladeDefaultPowerStateOff, SetBladeDefaultPowerStateOn,
        ///     and GetBladeDefaultPowerState. The test case verifies:
        ///     The commands return completion code success;
        ///     When default power state is set to off, blade state stay off when power is back on;
        ///     When default power state is set to on, blade state stay on when power is back on;
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        public bool SetGetBladeDefaultPowerStateTest()
        {
            if (this.bladeCommands != null)
            {
                return this.bladeCommands.SetGetBladeDefaultPowerStateTest();
            }
            throw new NullReferenceException("CmRestTest.bladeCommands");
        }

        /// <summary>
        ///     This method verifies:Baisc functional validation test for All set poweractions by all users.
        ///     Completion Code success for WcsAdmin user. Not Authorized error for WcsUser and WcsOperator
        /// </summary>
        /// <returns></returns>
        public bool SetPowerActionsByAllUsersTest()
        {
            if (this.bladeCommands != null)
            {
                return this.bladeCommands.SetPowerActionsByAllUsersTest();
            }
            throw new NullReferenceException("CmRestTest.bladeCommands");
        }

        /// <summary>
        ///     This method verifies: Baisc functional validation test for All Get poweractions by all users.
        ///     Success for all 3 users : WcsAdmin,WcsOperator and WcsUser
        /// </summary>
        /// <returns></returns>
        public bool GetPowerActionsByAllUsersTest()
        {
            if (this.bladeCommands != null)
            {
                return this.bladeCommands.GetPowerActionsByAllUsersTest();
            }
            throw new NullReferenceException("CmRestTest.bladeCommands");
        }

        /// <summary>
        ///     Test Command: SetAllBladesActivePowerCycle. The test case verifies:
        ///     The command returns completion code success;
        ///     The blades can be successfully power cycled and stay off during offTime;
        ///     The blades will be back on after offTime.
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        public bool SetAllBladesActivePowerCycleTest()
        {
            if (this.bladeCommands != null)
            {
                return this.bladeCommands.SetAllBladesActivePowerCycleTest();
            }
            throw new NullReferenceException("CmRestTest.bladeCommands");
        }

        public bool SetBladeActivePowerCycleTest()
        {
            if (this.bladeCommands != null)
            {
                return this.bladeCommands.SetBladeActivePowerCycleTest();
            }
            else
            {
                throw new NullReferenceException("CmRestTest.bladeCommands");
            }
        }

        /// <summary>
        ///     Test Command: GetChassisHealth. The test case verifies:
        ///     The command returns a success completion code;
        ///     The command returns all information for blades, fans, and PSUs;
        ///     All blades, fans and PSUs are in corrent numbers and in healthy state.
        /// </summary>
        /// <returns
        public bool GetChassisHealthTest()
        {
            if (this.chassisCommands != null)
            {
                return this.chassisCommands.GetChassisHealthTest();
            }
            throw new NullReferenceException("CmRestTest.chassisCommands");
        }

        /// <summary>
        ///     Test Command: GetChassisInfo.
        /// </summary>
        /// <returns
        public bool GetChassisInfoTest()
        {
            if (this.chassisCommands != null)
            {
                return this.chassisCommands.GetChassisInfoTest();
            }
            throw new NullReferenceException("CmRestTest.chassisCommands");
        }

        /// <summary>
        ///     Test Command: GetChassisInfo.
        /// </summary>
        /// <returns
        public bool GetChassisInfoByAllUsersTest()
        {
            if (this.chassisCommands != null)
            {
                return this.chassisCommands.GetChassisInfoByAllUserTest();
            }
            throw new NullReferenceException("CmRestTest.chassisCommands");
        }

        /// <summary>
        ///     Test Command: ReadBladeLog, ClearBladeLog. The test case verifies:
        ///     The command returns completion code success on server blades;
        ///     ReadBladeLog succeeds even after all logs are cleared.
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        public bool ReadClearBladeLogTest()
        {
            if (this.logCommands != null)
            {
                return this.logCommands.ReadClearBladeLogTest();
            }
            throw new NullReferenceException("CmRestTest.logCommands");
        }

        /// <summary>
        ///     Test Command: ReadBladeLogWithTimestamp. The test case verifies:
        ///     The command returns completion code success;
        ///     The command succeeds even after all logs are cleared.
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        public bool ReadBladeLogWithTimestampTest()
        {
            if (this.logCommands != null)
            {
                return this.logCommands.ReadBladeLogWithTimestampTest();
            }
            throw new NullReferenceException("CmRestTest.logCommands");
        }

        /// <summary>
        /// Test Command: User Logs. 
        /// The test case verifies: Only WcsCmAdmin group users can execute command
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        public bool ReadChassisLogTest()
        {
            if (this.logCommands != null)
            {
                return this.logCommands.ReadChassisLogTest();
            }
            else
            {
                throw new NullReferenceException("CmRestTest.logCOmmands");
            }
        }

        public bool ClearChassisLogTest()
        {
            if (this.logCommands != null)
            {
                return this.logCommands.ClearChassisLogTest();
            }
            else
            {
                throw new NullReferenceException("CmRestTest.logCOmmands");
            }
        }

        public bool ReadChassisLogWithTimestampTest()
        {
            if (this.logCommands != null)
            {
                return this.logCommands.ReadChassisLogWithTimestampTest();
            }
            else
            {
                throw new NullReferenceException("CmRestTest.logCOmmands");
            }
        }

        /// <summary>
        /// Test: User Logs. 
        /// The test case verifies: User Log exists
        /// Commands are being registered in User Logs
        /// CM service initialized is logged in user logs
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        public bool UserLogsTest()
        {
            if (this.logCommands != null)
            {
                return this.logCommands.UserLogsTest();
            }
            else
            {
                throw new NullReferenceException("CmRestTest.logCommands");
            }
        }

        /// <summary>
        /// Test Command: StartBladeSerialSession, StopBladeSerialSession. 
        /// The test case verifies:
        /// The command returns a success completion code;
        /// The serial session can be successfully started on a server blade;
        /// The serial session can be successfully stopped provided the session token.
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        public bool StartStopBladeSerialSessionTest()
        {
            if (this.serialCommands != null)
            {
                return this.serialCommands.StartStopBladeSerialSessionTest();
            }
            throw new NullReferenceException("CmRestTest.serialCommands");
        }

        /// <summary>
        ///     Creates REST channel and registers loggers.
        /// </summary>
        private void InitializeRestChannelCollection(Uri cMUri, string userName, string userPassword)
        {
            var testUsersCollection = new Dictionary<int, string>();

            // Default Test CM users
            // This users are created as part of CM image
            testUsersCollection.Add((int)WCSSecurityRole.WcsCmAdmin, "WcsAdmin");
            testUsersCollection.Add((int)WCSSecurityRole.WcsCmOperator, "WcsOperator");
            testUsersCollection.Add((int)WCSSecurityRole.WcsCmUser, "WcsUser");
            testUsersCollection.Add(CmConstants.TestConnectionLocalUserId, "LocalTestUser");
            testUsersCollection.Add(CmConstants.TestConnectionDomainUserId, "WcsTestUser");
            // This is already created for WCS Lab domain

            foreach (var testUser in testUsersCollection)
            {
                try
                {
                    var factory = new CmRestChannelFactory<IChassisManager>(cMUri.ToString());
                    factory.Timeout = 1000 * CmConstants.RequestTimeoutSeconds;
                    factory.Credential = new NetworkCredential(testUser.Value, userPassword);

                    this.restChannelContexts.Add(testUser.Key, factory.CreateChannel());
                }
                catch
                {
                    // To Do Check of Channel collection is creating correctly                    
                }
            }
        }

        /// <summary>
        ///     Creates REST channel and registers loggers.
        /// </summary>
        private void Initialize(Uri cmUrl, NetworkCredential credential, IEnumerable<ILogger> loggers)
        {
            // create REST channel
            var factory = new CmRestChannelFactory<IChassisManager>(cmUrl.ToString());
            factory.Timeout = 1000 * CmConstants.RequestTimeoutSeconds;
            if (credential != null)
            {
                factory.Credential = credential;
            }
            this.channel = factory.CreateChannel();

            // if no loggers specifid, use default
            if (loggers == null)
            {
                loggers = new Collection<ILogger>
                {
                    new TxtFileLogger(TestLogName.Instance),
                    new ConsoleLogger()
                };
            }
            // register loggers
            foreach (ILogger log in loggers)
            {
                Log.Loggers.Add(log);
            }
        }
    }
}
