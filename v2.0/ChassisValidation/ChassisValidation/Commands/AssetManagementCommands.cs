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
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Microsoft.GFS.WCS.Contracts;

    internal class AssetManagementCommands : CommandBase
    {
        internal AssetManagementCommands(IChassisManager channel, Dictionary<int, IChassisManager> TestChannelContexts) : base(channel, TestChannelContexts)
        {
        }

        internal AssetManagementCommands(IChassisManager channel, Dictionary<int, IChassisManager> TestChannelContexts, Dictionary<string, string> TestEnvironment) : base(channel, TestChannelContexts, TestEnvironment)
        {
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
            CmTestLog.Start();
            bool allPassed = true;
            string currentApi = "GetChassisManagerAssetInfo";

            try
            {
                // get chassis manager asset info : WorkItem(4702)
                CmTestLog.Info(string.Format("Calling {0}", currentApi));
                ChassisAssetInfoResponse cmAssetInfo = this.Channel.GetChassisManagerAssetInfo();

                // verify Chassis Manager FRU information is correct
                allPassed &= ChassisManagerTestHelper.AreEqual(CompletionCode.Success, cmAssetInfo.completionCode,
                    string.Format("{0}: Completion Code Success", currentApi));

                this.VerifyCmOrPdbAssetInfo(ref allPassed, true, (int)WCSSecurityRole.WcsCmAdmin, null);
                
                if (allPassed)
                {
                    // Log WorkItem 4702
                    ChassisManagerTestHelper.IsTrue(allPassed, string.Format("{0}: Command returns Completion Code Success and returns correct Fru information", currentApi));
                }
                else
                {
                    CmTestLog.Failure(string.Format("{0}: Command failed and additional tests will not be done for this API", currentApi));
                    CmTestLog.End(false);
                    return false;
                }

                // GetBladeAssetInfo returns completion code Success with correct FRU information for all users if server blade : WorkItem(4720)
                bool allUsersPassed = true;
                bool userPassed = true;

                foreach (WCSSecurityRole TestUser in Enum.GetValues(typeof(WCSSecurityRole)))
                {
                    CmTestLog.Info(string.Format("Calling {0} for user {1}", currentApi, TestUser));
                    userPassed = true;

                    // verify CM FRU information
                    this.VerifyCmOrPdbAssetInfo(ref userPassed, true, (int)TestUser, null);

                    allUsersPassed &= userPassed;
                    ChassisManagerTestHelper.IsTrue(userPassed, string.Format("{0}: executes for user {1}", currentApi, TestUser));
                }
                ChassisManagerTestHelper.IsTrue(allUsersPassed, string.Format("{0}: Command passes for all users", currentApi));
            }
            catch (Exception ex)
            {
                ChassisManagerTestHelper.IsTrue(false, string.Format("Exception: {0}", ex.Message));
                allPassed = false;
            }

            CmTestLog.End(allPassed);
            return allPassed;
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
            CmTestLog.Start();
            bool allPassed = true;
            string currentApi = "GetPdbAssetInfo";

            try
            {
                // get pdb asset info : WorkItem(4704)
                CmTestLog.Info(string.Format("Calling {0}", currentApi));
                ChassisAssetInfoResponse pdbAssetInfo = this.Channel.GetPdbAssetInfo();

                this.VerifyCmOrPdbAssetInfo(ref allPassed, false, (int)WCSSecurityRole.WcsCmAdmin, null);

                if (allPassed)
                {
                    // Log WorkItem 4704 
                    ChassisManagerTestHelper.IsTrue(allPassed, string.Format("{0}: Command returns Completion Code Success and returns correct Fru information", currentApi));
                }
                else
                {
                    CmTestLog.Failure(string.Format("{0}: Command failed and additional tests will not be done for this API", currentApi));
                    CmTestLog.End(false);
                    return false;
                }

                // GetBladeAssetInfo returns completion code Success with correct FRU information for all users if server blade : WorkItem(4722)
                bool allUsersPassed = true;
                bool userPassed = true;

                foreach (WCSSecurityRole TestUser in Enum.GetValues(typeof(WCSSecurityRole)))
                {
                    CmTestLog.Info(string.Format("Calling GetPdbAssetInfo for user {0}", TestUser));
                    userPassed = true;

                    // verify Pdb FRU information
                    this.VerifyCmOrPdbAssetInfo(ref userPassed, false, (int)TestUser, null);

                    allUsersPassed &= userPassed;
                    ChassisManagerTestHelper.IsTrue(userPassed, string.Format("{0}: executes for user {1}", currentApi, TestUser));
                }
                ChassisManagerTestHelper.IsTrue(allUsersPassed, string.Format("{0}: Command passes for all users", currentApi));
            }
            catch (Exception ex)
            {
                ChassisManagerTestHelper.IsTrue(false, string.Format("Exception: {0}", ex.Message));
                allPassed = false;
            }

            CmTestLog.End(allPassed);
            return allPassed;
        }

        /// <summary>
        /// Test Command: GetBladeAssetInfo 
        /// The test case verifies:
        /// The command returns a success completion code;
        /// Correct FRU Information is displayed
        /// </summary>
        public bool GetBladeAssetInfoTest()
        {
            CmTestLog.Start();
            bool allPassed = true;
            bool bladePassed;
            string currentApi = "GetBladeAssetInfo";

            this.EmptyLocations = null;
            this.JbodLocations = null;

            try
            {
                this.EmptyLocations = this.GetEmptyLocations();
                this.JbodLocations = this.GetJbodLocations();
            
                for (int bladeId = 1; bladeId <= CmConstants.Population; bladeId++)
                {
                    CmTestLog.Info(string.Format("Calling {0}", currentApi));
                    BladeAssetInfoResponse bladeAssetInfo = this.Channel.GetBladeAssetInfo(bladeId);
                    bladePassed = false;

                    // GetBladeAssetInfo returns completion code Failure if empty slot
                    if (this.EmptyLocations != null && this.EmptyLocations.Contains(bladeId))
                    {
                        bladePassed = ChassisManagerTestHelper.AreEqual(CompletionCode.Failure, bladeAssetInfo.completionCode,
                            string.Format("{1}: Completion Code Failure for bladeId {0} - Empty slot", bladeId.ToString(), currentApi));
                        allPassed &= bladePassed;

                        continue;
                    }
                    // GetBladeAssetInfo returns completion code CommandNotValidForBlade if JBOD : WorkItem(8683)
                    else if (this.JbodLocations != null && this.JbodLocations.Contains(bladeId))
                    {
                        bladePassed = ChassisManagerTestHelper.AreEqual(CompletionCode.CommandNotValidForBlade, bladeAssetInfo.completionCode,
                            string.Format("GetBladeAssetInfo: Completion Code CommandNotValidForBlade for bladeId {0} - JBOD", bladeId.ToString()));
                        allPassed &= bladePassed;
                        ChassisManagerTestHelper.IsTrue(bladePassed, string.Format("{1} Completion Code CommandNotValidForBlade for bladeId {0} - JBOD", bladeId.ToString(), currentApi));
                        continue;
                    }
                    // is server blade
                    else
                    {
                        // GetBladeAssetInfo returns completion code Success and correct Fru information if server blade : WorkItem(4703)
                        this.VerifyBladeAssetInfo(ref bladePassed, bladeId, (int)WCSSecurityRole.WcsCmAdmin, null);
                        
                        ChassisManagerTestHelper.IsTrue(bladePassed, string.Format("{1}: Completion Code Success and correct fru information for bladeId {0}", bladeId.ToString(), currentApi));
                        allPassed &= bladePassed;

                        // GetBladeAssetInfo returns completion code Success and correct FRU information if server blade is hard power cycled : WorkItem(8685)
                        if (bladePassed)
                        {
                            // Hard Power Cycle Blade (Power Off -> Power On)
                            if (!this.SetPowerState(PowerState.OFF, bladeId))
                            {
                                CmTestLog.Failure(string.Format("Power Off failed for bladeId {0}", bladeId));
                                continue;
                            }

                            if (!this.SetPowerState(PowerState.ON, bladeId))
                            {
                                CmTestLog.Failure(string.Format("Power On failed for bladeId {0}", bladeId));
                                continue;
                            }
                            else
                            {
                                CmTestLog.Info(string.Format("Power Cycle success for bladeId {0}", bladeId));
                            }

                            CmTestLog.Info("Calling GetBladeAssetInfo after hard power cycle");
                            bladeAssetInfo = this.Channel.GetBladeAssetInfo(bladeId);

                            // Verify blade Fru information
                            this.VerifyBladeAssetInfo(ref bladePassed, bladeId, (int)WCSSecurityRole.WcsCmAdmin, null);

                            ChassisManagerTestHelper.IsTrue(bladePassed, string.Format("{1}: Command passed after hard-power cycle for bladeId {0}", bladeId.ToString(), currentApi));
                            allPassed &= bladePassed;
                        }

                        // GetBladeAssetInfo returns completion code Success with correct FRU information for all users if server blade : WorkItem(4721)
                        bool allUsersPassed = true;
                        foreach (WCSSecurityRole TestUser in Enum.GetValues(typeof(WCSSecurityRole)))
                        {
                            CmTestLog.Info(string.Format("Calling GetBladeAssetInfo for user {0}", TestUser));

                            this.VerifyBladeAssetInfo(ref bladePassed, bladeId, (int)TestUser, null);
                            
                            allUsersPassed &= bladePassed;
                            ChassisManagerTestHelper.IsTrue(bladePassed, string.Format("GetBladeAssetInfo executes for user {0}", TestUser));
                        }
                        ChassisManagerTestHelper.IsTrue(allUsersPassed, string.Format("{0}: All users can execute the command for bladeId {1}", currentApi, bladeId));
                    }
                }
            }
            catch (Exception ex)
            {
                ChassisManagerTestHelper.IsTrue(false, string.Format("Exception: {0}", ex.Message));
                allPassed = false;
            }

            CmTestLog.End(allPassed);
            return allPassed;
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
            CmTestLog.Start();
            bool allPassed = true;
            string currentApi = "SetChassisManagerAssetInfo";

            MultiRecordResponse setCmAssetInfoResponse = null;
            ChassisAssetInfoResponse cmAssetInfo = null;         
            string emptyString = "";
            string valid1CharString = "X";
            string valid20CharStringWithSpaces = "   This is a test   ";
            string valid56CharString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1289";
            string invalid62CharString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            try
            { 
                // SetChassisManagerAssetInfo returns completion code Success with correct FRU information for all users : WorkItem(4822)
                bool allUsersPassed = true;
                bool userPassed = false;
                foreach (WCSSecurityRole TestUser in Enum.GetValues(typeof(WCSSecurityRole)))
                {
                    CmTestLog.Info(string.Format("Calling {0} for user {1}", currentApi, TestUser));

                    this.TestChannelContext = this.ListTestChannelContexts[(int)TestUser];

                    setCmAssetInfoResponse = this.TestChannelContext.SetChassisManagerAssetInfo(emptyString);

                    // verify Completion Code Success
                    userPassed = ChassisManagerTestHelper.AreEqual(CompletionCode.Success, setCmAssetInfoResponse.completionCode,
                        string.Format("{0}: Completion Code Success for user {1}", currentApi, TestUser));

                    // verify SetChassisManagerAssetInfo has set the Fru
                    if (userPassed)
                    {
                        cmAssetInfo = this.Channel.GetChassisManagerAssetInfo();
                        this.VerifyCmOrPdbAssetInfo(ref userPassed, true, (int)TestUser, emptyString);
                    }

                    allUsersPassed &= userPassed;
                    ChassisManagerTestHelper.IsTrue(userPassed, string.Format("{0}: Sets Fru correctly for user {1}", currentApi, TestUser));
                }
                ChassisManagerTestHelper.IsTrue(allUsersPassed, string.Format("{0}: All users can execute the command", currentApi));
                allPassed &= allUsersPassed;

                if (allPassed)
                {
                    // Verify SetChassisManagerAssetInfo can only set first two fields of payload : WorkItem(4705)
                    bool testPassed = true;

                    string payload = valid1CharString;
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, true, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command passes for payload 'valid1CharString'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format("{0},{1}", valid1CharString, valid1CharString);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, true, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command passes for payload 'valid1CharString,valid1CharString'", currentApi));
                    allPassed &= testPassed;

                    payload = valid20CharStringWithSpaces;
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, true, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command passes for payload 'valid20CharStringWithSpaces'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format("{0},{1}", valid20CharStringWithSpaces, valid20CharStringWithSpaces);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, true, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command passes for payload 'valid20CharStringWithSpaces,valid20CharStringWithSpaces'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format("{0},{1},{2}", valid56CharString, valid56CharString, valid56CharString);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, true, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command passes for payload 'valid56CharString,valid56CharString,valid56CharString'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format("{0},,{1}", valid56CharString, valid56CharString);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, true, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command passes for payload 'valid56CharString,,valid56CharString'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format("{0},,", valid56CharString);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, true, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command passes for payload 'valid56CharString,,'", currentApi));
                    allPassed &= testPassed;

                    // Verify SetChassisManagerAssetInfo sets truncated fields if fields are more than 56 characters : WorkItem(4708)
                    testPassed = true;

                    payload = invalid62CharString;
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, true, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command truncates fields greater than 56 characters for payload 'invalid62CharString'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format(",{0}", invalid62CharString);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, true, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command truncates fields greater than 56 characters for payload ',invalid62CharString'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format("{0},", invalid62CharString);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, true, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command truncates fields greater than 56 characters for payload 'invalid62CharString,'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format("{0},{1}", valid1CharString, invalid62CharString);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, true, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command truncates fields greater than 56 characters for payload 'valid1CharString,invalid62CharString'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format("{0},{1}", invalid62CharString, valid1CharString);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, true, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command truncates fields greater than 56 characters for payload 'invalid62CharString,valid1CharString'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format("{0},{1}", valid56CharString, invalid62CharString);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, true, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command truncates fields greater than 56 characters for payload 'valid56CharString,invalid62CharString'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format("{0},{1}", invalid62CharString, valid56CharString);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, true, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command truncates fields greater than 56 characters for payload 'invalid62CharString,valid56CharString'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format("{0},{1}", invalid62CharString, invalid62CharString);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, true, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command truncates fields greater than 56 characters for payload 'invalid62CharString,invalid62CharString'", currentApi));
                    allPassed &= testPassed;
                }
            }
            catch (Exception ex)
            {
                ChassisManagerTestHelper.IsTrue(false, string.Format("Exception: {0}", ex.Message));
                allPassed = false;
            }

            CmTestLog.End(allPassed);
            return allPassed;
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
            CmTestLog.Start();
            bool allPassed = true;
            string currentApi = "SetPdbAssetInfo";

            MultiRecordResponse setPdbAssetInfoResponse = null;
            ChassisAssetInfoResponse pdbAssetInfo = null;
            string emptyString = "";
            string valid1CharString = "X";
            string valid20CharStringWithSpaces = "   This is a test   ";
            string valid56CharString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1289";
            string invalid62CharString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            try
            {
                // SetPdbAssetInfo returns completion code Success with correct FRU information for all users : WorkItem(4823)
                bool allUsersPassed = true;
                bool userPassed = false;
                foreach (WCSSecurityRole TestUser in Enum.GetValues(typeof(WCSSecurityRole)))
                {
                    CmTestLog.Info(string.Format("Calling {0} for user {1}", currentApi, TestUser));

                    this.TestChannelContext = this.ListTestChannelContexts[(int)TestUser];

                    setPdbAssetInfoResponse = this.TestChannelContext.SetPdbAssetInfo(emptyString);

                    // verify Completion Code Success
                    userPassed = ChassisManagerTestHelper.AreEqual(CompletionCode.Success, setPdbAssetInfoResponse.completionCode,
                        string.Format("SetPdbAssetInfo: Completion Code Success for user {0}", TestUser));

                    // verify SetChassisManagerAssetInfo has set the Fru
                    if (userPassed)
                    {
                        pdbAssetInfo = this.Channel.GetPdbAssetInfo();
                        this.VerifyCmOrPdbAssetInfo(ref userPassed, false, (int)TestUser, emptyString);
                    }

                    allUsersPassed &= userPassed;
                    ChassisManagerTestHelper.IsTrue(userPassed, string.Format("{0}: Command sets Fru correctly for user {1}", currentApi, TestUser));
                }
                ChassisManagerTestHelper.IsTrue(allUsersPassed, string.Format("{0}: All users can execute the command", currentApi));
                allPassed &= allUsersPassed;

                if (allPassed)
                {
                    // Verify SetPdbAssetInfo can only set first two fields of payload : WorkItem(4707)
                    bool testPassed = true;

                    string payload = valid1CharString;
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, false, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command passes for payload 'valid1CharString'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format("{0},{1}", valid1CharString, valid1CharString);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, false, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command passes for payload 'valid1CharString,valid1CharString'", currentApi));
                    allPassed &= testPassed;

                    payload = valid20CharStringWithSpaces;
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, false, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command passes for payload 'valid20CharStringWithSpaces'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format("{0},{1}", valid20CharStringWithSpaces, valid20CharStringWithSpaces);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, false, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command passes for payload 'valid20CharStringWithSpaces,valid20CharStringWithSpaces'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format("{0},{1},{2}", valid56CharString, valid56CharString, valid56CharString);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, false, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command passes for payload 'valid56CharString,valid56CharString,valid56CharString'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format("{0},,{1}", valid56CharString, valid56CharString);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, false, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command passes for payload 'valid56CharString,,valid56CharString'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format("{0},,", valid56CharString);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, false, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command passes for payload 'valid56CharString,,'", currentApi));
                    allPassed &= testPassed;

                    // Verify SetPdbAssetInfo sets truncated fields if fields are more than 56 characters : WorkItem(4710)
                    testPassed = true;

                    payload = invalid62CharString;
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, false, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command truncates fields greater than 56 characters for payload 'invalid62CharString'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format(",{0}", invalid62CharString);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, false, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command truncates fields greater than 56 characters for payload ',invalid62CharString'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format("{0},", invalid62CharString);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, false, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command truncates fields greater than 56 characters for payload 'invalid62CharString,'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format("{0},{1}", valid1CharString, invalid62CharString);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, false, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command truncates fields greater than 56 characters for payload 'valid1CharString,invalid62CharString'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format("{0},{1}", invalid62CharString, valid1CharString);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, false, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command truncates fields greater than 56 characters for payload 'invalid62CharString,valid1CharString'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format("{0},{1}", valid56CharString, invalid62CharString);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, false, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command truncates fields greater than 56 characters for payload 'valid56CharString,invalid62CharString'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format("{0},{1}", invalid62CharString, valid56CharString);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, false, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command truncates fields greater than 56 characters for payload 'invalid62CharString,valid56CharString'", currentApi));
                    allPassed &= testPassed;

                    payload = string.Format("{0},{1}", invalid62CharString, invalid62CharString);
                    this.VerifySetCmOrPdbAssetInfo(ref testPassed, false, (int)WCSSecurityRole.WcsCmAdmin, payload);
                    ChassisManagerTestHelper.IsTrue(testPassed,
                        string.Format("{0}: Command truncates fields greater than 56 characters for payload 'invalid62CharString,invalid62CharString'", currentApi));
                    allPassed &= testPassed;
                }
            }
            catch (Exception ex)
            {
                ChassisManagerTestHelper.IsTrue(false, string.Format("Exception: {0}", ex.Message));
                allPassed = false;
            }

            CmTestLog.End(allPassed);
            return allPassed;
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
            CmTestLog.Start();
            bool allPassed = true;
            string currentApi = "SetBladeAssetInfo";

            BladeMultiRecordResponse setBladeAssetInfoResponse = null;
            string emptyString = "";
            string valid1CharString = "X";
            string valid20CharStringWithSpaces = "   This is a test   ";
            string valid56CharString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1289";
            string invalid62CharString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            this.EmptyLocations = this.GetEmptyLocations();
            this.JbodLocations = this.GetJbodLocations();

            try
            {
                for (int bladeId = 1; bladeId <= CmConstants.Population; bladeId++)
                {
                    this.TestChannelContext = this.ListTestChannelContexts[(int)WCSSecurityRole.WcsCmAdmin];

                    setBladeAssetInfoResponse = this.TestChannelContext.SetBladeAssetInfo(bladeId, emptyString);

                    if (this.EmptyLocations != null && this.EmptyLocations.Contains(bladeId))
                    {
                        allPassed &= ChassisManagerTestHelper.AreEqual(CompletionCode.Failure, setBladeAssetInfoResponse.completionCode,
                            string.Format("{1}: Completion Code Failure for bladeId {0} - Empty slot", bladeId.ToString(), currentApi));
                        continue;
                    }
                    // GetBladeAssetInfo returns completion code CommandNotValidForBlade if JBOD : WorkItem(8684)
                    else if (this.JbodLocations != null && this.JbodLocations.Contains(bladeId))
                    {
                        allPassed &= ChassisManagerTestHelper.AreEqual(CompletionCode.CommandNotValidForBlade, setBladeAssetInfoResponse.completionCode,
                            string.Format("{1}: Completion Code CommandNotValidForBlade for bladeId {0} - JBOD", bladeId.ToString(), currentApi));
                        continue;
                    }
                    else // is server blade
                    {
                        // SetBladeAssetInfo returns completion code Success with correct FRU information for all users if server blade : WorkItem(4755)
                        bool bladePassed = true;
                        bool userPassed = false;
                        foreach (WCSSecurityRole TestUser in Enum.GetValues(typeof(WCSSecurityRole)))
                        {
                            CmTestLog.Info(string.Format("Calling {0} for user {1}", currentApi, TestUser));

                            this.TestChannelContext = this.ListTestChannelContexts[(int)TestUser];

                            setBladeAssetInfoResponse = this.TestChannelContext.SetBladeAssetInfo(bladeId, emptyString);

                            // verify Completion Code Success
                            userPassed = ChassisManagerTestHelper.AreEqual(CompletionCode.Success, setBladeAssetInfoResponse.completionCode,
                                string.Format("{0}: Completion Code Success for user {1}", currentApi, TestUser));

                            // verify SetBladeAssetInfo has set the Fru
                            if (userPassed)
                            {
                                this.VerifyBladeAssetInfo(ref userPassed, bladeId, (int)TestUser, emptyString);
                            }

                            bladePassed &= userPassed;
                            ChassisManagerTestHelper.IsTrue(userPassed, string.Format("{0}: Command sets Fru correctly for user {1}", currentApi, TestUser));
                        }
                        ChassisManagerTestHelper.IsTrue(bladePassed, string.Format("{0}: All users can execute the command", currentApi));
                        allPassed &= bladePassed;

                        if (bladePassed)
                        {
                            // Verify SetBladeAssetInfo can only set first two fields of payload : WorkItem(4706)
                            bool testPassed = true;

                            string payload = valid1CharString;
                            this.VerifySetBladeAssetInfo(ref testPassed, bladeId, (int)WCSSecurityRole.WcsCmAdmin, payload);
                            ChassisManagerTestHelper.IsTrue(testPassed,
                                string.Format("{0}: Command passes for payload 'valid1CharString'", currentApi));
                            allPassed &= testPassed;

                            payload = string.Format("{0},{1}", valid1CharString, valid1CharString);
                            this.VerifySetBladeAssetInfo(ref testPassed, bladeId, (int)WCSSecurityRole.WcsCmAdmin, payload);
                            ChassisManagerTestHelper.IsTrue(testPassed,
                                string.Format("{0}: Command passes for payload 'valid1CharString,valid1CharString'", currentApi));
                            allPassed &= testPassed;

                            payload = valid20CharStringWithSpaces;
                            this.VerifySetBladeAssetInfo(ref testPassed, bladeId, (int)WCSSecurityRole.WcsCmAdmin, payload);
                            ChassisManagerTestHelper.IsTrue(testPassed,
                                string.Format("{0}: Command passes for payload 'valid20CharStringWithSpaces'", currentApi));
                            allPassed &= testPassed;

                            payload = string.Format("{0},{1}", valid20CharStringWithSpaces, valid20CharStringWithSpaces);
                            this.VerifySetBladeAssetInfo(ref testPassed, bladeId, (int)WCSSecurityRole.WcsCmAdmin, payload);
                            ChassisManagerTestHelper.IsTrue(testPassed,
                                string.Format("{0}: Command passes for payload 'valid20CharStringWithSpaces,valid20CharStringWithSpaces'", currentApi));
                            allPassed &= testPassed;

                            payload = string.Format("{0},{1},{2}", valid56CharString, valid56CharString, valid56CharString);
                            this.VerifySetBladeAssetInfo(ref testPassed, bladeId, (int)WCSSecurityRole.WcsCmAdmin, payload);
                            ChassisManagerTestHelper.IsTrue(testPassed,
                                string.Format("{0}: Command passes for payload 'valid56CharString,valid56CharString,valid56CharString'", currentApi));
                            allPassed &= testPassed;

                            payload = string.Format("{0},,{1}", valid56CharString, valid56CharString);
                            this.VerifySetBladeAssetInfo(ref testPassed, bladeId, (int)WCSSecurityRole.WcsCmAdmin, payload);
                            ChassisManagerTestHelper.IsTrue(testPassed,
                                string.Format("{0}: Command passes for payload 'valid56CharString,,valid56CharString'", currentApi));
                            allPassed &= testPassed;

                            payload = string.Format("{0},,", valid56CharString);
                            this.VerifySetBladeAssetInfo(ref testPassed, bladeId, (int)WCSSecurityRole.WcsCmAdmin, payload);
                            ChassisManagerTestHelper.IsTrue(testPassed,
                                string.Format("{0}: Command passes for payload 'valid56CharString,,'", currentApi));
                            allPassed &= testPassed;

                            // Verify SetBladeAssetInfo sets truncated fields if fields are more than 56 characters : WorkItem(4709)
                            testPassed = true;

                            payload = invalid62CharString;
                            this.VerifyBladeAssetInfo(ref testPassed, bladeId, (int)WCSSecurityRole.WcsCmAdmin, payload);
                            ChassisManagerTestHelper.IsTrue(testPassed,
                                string.Format("{0}: Command truncates fields greater than 56 characters for payload 'invalid62CharString'", currentApi));
                            allPassed &= testPassed;

                            payload = string.Format(",{0}", invalid62CharString);
                            this.VerifyBladeAssetInfo(ref testPassed, bladeId, (int)WCSSecurityRole.WcsCmAdmin, payload);
                            ChassisManagerTestHelper.IsTrue(testPassed,
                                string.Format("{0}: Command truncates fields greater than 56 characters for payload ',invalid62CharString'", currentApi));
                            allPassed &= testPassed;

                            payload = string.Format("{0},", invalid62CharString);
                            this.VerifyBladeAssetInfo(ref testPassed, bladeId, (int)WCSSecurityRole.WcsCmAdmin, payload);
                            ChassisManagerTestHelper.IsTrue(testPassed,
                                string.Format("{0}: Command truncates fields greater than 56 characters for payload 'invalid62CharString,'", currentApi));
                            allPassed &= testPassed;

                            payload = string.Format("{0},{1}", valid1CharString, invalid62CharString);
                            this.VerifyBladeAssetInfo(ref testPassed, bladeId, (int)WCSSecurityRole.WcsCmAdmin, payload);
                            ChassisManagerTestHelper.IsTrue(testPassed,
                                string.Format("{0}: Command truncates fields greater than 56 characters for payload 'valid1CharString,invalid62CharString'", currentApi));
                            allPassed &= testPassed;

                            payload = string.Format("{0},{1}", invalid62CharString, valid1CharString);
                            this.VerifyBladeAssetInfo(ref testPassed, bladeId, (int)WCSSecurityRole.WcsCmAdmin, payload);
                            ChassisManagerTestHelper.IsTrue(testPassed,
                                string.Format("{0}: Command truncates fields greater than 56 characters for payload 'invalid62CharString,valid1CharString'", currentApi));
                            allPassed &= testPassed;

                            payload = string.Format("{0},{1}", valid56CharString, invalid62CharString);
                            this.VerifyBladeAssetInfo(ref testPassed, bladeId, (int)WCSSecurityRole.WcsCmAdmin, payload);
                            ChassisManagerTestHelper.IsTrue(testPassed,
                                string.Format("{0}: Command truncates fields greater than 56 characters for payload 'valid56CharString,invalid62CharString'", currentApi));
                            allPassed &= testPassed;

                            payload = string.Format("{0},{1}", invalid62CharString, valid56CharString);
                            this.VerifyBladeAssetInfo(ref testPassed, bladeId, (int)WCSSecurityRole.WcsCmAdmin, payload);
                            ChassisManagerTestHelper.IsTrue(testPassed,
                                string.Format("{0}: Command truncates fields greater than 56 characters for payload 'invalid62CharString,valid56CharString'", currentApi));
                            allPassed &= testPassed;

                            payload = string.Format("{0},{1}", invalid62CharString, invalid62CharString);
                            this.VerifyBladeAssetInfo(ref testPassed, bladeId, (int)WCSSecurityRole.WcsCmAdmin, payload);
                            ChassisManagerTestHelper.IsTrue(testPassed,
                                string.Format("{0}: Command truncates fields greater than 56 characters for payload 'invalid62CharString,invalid62CharString'", currentApi));
                            allPassed &= testPassed;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ChassisManagerTestHelper.IsTrue(false, string.Format("Exception: {0}", ex.Message));
                allPassed = false;
            }

            CmTestLog.End(allPassed);
            return allPassed;
        }

        private void VerifyCmOrPdbAssetInfo(ref bool cmOrPdbPassed, bool verifyingCmAssetInfo, int user, string payload)
        {
            int maxStringLength = 56;
            string propertyValue;
            string currentApi = verifyingCmAssetInfo ? "GetChassisManagerAssetInfo" : "GetPdbAssetInfo";
            ChassisAssetInfoResponse cmOrPdbAssetInfo = new ChassisAssetInfoResponse();

            this.TestChannelContext = this.ListTestChannelContexts[user];

            if (verifyingCmAssetInfo)
            {
                cmOrPdbAssetInfo = this.TestChannelContext.GetChassisManagerAssetInfo();
            }
            else
            {
                cmOrPdbAssetInfo = this.TestChannelContext.GetPdbAssetInfo();
            }

            // get completion code success
            cmOrPdbPassed = ChassisManagerTestHelper.AreEqual(CompletionCode.Success, cmOrPdbAssetInfo.completionCode,
                string.Format("{0}: Completion Code Success", currentApi));

            if (cmOrPdbPassed)
            {
                // Get CM or PDB Fru XML Sample
                string fruDefFileDir = Path.Combine(Directory.GetCurrentDirectory(), "TestData");
                string fruFileName = verifyingCmAssetInfo ? Path.Combine(fruDefFileDir, "ChassisManagerFruSample.xml") : Path.Combine(fruDefFileDir, "PdbFruSample.xml");

                if (!File.Exists(fruFileName))
                {
                    throw new ApplicationException(string.Format("{0}: Sample Xml file is NOT found under the path {1}", currentApi, fruDefFileDir));
                }

                // Extract CmFruSample Xml file
                XmlReader cmOrPdbFruXml = XmlReader.Create(fruFileName);

                cmOrPdbFruXml.ReadToFollowing("chassisAreaPartNumber");
                propertyValue = cmOrPdbFruXml.ReadElementContentAsString();
                cmOrPdbPassed &= ChassisManagerTestHelper.AreEqual(propertyValue, cmOrPdbAssetInfo.chassisAreaPartNumber,
                    string.Format("{0}: Received Chassis Area Part Number", currentApi));

                cmOrPdbFruXml.ReadToFollowing("chassisAreaSerialNumber");
                propertyValue = cmOrPdbFruXml.ReadElementContentAsString();
                cmOrPdbPassed &= ChassisManagerTestHelper.AreEqual(propertyValue, cmOrPdbAssetInfo.chassisAreaSerialNumber,
                    string.Format("{0}: Received Chassis Area Serial Number", currentApi));

                cmOrPdbFruXml.ReadToFollowing("boardAreaManufacturerName");
                propertyValue = cmOrPdbFruXml.ReadElementContentAsString();
                cmOrPdbPassed &= ChassisManagerTestHelper.AreEqual(propertyValue, cmOrPdbAssetInfo.boardAreaManufacturerName,
                    string.Format("{0}: Received Board Area Manufacturer Name", currentApi));

                cmOrPdbFruXml.ReadToFollowing("boardAreaManufacturerDate");
                propertyValue = cmOrPdbFruXml.ReadElementContentAsString();
                Match propertyMatch = Regex.Match(propertyValue, @"0[1-9]|[1-12]/0[0-9]|[0-31]/[1-2][09][01789]\d\s\d?\d:\d?\d:\d?\d\s[aApP][mM]");
                cmOrPdbPassed &= ChassisManagerTestHelper.IsTrue(propertyMatch.Success,
                    string.Format("{0}: Received Board Area Manufacturer Date", currentApi));

                cmOrPdbFruXml.ReadToFollowing("boardAreaProductName");
                propertyValue = cmOrPdbFruXml.ReadElementContentAsString();
                cmOrPdbPassed &= ChassisManagerTestHelper.AreEqual(propertyValue, cmOrPdbAssetInfo.boardAreaProductName,
                    string.Format("{0}: Received Board Area Product Name", currentApi));

                cmOrPdbFruXml.ReadToFollowing("boardAreaSerialNumber");
                propertyValue = cmOrPdbFruXml.ReadElementContentAsString();
                cmOrPdbPassed &= ChassisManagerTestHelper.AreEqual(propertyValue, cmOrPdbAssetInfo.boardAreaSerialNumber,
                    string.Format("{0}: Received Board Area Serial Number", currentApi));

                cmOrPdbFruXml.ReadToFollowing("boardAreaPartNumber");
                propertyValue = cmOrPdbFruXml.ReadElementContentAsString();
                cmOrPdbPassed &= ChassisManagerTestHelper.AreEqual(propertyValue, cmOrPdbAssetInfo.boardAreaPartNumber,
                    string.Format("{0}: Received Board Area Part Number", currentApi));

                cmOrPdbFruXml.ReadToFollowing("productAreaManufactureName");
                propertyValue = cmOrPdbFruXml.ReadElementContentAsString();
                cmOrPdbPassed &= ChassisManagerTestHelper.AreEqual(propertyValue, cmOrPdbAssetInfo.productAreaManufactureName,
                    string.Format("{0}: Received Product Area Manufacture Name", currentApi));

                cmOrPdbFruXml.ReadToFollowing("productAreaProductName");
                propertyValue = cmOrPdbFruXml.ReadElementContentAsString();
                cmOrPdbPassed &= ChassisManagerTestHelper.AreEqual(propertyValue, cmOrPdbAssetInfo.productAreaProductName,
                    string.Format("{0}: Received Product Area Product Name", currentApi));

                cmOrPdbFruXml.ReadToFollowing("productAreaPartModelNumber");
                propertyValue = cmOrPdbFruXml.ReadElementContentAsString();
                cmOrPdbPassed &= ChassisManagerTestHelper.AreEqual(propertyValue, cmOrPdbAssetInfo.productAreaPartModelNumber,
                    string.Format("{0}: Received Product Area Part Model Number", currentApi));

                cmOrPdbFruXml.ReadToFollowing("productAreaProductVersion");
                propertyValue = cmOrPdbFruXml.ReadElementContentAsString();
                cmOrPdbPassed &= ChassisManagerTestHelper.AreEqual(propertyValue, cmOrPdbAssetInfo.productAreaProductVersion,
                    string.Format("{0}: Received Product Area Product Version", currentApi));

                cmOrPdbFruXml.ReadToFollowing("productAreaSerialNumber");
                propertyValue = cmOrPdbFruXml.ReadElementContentAsString();
                cmOrPdbPassed &= ChassisManagerTestHelper.AreEqual(propertyValue, cmOrPdbAssetInfo.productAreaSerialNumber,
                    string.Format("{0}: Received Product Area Serial Number", currentApi));

                cmOrPdbFruXml.ReadToFollowing("productAreaAssetTag");
                propertyValue = cmOrPdbFruXml.ReadElementContentAsString();
                cmOrPdbPassed &= ChassisManagerTestHelper.AreEqual(propertyValue, cmOrPdbAssetInfo.productAreaAssetTag,
                    string.Format("{0}: Received Product Area Asset Tag", currentApi));

                cmOrPdbFruXml.ReadToFollowing("manufacturer");
                propertyValue = cmOrPdbFruXml.ReadElementContentAsString();
                cmOrPdbPassed &= ChassisManagerTestHelper.AreEqual(propertyValue, cmOrPdbAssetInfo.manufacturer,
                    string.Format("{0}: Received Manufacturer", currentApi));

                cmOrPdbFruXml.ReadToFollowing("serviceVersion");
                propertyValue = cmOrPdbFruXml.ReadElementContentAsString();
                cmOrPdbPassed &= ChassisManagerTestHelper.AreEqual(propertyValue, cmOrPdbAssetInfo.serviceVersion,
                    string.Format("{0}: Received Service Version", currentApi));

                // Verify multirecord fields are set
                if (payload != null)
                {
                    string[] payLoadFields = payload.Split(',').Select(field => field.Trim()).ToArray();
                    
                    int fieldCount = 0;
                    string expectedField = null;

                    // Verify payLoad matches MultiRecordFields (only first two fields allowed)
                    if (cmOrPdbAssetInfo.multiRecordFields.Count() <= 2)
                    {
                        foreach (string actualField in cmOrPdbAssetInfo.multiRecordFields)
                        {
                            if (payLoadFields[fieldCount].Length > 56)
                            {
                                expectedField = payLoadFields[fieldCount].Substring(0, maxStringLength);
                            }
                            else
                            {
                                expectedField = payLoadFields[fieldCount];
                            }

                            cmOrPdbPassed &= ChassisManagerTestHelper.AreEqual(expectedField, actualField,
                                string.Format("{0}{1}", currentApi, string.Format(": Received Field{0} '{1}'", fieldCount.ToString(), payLoadFields[fieldCount])));
                            fieldCount++;
                        }
                    }
                    else
                    {
                        CmTestLog.Failure(string.Format("{0}: Command exceeded number of MultiRecord Fields allowed", currentApi));
                    }
                }

                // Close XmlReader
                cmOrPdbFruXml.Close();
            }
        }

        private void VerifyBladeAssetInfo(ref bool bladePassed, int bladeId, int user, string payload)
        {
            bool isTrue;
            int maxStringLength = 56;
            string propertyValue;

            BladeAssetInfoResponse bladeAssetInfo = new BladeAssetInfoResponse();

            this.TestChannelContext = this.ListTestChannelContexts[user];

            bladeAssetInfo = this.TestChannelContext.GetBladeAssetInfo(bladeId);

            // get completion code success
            bladePassed = ChassisManagerTestHelper.AreEqual(CompletionCode.Success, bladeAssetInfo.completionCode,
                string.Format("GetBladeAssetInfo: Completion Code Success for bladeId {0}", bladeId.ToString()));

            if (bladePassed)
            {
                // Get Blade Fru XML Sample
                string fruDefFileDir = Path.Combine(Directory.GetCurrentDirectory(), "TestData");
                string fruFileName = Path.Combine(fruDefFileDir, "BladeFruSample.xml");

                if (!File.Exists(fruFileName))
                {
                    throw new ApplicationException(string.Format("GetBladeAssetInfo: Sample Xml file is NOT found under the path {0}", fruDefFileDir));
                }

                // Extract BladeFruSample Xml file
                XmlReader bladeFruXml = XmlReader.Create(fruFileName);

                bladePassed &= ChassisManagerTestHelper.AreEqual(bladeId, bladeAssetInfo.bladeNumber,
                    string.Format("GetBladeAssetInfo: Received Blade Number for bladeId {0}", bladeId));

                bladeFruXml.ReadToFollowing("chassisAreaPartNumber");
                propertyValue = bladeFruXml.ReadElementContentAsString();
                bladePassed &= ChassisManagerTestHelper.AreEqual(propertyValue, bladeAssetInfo.chassisAreaPartNumber,
                    string.Format("GetBladeAssetInfo: Received Chassis Area Part Number for blade {0}", bladeId));

                isTrue = bladeAssetInfo.chassisAreaSerialNumber.Length <= 16;
                bladePassed &= ChassisManagerTestHelper.IsTrue(isTrue,
                    string.Format("GetBladeAssetInfo: Received Chassis Area Serial Number for blade {0}", bladeId));

                bladeFruXml.ReadToFollowing("boardAreaManufacturerName");
                propertyValue = bladeFruXml.ReadElementContentAsString();
                bladePassed &= ChassisManagerTestHelper.AreEqual(propertyValue, bladeAssetInfo.boardAreaManufacturerName,
                    string.Format("GetBladeAssetInfo: Received Board Area Manufacturer Name for blade {0}", bladeId));

                bladeFruXml.ReadToFollowing("boardAreaManufacturerDate");
                propertyValue = bladeFruXml.ReadElementContentAsString();
                Match propertyMatch = Regex.Match(propertyValue, @"0[1-9]|[1-12]/0[0-9]|[0-31]/[1-2][09][01789]\d\s\d?\d:\d?\d:\d?\d\s[aApP][mM]");
                bladePassed &= ChassisManagerTestHelper.IsTrue(propertyMatch.Success,
                    string.Format("GetBladeAssetInfo: Received Board Area Manufacturer Date for blade {0}", bladeId));

                bladeFruXml.ReadToFollowing("boardAreaProductName");
                propertyValue = bladeFruXml.ReadElementContentAsString();
                bladePassed &= ChassisManagerTestHelper.AreEqual(propertyValue, bladeAssetInfo.boardAreaProductName,
                    string.Format("BladeAssetInfo: Received Board Area Product Name for blade {0}", bladeId));

                isTrue = bladeAssetInfo.boardAreaSerialNumber.Length <= 16;
                bladePassed &= ChassisManagerTestHelper.IsTrue(isTrue,
                    string.Format("GetBladeAssetInfo: Received Board Area Serial Number for blade {0}", bladeId));

                bladeFruXml.ReadToFollowing("boardAreaPartNumber");
                propertyValue = bladeFruXml.ReadElementContentAsString();
                bladePassed &= ChassisManagerTestHelper.AreEqual(propertyValue, bladeAssetInfo.boardAreaPartNumber,
                    string.Format("GetBladeAssetInfo: Received Board Area Part Number for blade {0}", bladeId));

                bladeFruXml.ReadToFollowing("productAreaManufactureName");
                propertyValue = bladeFruXml.ReadElementContentAsString();
                bladePassed &= ChassisManagerTestHelper.AreEqual(propertyValue, bladeAssetInfo.productAreaManufactureName,
                    string.Format("GetBladeAssetInfo: Received Product Area Manufacture Name for blade {0}", bladeId));

                bladeFruXml.ReadToFollowing("productAreaProductName");
                propertyValue = bladeFruXml.ReadElementContentAsString();
                bladePassed &= ChassisManagerTestHelper.AreEqual(propertyValue, bladeAssetInfo.productAreaProductName,
                    string.Format("GetBladeAssetInfo: Received Product Area Product Name for blade {0}", bladeId));

                bladeFruXml.ReadToFollowing("productAreaPartModelNumber");
                propertyValue = bladeFruXml.ReadElementContentAsString();
                bladePassed &= ChassisManagerTestHelper.AreEqual(propertyValue, bladeAssetInfo.productAreaPartModelNumber,
                    string.Format("GetBladeAssetInfo: Received Product Area Part Model Number for blade {0}", bladeId));

                bladeFruXml.ReadToFollowing("productAreaProductVersion");
                propertyValue = bladeFruXml.ReadElementContentAsString();
                bladePassed &= ChassisManagerTestHelper.AreEqual(propertyValue, bladeAssetInfo.productAreaProductVersion,
                    string.Format("GetBladeAssetInfo: Received Product Area Product Version for blade {0}", bladeId));

                isTrue = bladeAssetInfo.productAreaSerialNumber.Length <= 16;
                bladePassed &= ChassisManagerTestHelper.IsTrue(isTrue,
                    string.Format("GetBladeAssetInfo: Received Product Area Serial Number for blade {0}", bladeId));

                isTrue = bladeAssetInfo.productAreaAssetTag.Length <= 10;
                bladePassed &= ChassisManagerTestHelper.IsTrue(isTrue,
                    string.Format("GetBladeAssetInfo: Received Product Area Asset Tag for blade {0}", bladeId));

                bladeFruXml.ReadToFollowing("manufacturer");
                propertyValue = bladeFruXml.ReadElementContentAsString();
                bladePassed &= ChassisManagerTestHelper.AreEqual(propertyValue, bladeAssetInfo.manufacturer,
                    string.Format("GetBladeAssetInfo: Received Manufacturer for blade {0}", bladeId));

                // Verify multirecord fields are set
                if (payload != null)
                {
                    string[] payLoadFields = payload.Split(',').Select(field => field.Trim()).ToArray();

                    int fieldCount = 0;
                    string expectedField = null;

                    // Verify payLoad matches MultiRecordFields (only first two fields allowed)
                    if (bladeAssetInfo.multiRecordFields.Count() <= 2)
                    {
                        foreach (string actualField in bladeAssetInfo.multiRecordFields)
                        {
                            if (payLoadFields[fieldCount].Length > 56)
                            {
                                expectedField = payLoadFields[fieldCount].Substring(0, maxStringLength);
                            }
                            else
                            {
                                expectedField = payLoadFields[fieldCount];
                            }

                            bladePassed &= ChassisManagerTestHelper.AreEqual(expectedField, actualField,
                                string.Format("GetBladeAssetInfo: Received Field{0} '{1}'", fieldCount.ToString(), payLoadFields[fieldCount]));
                            fieldCount++;
                        }
                    }
                    else
                    {
                        CmTestLog.Failure("GetBladeAssetInfo: Command exceeded number of MultiRecord Fields allowed");
                    }
                }

                // Close Xml Reader
                bladeFruXml.Close();
            }
        }

        private void VerifySetCmOrPdbAssetInfo(ref bool allPassed, bool verifyingSetCmAssetInfo, int user, string payload)
        {
            bool testPassed = true;
            string currentApi = verifyingSetCmAssetInfo ? "SetChassisManagerAssetInfo" : "SetPdbAssetInfo";
            MultiRecordResponse setCmOrPdbAssetInfo;

            // Verify SetChassisManagerAssetInfo can only set first two fields of payload
            this.TestChannelContext = this.ListTestChannelContexts[user];

            if (verifyingSetCmAssetInfo)
            {
                setCmOrPdbAssetInfo = this.TestChannelContext.SetChassisManagerAssetInfo(payload);
            }
            else
            {
                setCmOrPdbAssetInfo = this.TestChannelContext.SetPdbAssetInfo(payload);
            }

            testPassed = ChassisManagerTestHelper.AreEqual(CompletionCode.Success, setCmOrPdbAssetInfo.completionCode,
                string.Format("{0}: Completion Code Success", currentApi));
            allPassed &= testPassed;

            if (testPassed)
            {
                this.VerifyCmOrPdbAssetInfo(ref testPassed, verifyingSetCmAssetInfo, user, payload);
            }
            allPassed &= testPassed;
        }

        private void VerifySetBladeAssetInfo(ref bool allPassed, int bladeId, int user, string payload)
        {
            bool testPassed = true;
            string currentApi = "SetBladeAssetInfo";
            BladeMultiRecordResponse setBladeAssetInfo;

            // Verify SetChassisManagerAssetInfo can only set first two fields of payload
            this.TestChannelContext = this.ListTestChannelContexts[user];

            setBladeAssetInfo = this.TestChannelContext.SetBladeAssetInfo(bladeId, payload);

            testPassed = ChassisManagerTestHelper.AreEqual(CompletionCode.Success, setBladeAssetInfo.completionCode,
                string.Format("{0}: Completion Code Success", currentApi));
            allPassed &= testPassed;

            if (testPassed)
            {
                this.VerifyBladeAssetInfo(ref testPassed, bladeId, user, payload);
            }
            allPassed &= testPassed;
        }
    }
}
