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

namespace ChassisValidation
{
    /// <summary>
    /// Identifies a Type uniquely given its interface and base type.
    /// </summary>
    internal class ChassisManagerRestProxyKey
    {
        private readonly Type interfaceType;
        private readonly Type baseType;

        public ChassisManagerRestProxyKey(Type interfaceType, Type baseType)
        {
            this.interfaceType = interfaceType;
            this.baseType = baseType;
        }

        public override int GetHashCode()
        {
            return this.interfaceType.GetHashCode() ^ this.baseType.GetHashCode();
        }

        public override bool Equals(object o)
        {
            var key = o as ChassisManagerRestProxyKey;
            if (key == null)
            {
                return false;
            }
            return this.baseType == key.baseType && this.interfaceType == key.interfaceType;
        }
    }
}
