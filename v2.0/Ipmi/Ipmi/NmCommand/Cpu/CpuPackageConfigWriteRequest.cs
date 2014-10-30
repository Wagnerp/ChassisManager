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
    /// Represents the Node Manager 'Cpu Package Config Write' request message.
    /// </summary>
    [NodeManagerMessageRequest(NodeManagerFunctions.NodeManager, NodeManagerCommand.CpuPackageConfigWrite)]
    public class CpuPackageConfigWriteRequest : NodeManagerRequest
    {
        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        private readonly byte[] manufactureId = { 0x57, 0x01, 0x00 };

        /// <summary>
        /// Cpu Number
        /// [7:2] � Reserved.
        /// [1:0] � CPU number (starting from 0).
        /// </summary>
        private byte cpuNumber;

        /// <summary>
        /// PCS Index
        /// </summary>
        private byte pcsIndex;

        /// <summary>
        ///  Parameter Number (WORD)
        /// </summary>
        private ushort parameterNumber;

        /// <summary>
        ///  Parameter
        /// </summary>
        private byte[] parameter = new byte[2];

        /// <summary>
        /// Byte 8 � Write Length � number of bytes to write 
        /// [7:2] � Reserved.
        /// [1:0] � write Length � number of bytes to read: 
        ///         0 � Reserved � shouldn�t be used.
        ///         1 � 1 byte.
        ///         2 � 2 bytes (word).
        ///         3 � 4 bytes (double word)
        /// </summary>
        private byte writeLenght;

        /// <summary>
        /// Data to be written to CPU. Length of this data (1B, 2B, 4B) 
        /// depends on Write Length value included in Byte 8 of this request
        /// </summary>
        private byte[] cpuData;

        /// <summary>
        /// Initializes a new instance of the CpuPackageConfigWriteRequest class.
        /// </summary>
        internal CpuPackageConfigWriteRequest(byte cpuNumber, byte pcsIndex, 
            ushort parameterNo, byte[] parameter, byte writeLenght, byte[] cpuData)
        {
            this.cpuNumber = (byte)(cpuNumber & 0x03);
            this.pcsIndex = pcsIndex;
            this.parameterNumber = parameterNo;

            if (parameter != null)
            {
                if (parameter.Length == 2)
                {
                    this.parameter[0] = parameter[0];
                    this.parameter[1] = parameter[1];
                }
            }

            this.writeLenght = (byte)(writeLenght & 0x03);

            this.cpuData = cpuData;
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
        /// Cpu Number
        /// [7:2] � Reserved.
        /// [1:0] � CPU number (starting from 0).
        /// </summary>
        [NodeManagerMessageData(3)]
        public byte CpuNumber
        {
            get { return this.cpuNumber; }
        }

        /// <summary>
        /// PCS Index
        /// </summary>
        [NodeManagerMessageData(4)]
        public byte PcsIndex
        {
            get { return this.pcsIndex; }
        }

        /// <summary>
        /// Parameter Number
        /// </summary>
        [NodeManagerMessageData(5)]
        public ushort ParameterNumber
        {
            get { return this.parameterNumber; }
        }

        /// <summary>
        /// Parameter
        /// </summary>
        [NodeManagerMessageData(7,2)]
        public byte[] Parameter
        {
            get { return this.parameter; }
        }

        /// <summary>
        /// Write Lenght
        /// </summary>
        [NodeManagerMessageData(9)]
        public byte WriteLenght
        {
            get { return this.writeLenght; }
        }

        /// <summary>
        /// Write Data
        /// </summary>
        [NodeManagerMessageData(10)]
        public byte[] CpuData
        {
            get { return this.cpuData; }
        }

    }
}
