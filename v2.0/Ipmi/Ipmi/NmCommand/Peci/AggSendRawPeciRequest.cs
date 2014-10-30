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
    /// Represents the Node Manager 'Aggregated Send Raw Peci' request message.
    /// </summary>
    [NodeManagerMessageRequest(NodeManagerFunctions.Application, NodeManagerCommand.AggregatedSendRawPeci)]
    public class AggSendRawPeciRequest : NodeManagerRequest
    {
        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        private readonly byte[] manufactureId = { 0x57, 0x01, 0x00 };

        /// <summary>
        /// PECI Client Address and interface selection
        /// [7:6] � PECI Interface selection
        /// [5:0] - PECI Client Address 
        /// </summary>
        private byte peciAddress;

        /// <summary>
        /// Write Length (part of PECI standard header)
        /// </summary>
        private byte writeLenght;

        /// <summary>
        /// Read Length (part of PECI standard header)
        /// </summary>
        private byte readLenght;

        /// <summary>
        /// The remaining part of PECI command
        /// </summary>
        private byte[] peciCommands;

        /// <summary>
        /// Initializes a new instance of the AggSendRawPeciRequest class.
        /// </summary>
        internal AggSendRawPeciRequest(NodeManagerPeciInterface peciInterface, byte peciAddress, byte writeLenght, byte readLenght, byte[] peciCommands)
        {
            // peci interface
            byte peciInt = (byte)peciInterface;

            // PECI Client Address and interface selection
            this.peciAddress = (byte)(peciAddress & 0x3F);
            this.peciAddress = (byte)((peciInt << 6) | this.peciAddress);

            // Write Length (part of PECI standard header)
            this.writeLenght = writeLenght;

            // Read Length (part of PECI standard header)
            this.readLenght = readLenght;

            // if null set to zero byte array
            if (peciCommands == null)
                peciCommands = new byte[0];

            // The remaining part of PECI command
            this.peciCommands = peciCommands;
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
        /// PECI Client Address and interface selection
        /// [7:6] � PECI Interface selection
        /// [5:0] - PECI Client Address 
        /// </summary>
        [NodeManagerMessageData(3)]
        public byte PeciAddress
        {
            get { return this.peciAddress; }
        }

        /// <summary>
        /// Write Length (part of PECI standard header)
        /// </summary>
        [NodeManagerMessageData(4)]
        public byte WriteLenght
        {
            get { return this.writeLenght; }
        }

        /// <summary>
        /// Read Length (part of PECI standard header)
        /// </summary>
        [NodeManagerMessageData(5)]
        public byte ReadLenght
        {
            get { return this.readLenght; }
        }

        /// <summary>
        /// The remaining part of PECI command
        /// </summary>
        [NodeManagerMessageData(6)]
        public byte[] PeciCommands
        {
            get { return this.peciCommands; }
        }

    }
}
