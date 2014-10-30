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
    /// Represents the IPMI 'Set Default Power Limit' OEM request message.
    /// </summary>
    [IpmiMessageRequest(IpmiFunctions.OemGroup, IpmiCommand.SetDefaultPowerLimit)]
    internal class SetDefaultPowerLimitRequest : IpmiRequest
    {
        
        /// <summary>
        /// Default power limit in Watts
        /// </summary>
        private readonly ushort dpc;

        /// <summary>
        /// Delay time after applying DPC before deasserting
        /// Fast PROCHOT.  Default 100ms
        /// </summary>
        private readonly ushort delay;

        /// <summary>
        /// Enable/Disable DPC when PSU_Alert GPI is asserted
        /// </summary>
        private readonly byte enableDpc;


        /// <summary>
        /// Initialize instance of the class.
        /// </summary>
        /// <param name="dpc">Default Power Cap</param>
        /// <param name="waitTime">Delay after DPC before removing PROCHOT</param>
        /// <param name="enableCapping">Removes Default Power Cap</param>
        internal SetDefaultPowerLimitRequest(ushort defaultPowerCap, ushort waitTime, bool enableCapping)
        {
            this.dpc = defaultPowerCap;

            this.delay = waitTime;

            if (enableCapping)
                enableDpc = 0x01;
        }

        /// <summary>
        /// Default Power Cap
        /// </summary>       
        [IpmiMessageData(0)]
        public ushort DefaultPowerCap
        {
            get { return this.dpc; }

        }

        /// <summary>
        ///  Time in milliseconds after applying DPC to 
        ///  wait before deasserting the PROCHOT
        /// </summary>       
        [IpmiMessageData(2)]
        public ushort WaitTime
        {
            get { return this.delay; }

        }

        /// <summary>
        ///  Disable/Enable Default Power Cap on PSU_Alert GPI.
        /// </summary>       
        [IpmiMessageData(4)]
        public byte DefaultCapEnabled
        {
            get { return this.enableDpc; }

        }

    }
}
