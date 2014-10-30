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
    /// Represents a bad completion code from a IPMI response message.
    /// </summary>
    [Serializable()]
    public class IpmiResponseException : IpmiException
    {
        /// <summary>
        /// Non-zero completion code returned within a IPMI response message.
        /// </summary>
        private readonly byte completionCode;

        /// <summary>
        /// default constructor
        /// </summary>
        public IpmiResponseException(){
        }

        /// <summary>
        /// Initializes a new instance of the IpmiResponseException class.
        /// </summary>
        /// <remarks>Completion code returned within a IPMI response message.</remarks>
        public IpmiResponseException(byte completionCode)
        {           
            this.completionCode = completionCode;
        } 

        /// <summary>
        /// Initializes a new instance of the IpmiResponseException class with the
        /// specified string.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public IpmiResponseException(string message, byte completionCode)
            : base(message)
        {
            this.completionCode = completionCode;
        }

        /// <summary>
        /// Initializes a new instance of the IpmiResponseException class with the
        /// specified string.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public IpmiResponseException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the IpmiResponseException class with a
        /// specified error message and a reference to the inner exception that is the cause of
        /// this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception. If the innerException
        /// parameter is not a null reference, the current exception is raised in a catch block
        /// that handles the inner exception.
        /// </param>
        public IpmiResponseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the IpmiResponseException class with
        /// serialization information.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">
        /// The contextual information about the source or destination.
        /// </param>
        protected IpmiResponseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }


        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }


        /// <summary>
        /// Gets the completion code.
        /// </summary>
        /// <value>IPMI response message completion code.</value>
        public byte CompletionCode
        {
            get { return this.completionCode; }
        }
    }
}
