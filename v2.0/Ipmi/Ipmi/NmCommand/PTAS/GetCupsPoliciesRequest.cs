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
    /// Represents the Node Manager 'Get CUPS Policies' request message.
    /// </summary>
    [NodeManagerMessageRequest(NodeManagerFunctions.NodeManager, NodeManagerCommand.GetCupsPolicies)]
    public class GetCupsPoliciesRequest : NodeManagerRequest
    {
        /// <summary>
        /// Intel Manufacturer Id
        /// </summary>
        private readonly byte[] manufactureId = { 0x57, 0x01, 0x00 };

        /// <summary>
        /// Reserved
        /// </summary>
        private byte reserved = 0;

        /// <summary>
        /// CUPS Policy ID
        /// [0:3] Domain Identifier
        /// 0x00 - Core Domain
        /// 0x01 - IO Domain
        /// 0x02 - Memory Domain
        /// [4:7] Target Identifier
        /// 0x00 - BMC
        /// 0x01 - Remote Console
        /// </summary>
        private byte policyID;

        /// <summary>
        /// Initializes a new instance of the GetCupsPoliciesRequest class.
        /// </summary>
        internal GetCupsPoliciesRequest(NodeManagerCupsPolicyDomainId policyDomainId, NodeManagerCupsPolicyTargetId policyTargetId)
        {
            /// CUPS Policy ID
            // Bits [0:3] is the Domain Identifier
            byte tempPolicyID = (byte)((byte)policyDomainId & 0xf);
            // Bits [4:7] is the Target Identifier
            tempPolicyID = (byte)(tempPolicyID | (((byte)policyTargetId & 0xf) << 4));

            this.policyID = tempPolicyID;
        }

        /// <summary>
        /// Intel Manufacturer Id
        /// </summary>
        [NodeManagerMessageData(0,3)]
        public byte[] ManufactureId
        {
            get { return this.manufactureId; }
        }

        /// <summary>
        /// Reserved
        /// </summary>
        [NodeManagerMessageData(3)]
        public byte Reserved
        {
            get { return reserved; }
        }

        /// <summary>
        /// CUPS Policy ID
        /// [0:3] Domain Identifier
        /// 0x00 - Core Domain
        /// 0x01 - IO Domain
        /// 0x02 - Memory Domain
        /// [4:7] Target Identifier
        /// 0x00 - BMC
        /// 0x01 - Remote Console
        /// </summary>
        [NodeManagerMessageData(4)]
        public byte PolicyID
        {
            get { return policyID; }
        }
    }
}
