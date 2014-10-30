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
    using Microsoft.GFS.WCS.Contracts;
    
    internal class AcSocketPowerCommands : CommandBase
    {
        internal double BladePowerLowerLimit = 120;
        internal double BladePowerUpperLimit = 1000;

        internal AcSocketPowerCommands(IChassisManager channel) : base(channel)
        {
        }

        internal AcSocketPowerCommands(IChassisManager channel, Dictionary<int, IChassisManager> testChannelContexts) : base(channel, testChannelContexts)
        {
        }

        /// <summary>
        ///     Basic functional test for ACSocketPowerCommand
        /// </summary>
        /// <returns></returns>
        public bool AcSocketPowerCommandBasicValidationTest()
        {
            return (this.AcSocketPowerTest() &&
                    this.AcSocketPowerGetByAllUserTest() &&
                    this.AcSocketPowerSetByAdminOperatorTest());
        }

        /// <summary>
        ///     Basic functional validation test for AC SocketPower
        /// </summary>
        /// <returns></returns>
        protected bool AcSocketPowerTest()
        {
            CmTestLog.Start();
            bool testPassed = true;

            string failureMessage;

            ChassisResponse acSocketResponse = null;
            ACSocketStateResponse acSocketPower = null;
            uint NumACSocket = CmConstants.NumPowerSwitches;

            Console.WriteLine("!!!!!!!!! Started execution of ACSocketPowerTest.");

            for (uint testedAcSocket = 1; testedAcSocket <= NumACSocket; testedAcSocket++)
            {
                acSocketResponse = this.Channel.SetACSocketPowerStateOff(testedAcSocket);
                if (acSocketResponse.completionCode != CompletionCode.Success)
                {
                    failureMessage = string.Format("!!!Failed to power off from unknown state for AC socket#{0}", testedAcSocket);
                    CmTestLog.Failure(failureMessage);
                    testPassed = false;
                }

                acSocketPower = this.Channel.GetACSocketPowerState(testedAcSocket);
                if (acSocketPower.powerState != PowerState.OFF)
                {
                    failureMessage = string.Format("!!!Failed to get power state for AC socket#{0}", testedAcSocket);
                    CmTestLog.Failure(failureMessage);
                    testPassed = false;
                }

                acSocketResponse = this.Channel.SetACSocketPowerStateOff(testedAcSocket);
                if (acSocketResponse.completionCode != CompletionCode.Success)
                {
                    failureMessage = string.Format("!!!Failed to power off AC socket when it is already off for socket#{0}", testedAcSocket);
                    CmTestLog.Failure(failureMessage);
                    testPassed = false;
                }

                acSocketPower = this.Channel.GetACSocketPowerState(testedAcSocket);
                if (acSocketPower.powerState != PowerState.OFF)
                {
                    failureMessage = string.Format("!!!Failed to get power state for AC socket#{0}", testedAcSocket);
                    CmTestLog.Failure(failureMessage);
                    testPassed = false;
                }

                acSocketResponse = this.Channel.SetACSocketPowerStateOn(testedAcSocket);
                if (acSocketResponse.completionCode != CompletionCode.Success)
                {
                    failureMessage = string.Format("!!!Failed to power ON AC socket#{0}", testedAcSocket);
                    CmTestLog.Failure(failureMessage);
                    testPassed = false;
                }

                acSocketPower = this.Channel.GetACSocketPowerState(testedAcSocket);
                if (acSocketPower.powerState != PowerState.ON)
                {
                    failureMessage = string.Format("!!!Failed to get power state for AC socket#{0}", testedAcSocket);
                    CmTestLog.Failure(failureMessage);
                    testPassed = false;
                }

                acSocketResponse = this.Channel.SetACSocketPowerStateOn(testedAcSocket);
                if (acSocketResponse.completionCode != CompletionCode.Success)
                {
                    failureMessage = string.Format("!!!Failed to power ON AC socket when it is already ON for AC Socket#{0}", testedAcSocket);
                    CmTestLog.Failure(failureMessage);
                    testPassed = false;
                }

                acSocketResponse = this.Channel.SetACSocketPowerStateOff(testedAcSocket);
                if (acSocketResponse.completionCode != CompletionCode.Success)
                {
                    failureMessage = string.Format("!!!Failed to power off AC socket from ON state for AC Socket#{0}", testedAcSocket);
                    CmTestLog.Failure(failureMessage);
                    testPassed = false;
                }

                acSocketPower = this.Channel.GetACSocketPowerState(testedAcSocket);
                if (acSocketPower.powerState != PowerState.OFF)
                {
                    failureMessage = string.Format("!!!Failed to get power state for AC socket#{0}", testedAcSocket);
                    CmTestLog.Failure(failureMessage);
                    testPassed = false;
                }

                acSocketPower = this.Channel.GetACSocketPowerState(testedAcSocket);
                if (acSocketPower.powerState != PowerState.OFF)
                {
                    failureMessage = string.Format("!!!Failed to get power state for AC socket#{0}", testedAcSocket);
                    CmTestLog.Failure(failureMessage);
                    testPassed = false;
                }
            }

            //Test for invalid parameters
            acSocketResponse = this.Channel.SetACSocketPowerStateOn(0);
            if (acSocketResponse.completionCode != CompletionCode.ParameterOutOfRange)
            {
                failureMessage = string.Format("!!!Failed During SetACSocketPowerStateOn(0), response is: {0}", acSocketResponse.completionCode);
                CmTestLog.Failure(failureMessage);
                testPassed = false;
            }

            acSocketResponse = this.Channel.SetACSocketPowerStateOn(9999);
            if (acSocketResponse.completionCode != CompletionCode.ParameterOutOfRange)
            {
                failureMessage = string.Format("!!!Failed During SetACSocketPowerStateOn(0), response is: {0}", acSocketResponse.completionCode);
                CmTestLog.Failure(failureMessage);
                testPassed = false;
            }

            acSocketResponse = this.Channel.SetACSocketPowerStateOn(4);
            if (acSocketResponse.completionCode != CompletionCode.ParameterOutOfRange)
            {
                failureMessage = string.Format("!!!Failed During SetACSocketPowerStateOn(0), response is: {0}", acSocketResponse.completionCode);
                CmTestLog.Failure(failureMessage);
                testPassed = false;
            }

            Console.WriteLine("\n++++++++++++++++++++++++++++++++");
            failureMessage = "!!!!!!!!! Successfully finished execution of ACSocketPowerTests.";
            Console.WriteLine(failureMessage);

            CmTestLog.End(testPassed);
            return testPassed;
        }

        /// <summary>
        ///     GetACSocketPowerState: Verify that all users can execute the command
        /// </summary>
        /// <returns></returns>
        protected bool AcSocketPowerGetByAllUserTest()
        {
            CmTestLog.Start();
            bool testPassed = true;

            string failureMessage;

            ChassisResponse acSocketResponse = null;
            ACSocketStateResponse acSocketPower = null;
            uint numAcSocket = CmConstants.NumPowerSwitches;

            Console.WriteLine("!!!!!!!!! Started execution of ACSocketPowerTest.");

            foreach (WCSSecurityRole roleId in Enum.GetValues(typeof (WCSSecurityRole)))
            {
                //Change test connection to different role
                this.TestChannelContext = this.ListTestChannelContexts[(int)roleId];

                for (uint testedAcSocket = 1; testedAcSocket <= numAcSocket; testedAcSocket++)
                {
                    // Turn off ACSocket Power state
                    acSocketResponse = this.Channel.SetACSocketPowerStateOff(testedAcSocket);

                    if (acSocketResponse.completionCode == CompletionCode.Success)
                    {
                        // Check to see if socket power state is return correctly
                        acSocketPower = this.TestChannelContext.GetACSocketPowerState(testedAcSocket);
                        if (acSocketPower.powerState != PowerState.OFF)
                        {
                            failureMessage = string.Format("!!!Failed to get power state for AC socket#{0}", testedAcSocket);
                            CmTestLog.Failure(failureMessage);
                            testPassed = false;
                        }
                    }

                    // Turn on ACSocket Power state
                    acSocketResponse = this.Channel.SetACSocketPowerStateOn(testedAcSocket);

                    if (acSocketResponse.completionCode == CompletionCode.Success)
                    {
                        // Check to see if socket power state is return correctly
                        acSocketPower = this.TestChannelContext.GetACSocketPowerState(testedAcSocket);
                        if (acSocketPower.powerState != PowerState.ON)
                        {
                            failureMessage = string.Format("!!!Failed to get power state for AC socket#{0}", testedAcSocket);
                            CmTestLog.Failure(failureMessage);
                            testPassed = false;
                        }
                    }
                }
            }

            failureMessage = "!!!!!!!!! Successfully finished execution of ACSocketPowerTests.";
            Console.WriteLine(failureMessage);

            CmTestLog.End(testPassed);
            return testPassed;
        }

        /// <summary>
        ///     SetACSocketPowerStateOff: Verify that only Operator and Admin can execute the command
        /// </summary>
        /// <returns></returns>
        protected bool AcSocketPowerSetByAdminOperatorTest()
        {
            CmTestLog.Start();
            bool testPassed = true;

            string failureMessage = string.Empty;
            const uint NumACSocket = CmConstants.NumPowerSwitches;

            foreach (WCSSecurityRole roleId in Enum.GetValues(typeof (WCSSecurityRole)))
            {
                try
                {
                    this.AcSocketSetGetValidation(ref testPassed, ref failureMessage, NumACSocket, roleId);
                }
                catch (Exception e)
                {
                    // Check error is due to permission HTTP 401 unauthorize
                    if (!e.Message.Contains("401") && roleId == WCSSecurityRole.WcsCmUser)
                    {
                        // Test failed, http response should contain http 401 error
                        CmTestLog.Failure("We are expecting 401 error, but we received 400 instead.");
                    }
                }
            }

            Console.WriteLine("\n++++++++++++++++++++++++++++++++");
            failureMessage = "!!!!!!!!! Successfully finished execution of ACSocketPowerTests.";
            Console.WriteLine(failureMessage);

            CmTestLog.End(testPassed);
            return testPassed;
        }

        /// <summary>
        ///     SetACSocketPowerStateOff: Verify that only Operator and Admin can execute the command
        /// </summary>
        /// <param name="testPassed"></param>
        /// <param name="failureMessage"></param>
        /// <param name="numAcSocket"></param>
        /// <param name="roleId"></param>
        private void AcSocketSetGetValidation(ref bool testPassed, ref string failureMessage, uint numAcSocket,
            WCSSecurityRole roleId)
        {
            ChassisResponse acSocketResponse = null;
            ACSocketStateResponse acSocketPower = null;

            // Use different user context
            this.TestChannelContext = this.ListTestChannelContexts[(int)roleId];

            for (uint testedAcSocket = 1; testedAcSocket <= numAcSocket; testedAcSocket++)
            {
                // Turn On ACSocket
                acSocketResponse = this.TestChannelContext.SetACSocketPowerStateOn(testedAcSocket);
                if (acSocketResponse.completionCode != CompletionCode.Success &&
                    (roleId == WCSSecurityRole.WcsCmAdmin || roleId == WCSSecurityRole.WcsCmOperator))
                {
                    failureMessage =
                        string.Format("!!!Failed to power ON AC socket when it is already ON for AC Socket# {0}",
                            testedAcSocket);
                    CmTestLog.Failure(failureMessage);
                    testPassed = false;
                }
                else if (acSocketResponse.completionCode == CompletionCode.Success &&
                         (roleId == WCSSecurityRole.WcsCmUser))
                {
                    failureMessage =
                        string.Format("User is not allow to called out to SetACSocketPowerStateOn {0} Socket# {1}",
                            roleId, testedAcSocket);
                    CmTestLog.Failure(failureMessage);
                    testPassed = false;
                }
                else
                {
                    // Verify power state
                    acSocketPower = this.Channel.GetACSocketPowerState(testedAcSocket);
                    if (acSocketPower.powerState != PowerState.ON)
                    {
                        failureMessage =
                            string.Format(
                                "!!!Failed to power ON AC socket when it is already ON for AC Socket# {0}",
                                testedAcSocket);
                        CmTestLog.Failure(failureMessage);
                        testPassed = false;
                    }
                }

                // Turn  off ACSocket 
                acSocketResponse = this.TestChannelContext.SetACSocketPowerStateOff(testedAcSocket);
                if (acSocketResponse.completionCode != CompletionCode.Success &&
                    (roleId == WCSSecurityRole.WcsCmAdmin || roleId == WCSSecurityRole.WcsCmOperator))
                {
                    failureMessage =
                        string.Format("!!!Failed to power ON AC socket when it is already ON for AC Socket# {0}",
                            testedAcSocket);
                    CmTestLog.Failure(failureMessage);
                    testPassed = false;
                }
                else if (acSocketResponse.completionCode == CompletionCode.Success && roleId == WCSSecurityRole.WcsCmUser)
                {
                    failureMessage =
                        string.Format(
                            "User is not allow to called out to SetACSocketPowerStateOff {0} Socket# {1}", roleId,
                            testedAcSocket);
                    CmTestLog.Failure(failureMessage);
                    testPassed = false;
                }
                else
                {
                    // Verify power state
                    acSocketPower = this.Channel.GetACSocketPowerState(testedAcSocket);
                    if (acSocketPower.powerState != PowerState.OFF)
                    {
                        failureMessage =
                            string.Format(
                                "!!!Failed to power ON AC socket when it is already ON for AC Socket# {0}",
                                testedAcSocket);
                        CmTestLog.Failure(failureMessage);
                        testPassed = false;
                    }
                }
            }// end of for loop
        }
    }
}
