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
    /// Represents the Node Manager 'Get PMBUS Readings' response message.
    /// </summary>
    [NodeManagerMessageResponse(NodeManagerFunctions.Application, NodeManagerCommand.GetPmbusReadings)]
    public class GetPmbusReadingsResponse : NodeManagerResponse
    {
        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        private byte[] manufactureId;

        /// <summary>
        /// Timestamp
        /// </summary>
        private uint timestamp;

        /// <summary>
        /// Register Value of Monitored Register [First Register Offset]
        /// For READ_EIN and READ_EOUT this field contains value converted to Watts
        /// </summary>
        private ushort registerValue;

        /// <summary>
        /// Length of the response depends on number of monitored registers. 
        /// Bytes + are used only if the PMBUS-enabled device is monitored 
        /// for the sensors
        /// </summary>
        private byte[] registerOffsets;

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
        /// Timestamp
        /// </summary>
        [NodeManagerMessageData(3)]
        public uint TimeStamp
        {
            get { return this.timestamp; }
            set { this.timestamp = value; }
        }

        /// <summary>
        /// Register Value of Monitored Register [First Register Offset]
        /// For READ_EIN and READ_EOUT this field contains value converted to Watts
        /// </summary>
        [NodeManagerMessageData(7)]
        public ushort RegisterValue
        {
            get { return this.registerValue; }
            set { this.registerValue = value; }
        }

        /// <summary>
        /// Length of the response depends on number of monitored registers. 
        /// Bytes + are used only if the PMBUS-enabled device is monitored 
        /// for the sensors
        /// </summary>
        [NodeManagerMessageData(9)]
        public byte[] RegisterOffsets
        {
            get { return this.registerOffsets; }
            set { this.registerOffsets = value; }
        }




    }
}
