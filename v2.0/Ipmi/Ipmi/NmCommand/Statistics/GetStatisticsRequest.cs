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
    /// Represents the Node Manager 'Get Statistics' request message.
    /// </summary>
    [NodeManagerMessageRequest(NodeManagerFunctions.NodeManager, NodeManagerCommand.GetStatistics)]
    public class GetStatisticsRequest : NodeManagerRequest
    {
        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        private readonly byte[] manufactureId = { 0x57, 0x01, 0x00 };

        /// <summary>
        /// Mode
        /// [0:4] Mode
        /// [5:7] Reserved. Write as 00.
        /// </summary>
        private byte mode;

        /// <summary>
        /// Domain Id
        /// [0:3] Domain Id
        /// [4:7] Reserved. Write as 00.
        /// </summary>
        private byte domainId;

        /// <summary>
        /// Policy Id
        /// </summary>
        private byte policyId;

        /// <summary>
        /// Initializes a new instance of the GetStatisticsRequest class.
        /// </summary>
        internal GetStatisticsRequest(NodeManagerDomainId domainId, NodeManagerStatistics mode, byte policyId)
        {
            this.domainId = (byte)domainId;
            this.mode = (byte)mode;
            this.policyId = policyId;
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
        /// Mode
        /// </summary>
        [NodeManagerMessageData(3)]
        public byte Mode
        {
            get { return this.mode; }
        }

        /// <summary>
        /// Domain Id
        /// </summary>
        [NodeManagerMessageData(4)]
        public byte DomainId
        {
            get { return this.domainId; }
        }

        /// <summary>
        /// Policy Id
        /// </summary>
        [NodeManagerMessageData(5)]
        public byte PolicyId
        {
            get { return this.policyId; }
        }
    }
}
