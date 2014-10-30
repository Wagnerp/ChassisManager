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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.GFS.WCS.ChassisManager.Ipmi;
using System.Management;

namespace GetDeviceId
{
    class Program
    {
        static void Main(string[] args)
        {
            // servername
            string servername = Environment.MachineName;

            try
            {
                // wmi scope for target server
                ManagementScope _scope = new ManagementScope("\\\\" + servername + "\\root\\wmi");

                // initialize instance of the IPMI WMI Client
                IpmiWmiClient wmi = new IpmiWmiClient(_scope);

                BmcDeviceId deviceId = wmi.GetDeviceId();

                if (deviceId.CompletionCode == 0)
                {
                    Console.WriteLine("Firmware Version: {0}", deviceId.Firmware);
                    Console.WriteLine("Product  Id:      {0}", deviceId.ProductId);
                    Console.WriteLine("Manufacture Id:   {0}", deviceId.ManufactureId);
                }
                else
                {
                    Console.WriteLine("Ipmi response error: {0}", IpmiSharedFunctions.ByteToHexString(deviceId.CompletionCode));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Wmi Error: {0} Cause: {1}", ex.Source, ex.Message);
            }
        }
    }
}
