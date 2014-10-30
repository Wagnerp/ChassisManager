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
    /// Represents the Node Manager 'MIC IPMB Request' response message.
    /// </summary>
    [NodeManagerMessageResponse(NodeManagerFunctions.Application, NodeManagerCommand.MicIpmbRequest)]
    public class MicIpmbResponse : NodeManagerResponse
    {
        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        private byte[] manufactureId;

        /// <summary>
        /// MIC Response CompletionCode
        /// </summary>
        private byte micCompletionCode;

        /// <summary>
        /// MIC Response Data
        /// </summary>
        private byte[] responseData;

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
        /// MIC Response CompletionCode
        /// </summary>
        [NodeManagerMessageData(3)]
        public byte MicCompletionCode
        {
            get { return this.micCompletionCode; }
            set { this.micCompletionCode = value; }
        }

        /// <summary>
        /// MIC Response Data
        /// </summary>
        [NodeManagerMessageData(4)]
        public byte[] ResponseData
        {
            get { return this.responseData; }
            set { this.responseData = value; }
        }

    }
}
