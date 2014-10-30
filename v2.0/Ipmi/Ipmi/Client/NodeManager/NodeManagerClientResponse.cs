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
    /// Class that supports the Send Message / Get Message command.
    /// </summary>
    public class SendNodeMangerMessage : ResponseBase
    {
        /// <summary>
        /// Response message payload.
        /// </summary>
        private byte[] messageData;

        /// <summary>
        /// Request Sequence Number
        /// </summary>
        private byte rqSeq;


        private NodeManagerResponse response;

        /// <summary>
        /// Initialize class
        /// </summary>
        public SendNodeMangerMessage(byte completionCode)
        {
            base.CompletionCode = completionCode;
        }

        internal override void SetParamaters(byte[] param)
        {
            this.messageData = param;
        }

        internal void SetParamaters(byte[] messageData, byte rqSep)
        {
            this.messageData = messageData;
            this.rqSeq = rqSep;
        }

        /// <summary>
        /// Response message payload.
        /// </summary>
        public byte[] MessageData
        {
            get { return this.messageData; }
            set { this.messageData = value; }
        }

        /// <summary>
        /// Request message sequence.
        /// </summary>
        public byte RqSeq
        {
            get { return this.rqSeq; }
            internal set { this.rqSeq = value; }
        }

        /// <summary>
        /// Response message payload.
        /// </summary>
        public NodeManagerResponse Response
        {
            get { return this.response; }
            internal set { this.response = value; }
        }
    }

    /// <summary>
    /// Class that supports the Send Message / Get Message command.
    /// </summary>
    public class GetNodeMangerMessage : ResponseBase
    {
        /// <summary>
        /// Response message payload.
        /// </summary>
        private byte[] messageData;

        /// <summary>
        /// Request Sequence Number
        /// </summary>
        private NodeManagerResponse response;

        /// <summary>
        /// Initialize class
        /// </summary>
        public GetNodeMangerMessage(byte completionCode)
        {
            base.CompletionCode = completionCode;
        }

        internal override void SetParamaters(byte[] param)
        {
            this.messageData = param;
        }

        internal void SetParamaters(byte[] messageData, NodeManagerResponse response)
        {
            this.messageData = messageData;
            this.response = response;
        }

        /// <summary>
        /// Response message payload.
        /// </summary>
        public byte[] MessageData
        {
            get { return this.messageData; }
            set { this.messageData = value; }
        }

        /// <summary>
        /// Response message payload.
        /// </summary>
        public NodeManagerResponse Response
        {
            get { return this.response; }
            internal set { this.response = value; }
        }
    }

}
