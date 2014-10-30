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
    /// Represents the IPMI 'Get Channel Cipher Suite Response' application response message.
    /// </summary>
    [IpmiMessageRequest(IpmiFunctions.Application, IpmiCommand.GetChannelCipherSuites)]
    internal class GetChannelCipherSuitesResponse : IpmiResponse
    {
        /// <summary>
        /// Channel number.
        /// </summary>
        private byte channelNumber;

        /// <summary>
        /// Record Data
        /// </summary>
        private byte[] recordData;

        /// <summary>
        /// Gets and sets the Channel number.
        /// </summary>
        /// <value>Channel number.</value>
        [IpmiMessageData(0)]
        public byte ChannelNumber
        {
            get { return this.channelNumber; }
            set { this.channelNumber = value; }
        }

        /// <summary>
        /// Record Data.
        /// </summary>
        /// <value>16 bytes of cipher record.</value>
        [IpmiMessageData(1)]
        public byte[] RecordData
        {
            get { return this.recordData; }
            set { this.recordData = value; }
        }
    }
}
