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

    using System;

    /// <summary>
    /// Represents the IPMI 'Get Psu Alert' application response message.
    /// </summary>
    [IpmiMessageResponse(IpmiFunctions.OemGroup, IpmiCommand.GetPsuAlert)]
    internal class GetPsuAlertResponse : IpmiResponse
    {

        byte alertStatus;

        /// <summary>
        /// Alert Status
        /// </summary>       
        [IpmiMessageData(0)]
        public byte AlertStatus
        {
            get { return this.alertStatus; }
            set { this.alertStatus = value; }
        }

        /// <summary>
        /// PSU_Alert BMC GPI Status
        /// [7:6] BLADE_EN2 to BMC GPI
        /// </summary>
        public bool PsuAlertGpi
        {
            get { 
                    if((alertStatus & 0x40) == 0x40)
                        return true;
                    else 
                        return false;
                }
        }

        /// <summary>
        /// Auto PROCHOT on switch GPI
        /// [5:4] Auto FAST_PROCHOT Enabled
        /// </summary>
        public bool AutoProchotEnabled
        {
            get
            {
                if ((alertStatus & 0x10) == 0x10)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// BMC PROCHOT on switch GPI
        /// [3:0] BMC FAST_PROCHOT Enabled
        /// </summary>
        public bool BmcProchotEnabled
        {
            get
            {
                if ((alertStatus & 0x01) == 0x01)
                    return true;
                else
                    return false;
            }
        }



    }
}
