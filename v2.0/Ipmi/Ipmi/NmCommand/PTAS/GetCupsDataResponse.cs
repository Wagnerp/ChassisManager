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
    /// Represents the Node Manager 'Get CUPS Data' response message.
    /// </summary>
    [NodeManagerMessageResponse(NodeManagerFunctions.NodeManager, NodeManagerCommand.GetCupsData)]
    public class GetCupsDataResponse : NodeManagerResponse
    {
        /// <summary>
        /// Intel Manufacturer Id
        /// </summary>
        private byte[] manufactureId;

        /// <summary>
        /// CPU CUPS Value/CUPS Value
        /// </summary>
        private ushort cpuCupsValue;

        /// <summary>
        /// Memory CUPS Value
        /// </summary>
        private ushort memCupsValue;

        /// <summary>
        /// IO CUPS Value
        /// </summary>
        private ushort ioCupsValue;

        /// <summary>
        /// Reserved
        /// </summary>
        private ushort reserved;

        /// <summary>
        /// Intel Manufacturer Id
        /// </summary>
        [NodeManagerMessageData(0, 3)]
        public byte[] ManufactureId
        {
            get { return this.manufactureId; }
            set { this.manufactureId = value; }
        }

        /// <summary>
        /// CPU CUPS Value/CUPS Value
        /// </summary>
        [NodeManagerMessageData(3, 2)]
        public ushort CpuCupsValue
        {
            get { return this.cpuCupsValue; }
            set { this.cpuCupsValue = value; }
        }

        /// <summary>
        /// Memory CUPS Value
        /// </summary>
        [NodeManagerMessageData(5, 2)]
        public ushort MemCupsValue
        {
            get { return this.memCupsValue; }
            set { this.memCupsValue = value; }
        }

        /// <summary>
        /// IO CUPS Value
        /// </summary>
        [NodeManagerMessageData(7, 2)]
        public ushort IoCupsValue
        {
            get { return this.ioCupsValue; }
            set { this.ioCupsValue = value; }
        }

        /// <summary>
        /// Reserved byte
        /// </summary>
        [NodeManagerMessageData(9,2)]
        public ushort Reserved
        {
            get { return this.reserved; }
            set { this.reserved = value; }
        }
    }
}
