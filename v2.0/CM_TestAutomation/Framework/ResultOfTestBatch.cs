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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// This class object represents Execution of a batch and its results.
    /// </summary>
    [DataContract]
    public class ResultOfTestBatch
    {
        /// <summary> Initializes a new instance of the ResultOfTestBatch class. </summary>
        /// <param name="name"> Name of Batch. </param>
        /// <param name="chassisManagerEndPoint">Chassis Manager endpoint.</param>
        public ResultOfTestBatch(string name, string chassisManagerEndPoint)
        {
            this.Name = name;
            this.ChassisManagerEndPoint = chassisManagerEndPoint;
            this.TestResults = new List<ResultOfTest>();
            this.ExcludedApis = new List<string>();
            this.BatchState = TestRunState.NotStarted;
        }

        /// <summary> Gets Name. </summary>
        [DataMember]
        public string Name { get; private set; }

        /// <summary> Gets ChassisManagerEndPoint. </summary>
        [DataMember]
        public string ChassisManagerEndPoint { get; private set; }

        /// <summary> Gets or sets BatchStartTime. </summary>
        [DataMember]
        public DateTime? BatchStartTime { get; set; }

        /// <summary> Gets or sets BatchEndTime. </summary>
        [DataMember]
        public DateTime? BatchEndTime { get; set; }

        /// <summary> Gets or sets BatchEndTime. </summary>
        [DataMember]
        public TestRunState BatchState { get; set; }

        /// <summary> Gets or sets ExcludedApis. </summary>
        [DataMember]
        public IEnumerable<string> ExcludedApis { get; set; }

        /// <summary> Gets or sets NonExistingApis. </summary>
        [DataMember]
        public IEnumerable<string> NonExistingApis { get; set; }

        /// <summary> Gets TestRuns. </summary>
        [DataMember]
        public List<ResultOfTest> TestResults { get; private set; }

        /// <summary> Converts object to string.</summary>
        /// <returns> A formatted string. </returns>
        public override string ToString()
        {
            return string.Format(
                "BatchRun: Name={0}, State={1}, Chassis={2}, Start={3}, TimeNow={4}, TotalTestsExecuted={5}, Succ={6}",
                this.Name,
                this.BatchState,
                this.ChassisManagerEndPoint,
                this.BatchStartTime,
                DateTime.UtcNow,
                this.TestResults.Count,
                this.TestResults.Where(t => t.IterationsExecutedSuccessfully != 0));
        }

        /// <summary> Saves results to file. </summary>
        public void Save()
        {
            lock (this.TestResults)
            {
                var fName = string.Format("{0}-{1}.Results.xml", this.ChassisManagerEndPoint, this.Name);
                fName = Path.GetInvalidFileNameChars().Aggregate(fName, (ch, invalid) => ch.Replace(invalid, '_'));
                Helper.SaveToFile(this, fName);
            }
        }
    }
}
