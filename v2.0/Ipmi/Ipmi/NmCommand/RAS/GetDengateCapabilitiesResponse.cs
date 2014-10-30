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
    /// Represents the Node Manager 'Get Dengate Capabilities' response message.
    /// </summary>
    [NodeManagerMessageResponse(NodeManagerFunctions.Application, NodeManagerCommand.GetDengateCapabilities)]
    public class GetDengateCapabilitiesResponse : NodeManagerResponse
    {
        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        private byte[] manufactureId;


        /// <summary>
        /// Dengate Version (BCD)
        /// </summary>
        private byte version;

        /// <summary>
        /// Capabilities (0:31 bits)
        /// </summary>
        private byte[] capabilities;

        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        [NodeManagerMessageData(0, 3)]
        public byte[] ManufactureId
        {
            get { return this.manufactureId; }
            set { this.manufactureId = value; }
        }

        /// <summary>
        /// Dengate Version (BCD)
        /// </summary>
        [NodeManagerMessageData(3)]
        public byte Version
        {
            get { return this.version; }
            set { this.version = value; }
        }

        /// <summary>
        /// Capabilities (0:31 bits)
        /// </summary>
        [NodeManagerMessageData(4)]
        public byte[] Capabilities
        {
            get { return this.capabilities; }
            set { this.capabilities = value; }
        }

    }
}
