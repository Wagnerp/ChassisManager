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
    /// Represents the IPMI 'Enable Message Channel Receive Message' application response message.
    /// </summary>
    [IpmiMessageRequest(IpmiFunctions.Application, IpmiCommand.EnableMessageChannelReceive)]
    internal class EnableMessageChannelReceiveResponse : IpmiResponse
    {

        /// <summary>
        /// Channel to send the message.
        /// </summary>
        private byte channel;

        /// <summary>
        /// Channel Enable/Disable State.
        /// </summary>
        private byte channelState;

        /// <summary>
        /// Channel to send the request message.
        /// </summary>
        [IpmiMessageData(0)]
        public byte Channel
        {
            get { return this.channel; }
            set { this.channel = (byte)(value & 0x0f); }
        }

        /// <summary>
        /// Channel State
        /// </summary>
        [IpmiMessageData(1)]
        public byte ChannelState
        {
            get { return this.channelState; }
            set { this.channelState = (byte)(value & 0x01); }
        }

    }
}
