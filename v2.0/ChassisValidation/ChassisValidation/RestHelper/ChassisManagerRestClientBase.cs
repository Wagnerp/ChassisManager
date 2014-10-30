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
using System.Collections.Generic;
using System.Net;
using System.Net.Cache;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace ChassisValidation
{
    public abstract class ChassisManagerRestClientBase : ChassisManagerClientProxyBase
    {
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
        /// Makes a REST request given the REST API name and the parameters.
        /// </summary>
        /// <typeparam name="TResponse">
        /// The response type.
        /// </typeparam>
        protected override TResponse MakeRequest<TResponse>(string httpMethod, string apiName,
            IDictionary<string, object> apiParams)
        {
            if (string.IsNullOrWhiteSpace(apiName))
            {
                throw new ArgumentNullException("apiName");
            }
            if (this.ServiceUri == null)
            {
                throw new Exception("ServiceUri cannot be null");
            }

            // construct request uri
            string queryString = this.GetQueryStringParams(apiParams);

            var requestUri = new UriBuilder(this.ServiceUri)
            {
                Path = apiName,
                Query = queryString
            }.Uri;

            var request = (HttpWebRequest)WebRequest.Create(requestUri);
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            request.UserAgent = Dns.GetHostName();
            request.Method = httpMethod;

            if (this.Timeout != default(int))
            {
                request.Timeout = this.Timeout;
            }
            if (this.Credential != null)
            {
                request.PreAuthenticate = true;
                request.Credentials = this.Credential;
            }

            Log.Debug("CmRestProxy", string.Format("Request: {0}", requestUri.ToString()));
            
            // ignore CM service certificate validation errors
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, errors) => true;

            // get response from CM service
            object responseObject = null;
            var serializer = new DataContractSerializer(typeof (TResponse));
            var response = (HttpWebResponse)request.GetResponse();
            
            using (var stream = response.GetResponseStream())
            {
                responseObject = serializer.ReadObject(stream);
            }

            // make sure the returned object is not null
            // CM service should never return empty response
            if (responseObject == null)
            {
                throw new SerializationException();
            }
            
            // write the response to string for logging
            var responseString = new StringBuilder();
            using (var writer = XmlWriter.Create(responseString))
            {
                serializer.WriteObject(writer, responseObject);
            }
            Log.Debug("CmRestProxy", string.Format("Response: {0}", responseString.ToString()));

            return (TResponse)responseObject;
        }

        private string GetQueryStringParams(IDictionary<string, object> apiParams)       
        {
            StringBuilder queryStr = new StringBuilder();
            bool value;

            foreach (string key in apiParams.Keys)
            {
                //if the value is a bool then check if it is true, otherwise ignore it
                if (Boolean.TryParse(apiParams[key].ToString(), out value))
                {
                    if (value == true)
                    {
                        queryStr.Append(string.Format("{0}={1}&", key, value));
                    }
                }
                else
                {
                    queryStr.Append(string.Format("{0}={1}&", key, apiParams[key]));
                }
            }

            //Trim the last & character
            if (queryStr.Length > 0 && queryStr[queryStr.Length - 1] == '&')
            {
                queryStr = queryStr.Remove(queryStr.Length - 1, 1);
            }

            return queryStr.ToString();
        }
    }
}
