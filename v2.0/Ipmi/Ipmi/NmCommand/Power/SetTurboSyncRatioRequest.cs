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
    /// Represents the Node Manager 'Set Turbo Sync Ratio' request message.
    /// </summary>
    [NodeManagerMessageRequest(NodeManagerFunctions.NodeManager, NodeManagerCommand.SetTurboSyncRatio)]
    public abstract class SetTurboSyncRatioRequest : NodeManagerRequest
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
        /// FFh � apply settings to all active cores configuration 
        /// Others � Setting should be applied to configuration of given active 
        /// cores number
        /// </summary>
        private byte activeCores;

        /// <summary>
        /// Turbo Ratio Limit. 
        ///     00h � restore default settings 
        ///     Others � Turbo Ratio Limit to set
        /// </summary>
        private byte turboRatio;

        /// <summary>
        /// Initializes a new instance of the SetTurboSyncRatioRequest class.
        /// </summary>
        internal SetTurboSyncRatioRequest(byte socketNumber, byte activeCoreConfig, byte turboRatioLimit)
        {
            // CPU Socket Number
            this.socketNo = (byte)socketNumber;

            // Active cores configuration 
            this.activeCores = (byte)activeCoreConfig;

            // Turbo Ratio Limit. 
            this.turboRatio = (byte)turboRatioLimit;
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
        /// FFh � apply settings to all active cores configuration 
        /// </summary>
        [NodeManagerMessageData(4)]
        public byte ActiveCoreConfiguration
        {
            get { return this.activeCores; }
        }

        /// <summary>
        /// Turbo Ratio Limit. 
        ///     00h � restore default settings 
        ///     Others � Turbo Ratio Limit to set
        /// </summary>
        [NodeManagerMessageData(5)]
        public byte TurboRatioLimit
        {
            get { return this.turboRatio; }
        }


    }

    /// <summary>
    /// Sets the Turbo Ratio Limit on all active CPU cores
    /// </summary>
    public class SetAllCoreTurboSyncRatioRequest : SetTurboSyncRatioRequest
    { 
        /// <summary>
        /// Sets the Turbo Ratio Limit on all active CPU cores
        /// </summary>
        public SetAllCoreTurboSyncRatioRequest(byte turboRatioLimit)
            : base(0xFF, 0xFF, turboRatioLimit)
        { 
            
        }
    }

    /// <summary>
    /// Sets the Turbo Ratio Limit on specified active cores.
    /// </summary>
    public class SetIndividualCoreTurboSyncRatioRequest : SetTurboSyncRatioRequest
    { 
        /// <summary>
        /// Sets the Turbo Ratio Limit on all active CPU cores
        /// </summary>
        public SetIndividualCoreTurboSyncRatioRequest(byte socketNumber, byte activeCoreConfig, byte turboRatioLimit)
            : base(socketNumber, activeCoreConfig, turboRatioLimit)
        { 
            
        }
    }

    /// <summary>
    /// Resets Turbo Sync on all CPU to Default Settings
    /// </summary>
    public class ResetTurboSyncRatioRequest : SetTurboSyncRatioRequest
    {
        /// <summary>
        /// Resets Turbo Sync on all CPU to Default Settings
        /// </summary>
        public ResetTurboSyncRatioRequest()
            : base(0xFF, 0xFF, 0x00)
        { 
            
        }
    
    }

}
