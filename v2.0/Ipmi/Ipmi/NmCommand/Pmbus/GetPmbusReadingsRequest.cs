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
    [NodeManagerMessageRequest(NodeManagerFunctions.Application, NodeManagerCommand.GetPmbusReadings)]
    public class GetPmbusReadingsRequest : NodeManagerRequest
    {
        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        private readonly byte[] manufactureId = { 0x57, 0x01, 0x00 };

        /// <summary>
        /// Device Index
        /// [0:4] = PMBUS-enabled Device Index 
        /// [5:7] = Reserved. Write as 000b
        /// </summary>
        private byte devIndex;

        /// <summary>
        /// History index 
        /// [0:3] = History index. Supported values 0x00 -0x09 � to retrieve 
        ///         history samples and 0x0f to retrieve current samples 
        /// [7:4] � Page number � used only for devices which support pages. 
        /// For others Reserved.
        /// </summary>
        private byte historyIndex;

        /// <summary>
        /// First Register Offset 
        /// [7:4] - Reserved. Write as 00000b.
        /// [3:0] - First Register Offse
        /// </summary>
        private byte firstRegisterOffset;

        /// <summary>
        /// Initializes a new instance of the GetPmbusReadingsRequest (history) class.
        /// </summary>
        internal GetPmbusReadingsRequest(byte deviceIndex, byte historyIndex, byte firstRegisterOffset)
        {
            // [0:4] = PMBUS-enabled Device Index 
            this.devIndex = (byte)(deviceIndex & 0x1F);

            // History index. Supported values 0x00 -0x09 � to retrieve 
            // history samples and 0x0f to retrieve current samples 
            this.historyIndex = (byte)(historyIndex & 0x0F);

            // [7:4] - Reserved. Write as 00000b.
            // [3:0] - First Register Offset
            this.firstRegisterOffset = (byte)(firstRegisterOffset & 0x0F);
        }

        /// <summary>
        /// Initializes a new instance of the GetPmbusReadingsRequest (current samples) class.
        /// </summary>
        internal GetPmbusReadingsRequest(byte deviceIndex, byte firstRegisterOffset)
        {
            // [0:4] = PMBUS-enabled Device Index 
            this.devIndex = (byte)(deviceIndex & 0x1F);

            // History index. Supported values 0x00 -0x09 � to retrieve 
            // history samples and 0x0f to retrieve current samples 
            this.historyIndex = 0x0F;

            // [7:4] - Reserved. Write as 00000b.
            // [3:0] - First Register Offset
            this.firstRegisterOffset = (byte)(firstRegisterOffset & 0x0F);
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
        /// Device Index
        /// </summary>
        [NodeManagerMessageData(3)]
        public byte DeviceIndex
        {
            get { return this.devIndex; }
        }

        /// <summary>
        /// History index
        /// </summary>
        [NodeManagerMessageData(4)]
        public byte HistoryIndex
        {
            get { return this.historyIndex; }
        }

        /// <summary>
        ///  First Register Offset 
        /// </summary>
        [NodeManagerMessageData(5)]
        public byte FirstRegisterOffset
        {
            get { return this.firstRegisterOffset; }
        }


    }
}
