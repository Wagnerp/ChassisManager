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
    using System.Runtime.Serialization;

    /// <summary>
    /// This class object represents a User Credential.
    /// </summary>
    [DataContract]
    public class UserCredential
    {
        /// <summary>  Gets or sets Role. </summary>
        [DataMember(Order = 1, IsRequired = true)]
        public string Role { get; set; }

        /// <summary>  Gets or sets UserName. </summary>
        [DataMember(Order = 2, IsRequired = true)]
        public string UserName { get; set; }

        /// <summary> Gets or sets Password. </summary>
        [DataMember(Order = 3, IsRequired = true)]
        public string Password { get; set; }
    }
}
