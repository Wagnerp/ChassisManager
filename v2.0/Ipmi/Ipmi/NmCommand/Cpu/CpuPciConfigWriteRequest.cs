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
    using System;
    using System.Collections;

    /// <summary>
    /// Represents the Node Manager 'Cpu Pci Config Write' request message.
    /// </summary>
    [NodeManagerMessageRequest(NodeManagerFunctions.NodeManager, NodeManagerCommand.CpuPciConfigWrite)]
    public class CpuPciConfigWriteRequest : NodeManagerRequest
    {
        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        private readonly byte[] manufactureId = { 0x57, 0x01, 0x00 };

        /// <summary>
        /// Cpu Number
        /// [7] � Reserved.
        /// [6] = 1b � PCI local space; 
        ///         The RdPCIConfigLocal() command will be used that provides read 
        ///         access to the PCI configuration space that resides on the processor
        ///         itself (named here - �local� PCI space). Accessing the local PCI 
        ///         space is possible before BIOS has enumerated the systems buses.
        /// [5:2] � Reserved.
        /// [1:0] � CPU number (starting from 0).
        /// </summary>
        private byte cpuNumber;

        /// <summary>
        /// PCI Address:
        /// [31:28] � Reserved.
        /// [27:20] � Bus Number.
        /// [19:15] � Device Number.
        /// [14:12] � Function Number.
        /// [11:0]  � Register Address.
        /// </summary>
        private byte[] pciAddress = new byte[4];

        /// <summary>
        /// Write Length � number of bytes to read 
        /// [7:2] � Reserved.
        /// [1:0] � Read Length � number of bytes to read: 
        ///         0 � Reserved � shouldn�t be used.
        ///         1 � 1 byte.
        ///         2 � 2 bytes (word).
        ///         3 � 4 bytes (double word)
        /// </summary>
        private byte writeLenght;

        /// <summary>
        /// Register value
        /// </summary>
        private byte[] registerValue;

        /// <summary>
        /// Initializes a new instance of the CpuPciConfigWriteRequest class.
        /// </summary>
        internal CpuPciConfigWriteRequest(byte cpuNumber, bool localspace,
            byte busNumber, byte deviceNumber, byte function, ushort register, byte writeLenght, byte[] registerValue)
            : this(cpuNumber, localspace,
            busNumber, deviceNumber, function, BitConverter.GetBytes(register), writeLenght, registerValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CpuPciConfigWriteRequest class.
        /// </summary>
        internal CpuPciConfigWriteRequest(byte cpuNumber, bool localspace, 
            byte busNumber, byte deviceNumber, byte function, byte[] register, byte writeLenght, byte[] registerValue)
        {
            this.cpuNumber = (byte)(cpuNumber & 0x03);

            if (localspace)
                this.cpuNumber = (byte)(cpuNumber | 0x40);

            BitArray address = new BitArray(pciAddress);

            // register address byte 1 [0-7].
            IpmiSharedFunc.UpdateBitArray(ref address, 0, 7, register[0]);
            
            // register address byte 2 [8-11]
            IpmiSharedFunc.UpdateBitArray(ref address, 8, 11, register[1]);

            // function [12-14]
            IpmiSharedFunc.UpdateBitArray(ref address, 12, 14, function);

            // Device Number [15-19]
            IpmiSharedFunc.UpdateBitArray(ref address, 15, 19, deviceNumber);

            // Bus Number [20-27]
            IpmiSharedFunc.UpdateBitArray(ref address, 20, 27, busNumber);

            // copy all bits to byte array
            address.CopyTo(pciAddress, 0);

            this.writeLenght = (byte)(writeLenght & 0x03);

            if (registerValue != null)
                this.registerValue = registerValue;
            else
                this.registerValue = new byte[0];


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
        /// PCI Address
        /// </summary>
        [NodeManagerMessageData(4,4)]
        public byte[] PciAddress
        {
            get { return this.pciAddress; }
        }

        /// <summary>
        /// Write Lenght
        /// </summary>
        [NodeManagerMessageData(8)]
        public byte WriteLenght
        {
            get { return this.writeLenght; }
        }

        /// <summary>
        /// Register Value
        /// </summary>
        [NodeManagerMessageData(9)]
        public byte[] RegisterValue
        {
            get { return this.registerValue; }
        }

    }
}
