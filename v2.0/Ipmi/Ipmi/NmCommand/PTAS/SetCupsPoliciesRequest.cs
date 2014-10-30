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
    /// Represents the Node Manager 'Set Cups Policies' request message.
    /// </summary>
    [NodeManagerMessageRequest(NodeManagerFunctions.NodeManager, NodeManagerCommand.SetCupsPolicies)]
    public class SetCupsPoliciesRequest : NodeManagerRequest
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
        /// Policy enable
        /// [0] 0 - Policy disabled
        ///     1 - Policy enabled
        /// [1:7] Reserved.
        /// </summary>
        private byte enable;

        /// <summary>
        /// Policy Type
        /// [0:6] - Reserved
        /// [7] Policy storage option
        /// 0 - persistent storage (policy is saved to nonvolatile memory)
        /// 1 - volatile memory is used for storing the policy
        /// </summary>
        private byte policyType;

        /// <summary>
        /// Policy Excursion Actions
        /// [0] 0 - No action
        ///     1 - Send alert
        /// [1:7] - Reserved
        /// </summary>
        private byte policyExcursionAction;

        /// <summary>
        /// CUPS Threshold
        /// </summary>
        private ushort cupsThreshold;

        /// <summary>
        /// Averaging Window (in seconds)
        /// </summary>
        private ushort avgWindow;

        /// <summary>
        /// Initializes a new instance of the SetCupsPoliciesRequest class.
        /// </summary>
        /// <param name="policyDomainId">The policy domain unique identifier.</param>
        /// <param name="policyTargetId">The policy target unique identifier.</param>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        /// <param name="policyType">Type of the policy.</param>
        /// <param name="policyExcursionAlert">if set to <c>true</c> [policy excursion alert].</param>
        /// <param name="cupsThreshold">The cups threshold.</param>
        /// <param name="avgWindow">The average window.</param>
        internal SetCupsPoliciesRequest(NodeManagerCupsPolicyDomainId policyDomainId, NodeManagerCupsPolicyTargetId policyTargetId,
            bool enable, NodeManagerCupsPolicyType policyType, bool policyExcursionAlert, ushort cupsThreshold, ushort avgWindow)
        {
            /// CUPS Policy ID
            // Bits [0:3] is the Domain Identifier
            byte tempPolicyID = (byte)((byte)policyDomainId & 0xf);
            // Bits [4:7] is the Target Identifier
            tempPolicyID = (byte)(tempPolicyID | (((byte)policyTargetId & 0xf) << 4));

            this.policyID = tempPolicyID;

            // Enable Policy
            if (enable)
                this.enable = 0x1;

            // Policy Type
            this.policyType = (byte)(((byte)policyType & 0x1) << 7);

            // Policy Excursion Actions
            if (policyExcursionAlert)
                this.policyExcursionAction = 0x1;

            this.cupsThreshold = cupsThreshold;
            this.avgWindow = avgWindow;
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

        /// <summary>
        /// Policy enable
        /// [0] 0 - Policy disabled
        ///     1 - Policy enabled
        /// [1:7] Reserved.
        /// </summary>
        [NodeManagerMessageData(5)]
        public byte Enable
        {
            get { return enable; }
        }

        /// <summary>
        /// Policy Type
        /// [0:6] - Reserved
        /// [7] Policy storage option
        /// 0 - persistent storage (policy is saved to nonvolatile memory)
        /// 1 - volatile memory is used for storing the policy
        /// </summary>
        [NodeManagerMessageData(6)]
        public byte PolicyType
        {
            get { return policyType; }
        }

        /// <summary>
        /// Policy Excursion Actions
        /// [0] 0 - No action
        ///     1 - Send alert
        /// [1:7] - Reserved
        /// </summary>
        [NodeManagerMessageData(7)]
        public byte PolicyExcursionAction
        {
            get { return policyExcursionAction; }
        }

        /// <summary>
        /// CUPS Threshold
        /// </summary>
        [NodeManagerMessageData(8)]
        public ushort CupsThreshold
        {
            get { return cupsThreshold; }
        }

        /// <summary>
        /// Averaging Window (in seconds)
        /// </summary>
        [NodeManagerMessageData(10)]
        public ushort AvgWindow
        {
            get { return avgWindow; }
        }
    }
}
