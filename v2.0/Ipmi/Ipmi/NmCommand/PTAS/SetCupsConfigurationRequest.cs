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
    /// Represents the Node Manager 'Set Cups Configuration' request message.
    /// </summary>
    [NodeManagerMessageRequest(NodeManagerFunctions.NodeManager, NodeManagerCommand.SetCupsConfiguration)]
    public class SetCupsConfigurationRequest : NodeManagerRequest
    {
        /// <summary>
        /// Intel Manufacturer Id
        /// </summary>
        private readonly byte[] manufactureId = { 0x57, 0x01, 0x00 };

        /// <summary>
        /// Enable CUPS
        /// [0] 0 - Disable CUPS until next. 
        ///     1 - Enable CUPS
        /// [1:7] Reserved. Write as 00.
        /// </summary>
        private byte enable;

        /// <summary>
        /// CUPS Load Factor valid mask
        /// [0] - Core Load Factor Mask
        /// [1] - IO Load Factor Mask
        /// [2] - Memory Load Factor Mask
        /// [3:7] Reserved
        /// </summary>
        private byte loadFactorMask;

        /// <summary>
        /// Reserved
        /// </summary>
        private byte reserved = 0;

        /// <summary>
        /// Core Load Factor
        /// </summary>
        private ushort coreLoadFactor;

        /// <summary>
        /// IO Load Factor
        /// </summary>
        private ushort ioLoadFactor;

        /// <summary>
        /// Memory Load Factor
        /// </summary>
        private ushort memLoadFactor;

        /// <summary>
        /// Initializes a new instance of the SetCupsConfigurationRequest class.
        /// </summary>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        /// <param name="setCoreLoadFactor">if set to <c>true</c> [set core load factor].</param>
        /// <param name="setIoLoadFactor">if set to <c>true</c> [set io load factor].</param>
        /// <param name="setMemLoadFactor">if set to <c>true</c> [set memory load factor].</param>
        /// <param name="coreLoadFactor">The core load factor.</param>
        /// <param name="ioLoadFactor">The io load factor.</param>
        /// <param name="memLoadFactor">The memory load factor.</param>
        internal SetCupsConfigurationRequest(bool enable, bool setCoreLoadFactor, bool setIoLoadFactor, bool setMemLoadFactor,
            ushort coreLoadFactor, ushort ioLoadFactor, ushort memLoadFactor)
        {
            // Enable CUPS
            if (enable)
                this.enable = 0x1;

            // Set load factor masks
            // [0] - Core Load Factor Mask
            // [1] - IO Load Factor Mask
            // [2] - Memory Load Factor Mask
            // [3:7] Reserved
            if (setCoreLoadFactor)
                this.loadFactorMask = (byte)(this.loadFactorMask | 0x1);

            if (setIoLoadFactor)
                this.loadFactorMask = (byte)(this.loadFactorMask | 0x2);

            if (setMemLoadFactor)
                this.loadFactorMask = (byte)(this.loadFactorMask | 0x4);

            // Set load factor values
            this.coreLoadFactor = coreLoadFactor;
            this.ioLoadFactor = ioLoadFactor;
            this.memLoadFactor = memLoadFactor;
        }

        /// <summary>
        /// Intel Manufacturer Id
        /// </summary>
        [NodeManagerMessageData(0,3)]
        public byte[] ManufactureId
        {
            get { return this.manufactureId; }
        }

        /// <summary>
        /// Enable CUPS
        /// [0] 0 - Disable CUPS until next. 1- Enable CUPS
        /// [1:7] Reserved. Write as 00.
        /// </summary>
        [NodeManagerMessageData(3)]
        public byte Enable
        {
            get { return enable; }
        }

        /// <summary>
        /// CUPS Load Factor valid Domain mask
        /// [0] - Core Load Factor Mask
        /// [1] - IO Load Factor Mask
        /// [2] - Memory Load Factor Mask
        /// [3:7] Reserved
        /// </summary>
        [NodeManagerMessageData(4)]
        public byte LoadFactorMask
        {
            get { return loadFactorMask; }
        }

        /// <summary>
        /// Reserved
        /// </summary>
        [NodeManagerMessageData(5)]
        public byte Reserved
        {
            get { return reserved; }
        }

        /// <summary>
        /// Core Load Factor
        /// </summary>
        [NodeManagerMessageData(6)]
        public ushort CoreLoadFactor
        {
            get { return coreLoadFactor; }
        }

        /// <summary>
        /// IO Load Factor
        /// </summary>
        [NodeManagerMessageData(8)]
        public ushort IoLoadFactor
        {
            get { return ioLoadFactor; }
        }

        /// <summary>
        /// Memory Load Factor
        /// </summary>
        [NodeManagerMessageData(10)]
        public ushort MemLoadFactor
        {
            get { return memLoadFactor; }
        }
    }
}
