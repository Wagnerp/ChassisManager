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
    using Microsoft.GFS.WCS.Contracts;

    internal class SerialConsoleCommands : CommandBase
    {
        internal SerialConsoleCommands(IChassisManager channel) : base(channel)
        {
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
            CmTestLog.Start();

            // get all server blade locations
            int[] serverLocations;
            if (!this.GetBladeLocations(blade => blade.bladeType.Equals(CmConstants.ServerBladeType),
                out serverLocations) || serverLocations.Length == 0)
            {
                CmTestLog.Failure("Cannot find any server blade");
                CmTestLog.End(false);
                return false;
            }
            // pick up a random server blade
            var bladeId = serverLocations.RandomOrDefault();
            CmTestLog.Info(string.Format("Pick up a random server blade #{0} for test", bladeId));

            // make sure the blade is powered on
            if (!this.SetPowerState(PowerState.ON, bladeId))
            {
                CmTestLog.Failure(string.Format("Cannot power on Blade #{0}", bladeId));
                CmTestLog.End(false);
                return false;
            }

            bool testPassed = true;

            // kill any existing serial session first
            CmTestLog.Info(string.Format("Kill all existing serial sessions on Blade #{0}", bladeId));
            var killSession = this.Channel.StopBladeSerialSession(bladeId, null, true);
            if (!(CompletionCode.NoActiveSerialSession == killSession.completionCode ||
                  CompletionCode.Success == killSession.completionCode))
            {
                CmTestLog.Failure(killSession.statusDescription);
                CmTestLog.End(false);
                return false;
            }

            // start blade serial session 
            CmTestLog.Info(string.Format("Trying to start a serial session to Blade #{0}", bladeId));
            var startResponse = this.Channel.StartBladeSerialSession(bladeId, CmConstants.SerialTimeoutSeconds);
            testPassed &= ChassisManagerTestHelper.AreEqual(CompletionCode.Success, startResponse.completionCode,
                "Serial session started");
            var sessionToken = startResponse.serialSessionToken;
            testPassed &= ChassisManagerTestHelper.IsTrue(!string.IsNullOrEmpty(sessionToken),
                "Serial session token received");

            // stop blade serial session
            CmTestLog.Info(string.Format("Trying to stop the serial session to Blade #{0}", bladeId));
            var stopResponse = this.Channel.StopBladeSerialSession(bladeId, sessionToken);
            testPassed &= ChassisManagerTestHelper.AreEqual(CompletionCode.Success, stopResponse.completionCode,
                "Serial session stopped");

            // test ended
            CmTestLog.End(testPassed);
            return testPassed;
        }
    }
}
