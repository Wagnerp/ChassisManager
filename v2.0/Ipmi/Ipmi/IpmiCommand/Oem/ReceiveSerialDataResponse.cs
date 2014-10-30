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
    /// Represents the IPMI 'Receive Serial Data' application response message.
    /// </summary>
    [IpmiMessageResponse(IpmiFunctions.OemGroup, IpmiCommand.ReceiveSerialData)]
    internal class ReceiveSerialDataResponse : IpmiResponse
    {
        ushort payloadLenght;

        byte[] payload = { };

        /// <summary>
        /// Payload Lenght
        /// </summary>       
        [IpmiMessageData(0)]
        public ushort PayloadLenght
        {
            get { return this.payloadLenght; }
            set { this.payloadLenght = value; }
        }

        /// <summary>
        /// Payload
        /// </summary>       
        [IpmiMessageData(2)]
        public byte[] Payload
        {
            get { return this.payload; }
            set { this.payload = value; }
        }
    }
}
