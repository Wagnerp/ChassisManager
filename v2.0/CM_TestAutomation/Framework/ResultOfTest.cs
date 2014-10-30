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

namespace Microsoft.GFS.WCS.Test.Framework
{
    using System;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Xml;

    /// <summary>
    /// This class object represents a batch of tests to run against a Chassis Manager.
    /// </summary>
    [DataContract]
    public class ResultOfTest
    {
        /// <summary> Gets or sets Name. </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary> Gets FirstSuccessfulResponse. </summary>
        [DataMember]
        public string FirstSuccessfulResponse { get; private set; }

        /// <summary> Gets LastSuccessfulResponse. </summary>
        [DataMember]
        public string LastSuccessfulResponse { get; private set; }

        /// <summary> Gets FailedResponse. </summary>
        [DataMember]
        public string FailedResponse { get; private set; }

        /// <summary> Gets FailedResponseStatusCode. </summary>
        [DataMember]
        public HttpStatusCode? FailedResponseStatusCode { get; private set; }

        /// <summary> Gets or sets SequenceName. </summary>
        [DataMember]
        public string SequenceName { get; set; }

        /// <summary> Gets or sets RunAsUserName. </summary>
        [DataMember]
        public string RunAsUserName { get; set; }

        /// <summary> Gets or sets SequenceInstance. </summary>
        [DataMember]
        public int SequenceInstance { get; set; }

        /// <summary> Gets or sets RestUri. </summary>
        [DataMember]
        public string RestUri { get; set; }

        /// <summary> Gets or sets State.</summary>
        [DataMember]
        public TestRunState State { get; set; }

        /// <summary> Gets or sets State.</summary>
        [DataMember]
        public DateTime? StartTime { get; set; }

        /// <summary> Gets or sets TotalExecutionTime.</summary>
        [DataMember]
        public TimeSpan TotalExecutionTime { get; set; }

        /// <summary> Gets or sets IterationsExecuted.</summary>
        [DataMember]
        public uint IterationsExecutedSuccessfully { get; set; }

        /// <summary> Gets or sets ErrorMessage.</summary>
        [DataMember]
        public string ErrorMessage { get; set; }

        /// <summary> Gets AverageExecutionTime. </summary>
        [DataMember]
        public TimeSpan AverageExecutionTime { get; set; }

        /// <summary>
        /// Outputs key properties for easy display.
        /// </summary>
        /// <returns> A formatted string having key properties of object. </returns>
        public override string ToString()
        {
            var errMsg = this.FailedResponseStatusCode == null
                         ? string.Empty
                         : string.Format("{0}:{1}", this.FailedResponseStatusCode, this.ErrorMessage);
            return string.Format(
                "{0}\t{1}\t{2}; (x{3}={4}s) \t{5}",
                this.Name,
                this.RestUri,
                this.State,
                this.IterationsExecutedSuccessfully,
                this.TotalExecutionTime,
                errMsg);
        }

        /// <summary>
        /// Process HttpWeb response.
        /// </summary>
        /// <param name="response"> A Http web response object to be processed. </param>
        public void ProcessResponse(HttpWebResponse response)
        {
            if (response == null)
            {
                this.FailedResponse = "null Response";
                this.ErrorMessage = "null Response";
                this.State = TestRunState.RunFailed;
                return;
            }
            try
            {
                using (XmlReader xmlResult = new XmlTextReader(response.GetResponseStream()))
                {
                    xmlResult.ReadToFollowing("completionCode");
                    if (!xmlResult.ReadString().Equals("Success"))
                    {
                        this.State = TestRunState.RunFailed;
                        return;
                    }
                    else
                    {
                        this.State = TestRunState.RanSuccessfully;
                        this.IterationsExecutedSuccessfully++;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
