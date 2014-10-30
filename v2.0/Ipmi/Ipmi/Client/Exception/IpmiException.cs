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
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an IPMI Exception.
    /// </summary>
    [Serializable()]
    public class IpmiException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the IpmiException class.
        /// </summary>
        public IpmiException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the IpmiException class with the
        /// specified string.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public IpmiException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the IpmiException class with a
        /// specified error message and a reference to the inner exception that is the cause of
        /// this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception. If the innerException
        /// parameter is not a null reference, the current exception is raised in a catch block
        /// that handles the inner exception.
        /// </param>
        public IpmiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the IpmiException class with
        /// serialization information.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">
        /// The contextual information about the source or destination.
        /// </param>
        protected IpmiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
