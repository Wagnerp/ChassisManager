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
    /// Represents the Node Manager 'Get Turbo Sync Ratio' request message.
    /// </summary>
    [NodeManagerMessageRequest(NodeManagerFunctions.NodeManager, NodeManagerCommand.GetTurboSyncRatio)]
    public abstract class GetTurboSyncRatioRequest : NodeManagerRequest
    {
        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        private readonly byte[] manufactureId = { 0x57, 0x01, 0x00 };

        /// <summary>
        /// CPU Socket Number
        /// 00h � 07h � CPU socket number for which current settings should 
        ///       be read. Supported value could depend on system configuration. 
        /// 08h � FEh � reserved. 
        /// FFh � all sockets will return common maximum settings. 
        /// </summary>
        private byte socketNo;

        /// <summary>
        /// Active cores configuration 
        /// 00h � reserved.
        /// Others � Setting should be applied to configuration of given active 
        /// cores number
        /// </summary>
        private byte activeCores;

        /// <summary>
        /// Initializes a new instance of the GetTurboSyncRatioRequest class.
        /// </summary>
        public GetTurboSyncRatioRequest(byte socketNumber, byte activeCoreConfig)
        {
            // CPU Socket Number
            this.socketNo = (byte)socketNumber;

            // Active cores configuration 
            this.activeCores = (byte)activeCoreConfig;
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
        /// CPU Socket Number
        /// 00h � 07h � CPU socket number for which current settings should 
        /// be read. Supported value could depend on system configuration. 
        /// 08h � FEh � reserved. 
        /// FFh � all sockets will return common maximum settings. 
        /// </summary>
        [NodeManagerMessageData(3)]
        public byte CpuSocketNumber
        {
            get { return this.socketNo; }
        }

        /// <summary>
        /// Active cores configuration
        /// 00h � reserved.
        /// </summary>
        [NodeManagerMessageData(4)]
        public byte ActiveCoreConfiguration
        {
            get { return this.activeCores; }
        }
    }

    /// <summary>
    /// Sets the Turbo Ratio Limit on all active CPU cores
    /// </summary>
    public class GetAllCoreTurboSyncRatioRequest : GetTurboSyncRatioRequest
    { 
        /// <summary>
        /// Get the Turbo Ratio Limit on all active CPU cores
        /// </summary>
        public GetAllCoreTurboSyncRatioRequest()
            : base(0xFF, 0xFF)
        { 
            
        }
    }

    /// <summary>
    /// Sets the Turbo Ratio Limit on specified active cores.
    /// </summary>
    public class GetIndividualCoreTurboSyncRatioRequest : GetTurboSyncRatioRequest
    { 
        /// <summary>
        /// Get the Turbo Ratio Limit on specified CPU cores
        /// </summary>
        public GetIndividualCoreTurboSyncRatioRequest(byte socketNumber, byte activeCoreConfig)
            : base(socketNumber, activeCoreConfig)
        { 
            
        }
    }
}
