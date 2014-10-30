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

namespace Microsoft.GFS.WCS.ChassisManager.Ipmi.NodeManager
{

    /// <summary>
    /// Represents the Node Manager 'Get CUPS Capabilities' response message.
    /// </summary>
    [NodeManagerMessageResponse(NodeManagerFunctions.NodeManager, NodeManagerCommand.GetCupsCapabilities)]
    public class GetCupsCapabilitiesResponse : NodeManagerResponse
    {
        /// <summary>
        /// Intel Manufacturer Id
        /// </summary>
        private byte[] manufactureId;

        /// <summary>
        /// Capabilities (Bit 0)
        /// 1 - CUPS capability is supported
        /// 0 - CUPS capability is not supported
        /// </summary>
        private byte capabilities;

        /// <summary>
        /// CUPS Mode
        /// 0x01 - Pass-thru Mode
        /// 0x02 - Advanced Mode
        /// 0x03 - CUPS Policies available
        /// 0x04 - 0xFF - Reserved
        /// </summary>
        private byte mode;

        /// <summary>
        /// CUPS
        /// </summary>
        private byte cups;

        /// <summary>
        /// Reserved byte
        /// </summary>
        private byte reserved;

        /// <summary>
        /// Intel Manufacturer Id
        /// </summary>
        [NodeManagerMessageData(0, 3)]
        public byte[] ManufactureId
        {
            get { return this.manufactureId; }
            set { this.manufactureId = value; }
        }

        /// <summary>
        /// Capabilities (Bit 0)
        /// </summary>
        [NodeManagerMessageData(3)]
        public byte Capabilities
        {
            get { return this.capabilities; }
            set { this.capabilities = value; }
        }

        /// <summary>
        /// CUPS Mode
        /// </summary>
        [NodeManagerMessageData(4)]
        public byte Mode
        {
            get { return this.mode; }
            set { this.mode = value; }
        }

        /// <summary>
        /// CUPS
        /// </summary>
        [NodeManagerMessageData(5)]
        public byte Cups
        {
            get { return this.cups; }
            set { this.cups = value; }
        }

        /// <summary>
        /// Reserved byte
        /// </summary>
        [NodeManagerMessageData(6)]
        public byte Reserved
        {
            get { return this.reserved; }
            set { this.reserved = value; }
        }
    }
}
