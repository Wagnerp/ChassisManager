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
    /// Represents the Node Manager 'Get Version' response message.
    /// </summary>
    [NodeManagerMessageResponse(NodeManagerFunctions.NodeManager, NodeManagerCommand.GetVersion)]
    public class GetNodeManagerVersionResponse : NodeManagerResponse
    {
        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        private byte[] manufactureId;

        /// <summary>
        /// Version Support
        /// </summary>
        private byte versionSupport;

        /// <summary>
        /// IPMI interface version 
        /// 01h � IPMI version 1.0
        /// 02h � IPMI version 2.0
        /// </summary>
        private byte ipmiVersion;

        /// <summary>
        /// Patch version (binary encoded).
        /// </summary>
        private byte patchVersion;

        /// <summary>
        /// Major version
        /// </summary>
        private byte majorVersion;

        /// <summary>
        /// Minor version
        /// </summary>
        private byte minorVersion;

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
        /// Version Support
        /// 01h � NM 1.0 one power policy.
        /// 02h � NM 2.5 multiple policies and thermal triggers for power policy.
        /// 03h � NM 2.0 multiple policies and thermal triggers for power policy.
        /// 04h � NM 2.5
        /// 05h � FFh � Reserved for future use.
        /// </summary>
        [NodeManagerMessageData(3)]
        public byte VersionSupport
        {
            get { return this.versionSupport; }
            set { this.versionSupport = value; }
        }

        /// <summary>
        /// Ipmi Version
        /// </summary>
        [NodeManagerMessageData(4)]
        public byte IpmiVersion
        {
            get { return this.ipmiVersion; }
            set { this.ipmiVersion = value; }
        }

        /// <summary>
        /// Patch Version
        /// </summary>
        [NodeManagerMessageData(5)]
        public byte PatchVersion
        {
            get { return this.patchVersion; }
            set { this.patchVersion = value; }
        }

        /// <summary>
        /// Major Version
        /// </summary>
        [NodeManagerMessageData(6)]
        public byte MajorVersion
        {
            get { return this.majorVersion; }
            set { this.majorVersion = value; }
        }

        /// <summary>
        /// Minor Version
        /// </summary>
        [NodeManagerMessageData(7)]
        public byte MinorVersion
        {
            get { return this.minorVersion; }
            set { this.minorVersion = value; }
        }

    }
}
