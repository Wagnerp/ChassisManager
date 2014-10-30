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

namespace ChassisValidation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.GFS.WCS.Contracts;

    internal class ChassisCommands : CommandBase
    {
        internal ChassisCommands(IChassisManager channel) : base(channel)
        {
        }

        internal ChassisCommands(IChassisManager channel, Dictionary<int, IChassisManager> testChannelContexts) : base(channel, testChannelContexts)
        {
        }

        /// <summary>
        /// Test Command: GetChassisHealth. The test case verifies:
        /// The command returns a success completion code;
        /// The command returns all information for blades, fans, and PSUs;
        /// All blades, fans and PSUs are in corrent numbers and in healthy state.
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        public bool GetChassisHealthTest()
        {
            CmTestLog.Start();

            bool testPassed = true;

            this.EmptyLocations = this.GetEmptyLocations();
            this.JbodLocations = this.GetJbodLocations();

            // Loop through different user types and get chassis health
            foreach (WCSSecurityRole roleId in Enum.GetValues(typeof(WCSSecurityRole)))
            {
                try
                {
                    CmTestLog.Info(string.Format("Get Chassis Health with user type {0} \n", Enum.GetName(typeof(WCSSecurityRole), roleId)));                           
                    testPassed = this.GetChassisHealth(testPassed, roleId);
                }
                catch (Exception ex)
                {
                    ChassisManagerTestHelper.IsTrue(false, ex.Message);
                    testPassed = false;
                }
            }

            return testPassed;
        }

        public bool GetChassisHealth(bool allPassed, WCSSecurityRole roleId)
        {
            try
            {
                this.TestChannelContext = this.ListTestChannelContexts[(int)roleId];
                if (this.TestChannelContext != null)
                {
                    // Test for chassishealth components are returned when all params are true : WorkItem(3373 & 2746)
                    CmTestLog.Info("Verifying chassis health information when all params are true");
                    ChassisHealthResponse chassisHealth = this.TestChannelContext.GetChassisHealth(true, true, true, true);
                    allPassed = this.VerifyChassisHealth(allPassed, chassisHealth);
                    CmTestLog.Info("!!!! Finished verification of Getchassishealth when all params are true !!!!\n");

                    // Test for chassishealth with no params : WorkItem(4776)
                    CmTestLog.Info("Verifying chassis health information when all params are false OR no Params");
                    chassisHealth = this.TestChannelContext.GetChassisHealth(false, false, false, false);
                    allPassed = this.VerifyChassisHealth(allPassed, chassisHealth);
                    CmTestLog.Info("!!!! Finished verification of Getchassishealth with no params !!!!\n");

                    // Test for chassishealth get only bladeshell information : WorkItem(3157)
                    CmTestLog.Info("Verifying chassis health information for only BladeHealth param is true");
                    chassisHealth = this.TestChannelContext.GetChassisHealth(true, false, false, false);
                    allPassed = this.VerifyOnlyChassisBladeHealth(allPassed, chassisHealth);
                    CmTestLog.Info("!!!! Finished verification of Getchassishealth for only BladeHealth !!!!\n");

                    // Test for chassishealth get only PSU's health information : WorkItem(3159)
                    CmTestLog.Info("Verifying chassis health information for only psuHealth param is true");
                    chassisHealth = this.TestChannelContext.GetChassisHealth(false, true, false, false);
                    allPassed = this.VerifyOnlyChassisPsuHealth(allPassed, chassisHealth);
                    CmTestLog.Info("!!!! Finished verification of Getchassishealth for only psuHealth !!!!\n");

                    // Test for chassishealth get only Fan's health information : WorkItem(3158)
                    CmTestLog.Info("Verifying chassis health information for only fanHealth param is true");
                    chassisHealth = this.TestChannelContext.GetChassisHealth(false, false, true, false);
                    allPassed = this.VerifyOnlyChassisFanHealth(allPassed, chassisHealth);
                    CmTestLog.Info("!!!! Finished verification of Getchassishealth for only fanHealth !!!!");
                }
            }
            catch (Exception ex)
            {
                ChassisManagerTestHelper.IsTrue(false, ex.Message);
                allPassed = false;
            }
            CmTestLog.End(allPassed);
            return allPassed;
        }

        /// <summary>
        /// Test GetChassisInfo for all possible parameters
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        public bool GetChassisInfoTest()
        {
            CmTestLog.Start();
            bool allPassed = true;

            this.EmptyLocations = this.GetEmptyLocations();
            this.JbodLocations = this.GetJbodLocations();

            try
            {
                ChassisInfoResponse chassisInfo = new ChassisInfoResponse();

                // Test GetChassisInfo, get all information
                CmTestLog.Info("Verifying chassis information when all params are true");
                chassisInfo = this.Channel.GetChassisInfo(true, true, true, true);
                allPassed = this.VerifyChassisInfo(allPassed, chassisInfo);
                CmTestLog.Info("Finished verification of GetchassisInfo when all information was returned blade, PSU, battery and chassis controller \n");

                // Test GetChassisInfo with no params 
                CmTestLog.Info("Verifying chassis information when all params are false OR no Params");
                chassisInfo = this.Channel.GetChassisInfo(false, false, false, false);
                allPassed = this.VerifyChassisInfo(allPassed, chassisInfo);
                CmTestLog.Info("Finished verification of GetchassisInfo with no params \n");

                // Test for GetChassisInfo with only blade info
                CmTestLog.Info("Verifying chassis information for only Blade, bladeInfo param is true");
                chassisInfo = this.Channel.GetChassisInfo(true, false, false, false);
                allPassed = this.VerifyOnlyChassisBladeInfo(allPassed, chassisInfo);
                CmTestLog.Info("Finished verification of GetchassisInfo for only Blade information \n");

                // Test for GetChassisInfo for only PSU information
                CmTestLog.Info("Verifying chassis information for only psuInfo param is true");
                chassisInfo = this.Channel.GetChassisInfo(false, true, false, false);
                allPassed = this.VerifyOnlyChassisPsuInfo(allPassed, chassisInfo);
                CmTestLog.Info("Finished verification of GetChassisInfo for only PSU information \n");

                // Test for GetChassisInfo for only Battery information
                CmTestLog.Info("Verifying chassis information for only batteryInfo param is true");
                chassisInfo = this.Channel.GetChassisInfo(false, false, false, true);
                allPassed = this.VerifyOnlyChassisBatteryInfo(allPassed, chassisInfo);
                CmTestLog.Info("Finished verification of GetChassisInfo for only battery information \n");

                // Test for GetChassisInfo for only chassis controller information
                CmTestLog.Info("Verifying chassis information for only chassisControllerInfo param is true");
                chassisInfo = this.Channel.GetChassisInfo(false, false, true, false);
                allPassed = this.VerifyOnlyChassisControllerInfo(allPassed, chassisInfo);
                CmTestLog.Info("Finished verification of GetChassisInfo for only chassis controller information \n");
            }
            catch (Exception ex)
            {
                ChassisManagerTestHelper.IsTrue(false, ex.Message);
                allPassed = false;
            }

            CmTestLog.End(allPassed);
            return allPassed;
        }

        /// <summary>
        /// Test GetChassisInfo for all possible parameters by different users
        /// </summary>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        public bool GetChassisInfoByAllUserTest()
        {
            CmTestLog.Start();

            bool testPassed = true;
            
            this.EmptyLocations = this.GetEmptyLocations();
            this.JbodLocations = this.GetJbodLocations();
            
            // Loop through different user types and get power Reading
            foreach (WCSSecurityRole roleId in Enum.GetValues(typeof(WCSSecurityRole)))
            { 
                try
                {
                    testPassed = this.GetChassisInfoByAllUsers(testPassed, roleId);
                }
                catch (Exception ex)
                {
                    ChassisManagerTestHelper.IsTrue(false, ex.Message);
                    testPassed = false;
                }
            }

            return testPassed;
        }

        private static bool VerifyPsuHealth(bool allPassed, ChassisHealthResponse chassisHealth)
        {
            CmTestLog.Info("Verifying PSU health state");
            allPassed &= ChassisManagerTestHelper.AreEqual(CmConstants.NumPsus, chassisHealth.psuInfoCollection.Count, "Verified the number of PSUs is correct");

            foreach (var psu in chassisHealth.psuInfoCollection)
            {
                allPassed &= ChassisManagerTestHelper.AreEqual(CompletionCode.Success, psu.completionCode, string.Format("PSU #{0} returns success", psu.id));
                allPassed &= ChassisManagerTestHelper.AreEqual(PowerState.ON, psu.state, string.Format("PSU #{0} is ON", psu.id));
            }
            return allPassed;
        }

        private static bool VerifyFanHealth(bool allPassed, ChassisHealthResponse chassisHealth)
        {
            CmTestLog.Info("Verifying Fan health state");
            allPassed &= ChassisManagerTestHelper.AreEqual(CmConstants.NumFans, chassisHealth.fanInfoCollection.Count, "Verified the number of fans");

            foreach (var fan in chassisHealth.fanInfoCollection)
            {
                allPassed &= ChassisManagerTestHelper.AreEqual(CompletionCode.Success, fan.completionCode, string.Format("Fan #{0} returns success", fan.fanId));
                allPassed &= ChassisManagerTestHelper.IsTrue(fan.isFanHealthy, string.Format("Fan #{0} is in good health", fan.fanId));
            }
            return allPassed;
        }

        /// <summary>
        /// Verify chassis PSU info
        /// </summary>
        /// <param name="allPassed">Flag indicating success/failure</param>
        /// <param name="chassisInfo">Chassis info response</param>
        /// <returns>returns success/failure</returns>
        private static bool VerifyChassisPsuInfo(bool allPassed, ChassisInfoResponse chassisInfo)
        {
            CmTestLog.Info("Verifying PSU info");
            allPassed &= ChassisManagerTestHelper.AreEqual(CmConstants.NumPsus, chassisInfo.psuCollections.Count, "Verified the number of PSUs is correct");

            foreach (var psu in chassisInfo.psuCollections)
            {
                allPassed &= ChassisManagerTestHelper.AreEqual(CompletionCode.Success, psu.completionCode, string.Format("PSU #{0} returns success", psu.id));
                allPassed &= ChassisManagerTestHelper.AreEqual(PowerState.ON, psu.state, string.Format("PSU #{0} is ON", psu.id));
            }
            return allPassed;
        }

        /// <summary>
        /// Verify chassis battery info
        /// </summary>
        /// <param name="allPassed">Flag indicating success/failure</param>
        /// <param name="chassisInfo">Chassis info response</param>
        /// <returns>returns success/failure</returns>
        private static bool VerifyChassisBatteryInfo(bool allPassed, ChassisInfoResponse chassisInfo)
        {
            CmTestLog.Info("Verifying battery info");
            allPassed &= ChassisManagerTestHelper.AreEqual(CmConstants.NumBattery, chassisInfo.batteryCollections.Count, "Verified the number of Batteries");

            foreach (var battery in chassisInfo.batteryCollections)
            {
                allPassed &= ChassisManagerTestHelper.AreEqual(CompletionCode.Success, battery.completionCode, string.Format("Battery #{0} returns success", battery.id));
            }
            return allPassed;
        }

        /// <summary>
        /// Verify chassis controller info
        /// </summary>
        /// <param name="allPassed">Flag indicating success/failure</param>
        /// <param name="chassisInfo">Chassis info response</param>
        /// <returns>returns success/failure</returns>
        private static bool VerifyChassisControllerInfo(bool allPassed, ChassisInfoResponse chassisInfo)
        {
            CmTestLog.Info("Verifying chassis controller info");

            allPassed &= ChassisManagerTestHelper.AreEqual(CompletionCode.Success, chassisInfo.chassisController.completionCode, "Chassis controller returns success");
            return allPassed;
        }

        private bool GetChassisInfoByAllUsers(bool testPassed, WCSSecurityRole roleId)
        {
            try
            {
                // Use the Domain User Channel
                this.TestChannelContext = this.ListTestChannelContexts[(int)roleId];
                if (this.TestChannelContext != null)
                {
                    ChassisInfoResponse chassisInfo = new ChassisInfoResponse();

                    // Test GetChassisInfo, get all information
                    CmTestLog.Info("Verifying chassis information when all params are true");
                    chassisInfo = this.TestChannelContext.GetChassisInfo(true, true, true, true);
                    testPassed = this.VerifyChassisInfo(testPassed, chassisInfo);
                    CmTestLog.Info("Finished verification of GetchassisInfo when all information was returned blade, PSU, battery and chassis controller \n");

                    // Test GetChassisInfo with no params 
                    CmTestLog.Info("Verifying chassis information when all params are false OR no Params");
                    chassisInfo = this.TestChannelContext.GetChassisInfo(false, false, false, false);
                    testPassed = this.VerifyChassisInfo(testPassed, chassisInfo);
                    CmTestLog.Info("Finished verification of GetchassisInfo with no params \n");

                    // Test for GetChassisInfo with only blade info
                    CmTestLog.Info("Verifying chassis information for only Blade, bladeInfo param is true");
                    chassisInfo = this.TestChannelContext.GetChassisInfo(true, false, false, false);
                    testPassed = this.VerifyOnlyChassisBladeInfo(testPassed, chassisInfo);
                    CmTestLog.Info("Finished verification of GetchassisInfo for only Blade information \n");

                    // Test for GetChassisInfo for only PSU information
                    CmTestLog.Info("Verifying chassis information for only psuInfo param is true");
                    chassisInfo = this.TestChannelContext.GetChassisInfo(false, true, false, false);
                    testPassed = this.VerifyOnlyChassisPsuInfo(testPassed, chassisInfo);
                    CmTestLog.Info("Finished verification of GetChassisInfo for only PSU information \n");

                    // Test for GetChassisInfo for only Battery information
                    CmTestLog.Info("Verifying chassis information for only batteryInfo param is true");
                    chassisInfo = this.TestChannelContext.GetChassisInfo(false, false, false, true);
                    testPassed = this.VerifyOnlyChassisBatteryInfo(testPassed, chassisInfo);
                    CmTestLog.Info("Finished verification of GetChassisInfo for only battery information \n");

                    // Test for GetChassisInfo for only chassis controller information
                    CmTestLog.Info("Verifying chassis information for only chassisControllerInfo param is true");
                    chassisInfo = this.TestChannelContext.GetChassisInfo(false, false, true, false);
                    testPassed = this.VerifyOnlyChassisControllerInfo(testPassed, chassisInfo);
                    CmTestLog.Info("Finished verification of GetChassisInfo for only chassis controller information \n");
                }
            }
            catch (Exception ex)
            {
                ChassisManagerTestHelper.IsTrue(false, ex.Message);
                testPassed = false;
            }
            return testPassed;
        }

        private bool VerifyChassisHealth(bool allPassed, ChassisHealthResponse chassisHealth)
        {
            allPassed &= ChassisManagerTestHelper.IsTrue(chassisHealth != null, "Received chasssis health ");

            if (chassisHealth.completionCode != CompletionCode.Success)
            {
                CmTestLog.Failure("Failed to get chassis health");
                return false;
            }
            CmTestLog.Success("Successfully get chassis health");

            allPassed = this.VerifyChassisBladeHealth(allPassed, chassisHealth);

            allPassed = VerifyFanHealth(allPassed, chassisHealth);

            allPassed = VerifyPsuHealth(allPassed, chassisHealth);
            return allPassed;
        }

        private bool VerifyChassisBladeHealth(bool allPassed, ChassisHealthResponse chassisHealth)
        {
            CmTestLog.Info("Verifying blade shell health state");
            allPassed &= ChassisManagerTestHelper.AreEqual(CmConstants.Population, chassisHealth.bladeShellCollection.Count, "Verified the number of blades in the chassis");

            foreach (var shell in chassisHealth.bladeShellCollection)
            {
                allPassed &= ChassisManagerTestHelper.AreEqual(CompletionCode.Success, shell.completionCode, string.Format("Blade #{0} returns success", shell.bladeNumber));
                // [TFS WorkItem: 3375] GetChassishealth: Command shows blade failures when they exist
                if (this.EmptyLocations != null && this.EmptyLocations.Contains(shell.bladeNumber))
                {
                    allPassed &= ChassisManagerTestHelper.AreEqual(CmConstants.HealthyBladeState, shell.bladeState, string.Format("{0} Blade #{1} is not healthy", shell.bladeState, shell.bladeNumber));
                    allPassed &= ChassisManagerTestHelper.AreEqual(CmConstants.UnKnownBladeType, shell.bladeType, string.Format("{0} Blade #{1} is Unknown", shell.bladeType, shell.bladeNumber));
                }
                else if (this.JbodLocations != null && this.JbodLocations.Contains(shell.bladeNumber))
                {
                    allPassed &= ChassisManagerTestHelper.AreEqual(CmConstants.HealthyBladeState, shell.bladeState, string.Format("{0} Blade #{1} is in good healthy", shell.bladeState, shell.bladeNumber));
                    allPassed &= ChassisManagerTestHelper.AreEqual(CmConstants.JbodBladeType, shell.bladeType, string.Format("{0} Blade #{1} is JBOD", shell.bladeType, shell.bladeNumber));
                }
                else
                {
                    allPassed &= ChassisManagerTestHelper.AreEqual(CmConstants.HealthyBladeState, shell.bladeState, string.Format("{0} Blade #{1} is in good healthy", shell.bladeState, shell.bladeNumber));
                    allPassed &= ChassisManagerTestHelper.AreEqual(CmConstants.ServerBladeType, shell.bladeType, string.Format("{0} Blade #{1} is Server ", shell.bladeType, shell.bladeNumber));
                }
            }
            return allPassed;
        }

        /// <summary>
        /// Test Command: GetChassisHealth: Get Only bladeHealth information (bladehealth=true). Test Verifies:
        /// Command returns completion code Success;
        /// Command returns full blade shell collection and verifies population,bladeType and bladeState
        /// Command returns Empty for Fan, PSU and Battery information.
        /// </summary>
        /// <param name="allPassed"></param>
        /// <param name="chassisHealth"></param>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        private bool VerifyOnlyChassisBladeHealth(bool allPassed, ChassisHealthResponse chassisHealth)
        {
            try
            {
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisHealth != null, "Received chasssis health ");

                if (chassisHealth.completionCode != CompletionCode.Success)
                {
                    CmTestLog.Failure("Failed to get chassis health");
                    return false;
                }
                CmTestLog.Success("Successfully get chassis health");

                allPassed = this.VerifyChassisBladeHealth(allPassed, chassisHealth);
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisHealth.fanInfoCollection.Count == 0, "Fan information is empty");
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisHealth.psuInfoCollection.Count == 0, "PSU information is empty");
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisHealth.batteryInfoCollection.Count == 0, "Battery information is empty");
            }
            catch (Exception ex)
            {
                ChassisManagerTestHelper.IsTrue(false, ex.Message);
                allPassed = false;
            }

            return allPassed;
        }

        /// <summary>
        /// Test Command: GetChassisHealth: Get Only Psu's information (psuhealth=true). Test Verifies:
        /// Command returns completion code Success;
        /// Command returns full psu's information
        /// Command returns Empty for bladeshell,Fan and Battery information.
        /// </summary>
        /// <param name="allPassed"></param>
        /// <param name="chassisHealth"></param>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        private bool VerifyOnlyChassisPsuHealth(bool allPassed, ChassisHealthResponse chassisHealth)
        {
            try
            {
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisHealth != null, "Received chasssis health ");
                if (chassisHealth.completionCode != CompletionCode.Success)
                {
                    CmTestLog.Failure("Failed to get chassis health");
                    return false;
                }
                CmTestLog.Success("Successfully get chassis health");

                allPassed = VerifyPsuHealth(allPassed, chassisHealth);
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisHealth.bladeShellCollection.Count == 0, "BladeShell information is empty");
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisHealth.fanInfoCollection.Count == 0, "Fan information is empty");
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisHealth.batteryInfoCollection.Count == 0, "Battery information is empty");
            }
            catch (Exception ex)
            {
                ChassisManagerTestHelper.IsTrue(false, ex.Message);
                allPassed = false;
            }

            return allPassed;
        }

        /// <summary>
        /// Test Command: GetChassisHealth: Get Only Fan's information (fanHealth=true). Test Verifies:
        /// Command returns completion code Success;
        /// Command returns full Fan's information
        /// Command returns Empty for bladeshell,Psu and Battery information.
        /// </summary>
        /// <param name="allPassed"></param>
        /// <param name="chassisHealth"></param>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        private bool VerifyOnlyChassisFanHealth(bool allPassed, ChassisHealthResponse chassisHealth)
        {
            try
            {
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisHealth != null, "Received chasssis health ");
                if (chassisHealth.completionCode != CompletionCode.Success)
                {
                    CmTestLog.Failure("Failed to get chassis health");
                    return false;
                }
                CmTestLog.Success("Successfully get chassis health");

                allPassed = VerifyFanHealth(allPassed, chassisHealth);
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisHealth.bladeShellCollection.Count == 0, "BladeShell information is empty");
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisHealth.psuInfoCollection.Count == 0, "PSU information is empty");
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisHealth.batteryInfoCollection.Count == 0, "Battery information is empty");
            }
            catch (Exception ex)
            {
                ChassisManagerTestHelper.IsTrue(false, ex.Message);
                allPassed = false;
            }

            return allPassed;
        }

        /// <summary>
        /// Verify chassis info
        /// </summary>
        /// <param name="allPassed">Flag indicating success/failure</param>
        /// <param name="chassisInfo">Chassis info response</param>
        /// <returns>returns success/failure</returns>
        private bool VerifyChassisInfo(bool allPassed, ChassisInfoResponse chassisInfo)
        {
            allPassed &= ChassisManagerTestHelper.IsTrue(chassisInfo != null, "Received chasssis information ");

            if (chassisInfo.completionCode != CompletionCode.Success)
            {
                CmTestLog.Failure(string.Format("Failed to get chassis info, completion code returned : {0}", chassisInfo.completionCode));
                return false;
            }
            CmTestLog.Success("Successfully received chassis info");

            allPassed = this.VerifyChassisBladeInfo(allPassed, chassisInfo);

            allPassed = VerifyChassisPsuInfo(allPassed, chassisInfo);

            allPassed = VerifyChassisBatteryInfo(allPassed, chassisInfo);

            allPassed = VerifyChassisControllerInfo(allPassed, chassisInfo);

            return allPassed;
        }

        /// <summary>
        /// Verify chassis blade info
        /// </summary>
        /// <param name="allPassed">Flag indicating success/failure</param>
        /// <param name="chassisInfo">Chassis info response</param>
        /// <returns>returns success/failure</returns>
        private bool VerifyChassisBladeInfo(bool allPassed, ChassisInfoResponse chassisInfo)
        {
            CmTestLog.Info("Verifying blade info");
            allPassed &= ChassisManagerTestHelper.AreEqual(CmConstants.Population, chassisInfo.bladeCollections.Count, "Verified the number of blades in the chassis");

            foreach (var bladeInfo in chassisInfo.bladeCollections)
            {
                allPassed &= ChassisManagerTestHelper.AreEqual(CompletionCode.Success, bladeInfo.completionCode, string.Format("Blade #{0} returns success", bladeInfo.bladeNumber));               
            }
            return allPassed;
        }

        /// <summary>
        /// Test Command: GetChassisInfo: Get Only blade information (bladeInfo=true). 
        /// </summary>
        /// <param name="allPassed"></param>
        /// <param name="chassisInfo"></param>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        private bool VerifyOnlyChassisBladeInfo(bool allPassed, ChassisInfoResponse chassisInfo)
        {
            try
            {
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisInfo != null, "Received chasssis information ");

                if (chassisInfo.completionCode != CompletionCode.Success)
                {
                    CmTestLog.Failure("Failed to get chassis information");
                    return false;
                }
                CmTestLog.Success("Successfully received chassis information");

                allPassed = this.VerifyChassisBladeInfo(allPassed, chassisInfo);
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisInfo.batteryCollections.Count == 0, "Battery information is empty");
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisInfo.psuCollections.Count == 0, "PSU information is empty");
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisInfo.chassisController == null, "Chassis controller information is empty");
            }
            catch (Exception ex)
            {
                ChassisManagerTestHelper.IsTrue(false, ex.Message);
                allPassed = false;
            }

            return allPassed;
        }

        /// <summary>
        /// Test Command: GetChassisInfo: Get Only PSU information (psuInfo=true). 
        /// </summary>
        /// <param name="allPassed"></param>
        /// <param name="chassisInfo"></param>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        private bool VerifyOnlyChassisPsuInfo(bool allPassed, ChassisInfoResponse chassisInfo)
        {
            try
            {
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisInfo != null, "Received chasssis information ");
                if (chassisInfo.completionCode != CompletionCode.Success)
                {
                    CmTestLog.Failure("Failed to get chassis info");
                    return false;
                }
                CmTestLog.Success("Successfully get chassis info");

                allPassed = VerifyChassisPsuInfo(allPassed, chassisInfo);
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisInfo.bladeCollections.Count == 0, "Blade information is empty");
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisInfo.batteryCollections.Count == 0, "Battery information is empty");
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisInfo.chassisController == null, "Chassis controller information is empty");
            }
            catch (Exception ex)
            {
                ChassisManagerTestHelper.IsTrue(false, ex.Message);
                allPassed = false;
            }

            return allPassed;
        }

        /// <summary>
        /// Test Command: GetChassisInfo: Get Only Battery information (batteryInfo=true). 
        /// </summary>
        /// <param name="allPassed"></param>
        /// <param name="chassisHealth"></param>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        private bool VerifyOnlyChassisBatteryInfo(bool allPassed, ChassisInfoResponse chassisInfo)
        {
            try
            {
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisInfo != null, "Received chasssis info ");
                if (chassisInfo.completionCode != CompletionCode.Success)
                {
                    CmTestLog.Failure("Failed to get chassis info");
                    return false;
                }
                CmTestLog.Success("Successfully received chassis info");

                allPassed = VerifyChassisBatteryInfo(allPassed, chassisInfo);
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisInfo.bladeCollections.Count == 0, "Blade information is empty");
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisInfo.psuCollections.Count == 0, "PSU information is empty");
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisInfo.chassisController == null, "Chassis controller information is empty");
            }
            catch (Exception ex)
            {
                ChassisManagerTestHelper.IsTrue(false, ex.Message);
                allPassed = false;
            }

            return allPassed;
        }

        /// <summary>
        /// Test Command: GetChassisInfo: Get Only Chassis controller information (chassisControllerInfo=true). 
        /// </summary>
        /// <param name="allPassed"></param>
        /// <param name="chassisHealth"></param>
        /// <returns>True if all check-points pass; false, otherwise.</returns>
        private bool VerifyOnlyChassisControllerInfo(bool allPassed, ChassisInfoResponse chassisInfo)
        {
            try
            {
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisInfo != null, "Received chasssis info ");
                if (chassisInfo.completionCode != CompletionCode.Success)
                {
                    CmTestLog.Failure("Failed to get chassis info");
                    return false;
                }
                CmTestLog.Success("Successfully received chassis info");

                allPassed = VerifyChassisControllerInfo(allPassed, chassisInfo);
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisInfo.bladeCollections.Count == 0, "Blade information is empty");
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisInfo.psuCollections.Count == 0, "PSU information is empty");
                allPassed &= ChassisManagerTestHelper.IsTrue(chassisInfo.batteryCollections.Count == 0, "Battery information is empty");
            }
            catch (Exception ex)
            {
                ChassisManagerTestHelper.IsTrue(false, ex.Message);
                allPassed = false;
            }

            return allPassed;
        }
    }
}
