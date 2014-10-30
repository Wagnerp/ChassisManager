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
    /// Represents the IPMI 'Get Message' application response message.
    /// </summary>
    [IpmiMessageRequest(IpmiFunctions.Application, IpmiCommand.GetMessage)]
    internal class GetMessageResponse : IpmiResponse
    {

        /// <summary>
        /// Channel Number.
        /// </summary>
        private byte channel;

        /// <summary>
        /// Privilege Level.
        /// </summary>
        private byte privilegeLevel;

        /// <summary>
        /// Response message payload.
        /// </summary>
        private byte[] messageData;

        /// <summary>
        /// Channel Number
        /// </summary>
        [IpmiMessageData(0)]
        public byte Channel
        {
            get { return this.channel; }
            set { this.channel = (byte)(value & 0x0f);
                  this.privilegeLevel = (byte)((value >> 4) & 0x0f);
                }
        }

        /// <summary>
        /// Response message payload.
        /// </summary>
        [IpmiMessageData(1)]
        public byte[] MessageData
        {
            get { return this.messageData; }
            set { this.messageData = value; }
        }

        /// <summary>
        /// Privilege Level
        /// </summary>
        public byte PrivilegeLevel
        {
            get { return this.privilegeLevel; }
        }
    }
}
