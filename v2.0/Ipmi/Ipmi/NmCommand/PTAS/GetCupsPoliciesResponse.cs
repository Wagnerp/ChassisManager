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
    /// Represents the Node Manager 'Get CUPS Policies' response message.
    /// </summary>
    [NodeManagerMessageResponse(NodeManagerFunctions.NodeManager, NodeManagerCommand.GetCupsPolicies)]
    public class GetCupsPoliciesResponse : NodeManagerResponse
    {
        /// <summary>
        /// Intel Manufacturer Id
        /// </summary>
        private byte[] manufactureId;

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
        /// Contains lowest valid Policy ID that is higher than Policy ID specified in the request.
        /// Only valid if Completion Code = 80h (Policy ID Invalid)
        /// </summary>
        private byte nextValidPolicyID;

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
        /// Policy enable
        /// [0] 0 - Policy disabled
        ///     1 - Policy enabled
        /// [1:7] Reserved.
        /// </summary>
        [NodeManagerMessageData(3)]
        public byte Enable
        {
            get { return this.enable; }
            set { this.enable = (byte)(value & 0x1); }
        }

        /// <summary>
        /// Contains lowest valid Policy ID that is higher than Policy ID specified in the request.
        /// Only valid if Completion Code = 80h (Policy ID Invalid)
        /// This shares the same byte as the enable byte.
        /// </summary>
        [NodeManagerMessageData(3)]
        public byte NextValidPolicyID
        {
            get { return this.nextValidPolicyID; }
            set { this.nextValidPolicyID = value; }
        }

        /// <summary>
        /// Policy Type
        /// [0:6] - Reserved
        /// [7] Policy storage option
        /// 0 - persistent storage (policy is saved to nonvolatile memory)
        /// 1 - volatile memory is used for storing the policy
        /// </summary>
        [NodeManagerMessageData(4)]
        public byte PolicyType
        {
            get { return this.policyType; }
            set { this.policyType = (byte)(value & 0x80); }
        }

        /// <summary>
        /// Policy Excursion Actions
        /// [0] 0 - No action
        ///     1 - Send alert
        /// [1:7] - Reserved
        /// </summary>
        [NodeManagerMessageData(5)]
        public byte PolicyExcursionAction
        {
            get { return this.policyExcursionAction; }
            set { this.policyExcursionAction = (byte)(value & 0x1); }
        }

        /// <summary>
        /// CUPS Threshold
        /// </summary>
        [NodeManagerMessageData(6)]
        public ushort CupsThreshold
        {
            get { return this.cupsThreshold; }
            set { this.cupsThreshold = value; }
        }

        /// <summary>
        /// Averaging Window (in seconds)
        /// </summary>
        [NodeManagerMessageData(8)]
        public ushort AvgWindow
        {
            get { return this.avgWindow; }
            set { this.avgWindow = value; }
        }
    }
}
