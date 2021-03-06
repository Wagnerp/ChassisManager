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
    using System.Reflection;
    using System.Management;
    using System.Diagnostics;

    /// <summary>
    /// WMI Management class for IPMI RequestResponse invoke and transformation.
    /// </summary>
    internal sealed partial class IpmiWmiClient : IpmiClientNodeManager
    {
        private bool debugEnabled = false;

        /// <summary>
        /// ipmi method paramaters
        /// </summary>
        private ManagementBaseObject wmiPacket;

        /// <summary>
        /// microsoft_ipmi instances
        /// </summary>
        private ManagementObjectCollection ipmi_Instance;

        /// <summary>
        /// wmi method name
        /// </summary>
        private string ipmi_Method = "RequestResponse";

        public IpmiWmiClient(ManagementScope scope, bool debugEnabled)
        {
            // wmi scope
            ManagementScope wmiScope = scope;

            this.debugEnabled = debugEnabled;

            // Management Path
            ManagementPath path = new ManagementPath("microsoft_ipmi");

            // Management Class
            using (ManagementClass _ipmiClass = new ManagementClass(wmiScope, path, null))
            {
                // ipmi instances
                ipmi_Instance = _ipmiClass.GetInstances();

                // Get RequestResponse mothod paramaters
                wmiPacket = _ipmiClass.GetMethodParameters(ipmi_Method);
            }
        }

        /// <summary>
        /// Generics method IpmiSendReceive for easier use
        /// </summary>
        internal override T IpmiSendReceive<T>(IpmiRequest ipmiRequest)
        {
            return (T)this.IpmiSendReceive(ipmiRequest, typeof(T));
        }

        /// <summary>
        /// Create instance of ManagementBaseObject derived from packet frame
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private ManagementBaseObject GetManagementObject(byte[] message)
        {
            // create management base object
            ManagementBaseObject response = wmiPacket;

            // create payload
            byte[] payload = new byte[(message.Length - 5)];

            // extract command payload
            Buffer.BlockCopy(message, 5, payload, 0, payload.Length);

            // complete management object header
            response["Command"] = message[0]; // IpmiCommand
            response["NetworkFunction"] = message[1]; // IpmiFunction
            response["Lun"] = message[2]; // 0x00
            response["RequestData"] = payload; // Ipmi payload
            response["RequestDataSize"] = message[3]; // dataLength
            response["ResponderAddress"] = message[4]; // 32

            return response;
        }

        /// <summary>
        /// Send Receive Ipmi messages
        /// </summary>
        internal override IpmiResponse IpmiSendReceive(IpmiRequest ipmiRequest, Type responseType, bool allowRetry = true)
        {
            byte[] message = ipmiRequest.GetBytes(IpmiTransport.Wmi, 0x00);

            // Create the response based on the provided type
            ConstructorInfo constructorInfo = responseType.GetConstructor(Type.EmptyTypes);
            IpmiResponse ipmiResponse = (IpmiResponse)constructorInfo.Invoke(new Object[0]);

            // Serialize the IPMI request into bytes.
            ManagementBaseObject ipmiRequestMessage = this.GetManagementObject(message);

            // invoke new method options
            InvokeMethodOptions methodOptions = new InvokeMethodOptions(null, System.TimeSpan.FromMilliseconds(base.Timeout));

            // management return object
            ManagementBaseObject ipmiResponseMessage = null;

            // get instance and invoke RequestResponse method
            foreach (ManagementObject mo in ipmi_Instance)
            {
                ipmiResponseMessage = mo.InvokeMethod(ipmi_Method, wmiPacket, methodOptions);
            }

            if (ipmiResponseMessage == null)
            {
                // Assume the request timed out.
                ipmiResponse.CompletionCode = 0xA3;
            }
            else
            {
                
                ipmiResponse.CompletionCode = (byte)ipmiResponseMessage["CompletionCode"];

                if (ipmiResponse.CompletionCode == 0)
                {
                    try
                    {
                        uint dataLenght = (uint)ipmiResponseMessage["ResponseDataSize"];

                        // expected to be true, as ResponseDataSize includes completionCode
                        if(dataLenght != 0)
                        {
                            // extract response data array
                            byte[] responseData = (byte[])ipmiResponseMessage["ResponseData"];

                            // extract response message lenght
                            if (responseData != null)
                            {
                                int lenght = responseData.Length;

                                if (this.debugEnabled)
                                {
                                    string cmd = ipmiRequest.GetType().ToString();

                                    IpmiSharedFunc.WriteTrace(string.Format("Command: {0} Request: {1}", cmd, IpmiSharedFunc.ByteArrayToHexString(message)));

                                    if (responseData != null)
                                    {
                                        IpmiSharedFunc.WriteTrace(string.Format("Command: {0} Response: {1}", cmd, IpmiSharedFunc.ByteArrayToHexString(responseData)));
                                    }
                                    else
                                    {
                                        IpmiSharedFunc.WriteTrace(string.Format("Request: {0} Response: null", cmd));
                                    }
                                }

                                // initialize the response to set the paramaters.
                                ipmiResponse.Initialize(IpmiTransport.Wmi, responseData, lenght, 0x00);
                                ipmiResponseMessage = null;
                            }
                            else
                            {
                                // IpmiCannotReturnRequestedDataBytes, data lenght is greater than zero
                                // but responseData is null.
                                ipmiResponse.CompletionCode = 0xCA;

                                if (this.debugEnabled)
                                    IpmiSharedFunc.WriteTrace(string.Format("Response Lenght: {0} Response: null.  Asserting 0xCA CompletionCode ", dataLenght));
                            } 
                        }
                        else
                        {
                            // Asserting IpmiResponseNotProvided
                            ipmiResponse.CompletionCode = 0xCE;

                            if (this.debugEnabled)
                              IpmiSharedFunc.WriteTrace(string.Format("Unable to obtain Response Data Lenght: {0} Response: null.  Asserting 0xCE CompletionCode ", dataLenght));
                        }
                    }
                    catch (Exception ex)
                    {
                        // Response data Invalid, data convertion failed.
                        // unexpected error, return:
                        ipmiResponse.CompletionCode = 0xAD;
                        
                        if (this.debugEnabled)
                            IpmiSharedFunc.WriteTrace(string.Format("Exception Source: {0} Message{1}", ex.Source.ToString(), ex.Message.ToString()));
                    }
                }
                else
                {
                    if (this.debugEnabled)
                    {
                        // throw ipmi/dcmi response exception with a custom string message and the ipmi completion code
                        IpmiSharedFunc.WriteTrace(string.Format("Completion Code: " + IpmiSharedFunc.ByteToHexString(ipmiResponse.CompletionCode)));
                
                        if (ipmiResponseMessage == null)
                        IpmiSharedFunc.WriteTrace(string.Format("Request Type: {0} Response Packet: null Completion Code {1}", ipmiRequest.GetType().ToString(),            
                            IpmiSharedFunc.ByteToHexString(ipmiResponse.CompletionCode)));
                    }
                }
            }
            // Response to the IPMI request message.
            return ipmiResponse;
        }
    }
}
