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
    /// Represents the Node Manager 'Get Mic Card Info' request message.
    /// </summary>
    [NodeManagerMessageRequest(NodeManagerFunctions.NodeManager, NodeManagerCommand.GetMicCardInfo)]
    public class GetMicCardInfoRequest : NodeManagerRequest
    {
        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        private readonly byte[] manufactureId = { 0x57, 0x01, 0x00 };

        /// <summary>
        /// Card instance (1-based) for which information is 
        /// requested. If this byte is zero, only the total number of cards 
        /// detected will be returned
        /// </summary>
        private byte cardInstance;

        /// <summary>
        /// Initializes a new instance of the GetMicCardInfoRequest class.
        /// </summary>
        internal GetMicCardInfoRequest(byte cardInstance)
        {
            this.cardInstance = cardInstance;
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
        /// Card instance (1-based) for which information is 
        /// requested. If this byte is zero, only the total number 
        /// of cards detected will be returned
        /// </summary>
        [NodeManagerMessageData(3)]
        public byte CardInstance
        {
            get { return this.cardInstance; }
        }

    }
}
