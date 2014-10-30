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

using System;
using System.Linq;
using System.Net;

namespace ChassisValidation
{
    /// <summary>
    /// The factory class to generate REST channel instances.
    /// The instance will be derived from CmRestClientBase and implement
    /// TChannel interface.
    /// </summary>
    /// <typeparam name="TChannel">
    /// The interface including all the APIs.
    /// </typeparam>
    public class CmRestChannelFactory<TChannel>
    {
        public CmRestChannelFactory(string serviceUri)
        {
            this.ServiceUri = new Uri(serviceUri);
        }

        /// <summary>
        /// The URI of the REST service endpoint.
        /// </summary>
        public Uri ServiceUri { get; set; }

        /// <summary>
        /// The timeout value in milliseconds for waiting the response
        /// </summary>
        public Int32 Timeout { get; set; }

        /// <summary>
        /// The client credential.
        /// </summary>
        public NetworkCredential Credential { get; set; }

        /// <summary>
        /// Create a channel instance which implements the TChannel interface.
        /// </summary>
        /// <returns>
        /// An instance which implements the TChannel interface.
        /// </returns>
        public TChannel CreateChannel()
        {
            var type = ChassisManagerRestProxyGenerator.CreateType(typeof(TChannel), typeof(ChassisManagerRestClientBase));
            var obj = Activator.CreateInstance(type);
            var client = (ChassisManagerRestClientBase)obj;
            
            // pass over properties needed to make REST requests
            client.ServiceUri = this.ServiceUri;
            client.Credential = this.Credential;
            client.Timeout = this.Timeout;

            return (TChannel)obj;
        }
    }
}
