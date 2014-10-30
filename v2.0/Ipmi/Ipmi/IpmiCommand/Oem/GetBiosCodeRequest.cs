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

namespace Microsoft.GFS.WCS.ChassisManager.Ipmi
{

    /// <summary>
    /// Represents the IPMI 'Get BIOS Code' OEM request message.
    /// </summary>
    [IpmiMessageRequest(IpmiFunctions.OemCustomGroup, IpmiCommand.GetBiosCode)]
    internal class GetBiosCodeRequest : IpmiRequest
    {

        /// <summary>
        /// Read Current or Previous
        ///     0 = Current
        ///     1 = Previous
        /// </summary>
        private byte retrieveVersion;

        /// <summary>
        /// Read BIOS Error Code
        /// </summary>
        /// <param name="version">0 = Current, 1 = Previous</param>
        internal GetBiosCodeRequest(byte version)
        {
            this.retrieveVersion = version;
        }

        /// <summary>
        /// Version to Retrieve:
        ///     0 = Read Current BIOS Code
        ///     1 = Read Previous BIOS code
        /// </summary>       
        [IpmiMessageData(0)]
        public byte ReadVersion
        {
            get { return this.retrieveVersion; }

        }
        
    }
}
