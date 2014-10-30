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

namespace Microsoft.GFS.WCS.Test.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// This exception is thrown whenever a CM REST API returns code other then OK.
    /// </summary>
    [Serializable,]
    public class CmTestRestStatusNotOk : Exception
    {
        /// <summary> Initializes a new instance of the CmTestRestStatusNotOk class. </summary>
        public CmTestRestStatusNotOk()
        {
        }

        /// <summary> Initializes a new instance of the CmTestRestStatusNotOk class. </summary>
        /// <param name="message">Error text.</param>
        public CmTestRestStatusNotOk(string message) : base(message)
        {
        }

        /// <summary> Initializes a new instance of the CmTestRestStatusNotOk class. </summary>
        /// <param name="message">Error text.</param>
        /// <param name="innerException">Original exception.</param>
        public CmTestRestStatusNotOk(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary> Initializes a new instance of the CmTestRestStatusNotOk class. </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected CmTestRestStatusNotOk(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
