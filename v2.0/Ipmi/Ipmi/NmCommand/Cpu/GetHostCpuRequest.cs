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
    /// Represents the Node Manager 'Get Host Cpu Data' request message.
    /// </summary>
    [NodeManagerMessageRequest(NodeManagerFunctions.NodeManager, NodeManagerCommand.GetHostCpuData)]
    public class GetHostCpuRequest : NodeManagerRequest
    {
        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        private readonly byte[] manufactureId = { 0x57, 0x01, 0x00 };

        /// <summary>
        /// Domain Id
        /// [0:3] Domain Id
        /// [4:7] Reserved. Write as 00.
        /// </summary>
        private const byte domainId = 0;

        /// <summary>
        /// Initializes a new instance of the GetHostCpuRequest class.
        /// </summary>
        internal GetHostCpuRequest()
        {
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
        /// Domain Id
        /// </summary>
        [NodeManagerMessageData(3)]
        public byte DomainId
        {
            get { return domainId; }
        }
    }
}
