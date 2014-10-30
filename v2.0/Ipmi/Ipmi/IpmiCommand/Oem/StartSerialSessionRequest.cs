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
    /// Represents the IPMI 'Start Serial Session' OEM request message.
    /// </summary>
    [IpmiMessageRequest(IpmiFunctions.OemGroup, IpmiCommand.StartSerialSession)]
    internal class StartSerialSessionRequest : IpmiRequest
    {
        /// <summary>
        /// [0]   = Flush Buffer
        /// [1:7] = Inativiity Timeout
        /// </summary>
        private readonly byte messagePayload;


        /// <summary>
        /// Initialize instance of the class.
        /// </summary>
        /// <param name="flushBuffer">Flush the internal Console Buffer</param>
        /// <param name="inactivityTimeout">Inactivity Timeout, in 30 second intervals</param>
        /// </summary>  
        internal StartSerialSessionRequest(bool flushBuffer, int inactivityTimeout)
        {
            byte payload = (byte)(inactivityTimeout << 1);

            if (flushBuffer)
                payload = (byte)(payload & 0x01);

            this.messagePayload = payload;

        }

        /// <summary>
        /// [0]   = Flush Buffer
        /// [1:7] = Inativiity Timeout
        /// </summary>       
        [IpmiMessageData(0)]
        public byte MessagePayload
        {
            get { return this.messagePayload; }

        }

    }
}
