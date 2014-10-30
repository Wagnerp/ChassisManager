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

namespace Microsoft.GFS.WCS.ChassisManager
{
    using System.Text;
    using System.Collections.Generic;
    using Microsoft.GFS.WCS.ChassisManager.Ipmi;

    public static class HelperFunction
    {
        /// <summary>
        /// Known Inlet sensor offsets needed by Manufacture Id and product.
        /// </summary>
        internal static Dictionary<WcsBladeIdentity, InletTempOffset> InletTempOffsets = new Dictionary<WcsBladeIdentity, InletTempOffset>(2) 
        {
            {new WcsBladeIdentity(7244, 1030), new InletTempOffset(0.0018, 2, 0.3321, 20.357)},
            {new WcsBladeIdentity(40092, 0), new InletTempOffset(18.831, 2, 33.597, 19.124)} //TODO:  Change Product Id to: 1030 when firmware aligns.
        };


        /// <summary>
        /// Stores Max Pwm Requirement.  Used for Data Center AHU integration.
        /// </summary>
        public static volatile byte MaxPwmRequirement;

        /// <summary>
        /// Generates the text representation of an array of bytes
        /// </summary>
        /// <param name="Bytes"></param>
        /// <returns></returns>
        public static string ByteArrayToText(byte[] byteArray)
        {
            return IpmiSharedFunc.ByteArrayToHexString(byteArray);
        }

        /// <summary>
        /// Byte to Hex string representation
        /// </summary>  
        public static string ByteToHexString(byte Bytes)
        {
            return IpmiSharedFunc.ByteToHexString(Bytes);
        }

        /// <summary>
        /// Convert string to byte array
        /// </summary>
        /// <param name="str">input string</param>
        /// <returns>byte array representing string</returns>
        internal static byte[] GetBytes(string str)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(str);
            return bytes;
        }
    }
}
