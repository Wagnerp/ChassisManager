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

    using System;
    using System.Text;

    /// <summary>
    /// Represents the IPMI 'Get BIOS Code' application response message.
    /// </summary>
    [IpmiMessageResponse(IpmiFunctions.OemCustomGroup, IpmiCommand.GetBiosCode)]
    internal class GetBiosCodeResponse : IpmiResponse
    {
        /// <summary>
        /// BIOS Post Code
        /// </summary>
        private byte[] postCode = {};

        /// <summary>
        /// Default Power Cap in Watts
        /// </summary>       
        [IpmiMessageData(0)]
        public byte[] RawPostCode
        {
            get { return this.postCode; }
            set { this.postCode = value; }
        }

        /// <summary>
        /// BIOS Port 80 Code
        /// </summary>
        public string PostCode
        {
            get 
            {
                if (postCode != null)
                {
                    return IpmiSharedFunc.ByteArrayToHexString(postCode);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

    }
}
