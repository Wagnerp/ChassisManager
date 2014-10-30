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
    /// Represents the Node Manager 'Get Dengate Health Status' response message.
    /// </summary>
    [NodeManagerMessageResponse(NodeManagerFunctions.Application, NodeManagerCommand.GetDengateHealthStatus)]
    public class GetDengateHealthStatusResponse : NodeManagerResponse
    {
        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        private byte[] manufactureId;
        
        /// <summary>
        /// Tag
        /// Bits [0:14] - Tag � Same as in the request
        /// Bit[15] � Last fragment indicator
        ///      0 � there are subsequent records to be retrieved; the 
        ///          requestor sets Tag=0 in the initial request and iterates until 
        ///          ME returns �last fragment indicator� =1
        ///      1 � this is the last record
        /// </summary>
        private ushort tag;

        /// <summary>
        /// Health Status Type
        /// </summary>
        private byte healthStatus;

        /// <summary>
        /// Health Status type specific.
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
        /// Tag
        /// Bits [0:14] - Tag � Same as in the request
        /// Bit[15] � Last fragment indicator
        ///      0 � there are subsequent records to be retrieved; the 
        ///          requestor sets Tag=0 in the initial request and iterates until 
        ///          ME returns �last fragment indicator� =1
        ///      1 � this is the last record
        /// </summary>
        [NodeManagerMessageData(3)]
        public ushort Tag
        {
            get { return this.tag; }
            set { this.tag = value; }
        }

        /// <summary>
        /// Health Status Type
        /// </summary>
        [NodeManagerMessageData(5)]
        public byte HealthStatus
        {
            get { return this.healthStatus; }
            set { this.healthStatus = value; }
        }

        /// <summary>
        /// Response Data is Health Status type specific.
        /// </summary>
        [NodeManagerMessageData(6)]
        public byte[] ResponseData
        {
            get { return this.responseData; }
            set { this.responseData = value; }
        }

    }
}
