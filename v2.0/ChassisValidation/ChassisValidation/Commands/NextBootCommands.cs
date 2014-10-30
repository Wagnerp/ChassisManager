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
    using System.Threading;
    using Microsoft.GFS.WCS.Contracts;

    internal class NextBootCommands : CommandBase
    {
        internal NextBootCommands(IChassisManager channel) :
            base(channel)
        {
        }

        internal NextBootCommands(IChassisManager channel, Dictionary<int, IChassisManager> testChannelContexts) :
            base(channel, testChannelContexts)
        {
        }

        public Boolean NextBootCommandBasicValidationTest()
        {
            return (
                SetNextBootTest() &&
                SetNextBootByAllUserTest() &&
                SetNextBootToJbodTest()
                );
        }


        /// <summary>
        ///     Basic validation test to set NextBoot test
        /// </summary>
        protected Boolean SetNextBootTest()
        {
            CmTestLog.Start();

            bool testPassed = true;
            bool isServer = false;

            int bladeIndex = 1;
            string failureMessage;

            EmptyLocations = GetEmptyLocations();
            JbodLocations = GetJbodLocations();

            CmTestLog.Info("!!!!!!!!! Starting execution of SetNextBootTest.");

            //Make sure blade is reacheable
            //Loop through servers listing and find first Blad to test
            while (!isServer && bladeIndex <= CmConstants.Population)
            {
                if (!JbodLocations.Contains(bladeIndex) && !EmptyLocations.Contains(bladeIndex))
                    isServer = true;
                else
                    bladeIndex++;
            }

            if (bladeIndex > CmConstants.Population)
            {
                failureMessage = "!!!Failed to find a server blade to run the test.";
                CmTestLog.Failure(failureMessage);
                return false;
            }

            Channel.SetPowerOn(bladeIndex);
            Thread.Sleep(50000);

            BootResponse bBootType;

            int index = 1;

            foreach (BladeBootType testedBootType in Enum.GetValues(typeof (BladeBootType)))
            {
                //Doing the same setting twice to make sure we are handling this properly.
                if (testedBootType.ToString() != BladeBootType.Unknown.ToString())
                {
                    //set to persistent.
                    bBootType = Channel.SetNextBoot(bladeIndex, testedBootType, false, false, 0);
                    if (bBootType.completionCode != CompletionCode.Success)
                    {
                        failureMessage = string.Format("!!!Failed to set non persistant boot type to: {0}",
                            testedBootType);
                        CmTestLog.Failure(failureMessage);
                        testPassed = false;
                    }

                    bBootType = Channel.GetNextBoot(bladeIndex);
                    if (testedBootType.ToString() != bBootType.nextBoot.ToString())
                    {
                        failureMessage = "!!!The Non persistent boot type did not match what it was set to.";
                        CmTestLog.Failure(failureMessage);
                        testPassed = false;
                    }

                    //set to non persistent.
                    bBootType = Channel.SetNextBoot(bladeIndex, testedBootType, false, true, 1);
                    if (bBootType.completionCode != CompletionCode.Success)
                    {
                        failureMessage = string.Format("!!!Failed to set Persistent boot type to: {0}", testedBootType);
                        CmTestLog.Failure(failureMessage);
                        testPassed = false;
                    }

                    //Make sure if no restart happens it keeps its value.
                    bBootType = Channel.GetNextBoot(bladeIndex);
                    if (testedBootType.ToString() != bBootType.nextBoot.ToString())
                    {
                        failureMessage = "!!!The boot type did not match what it was set to.";
                        CmTestLog.Failure(failureMessage);
                        testPassed = false;
                    }
                    //Make sure it loses its value after restart
                    Channel.SetBladeActivePowerCycle(bladeIndex, 0);
                    Thread.Sleep(60000);
                    bBootType = Channel.GetNextBoot(bladeIndex);
                    if (testedBootType.ToString() != BladeBootType.ForcePxe.ToString() &&
                        testedBootType.ToString() != bBootType.nextBoot.ToString())
                    {
                        failureMessage =
                            string.Format(
                                "!!!The boot type did not match what it was set to before power cycle. {0} vs {1} this is round# {2}",
                                testedBootType, bBootType.nextBoot, index);
                        CmTestLog.Failure(failureMessage);
                        testPassed = false;
                    }
                }
            }
            Channel.SetNextBoot(bladeIndex, BladeBootType.NoOverride, false, true, 0);

            CmTestLog.Info("!!!!!!!!! Successfully finished execution of SetNextBoot tests.");

            // end of the test
            CmTestLog.End(testPassed);

            return testPassed;
        }

        /// <summary>
        ///     Basic validation test to set NextBoot test
        /// </summary>
        protected Boolean SetNextBootByAllUserTest()
        {
            CmTestLog.Start();

            bool testPassed = true;
            bool isServer = false;

            int bladeIndex = 1;
            string failureMessage = string.Empty;

            EmptyLocations = GetEmptyLocations();
            JbodLocations = GetJbodLocations();

            CmTestLog.Info("!!!!!!!!! Starting execution of SetNextBootTest.");

            //Make sure blade is reacheable
            //Loop through servers listing and find first Blad to test
            while (!isServer && bladeIndex <= CmConstants.Population)
            {
                if (!JbodLocations.Contains(bladeIndex) && !EmptyLocations.Contains(bladeIndex))
                    isServer = true;
                else
                    bladeIndex++;
            }

            if (bladeIndex > CmConstants.Population)
            {
                failureMessage = "!!!Failed to find a server blade to run the test.";
                CmTestLog.Failure(failureMessage);
                return false;
            }

            Channel.SetPowerOn(bladeIndex);
            Thread.Sleep(50000);

           
            int index = 1;

            foreach (WCSSecurityRole roleId in Enum.GetValues(typeof (WCSSecurityRole)))
            {
                try
                {
                    if (!GetSetNextBoots(testPassed, bladeIndex, failureMessage, index, roleId))
                    {
                        failureMessage = string.Format("Failed to set Persistent boot type by user type: {0}", roleId);
                        CmTestLog.Failure(failureMessage);
                        testPassed = false;
                        break;
                    }
                }
                catch (Exception e)
                {
                    // Check if we got 401 error
                    // Check error is due to permission HTTP 401 unauthorize
                    if (!e.Message.Contains("401"))
                    {
                        // Test failed, http response should contain http 401 error
                        CmTestLog.Failure("We are expecting 401 error, but we received 400 instead.");
                    }
                }
            }


            CmTestLog.Info("!!!!!!!!! Successfully finished execution of SetNextBoot tests.");

            // end of the test
            CmTestLog.End(testPassed);

            return testPassed;
        }

        /// <summary>
        ///     Basic validation test to set NextBoot test
        /// </summary>
        protected Boolean SetNextBootToJbodTest()
        {
            CmTestLog.Start();

            bool testPassed = true;

            bool isJBodServer = false;
            int bladeIndex = 1;
            string failureMessage;

            EmptyLocations = GetEmptyLocations();
            JbodLocations = GetJbodLocations();

            CmTestLog.Info("!!!!!!!!! Starting execution of SetNextBootTest.");

            //Find first Jbod Server
            while (!isJBodServer && bladeIndex <= CmConstants.Population)
            {
                if (JbodLocations.Contains(bladeIndex))
                    isJBodServer = true;
                else
                    bladeIndex++;
            }

            if (bladeIndex > CmConstants.Population)
            {
                failureMessage = "!!!Failed to find a server blade to run the test.";
                CmTestLog.Failure(failureMessage);
                return false;
            }

            Channel.SetPowerOn(bladeIndex);
            Thread.Sleep(50000);

            BootResponse bBootType;

            foreach (BladeBootType testedBootType in Enum.GetValues(typeof (BladeBootType)))
            {
                //Doing the same setting twice to make sure we are handling this properly.
                if (testedBootType.ToString() != BladeBootType.Unknown.ToString())
                {
                    //set to persistent.
                    bBootType = Channel.SetNextBoot(bladeIndex, testedBootType, false, false, 0);
                    if (bBootType.completionCode == CompletionCode.Success &&
                        bBootType.completionCode != CompletionCode.CommandNotValidForBlade)
                    {
                        failureMessage = string.Format("Test failed, successfully set nextboot to JBOD type to: {0}",
                            testedBootType);
                        CmTestLog.Failure(failureMessage);
                        testPassed = false;
                    }

                    bBootType = Channel.GetNextBoot(bladeIndex);
                    if (bBootType.completionCode == CompletionCode.Success &&
                        bBootType.completionCode != CompletionCode.CommandNotValidForBlade)
                    {
                        failureMessage = string.Format("Test failed, successfully GET nextboot to JBOD type to: {0}",
                            testedBootType);
                        CmTestLog.Failure(failureMessage);
                        testPassed = false;
                    }
                }
            }

            // end of the test
            CmTestLog.End(testPassed);

            return testPassed;
        }

        private bool GetSetNextBoots(bool testPassed, int bladeIndex, string failureMessage, int index,
            WCSSecurityRole roleId)
        {
            // Use different user context
            TestChannelContext = ListTestChannelContexts[(int) roleId];

            foreach (BladeBootType testedBootType in Enum.GetValues(typeof (BladeBootType)))
            {
                //Doing the same setting twice to make sure we are handling this properly.
                if (testedBootType.ToString() != BladeBootType.Unknown.ToString())
                {
                    //set to persistent.
                    BootResponse bBootType = TestChannelContext.SetNextBoot(bladeIndex, testedBootType, false, false, 0);
                    if (bBootType.completionCode != CompletionCode.Success)
                    {
                        failureMessage = string.Format("!!!Failed to set non persistant boot type to: {0}",
                            testedBootType);
                        CmTestLog.Failure(failureMessage);
                        testPassed = false;
                    }

                    bBootType = TestChannelContext.GetNextBoot(bladeIndex);
                    if (testedBootType.ToString() != bBootType.nextBoot.ToString())
                    {
                        failureMessage = "!!!The Non persistent boot type did not match what it was set to.";
                        CmTestLog.Failure(failureMessage);
                        testPassed = false;
                    }

                    //set to non persistent.
                    bBootType = TestChannelContext.SetNextBoot(bladeIndex, testedBootType, false, true, 1);
                    if (bBootType.completionCode != CompletionCode.Success)
                    {
                        failureMessage = string.Format("!!!Failed to set Persistent boot type to: {0}", testedBootType);
                        CmTestLog.Failure(failureMessage);
                        testPassed = false;
                    }

                    //Make sure if no restart happens it keeps its value.
                    bBootType = TestChannelContext.GetNextBoot(bladeIndex);
                    if (testedBootType.ToString() != bBootType.nextBoot.ToString())
                    {
                        failureMessage = "!!!The boot type did not match what it was set to.";
                        CmTestLog.Failure(failureMessage);
                        testPassed = false;
                    }
                    //Make sure it loses its value after restart
                    Channel.SetBladeActivePowerCycle(bladeIndex, 0);
                    Thread.Sleep(60000);
                    bBootType = Channel.GetNextBoot(bladeIndex);
                    if (testedBootType.ToString() != BladeBootType.ForcePxe.ToString() &&
                        testedBootType.ToString() != bBootType.nextBoot.ToString())
                    {
                        failureMessage =
                            string.Format(
                                "!!!The boot type did not match what it was set to before power cycle. {0} vs {1} this is round# {2}",
                                testedBootType, bBootType.nextBoot, index);
                        CmTestLog.Failure(failureMessage);
                        testPassed = false;
                    }
                }
            }

            //reset for next test
            Channel.SetNextBoot(bladeIndex, BladeBootType.NoOverride, false, true, 0);


            return testPassed;
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
            CmTestLog.Start();

            // get all server blade locations
            int[] serverLocations;
            if (!GetBladeLocations(blade => blade.bladeType.Equals(CmConstants.ServerBladeType),
                out serverLocations) || serverLocations.Length == 0)
            {
                CmTestLog.Failure("Cannot find any server blade");
                CmTestLog.End(false);
                return false;
            }
            // pick up a random server blade
            int bladeId = serverLocations.RandomOrDefault();
            CmTestLog.Info(string.Format("Pick up a random server blade #{0} for test", bladeId));

            // make sure the blade is powered on
            if (!SetPowerState(PowerState.ON, bladeId))
            {
                CmTestLog.Failure("Cannot power on Blade #" + bladeId);
                CmTestLog.End(false);
                return false;
            }

            bool allPassed = true;

            foreach (BladeBootType bootType in Enum.GetValues(typeof (BladeBootType)))
            {
                if (bootType == BladeBootType.Unknown) continue;

                //// set next boot to persistent
                CmTestLog.Info(string.Format("Trying to set next boot to Type {0} with persistent", bootType));
                BootResponse response = Channel.SetNextBoot(bladeId, bootType, false, true, 0);
                allPassed &= ChassisManagerTestHelper.AreEqual(CompletionCode.Success, response.completionCode,
                    "Set persistent boot type");
                // get next boot and verify
                CmTestLog.Info("Trying to get the next boot");
                response = Channel.GetNextBoot(bladeId);
                allPassed &= ChassisManagerTestHelper.AreEqual(CompletionCode.Success,
                    response.completionCode, "Get next boot value");
                allPassed &= ChassisManagerTestHelper.AreEqual(bootType, response.nextBoot,
                    "The boot type matches what it was set to");

                //// set to non persistent
                CmTestLog.Info(string.Format("Trying to set next boot to Type {0} with non-persistent", bootType));
                response = Channel.SetNextBoot(bladeId, bootType, false, false, 1);
                allPassed &= ChassisManagerTestHelper.AreEqual(CompletionCode.Success, response.completionCode,
                    "Set non-persistent boot type");
                // get next boot and verify
                CmTestLog.Info("Trying to get the next boot");
                response = Channel.GetNextBoot(bladeId);
                allPassed &= ChassisManagerTestHelper.AreEqual(CompletionCode.Success,
                    response.completionCode, "Get next boot value");
                allPassed &= ChassisManagerTestHelper.AreEqual(bootType, response.nextBoot,
                    "The boot type matches what it was set to");
                // make sure it loses its value after restart
                if (!(bootType == BladeBootType.ForcePxe || bootType == BladeBootType.NoOverride))
                {
                    CmTestLog.Info("Trying to restart the blade and get next boot value again");
                    Channel.SetBladeActivePowerCycle(bladeId, 0);
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                    response = Channel.GetNextBoot(bladeId);
                    allPassed &= ChassisManagerTestHelper.IsTrue(bootType != response.nextBoot,
                        "The blade loses its next boot value after power cycle");
                }
            }

            // end of the test
            CmTestLog.End(allPassed);
            return allPassed;
        }
    }
}
