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
    /// Represents the Node Manager 'Get Pmbus Readings' request message.
    /// </summary>
    [NodeManagerMessageRequest(NodeManagerFunctions.Application, NodeManagerCommand.GetAggPmbusReadings)]
    public class GetAggPmbusReadingsRequest : NodeManagerRequest
    {
        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        private readonly byte[] manufactureId = { 0x57, 0x01, 0x00 };

        /// <summary>
        /// Register Offset 
        /// [0:3] = Offset of the register 
        /// [4:7] = Page number � used only for devices which support pages. 
        ///         For others Reserved
        /// </summary>
        private byte registerOffset;

        /// <summary>
        /// Device Indexes
        /// Each byte should contain Device Index
        /// </summary>
        private byte[] deviceIndexes;

        /// <summary>
        /// Initializes a new instance of the GetAggPmbusReadingsRequest (history) class.
        /// </summary>
        internal GetAggPmbusReadingsRequest(byte registerOffset, byte pageNumber, byte[] deviceIndexes)
        {

            // Register Offset 
            // [0:3] = Offset of the register 
            this.registerOffset = (byte)(registerOffset & 0x0F);

            /// Register Offset 
            /// [4:7] = Page number � used only for devices which support pages. 
            this.registerOffset = (byte)(this.registerOffset | (byte)((pageNumber & 0x0F) << 4));

            // Each byte should contain Device Index
            if (deviceIndexes != null)
                this.deviceIndexes = deviceIndexes;
            else
                this.deviceIndexes = new byte[0];
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
        /// Register Offset 
        /// [0:3] = Offset of the register 
        /// [4:7] = Page number � used only for devices which support pages. 
        ///         For others Reserved
        /// </summary>
        [NodeManagerMessageData(3)]
        public byte RegisterOffset
        {
            get { return this.registerOffset; }
        }

        /// <summary>
        /// Device Indexes
        /// Each byte should contain Device Index
        /// </summary>
        [NodeManagerMessageData(4)]
        public byte[] DeviceIndexes
        {
            get { return this.deviceIndexes; }
        }
    }
}
