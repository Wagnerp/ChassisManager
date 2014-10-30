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

    /// <summary>
    /// Represents the Node Manager 'Get Dengate Health Status' request message.
    /// </summary>
    [NodeManagerMessageRequest(NodeManagerFunctions.Application, NodeManagerCommand.GetDengateHealthStatus)]
    public class GetDengateHealthRequest : NodeManagerRequest
    {
        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        private readonly byte[] manufactureId = { 0x57, 0x01, 0x00 };

        /// <summary>
        /// Tag: only relevant for Health Status Type = 3, 4, 5, 
        /// and 6 (shall always be set to zero for other Health Status Type 
        /// values)
        /// Bit [15] � reserved � must be zero
        /// </summary>
        private byte[] tag = new byte[2];

        /// <summary>
        /// Dengate Health Type
        /// </summary>
        private byte dengateReqType;

        /// <summary>
        /// Physical address: only relevant for Health Status 
        /// Type = 6 (reserved otherwise)
        /// </summary>
        private byte[] physicalAddress;

        /// <summary>
        /// Reserved
        /// </summary>
        private readonly byte reserved = 0x00;

        /// <summary>
        /// Initializes a new instance of the GetDengateHealthRequest class.
        /// </summary>
        internal GetDengateHealthRequest(NodeManagerDengateHealth dengateReqType, ulong physicalAddress)
        {
            this.dengateReqType = (byte)dengateReqType;

            this.physicalAddress = BitConverter.GetBytes(physicalAddress);
        }

        /// <summary>
        /// Initializes a new instance of the GetDengateHealthRequest class.
        /// </summary>
        internal GetDengateHealthRequest(NodeManagerDengateHealth dengateReqType, ushort tag, ulong physicalAddress)
            : this(dengateReqType, BitConverter.GetBytes(tag), physicalAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetDengateHealthRequest class.
        /// </summary>
        internal GetDengateHealthRequest(NodeManagerDengateHealth dengateReqType, byte[] tag, ulong physicalAddress)
            : this(dengateReqType, physicalAddress)
        {
            this.dengateReqType = (byte)dengateReqType;

            if(tag != null)
            {
                int lenght = tag.Length;
                
                if(lenght > this.tag.Length)
                    lenght = this.tag.Length;

                Buffer.BlockCopy(tag, 0, this.tag, 0, lenght);

                // Bit [15] � reserved � must be zero
                this.tag[1] = (byte)(this.tag[1] & 0x7F);
            }
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
        /// Tag: only relevant for Health Status Type = 3, 4, 5, 
        /// and 6 (shall always be set to zero for other Health Status Type 
        /// values)
        /// Bit [15] � reserved � must be zero
        /// </summary>
        [NodeManagerMessageData(3,2)]
        public byte[] Tag
        {
            get { return this.tag; }
        }

        /// <summary>
        /// Dengate Health Type
        /// </summary>
        [NodeManagerMessageData(5)]
        public byte DengateReqType
        {
            get { return this.dengateReqType; }
        }

        /// <summary>
        /// Physical address: only relevant for Health Status 
        /// Type = 6 (reserved otherwise)
        /// </summary>
        [NodeManagerMessageData(6,8)]
        public byte[] PhysicalAddress
        {
            get { return this.physicalAddress; }
        }

        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        [NodeManagerMessageData(14)]
        public byte Reserved
        {
            get { return this.reserved; }
        }

    }
}
