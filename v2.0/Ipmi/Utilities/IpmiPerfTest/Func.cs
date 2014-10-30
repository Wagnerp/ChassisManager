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

namespace IpmiPerfTest
{
    using System;
    using System.Text;
    using System.Globalization;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    internal static class Func
    {
        /// <summary>
        /// COM Port Name
        /// </summary>
        internal static string ComPort = string.Empty;

        /// <summary>
        /// integer determines test
        /// </summary>
        internal static int TestPass;

        /// <summary>
        /// indicates a session must be established
        /// </summary>
        internal static int Logon;

        /// <summary>
        /// Validates sessoin, Active Session or Set Session Privilege may fail 
        /// with 0x81 if session is already established. 
        /// </summary>
        internal static int ValidateSession;

        /// <summary>
        /// Session Logon Payloads.
        /// </summary>
        internal static List<byte[]> LogonRequests = new List<byte[]>(3)
        {
            // Ipmi.GetSessionChallengeRequest Seq: 0
            new byte[26] { 0xA0, 0x20, 0x18, 0xC8, 0x81, 0x00, 0x39, 0x04, 0x61, 0x64, 0x6D, 0x69, 0x6E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x39, 0xA5 },
            // Ipmi.ActivateSessionRequest Seq: 1
            new byte[31] {0xA0,0x20,0x18,0xC8,0x81,0x04,0x3A,0x04,0x04,0x61,0x64,0x6D,0x69,0x6E,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x2F,0xA5},
            // Ipmi.SetSessionPrivilegeLevelRequest Seq: 1
            new byte[10]  {0xA0,0x20,0x18,0xC8,0x81,0x04,0x3B,0x04,0x3C,0xA5}
        };

        /// <summary>
        /// Get Channel Authentication Capabilities Requests with incremental sequence numbers
        /// </summary>
        internal static List<byte[]> ChannelAuthRequests = new List<byte[]>(31)
        {
            // GetChannelAuthenticationCapabilitiesRequest Seq: 1
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x04,0x38,0x0E,0x04,0x31,0xA5 },
            // GetChannelAuthenticationCapabilitiesRequest Seq: 2
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x08,0x38,0x0E,0x04,0x2D,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 3
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x0C,0x38,0x0E,0x04,0x29,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 4
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x10,0x38,0x0E,0x04,0x25,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 5
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x14,0x38,0x0E,0x04,0x21,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 6
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x18,0x38,0x0E,0x04,0x1D,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 7
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x1C,0x38,0x0E,0x04,0x19,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 8
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x20,0x38,0x0E,0x04,0x15,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 9
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x24,0x38,0x0E,0x04,0x11,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 10
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x28,0x38,0x0E,0x04,0x0D,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 11
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x2C,0x38,0x0E,0x04,0x09,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 12
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x30,0x38,0x0E,0x04,0x05,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 13
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x34,0x38,0x0E,0x04,0x01,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 14
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x38,0x38,0x0E,0x04,0xFD,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 15
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x3C,0x38,0x0E,0x04,0xF9,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 16
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x40,0x38,0x0E,0x04,0xF5,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 17
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x44,0x38,0x0E,0x04,0xF1,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 18,0x
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x48,0x38,0x0E,0x04,0xED,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 19
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x4C,0x38,0x0E,0x04,0xE9,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 20,0x
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x50,0x38,0x0E,0x04,0xE5,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 21
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x54,0x38,0x0E,0x04,0xE1,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 22
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x58,0x38,0x0E,0x04,0xDD,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 23
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x5C,0x38,0x0E,0x04,0xD9,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 24
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x60,0x38,0x0E,0x04,0xD5,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 25
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x64,0x38,0x0E,0x04,0xD1,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 26
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x68,0x38,0x0E,0x04,0xCD,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 27
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x6C,0x38,0x0E,0x04,0xC9,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 28
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x70,0x38,0x0E,0x04,0xC5,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 29
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x74,0x38,0x0E,0x04,0xC1,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 30
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x78,0x38,0x0E,0x04,0xBD,0xA5},
            // GetChannelAuthenticationCapabilitiesRequest Seq: 31
            new byte[11] { 0xA0,0x20,0x18,0xC8,0x81,0x7C,0x38,0x0E,0x04,0xB9,0xA5}
        };

        /// <summary>
        /// Checks and sets the console arguments
        /// </summary>
        internal static bool CheckSyntax(string[] args)
        {
            //  return false if no
            // arguments are supplied.
            if (args.Length == 0)
            {
                return false;
            }
            // return false if ? is contained
            // in the first argument.
            else if (args[0].Contains("?"))
            {
                return false;
            }

            // argument paramater string
            string param;

            // argument value string
            string value;

            // input regex
            string regex = @"(?<=-|/)(?<arg>\w+):(?<value>[a-zA-Z0-9_-]+)";

            foreach (string arg in args)
            {
                // match regex pattern
                Match match = Regex.Match(arg, regex);

                // capture match success
                if (match.Success)
                {
                    // check the argument value is not nothing.
                    if (match.Groups["value"] != null)
                    {
                        // set the paramater
                        param = match.Groups["arg"].Value;
                        // set the argument value
                        value = match.Groups["value"].Value;

                        // switch upper case paramater
                        // and set variables
                        switch (param.ToUpper(CultureInfo.InvariantCulture))
                        {
                            case "P":
                                ComPort = value.ToString().Replace(" ", "");
                                break;
                            case "T":
                                if (Int32.TryParse(value, out TestPass))
                                { }
                                break;
                            case "L":
                                if (Int32.TryParse(value, out Logon))
                                { }
                                break;
                            case "V":
                                if (Int32.TryParse(value, out ValidateSession))
                                { }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(ComPort) && (TestPass >= 1 && TestPass <= 2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Convert Byte to Hex String
        /// </summary>
        internal static string ByteToHexString(byte byteval)
        {
            return ByteArrayToHexString(new byte[1] { byteval });
        }

        /// <summary>
        /// Convert Byte Array to Hex String
        /// </summary>
        /// <param name="Bytes"></param>
        /// <returns></returns>
        internal static string ByteArrayToHexString(byte[] Bytes)
        {
            StringBuilder Result = new StringBuilder();
            string HexAlphabet = "0123456789ABCDEF";

            foreach (byte B in Bytes)
            {
                Result.Append(HexAlphabet[(int)(B >> 4)]);
                Result.Append(HexAlphabet[(int)(B & 0xF)]);
            }
            return Result.ToString();
        }


    }
}
