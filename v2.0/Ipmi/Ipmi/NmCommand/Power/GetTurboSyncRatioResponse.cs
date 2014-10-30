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
    /// Represents the Node Manager 'Get Turbo Sync Ratio' response message.
    /// </summary>
    [NodeManagerMessageResponse(NodeManagerFunctions.NodeManager, NodeManagerCommand.GetTurboSyncRatio)]
    public class GetTurboSyncRatioResponse : NodeManagerResponse
    {
        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        private byte[] manufactureId;

        /// <summary>
        /// Turbo Ratio Limit
        /// When socket number and/or active core 
        /// configurations are set to 0xFF � valid current
        /// ratio if all selected active core configurations are synchronized with 
        /// the same value, 0 when there is no synchronization
        /// </summary>
        private byte turboRationLimit;

        /// <summary>
        /// Default Turbo Ratio Limit 
        /// When socket number and/or active core 
        /// configurations are set to 0xFF � valid default 
        /// ratio if all selected active core configurations are synchronized with 
        /// the same value, 0 when there is no synchronization
        /// </summary>
        private byte defaultTurboRationLimit;

        /// <summary>
        /// Maximum Turbo Ratio Limit 
        /// In case of socket number set to FFh this is 
        /// maximum Turbo Ratio Limit that could be set on all CPUs.
        /// </summary>
        private byte maxTurboRationLimit;

        /// <summary>
        /// Minimum Turbo Ratio Limit 
        /// In case of socket number set to FFh this is 
        /// minimum Turbo Ratio Limit that could be set on all CPUs.
        /// </summary>
        private byte minTurboRationLimit;

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
        /// Turbo Ratio Limit
        /// When socket number and/or active core 
        /// configurations are set to 0xFF � valid current
        /// ratio if all selected active core configurations are synchronized with 
        /// the same value, 0 when there is no synchronization
        /// </summary>
        [NodeManagerMessageData(3)]
        public byte TurboRationLimit
        {
            get { return this.turboRationLimit; }
            set { this.turboRationLimit = value; }
        }

        /// <summary>
        /// Default Turbo Ratio Limit 
        /// When socket number and/or active core 
        /// configurations are set to 0xFF � valid default 
        /// ratio if all selected active core configurations are synchronized with 
        /// the same value, 0 when there is no synchronization
        /// </summary>
        [NodeManagerMessageData(4)]
        public byte DefaultTurboRationLimit
        {
            get { return this.defaultTurboRationLimit; }
            set { this.defaultTurboRationLimit = value; }
        }

        /// <summary>
        /// Maximum Turbo Ratio Limit 
        /// In case of socket number set to FFh this is 
        /// maximum Turbo Ratio Limit that could be set on all CPUs.
        /// </summary>
        [NodeManagerMessageData(5)]
        public byte MaximumTurboRationLimit
        {
            get { return this.maxTurboRationLimit; }
            set { this.maxTurboRationLimit = value; }
        }

        /// <summary>
        /// Minimum Turbo Ratio Limit 
        /// In case of socket number set to FFh this is 
        /// minimum Turbo Ratio Limit that could be set on all CPUs.
        /// </summary>
        [NodeManagerMessageData(6)]
        public byte MinimumTurboRationLimit
        {
            get { return this.minTurboRationLimit; }
            set { this.minTurboRationLimit = value; }
        }


    }
}
