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

namespace Microsoft.GFS.WCS.ChassisManager.Ipmi
{

    /// <summary>
    /// Represents the IPMI 'Set Psu Alert' OEM request message.
    /// </summary>
    [IpmiMessageRequest(IpmiFunctions.OemGroup, IpmiCommand.SetPsuAlert)]
    internal class SetPsuAlertRequest : IpmiRequest
    {
        /// <summary>
        /// Drives GPIOF6 for simulation
        /// 00h = Deassert PSU_ALERT GPI (BLADE_EN2 to BMC)
        /// 01h = Assert PSU_ALERT GPI (BLADE_EN2 to BMC)
        /// </summary>
        private readonly byte assert;

        /// <summary>
        /// Initialize instance of the class.  This command is for testing BMC 
        /// functionality when the PSU_ALERT GPI is asserted
        /// </summary>
        /// <param name="Assert PSU_ALERT">Enables/Disables PSU_Alert</param>
        internal SetPsuAlertRequest(bool enablePsuAlert)
        {
            if (enablePsuAlert)
                assert = 0x01;
        }

        /// <summary>
        /// Assert/Deassert PSU_ALERT GPI (BLADE_EN2 to BMC)
        /// </summary>       
        [IpmiMessageData(0)]
        public byte PsuAlertEnabled
        {
            get { return this.assert; }

        }
    }
}
