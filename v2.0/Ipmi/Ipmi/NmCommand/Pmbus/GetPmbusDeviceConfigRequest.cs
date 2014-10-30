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
    /// Represents the Node Manager 'Get PMBUS Device Configuration' request message.
    /// </summary>
    [NodeManagerMessageRequest(NodeManagerFunctions.Application, NodeManagerCommand.GetPmbusDeviceConfig)]
    public class GetPmbusDeviceConfigRequest : NodeManagerRequest
    {
        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        private readonly byte[] manufactureId = { 0x57, 0x01, 0x00 };

        /// <summary>
        /// Device Index
        /// [0:4] = PMBUS-enabled Device Index 
        /// [5] � Reserved. Write as 0.
        /// [7:6] � Device address format. 
        ///     0h � Standard device address
        ///     1h � Extended device address
        ///     3h � Common configuration
        ///     Other � reserved
        /// </summary>
        private byte devIndex;

        /// <summary>
        /// Initializes a new instance of the SetPmbusDeviceConfigBase class.
        /// </summary>
        internal GetPmbusDeviceConfigRequest(byte deviceIndex, byte deviceAddressFormat)
        {
            // [0:4] = PMBUS-enabled Device Index 
            this.devIndex = deviceIndex;

            // [5] � Reserved. Write as 0.
            this.devIndex = (byte)(this.devIndex & 0x1F);

            byte addressFormat = (byte)((deviceAddressFormat & 0x03) << 6);

            // [7:6] � Device address format.
            this.devIndex = (byte)(this.devIndex | addressFormat);

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
        /// Device Index
        /// </summary>
        [NodeManagerMessageData(3)]
        public byte DeviceIndex
        {
            get { return this.devIndex; }
        }


    }
}
