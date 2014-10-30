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
    [NodeManagerMessageResponse(NodeManagerFunctions.Application, NodeManagerCommand.GetAggPmbusReadings)]
    public class GetAggPmbusReadingsResponse : NodeManagerResponse
    {
        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        private byte[] manufactureId;

        /// <summary>
        /// Length of the response depends on number of requested registers
        /// </summary>
        private byte[] registerValues;


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
        public byte[] RegisterValues
        {
            get { return this.registerValues; }
            set { this.registerValues = value; }
        }
    }
}
