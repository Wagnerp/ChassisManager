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
    /// Represents the IPMI 'Get Default Power Limit' application response message.
    /// </summary>
    [IpmiMessageResponse(IpmiFunctions.OemGroup, IpmiCommand.GetDefaultPowerLimit)]
    internal class GetDefaultPowerLimitResponse : IpmiResponse
    {
        /// <summary>
        /// Default Power Cap in Watts
        /// </summary>
        private ushort dpc;

        /// <summary>
        /// Delay interval in milliseconds between activating
        /// the DPC and deasserting the PROCHOT
        /// </summary>
        private ushort delay;

        /// <summary>
        /// Default Power Cap enabled/disabled
        /// </summary>
        private byte dpcEnabled;

        /// <summary>
        /// Default Power Cap in Watts
        /// </summary>       
        [IpmiMessageData(0)]
        public ushort DefaultPowerCap
        {
            get { return this.dpc; }
            set { this.dpc = value; }
        }

        /// <summary>
        ///  Time in milliseconds after applying DPC to 
        ///  wait before deasserting the PROCHOT
        /// </summary>    
        [IpmiMessageData(2)]
        public ushort WaitTime
        {
            get { return this.delay; }
            set { this.delay = value; }
        }

        /// <summary>
        ///  Disable/Enable Default Power Cap on PSU_Alert GPI.
        /// </summary>   
        [IpmiMessageData(4)]
        public byte DefaultCapEnabled
        {
            get { return this.dpcEnabled; }
            set { this.dpcEnabled = value; }
        }
    }
}
