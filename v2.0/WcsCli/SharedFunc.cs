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
using System.ServiceModel;
using Microsoft.GFS.WCS.Contracts;
using System.Text;

namespace Microsoft.GFS.WCS.WcsCli
{
    static class SharedFunc
    {
        
        /// <summary>
        /// Global flat to Signals Serial Session is enabled
        /// </summary>
        private static volatile bool enableSerialSession = false;

        internal static bool ActiveSerialSession
        {
            get { return enableSerialSession; }
            private set { enableSerialSession = value; }
        }

        internal static void SetSerialSession(bool enabled)
        {
            enableSerialSession = enabled;
        }

        /// <summary>
        /// Byte to Hex string representation
        /// </summary>  
        internal static string ByteToHexString(byte bytevalue)
        {
            return string.Format("0x{0:X2}", bytevalue);
        }

        /// <summary>
        /// Byte Array to Hex string representation
        /// </summary>  
        internal static string ByteArrayToHexString(byte[] Bytes)
        {
            string result = string.Empty;
            result += "0x";

            foreach (byte B in Bytes)
            {
                result += string.Format("{0:X2}", B);
            }
            return result;
        }

        /// <summary>
        /// Compare two byte arrays. 
        /// </summary>
        internal static bool CompareByteArray(byte[] arrayA, byte[] arrayB)
        {
            bool response = false;
            if (arrayA.Length == arrayB.Length)
            {
                int i = 0;
                while ((i < arrayA.Length) && (arrayA[i] == arrayB[i]))
                {
                    i += 1;
                }

                if (i == arrayA.Length)
                {
                    response = true;
                }
            }
            return response;
        }

        /// <summary>
        /// Generic Exception Handling method
        /// </summary>
        internal static void ExceptionOutput(Exception ex)
        {
            if (ex is TimeoutException)
            {
                Console.WriteLine(WcsCliConstants.commandFailure + ". Communication with Chassis Manager timed out.");
            }
            else if (ex is FaultException)
            {
                Console.WriteLine(WcsCliConstants.commandFailure + "Communication fault.");
            }
            else if (ex is CommunicationException)
            {
                // When http status code is set to 500( internal server error), CommunicationException is thrown.
                // Displaying the ex.message which shows the http error description 
                Console.WriteLine(WcsCliConstants.commandFailure + " " + ex.Message);
            }
            else if (ex is Exception)
            {
                Console.WriteLine(WcsCliConstants.commandFailure + " Exception: " + ex.Message);
            }
        }
    }

    internal static class ResponseValidation
    {
        internal static bool ValidateBladeResponse(int bladeId, string message, ChassisResponse response, bool echoSuccess = true)
        {
            if (response == null)
            {
                Console.WriteLine(WcsCliConstants.serviceResponseEmpty);
                return false;
            }

            if (response.completionCode == Contracts.CompletionCode.Success)
            {
                if (echoSuccess)
                {
                    Console.WriteLine(WcsCliConstants.commandSuccess + " Blade " + bladeId + " " + (string.IsNullOrEmpty(message) ? WcsCliConstants.commandComplete : message));
                }
                return true;
            }
            else if (response.completionCode == Contracts.CompletionCode.Unknown)
            {
                Console.WriteLine(WcsCliConstants.bladeStateUnknown + " Blade " + bladeId);
                return false;
            }
            else if (response.completionCode == Contracts.CompletionCode.Failure)
            {
                Console.WriteLine(WcsCliConstants.commandFailure + " Blade " + bladeId + " " + response.completionCode.ToString());
                return false;
            }
            else if (response.completionCode == Contracts.CompletionCode.FirmwareDecompressing)
            {
                Console.WriteLine(WcsCliConstants.decompressing + " Blade " + bladeId + " " + (string.IsNullOrEmpty(response.statusDescription) ? WcsCliConstants.defaultTimeout : response.statusDescription));
                return false;
            }
            else
            {
                Console.WriteLine(WcsCliConstants.commandFailure + string.Format(" Blade {0} Completion Code: {1}", bladeId, response.completionCode));
                return false;
            }
        }

        internal static bool ValidateResponse(string message, ChassisResponse response, bool echoSuccess = true)
        {
            if (response == null)
            {
                Console.WriteLine(WcsCliConstants.serviceResponseEmpty);
                return false;
            }

            if (response.completionCode == Contracts.CompletionCode.Success)
            {
                if (echoSuccess)
                {
                    Console.WriteLine(WcsCliConstants.commandSuccess + " " + (string.IsNullOrEmpty(message) ? WcsCliConstants.commandComplete : message));
                }
                return true;
            }

            if (response.completionCode == Contracts.CompletionCode.Failure)
            {
                Console.WriteLine(WcsCliConstants.commandFailure);
                return false;
            }
            else if (response.completionCode == Contracts.CompletionCode.Timeout)
            {
                Console.WriteLine(WcsCliConstants.commandTimeout);
                return false;
            }
            else if (response.completionCode == Contracts.CompletionCode.SerialSessionActive)
            {
                Console.WriteLine(WcsCliConstants.commandSerialSessionActive);
                return false;
            }
            else if (response.completionCode == Contracts.CompletionCode.UserNotFound)
            {
                Console.WriteLine(WcsCliConstants.commandUserNotFound);
                return false;
            }
            else if (response.completionCode == Contracts.CompletionCode.UserPasswordDoesNotMeetRequirement)
            {
                Console.WriteLine(WcsCliConstants.commandUserPwdDoesNotMeetReq);
                return false;
            }
            else if (response.completionCode == Contracts.CompletionCode.FirmwareDecompressing)
            {
                Console.WriteLine(WcsCliConstants.decompressing + (string.IsNullOrEmpty(response.statusDescription) ? WcsCliConstants.defaultTimeout : response.statusDescription));
                return false;
            }
            else
            {
                Console.WriteLine(WcsCliConstants.commandFailure + string.Format(" Completion Code: {0}", response.completionCode));
                return false;
            }
        }
    }
    
}
