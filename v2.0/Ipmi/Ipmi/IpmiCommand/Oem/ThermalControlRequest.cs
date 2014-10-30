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
    /// Represents the IPMI 'Thermal Control' OEM request message.
    /// </summary>
    [IpmiMessageRequest(IpmiFunctions.OemGroup, IpmiCommand.ThermalControl)]
    internal class ThermalControlRequest : IpmiRequest
    {

        /// <summary>
        /// Set/Get Thermal Control Feature
        /// </summary>
        private readonly byte function;

        /// <summary>
        /// Initialize instance of the class.
        /// </summary>
        /// <param name="function">Get/Enable/Disable</param>
        internal ThermalControlRequest(byte function)
        {
            this.function = (byte)(function & 0x03);
        }

        /// <summary>
        /// Thermal Control Function
        /// 0 - Get Status of Thermal Control feature in the BMC
        /// 1 – Set Thermal Control Feature enabled 
        /// 2 – Set Thermal Control Feature disabled
        /// </summary>       
        [IpmiMessageData(0)]
        public byte Function
        {
            get { return this.function; }
        }


    }
}
