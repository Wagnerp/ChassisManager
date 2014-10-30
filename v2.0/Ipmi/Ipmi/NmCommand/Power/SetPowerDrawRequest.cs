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
    /// Represents the Node Manager 'Set Power Draw Range' request message.
    /// </summary>
    [NodeManagerMessageRequest(NodeManagerFunctions.NodeManager, NodeManagerCommand.SetPowerDrawRange)]
    public class SetPowerDrawRangeRequest : NodeManagerRequest
    {
        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        private readonly byte[] manufactureId = { 0x57, 0x01, 0x00 };

        /// <summary>
        /// Domain Id
        /// [0:3] Domain Id
        /// [4:7] Reserved. Write as 00.
        /// </summary>
        private byte domainId;

        /// <summary>
        /// Minimum power draw in watts.
        /// </summary>
        private ushort minPower;

        /// <summary>
        /// Maximum power draw in watts.
        /// </summary>
        private ushort maxPower; 

        /// <summary>
        /// Initializes a new instance of the SetPowerDrawRangeRequest class.
        /// </summary>
        internal SetPowerDrawRangeRequest(NodeManagerDomainId domainId, ushort minPower, ushort maxPower)
        {
            this.domainId = (byte)domainId;
            this.minPower = minPower;
            this.maxPower = maxPower;

        }

        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        [NodeManagerMessageData(0,3)]
        public byte[] ManufactureId
        {
            get { return this.manufactureId; }
        }

        /// <summary>
        /// Domain Id
        /// </summary>
        [NodeManagerMessageData(3)]
        public byte DomainId
        {
            get { return this.domainId; }
        }

        /// <summary>
        /// Minimum Power Draw
        /// </summary>
        [NodeManagerMessageData(4)]
        public ushort MinimumPower
        {
            get { return this.minPower; }
        }

        /// <summary>
        /// Maximum Power Draw
        /// </summary>
        [NodeManagerMessageData(6)]
        public ushort MaximumPower
        {
            get { return this.maxPower; }
        }
    }
}
