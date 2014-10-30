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
    /// <summary>
    /// Various states of a Batch run.
    /// </summary>
    public enum TestRunState
    {
        /// <summary> Unknown state. </summary>
        Unknown = 0,

        /// <summary> Run has not been started.  </summary>
        NotStarted = 1,

        /// <summary> Currently running.</summary>
        Running = 2,

        /// <summary> Successfully Ran.</summary>
        RanSuccessfully = 3,

        /// <summary> Run has failed. </summary>
        RunFailed = 99
    }
}
