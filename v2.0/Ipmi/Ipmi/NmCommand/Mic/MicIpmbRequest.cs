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
    /// Represents the Node Manager 'MIC IPMB Request' request message.
    /// </summary>
    [NodeManagerMessageRequest(NodeManagerFunctions.NodeManager, NodeManagerCommand.MicIpmbRequest)]
    public class MicIpmbRequest : NodeManagerRequest
    {
        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        private readonly byte[] manufactureId = { 0x57, 0x01, 0x00 };

        /// <summary>
        /// Address Type / Bus Number
        /// [7:6] Address Type
        ///     00b � Bus/Slot/Address
        ///     Other values reserved
        /// [5:4] Reserved
        /// [3:0] Bus Number � Identifies SMBus interface to be used by 
        /// ME to send the request. Must be set to 1 for Grantley platform
        /// </summary>
        private byte addressType;

        /// <summary>
        /// Slot Number � identifies PCIe slot in which the MIC 
        /// device is inserted. Valid values are 0�15.
        /// </summary>
        private byte slotNumber;

        /// <summary>
        /// Slave Address - the I2C* slave address (8 bit �write� 
        /// address) of the MIC device
        /// </summary>
        private byte slaveAddress;

        /// <summary>
        /// Net function value for the IPMB request to be sent to the 
        /// MIC device
        /// </summary>
        private byte netFunction;

        /// <summary>
        ///  MIC Command Code � Command Code field value for the 
        ///  IPMB request to be sent to the MIC device
        /// </summary>
        private byte command;

        /// <summary>
        /// MIC Request Data - Optional data bytes of the IPMB 
        /// request Supported length of the field is up to 29 bytes
        /// </summary>
        private byte[] requestData;

        /// <summary>
        /// Initializes a new instance of the MicIpmbRequest class.
        /// </summary>
        internal MicIpmbRequest(byte addressType, byte busNumber, byte slotNumber, 
            byte slaveAddress, byte netFunction, byte command, byte[] requestData)
        {

            // set the address type byte.
            // [7:6] Address Type: 00b � Bus/Slot/Address
            // [5:4] Reserved
            // [3:0] Bus Number 
            this.addressType = (byte)((addressType & 0x03) << 6);
            this.addressType = (byte)(this.addressType & (byte)(busNumber & 0x0F));

            // set the slot number
            this.slotNumber = slotNumber;

            // set the slave address
            this.slaveAddress = slaveAddress;

            // set the net function
            this.netFunction = netFunction;

            // set the comamnd
            this.command = command;

            if (requestData != null)
                this.requestData = requestData;
            else
                this.requestData = new byte[0];
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
        /// Address Type / Bus Number
        /// [7:6] Address Type
        ///     00b � Bus/Slot/Address
        ///     Other values reserved
        /// [5:4] Reserved
        /// [3:0] Bus Number � Identifies SMBus interface to be used by 
        /// ME to send the request. Must be set to 1 for Grantley platform
        /// </summary>
        [NodeManagerMessageData(3)]
        public byte AddressType
        {
            get { return this.addressType; }
        }

        /// <summary>
        /// Slot Number � identifies PCIe slot in which the MIC 
        /// device is inserted. Valid values are 0�15.
        /// </summary>
        [NodeManagerMessageData(4)]
        public byte SlotNumber
        {
            get { return this.slotNumber; }
        }

        /// <summary>
        /// Slave Address - the I2C* slave address (8 bit �write� 
        /// address) of the MIC device
        /// </summary>
        [NodeManagerMessageData(5)]
        public byte SlaveAddress
        {
            get { return this.slaveAddress; }
        }

        /// <summary>
        /// Net function value for the IPMB request to be sent to the 
        /// MIC device
        /// </summary>
        [NodeManagerMessageData(6)]
        public byte NetFunction
        {
            get { return this.netFunction; }
        }

        /// <summary>
        ///  MIC Command Code � Command Code field value for the 
        ///  IPMB request to be sent to the MIC device
        /// </summary>
        [NodeManagerMessageData(7)]
        public byte Command
        {
            get { return this.command; }
        }

        /// <summary>
        /// MIC Request Data - Optional data bytes of the IPMB 
        /// request Supported length of the field is up to 29 bytes
        /// </summary>
        [NodeManagerMessageData(8)]
        public byte[] RequestData
        {
            get { return this.requestData; }
        }

    }
}
