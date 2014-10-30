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

namespace WcsTestUtil
{
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;
    using Microsoft.GFS.WCS.ChassisManager.Ipmi;
    using Microsoft.GFS.WCS.ChassisManager.Ipmi.NodeManager;

    class ProcCmd
    {
        IpmiClientNodeManager ipmi;

        bool showDetail = false;
        bool checkDefaultValues = false;

        private int WriteException(Exception ex)
        {
            Debug.WriteLine(string.Format("Command failed: {0}", ex.TargetSite.Name.ToString()));
            Tracer.WriteError(string.Format("Command failed: {0}", ex.TargetSite.Name.ToString()));
            Tracer.WriteError(ex.ToString());

            // Return 0 to indicate a test failure
            return 0;
        }

        public ProcCmd(IpmiClientNodeManager client, bool detail, bool checkDefault)
        {
            this.ipmi = client;
            this.showDetail = detail;
            this.checkDefaultValues = checkDefault;
        }

        #region Ipmi Commands

        #region NextBoot
        /// <summary>
        /// Queries BMC for the currently set boot device.
        /// </summary>
        /// <returns>Flags indicating the boot device.</returns>
        public int GetNextBoot()
        {
            try
            {
                NextBoot response = ipmi.GetNextBoot();

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Get Next Boot");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get Next Boot", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }

        }

        /// <summary>
        /// The helper for several boot type setting methods, as they
        /// essentially send the same sequence of messages.
        /// </summary>
        public int SetNextBootBios()
        {
            BootType bootType = BootType.ForceIntoBiosSetup;

            try
            {
                NextBoot response = ipmi.SetNextBoot(bootType, true, false, 0, false);

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "SetNextBootBios");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "SetNextBootBios", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// The helper for several boot type setting methods, as they
        /// essentially send the same sequence of messages.
        /// </summary>
        public int SetNextBootPxe()
        {
            BootType bootType = BootType.ForcePxe;

            try
            {
                NextBoot response = ipmi.SetNextBoot(bootType, true, false, 0, false);

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "SetNextBootPxe");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "SetNextBootPxe", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// The helper for several boot type setting methods, as they
        /// essentially send the same sequence of messages.
        /// </summary>
        public int SetNextBootNormal()
        {
            BootType bootType = BootType.ForceDefaultHdd;

            try
            {
                NextBoot response = ipmi.SetNextBoot(bootType, true, false, 0, false);

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "SetNextBootNormal");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "SetNextBootNormal", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }
        #endregion

        #region Identify

        /// <summary>
        /// Physically identify the computer by using a light or sound.
        /// </summary>
        /// <param name="interval">Identify interval in seconds or 255 for indefinite.</param>
        public int Identify()
        {
            try
            {
                if (ipmi.Identify(10))
                {
                    Console.WriteLine("Command Passed: {0}", "Identify");
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0}", "Identify");
                    return 0;
                }



            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }

        }

        /// <summary>
        /// Queries BMC for the GUID of the system.
        /// </summary>
        /// <returns>GUID reported by Baseboard Management Controller.</returns>
        public int GetSystemGuid()
        {
            try
            {
                DeviceGuid response = ipmi.GetSystemGuid();

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Device Guid");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Device Guid", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }

        }

        #endregion

        #region Sensor Reading

        /// <summary>
        ///  Get Sensor Data Repository. Returns completion code of the test.
        /// </summary>
        public int GetSdr()
        {
            try
            {
                SdrCollection response = ipmi.GetSdr();

                if ((response.CompletionCode == 0x00) && (response.Count > 0))
                {
                    Console.WriteLine("Command Passed: {0}", "Get Sdr");

                    if (showDetail)
                    {
                        Console.WriteLine();
                        Console.WriteLine(" Enumerating Objects for: {0}", "System Event Log");
                        Console.WriteLine(" ===================================");
                        Console.WriteLine();

                        foreach (SensorMetadataBase sdr in response)
                        {
                            Console.WriteLine("Sdr Type            {0}", sdr.GetType().ToString());
                            Console.WriteLine("Sensor Number:      {0}", sdr.SensorNumber);
                            Console.WriteLine("Sensor Type#:       {0} Detail: {1}", SharedFunc.ByteToHexString(sdr.RawSensorType), sdr.SensorType);
                            Console.WriteLine("Sensor Description: {0}", sdr.Description);
                            Console.WriteLine();
                        }

                        Console.WriteLine(" ===================================");
                        Console.WriteLine();
                    }
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get Sdr", response.CompletionCode);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        ///  Get Sensor Data Repository Information Incrementally. Returns SDR Info.
        /// </summary>
        public int GetSdrIncrement()
        {
            try
            {
                SdrCollection response = ipmi.GetSdrIncrement();

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Get Sdr Increment");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get Sdr Increment", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        ///  Get Sensor Data Record Information. Returns Sdr Info.
        /// </summary>
        public int GetSdrInfo()
        {
            try
            {
                SdrRepositoryInfo response = ipmi.GetSdrInfo();

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Get Sdr Info");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get Sdr Info", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Gets Sensor Reading
        /// </summary>
        public int GetSensorReading()
        {
            byte SensorNumber = ConfigLoad.SensorNo;
            byte SensorType = ConfigLoad.SensorType;

            try
            {
                SensorReading response = ipmi.GetSensorReading(SensorNumber, SensorType);

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Get Sensor Reading");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get Sensor Reading", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Gets Sensor Reading Factors
        /// </summary>
        public int GetSensorReadingFactors()
        {
            byte SensorNumber = ConfigLoad.SensorNo;
            byte SensorType = ConfigLoad.SensorType;

            try
            {
                SensorReading response = ipmi.GetSensorReading(SensorNumber, SensorType);

                if (response.CompletionCode == 0x00)
                {
                    ReadingFactorsResponse factors = ipmi.GetSensorFactors(SensorNumber, response.RawReading);

                    if (factors.CompletionCode == 0x00)
                    {
                        Console.WriteLine("Command Passed: {0}", "Get Sensor Reading Factors");
                    }
                    else
                    {
                        Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get Sensor Reading Factors", factors.CompletionCode);
                    }

                    if (showDetail)
                        EnumProp.EnumerableObject(factors);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get Sensor Reading", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Gets Sensor Type
        /// </summary>
        public int GetSensorType()
        {
            byte SensorNumber = ConfigLoad.SensorNo;

            try
            {

                SensorTypeCode response = ipmi.GetSensorType(SensorNumber);

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Get Sensor Type");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get Sensor Type", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        #endregion

        #region Power

        /// <summary>
        /// Set the computer power state.
        /// </summary>
        /// <param name="powerState">Power state to set.</param>
        public int SetPowerOff()
        {
            try
            {
                byte response = ipmi.SetPowerState(IpmiPowerState.Off);

                if (response == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Set Power Off");
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Set Power Off", response);
                }

                return response == 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }

        }

        /// <summary>
        /// Set the computer power state.
        /// </summary>
        /// <param name="powerState">Power state to set.</param>
        public int SetPowerOn()
        {
            try
            {
                byte response = ipmi.SetPowerState(IpmiPowerState.On);

                if (response == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Set Power On");
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Set Power On", response);
                }

                return response == 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }

        }

        /// <summary>
        /// Set the computer power state.
        /// </summary>
        /// <param name="powerState">Power state to set.</param>
        public int SetPowerReset()
        {
            try
            {
                byte response = ipmi.SetPowerState(IpmiPowerState.Reset);

                if (response == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Set Power Reset");
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Set Power Reset", response);
                }

                return response == 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }

        }

        /// <summary>
        /// Sets the Power-On time
        /// </summary>
        /// <param name="interval">00 interval is none, other integers are interpretted as seconds.</param>
        public int SetPowerOnTime()
        {
            try
            {
                bool response = ipmi.SetPowerOnTime(8);

                if (response)
                {
                    Console.WriteLine("Command Passed: {0}", "Set Power On Time");
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Set Power On Time", response);
                }

                return response == true ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }

        }

        /// <summary>
        /// Get the current power state of the host computer.
        /// </summary>
        /// <returns>ImpiPowerState enumeration.</returns>
        /// <devdoc>
        /// Originally used the 'Get ACPI Power State' message to retrieve the power state but not supported
        /// by the Arima's Scorpio IPMI card with firmware 1.10.00610100.  The 'Get Chassis Status' message
        /// returns the correct information for all IPMI cards tested.
        /// </devdoc>
        public int GetPowerState()
        {
            try
            {
                SystemStatus response = ipmi.GetChassisState();

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Get Power State");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get Power State", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Get the current power state of the host computer.
        /// </summary>
        /// <returns>ImpiPowerState enumeration.</returns>
        /// <devdoc>
        /// Originally used the 'Get ACPI Power State' message to retrieve the power state but not supported
        /// by the Arima's Scorpio IPMI card with firmware 1.10.00610100.  The 'Get Chassis Status' message
        /// returns the correct information for all IPMI cards tested.
        /// </devdoc>
        public int GetChassisState()
        {
            try
            {
                SystemStatus response = ipmi.GetChassisState();

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Get Chassis State");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get Chassis State", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Get Chassis Power Restore Policy.
        /// </summary>
        public int GetPowerRestorePolicy()
        {
            int test_result = 0;

            try
            {
                SystemStatus response = ipmi.GetChassisState();

                if (response.CompletionCode == 0x00)
                {
                    if (checkDefaultValues)
                    {
                        if (response.PowerOnPolicy == WcsTestUtilDefaults.powerRestorePolicy)
                        {
                            test_result = 1;
                        }
                        else
                        {
                            Console.WriteLine("Wrong default value. {0}: Expected: {1}. Value Read: {2}",
                                "PowerOnPolicy", WcsTestUtilDefaults.powerRestorePolicy, response.PowerOnPolicy);
                            test_result = 0;
                        }
                    }
                    else
                    {
                        test_result = 1;
                    }
                }
                else
                {
                    test_result = 0;
                }

                if (test_result == 1)
                {
                    Console.WriteLine("Command Passed: {0}", "Get Power Restore Policy");
                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get Power Restore Policy", response.CompletionCode);
                }
                return test_result;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Set Chassis Power Restore Policy.
        /// </summary>
        public int SetPowerRestorePolicyOff()
        {
            try
            {
                PowerRestorePolicy response = ipmi.SetPowerRestorePolicy(PowerRestoreOption.StayOff);

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Set Power Restore Policy Off");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Set Power Restore Policy Off", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }

        }

        /// <summary>
        /// Set Chassis Power Restore Policy.
        /// </summary>
        public int SetPowerRestorePolicyOn()
        {
            try
            {
                PowerRestorePolicy response = ipmi.SetPowerRestorePolicy(PowerRestoreOption.AlwaysPowerUp);

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Set Power Restore Policy On");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Set Power Restore Policy On", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }

        }

        /// <summary>
        /// Get the Power-On-Hours (POH) of the host computer.
        /// </summary>
        /// <returns>System Power On Hours.</returns>
        /// <remarks> Specification Note: Power-on hours shall accumulate whenever the system is in 
        /// the operational (S0) state. An implementation may elect to increment power-on hours in the S1 
        /// and S2 states as well.
        /// </remarks>
        public int PowerOnHours()
        {

            try
            {
                PowerOnHours response = ipmi.PowerOnHours();

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Power On Hours");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Power On Hours", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        #endregion

        #region FRU

        /// <summary>
        /// Write Fru Data Command.  Note:
        ///     The command writes the specified byte or word to the FRU Inventory Info area. This is a ‘low level’ direct 
        ///     interface to a non-volatile storage area. The interface does not interpret or check any semantics or 
        ///     formatting for the data being written.  The offset used in this command is a ‘logical’ offset that may or may not 
        ///     correspond to the physical address. For example, FRU information could be kept in FLASH at physical address 1234h, 
        ///     however offset 0000h would still be used with this command to access the start of the FRU information.
        ///     
        ///     IPMI FRU device data (devices that are formatted per [FRU]) as well as processor and DIMM FRU data always starts 
        ///     from offset 0000h unless otherwise noted.
        /// </summary>
        public int WriteFruData()
        {
            try
            {
                FruCommonHeader commonHeader = null;
                FruMultiRecordInfo multiRecordInfo = null;

                FruDevice fruData = ipmi.GetFruDeviceInfo(0);
                commonHeader = fruData.CommonHeader;
                multiRecordInfo = fruData.MultiRecordInfo;

                if (fruData.CompletionCode == 0x00)
                {
                    if (commonHeader == null)
                    {
                        Console.WriteLine("Command failed: {0} Completion Code: {1}", "Read Fru Data", fruData.CompletionCode);
                        Console.WriteLine("FRU common header is null. ");
                        return 0;
                    }

                    int offset = commonHeader.MultiRecordAreaStartingOffset;
                    // If multi record area has not been set up in the FRU EEPROM, the multi record offset will be 0.
                    // Set to 208 which is the default
                    if (offset == 0)
                    {
                        offset = 208;
                    }

                    // Write "Test" to the custom field
                    byte[] payLoad = new byte[15] { 0xD5, 0x01, 0x03, 0x0A, 0x1D, 0x00, 0x01, 0x37, 0x00, 0xFE, 0xCB, 0x54, 0x65, 0x73, 0x74 };
                    WriteFruDevice response = ipmi.WriteFruDevice(0, (ushort)offset, payLoad);

                    if (response.CompletionCode == 0x00)
                    {
                        Console.WriteLine("Command Passed: {0}", "Write Fru Data");

                        if (showDetail)
                            EnumProp.EnumerableObject(response);
                    }
                    else
                    {
                        Console.WriteLine("Command failed: {0} Completion Code: {1}", "Write Fru Data", response.CompletionCode);
                    }

                    return response.CompletionCode == 0 ? 1 : 0;
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Read Fru Data", fruData.CompletionCode);
                    return 0;
                }



            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }

        }

        /// <summary>
        /// Reads raw fru data, and returns a byte array.
        /// </summary>
        public int ReadFruData()
        {
            try
            {
                FruDevice response = ipmi.GetFruDeviceInfo(0, true);
                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Read Fru Data");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Read Fru Data", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Get Fru Inventory Area
        /// </summary>
        public int GetFruInventoryArea()
        {
            try
            {
                FruInventoryArea response = ipmi.GetFruInventoryArea(0);

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Fru Inventory Area");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Fru Inventory Area", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        #endregion

        #region Firmware

        /// <summary>
        /// Gets BMC firmware revision.  Returns HEX string.
        /// </summary>
        /// <returns>firmware revision</returns>
        public int GetFirmware()
        {
            try
            {
                BmcFirmware response = ipmi.GetFirmware();

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Get Firmware");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get Firmware", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }

        }

        /// <summary>
        /// Gets Device Id.  Returns HEX string.
        /// </summary>
        /// <returns>firmware revision</returns>
        public int GetDeviceId()
        {
            try
            {
                BmcDeviceId response = ipmi.GetDeviceId();

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Get DeviceId");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get DeviceId", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        #endregion

        #region User

        /// <summary>
        /// Get Users. Returns dictionary of User Ids and corresponding User names
        /// </summary>
        public int GetUsers()
        {
            try
            {

                Dictionary<int, string> response = ipmi.GetUsers();

                if (response.Count > 0)
                {
                    Console.WriteLine("Command Passed: {0}", "Get Users");

                }
                else
                {
                    Console.WriteLine("Command failed: {0}", "Get Users");
                }

                return response.Count == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Set Password
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <param name="operation">operation. setPassword, testPassword, disable\enable User</param>
        /// <param name="password">password to be set, 16 byte max for IPMI V1.5 and 20 byte max for V2.0</param>
        public int SetUserPassword()
        {
            try
            {
                int userId = 1;
                string password = ConfigLoad.Password;
                IpmiAccountManagment operation = IpmiAccountManagment.SetPassword;

                if (ipmi.SetUserPassword(userId, operation, password))
                {
                    Console.WriteLine("Command Passed: {0}", "Set User Password");
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0}", "Set User Password");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }

        }

        /// <summary>
        /// Set the User Name for a given User Id
        /// </summary>       
        public int SetUserName()
        {
            try
            {
                byte userId = (byte)ConfigLoad.UserId;
                string userName = ConfigLoad.SecondUser;

                if (ipmi.SetUserName(userId, userName))
                {
                    Console.WriteLine("Command Passed: {0}", "Set User Name");
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0}", "Set User Name");
                    return 0;
                }

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }

        }

        /// <summary>
        /// Get User Name
        /// </summary>
        /// <param name="userId">User Id.</param>
        public int GetUserName()
        {
            try
            {
                byte userId = (byte)ConfigLoad.UserId;
                UserName response = ipmi.GetUserName(userId);

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Get User Name");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0}", "Get User Name");
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Set User Access
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <param name="userLmit">User Privilege Level.</param>
        /// <param name="allowBitMod">True|False, allow modification of bits in request byte</param>
        /// <param name="callBack">True|False, allow callbacks, usually set to False</param>
        /// <param name="linkAuth">True|False, allow link authoriation, usually set to True</param>
        /// <param name="ipmiMessage">allow Impi messaging, usually set to True</param>
        /// <param name="channel">channel used to communicate with BMC, 1-7</param>
        public int SetUserAccess()
        {

            try
            {
                int userId = ConfigLoad.UserId;
                PrivilegeLevel priv = PrivilegeLevel.Administrator;
                bool allowBitMod = true;
                bool callback = false;
                bool linkAuth = true;
                bool ipmiMessage = false;
                int channel = 2;

                if (ipmi.SetUserAccess(userId, priv, allowBitMod, callback, linkAuth, ipmiMessage, channel))
                {
                    Console.WriteLine("Command Passed: {0}", "Set User Acces");
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0}", "Set User Acces");
                    return 0;
                }

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }

        }

        /// <summary>
        /// Get user privilege level
        /// </summary>
        public int GetUserPrivlige()
        {
            try
            {
                byte userId = (byte)ConfigLoad.UserId;
                byte channel = 2;
                UserPrivilege response = ipmi.GetUserPrivlige(userId, channel);

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Get User Privlige");
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get User Privlige", response.CompletionCode);
                }
                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        #endregion

        #region System Event Log

        /// <summary>
        /// Reset SEL Log
        /// </summary>
        public int ClearSel()
        {
            try
            {
                if (ipmi.ClearSel())
                {
                    Console.WriteLine("Command Passed: {0}", "Clear Sel");
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0}", "Clear Sel");
                    return 0;
                }

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        ///  Get System Event Log Information. Returns SEL Info.
        /// </summary>
        public int GetSelInfo()
        {
            try
            {
                SystemEventLogInfo response = ipmi.GetSelInfo();

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Get System Event Log Info");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get System Event Log Info", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Recursively retrieves System Event Log entries.
        /// </summary>
        public int GetSel()
        {
            try
            {
                SystemEventLog response = ipmi.GetSel();

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Get System Event Log");

                    if (showDetail)
                    {
                        Console.WriteLine();
                        Console.WriteLine(" Enumerating Objects for: {0}", "System Event Log");
                        Console.WriteLine(" ===================================");
                        Console.WriteLine();

                        if (response.EventLog != null)
                        {
                            foreach (SystemEventLogMessage msg in response.EventLog)
                            {
                                Console.WriteLine("Message Type {0}", msg.GetType().ToString());
                                Console.WriteLine("Message Payload Data: {0}", SharedFunc.ByteArrayToHexString(msg.RawPayload));
                            }
                        }
                        else
                        {
                            Console.WriteLine(" Error: Getting Event Log Records ");
                            Console.WriteLine();
                        }
                        Console.WriteLine(" ===================================");
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get System Event Log", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Add System Event Log entry
        /// </summary>
        public int AddSel()
        {
            try
            {
                byte[] selRecord = new byte[16];

                // Record ID
                selRecord[0] = 0x00;
                selRecord[1] = 0xAB;

                // Record Type (0x02 => System Event Record)
                selRecord[2] = 0x02;

                // Timestamp
                selRecord[3] = 0x53;
                selRecord[4] = 0xDB;
                selRecord[5] = 0x76;
                selRecord[6] = 0x4E;

                // Generator ID
                selRecord[7] = 0x01;
                selRecord[8] = 0x00;

                // EvM Rev
                selRecord[9] = 0x04;

                // Sensor Type (0x0C => Memory)
                selRecord[10] = 0x0C;

                // Sensor Number
                selRecord[11] = 0x87;

                // Event Dir/Type
                selRecord[12] = 0x6F;

                // Event Data 1-3 (Correctable ECC error)
                selRecord[13] = 0xA0;
                selRecord[14] = 0x00;
                selRecord[15] = 0x10;

                if (ipmi.AddSel(selRecord))
                {
                    Console.WriteLine("Command Passed: {0}", "Add Sel");
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0}", "Add Sel");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        #endregion

        #region Serial Modem

        /// <summary>
        /// Set Serial Mux Switch to System for Console Redirection.
        /// </summary>
        public int SetSerialMuxSwitch()
        {
            try
            {
                SerialMuxSwitch response;

                ChannelAuthenticationCapabilities auth = ipmi.GetAuthenticationCapabilities(PrivilegeLevel.Administrator);

                // Check for device type (Compute Blade)
                if ((auth.CompletionCode == 0x00) && (auth.AuxiliaryData == 0x04))
                {
                    // IPMI SPEC: Current Channel = 0x0E.  Serial Channel = 0x02
                    response = ipmi.SetSerialMuxSwitch(auth.ChannelNumber);

                    if (response.CompletionCode == 0x00)
                    {
                        Console.WriteLine("Command Passed: {0}", "Set Serial/Modem Mux Switch");

                        if (showDetail)
                            EnumProp.EnumerableObject(response);

                        return 1;
                    }
                    else
                    {
                        Console.WriteLine("Command failed: {0} Completion Code: {1}", "Set Serial/Modem Mux Switch", response.CompletionCode);
                        return 0;
                    }
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Set Serial/Modem Mux Switch", auth.CompletionCode);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Switches Serial control from System serial port to Bmc to close console redirection
        /// </summary>
        public int ResetSerialMux()
        {
            try
            {
                SerialMuxSwitch response;

                // Send GetAuthenticationCapabilities to get the channel number and identify the blade type
                ChannelAuthenticationCapabilities auth = ipmi.GetAuthenticationCapabilities(PrivilegeLevel.Administrator);

                // try 1 more time, as serial console snooping my not have detected the 1st request
                if (auth.CompletionCode != 0)
                {
                    auth = ipmi.GetAuthenticationCapabilities(PrivilegeLevel.Administrator);
                }

                // Check for device type (Compute Blade)
                if ((auth.CompletionCode == 0x00) && (auth.AuxiliaryData == 0x04))
                {
                    response = ipmi.ResetSerialMux(auth.ChannelNumber);

                    if (response.CompletionCode == 0x00)
                    {
                        Console.WriteLine("Command Passed: {0}", "Reset Serial/Modem Mux Switch");

                        if (showDetail)
                            EnumProp.EnumerableObject(response);

                        return 1;
                    }
                    else
                    {
                        Console.WriteLine("Command failed: {0} Completion Code: {1}", "Reset Serial/Modem Mux Switch", response.CompletionCode);
                        return 0;
                    }
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Reset Serial/Modem Mux Switch", auth.CompletionCode);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Stress Set and Reset Serial Mux Commands
        /// </summary>
        public int SerialMuxSwitchStress()
        {
            int num_iteration = 10;

            try
            {
                ChannelAuthenticationCapabilities auth;
                SerialMuxSwitch response;

                for (byte i = 1; i <= num_iteration; i++)
                {
                    Console.WriteLine();
                    Console.Write("Iteration {0}: ", i);

                    for (byte test_type = 0; test_type <= 1; test_type++)
                    {
                        // Get the Channel Authentication Capabilities to identify the blade type.
                        auth = ipmi.GetAuthenticationCapabilities(PrivilegeLevel.Administrator);

                        // Check for device type (Compute Blade)
                        if ((auth.CompletionCode == 0x00) && (auth.AuxiliaryData == 0x04))
                        {
                            if (test_type == 0)
                            {
                                response = ipmi.SetSerialMuxSwitch(auth.ChannelNumber);
                                Console.Write("Set Mux to System... ");
                            }
                            else
                            {
                                response = ipmi.ResetSerialMux(auth.ChannelNumber);
                                Console.Write("Set Mux to BMC... ");
                            }
                            if (response.CompletionCode == 00)
                            {
                                if (showDetail)
                                    EnumProp.EnumerableObject(response);
                            }
                            else
                            {
                                Console.WriteLine("Command failed: {0} Completion Code: {1}", "SerialMuxSwitchStress", response.CompletionCode);
                                return 0;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Command failed: {0} Completion Code: {1}", "SerialMuxSwitchStress", auth.CompletionCode);
                            return 0;
                        }

            }
                }
                Console.WriteLine();
                Console.WriteLine("Command Passed: {0}", "SerialMuxSwitchStress");
                return 1;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Ipmi Set Serial/Modem Configuration
        /// </summary>
        public int GetSerialTimeout()
        {
            try
            {
                SerialConfig.SessionTimeout test = ipmi.GetSerialConfig<SerialConfig.SessionTimeout>(new SerialConfig.SessionTimeout());

                if (test.TimeOut > 0)
                {
                    Console.WriteLine("Command Passed: {0} Timeout {1}", "GetSerialTimeout", test.TimeOut);
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0}", "GetSerialTimeout does not timeout");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }

        }

        /// <summary>
        /// Ipmi Set Serial/Modem Configuration
        /// </summary>
        public int SetSerialTimeout()
        {
            try
            {
                if (ipmi.SetSerialConfig<SerialConfig.SessionTimeout>(new SerialConfig.SessionTimeout(0x06)))
                {
                    Console.WriteLine("Command Passed: {0}", "Set Serial Timeout");
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0}", "Set Serial Timeout");
                    return 0;

                }
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }

        }

        /// <summary>
        /// Ipmi Set Serial/Modem Configuration
        /// </summary>
        public int SetSerialTermination()
        {
            try
            {
                if (ipmi.SetSerialConfig<SerialConfig.SessionTermination>(new SerialConfig.SessionTermination(false, true)))
                {
                    Console.WriteLine("Command Passed: {0}", "Set Serial Termination");
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0}", "Set Serial Termination");
                    return 0;
                }

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }

        }

        /// <summary>
        /// Ipmi Set Serial/Modem Configuration
        /// </summary>
        public int GetSerialTermination()
        {
            try
            {
                SerialConfig.SessionTermination test = ipmi.GetSerialConfig<SerialConfig.SessionTermination>(new SerialConfig.SessionTermination());

                if (test.SessionTimeout)
                {
                    Console.WriteLine("Command Passed: {0}", "Get Serial Termination does timeout");
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command Failed: {0}", "Get Serial Termination does not timeout");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }

        }

        /// <summary>
        /// Ipmi Get Channel Info command
        /// </summary>
        public int GetChannelInfo()
        {
            try
            {
                ChannelInfo response = ipmi.GetChannelInfo(0x0E);

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Get Channel Info");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get Channel Info", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        #endregion

        #region Session

        /// <summary>
        /// Negotiates the ipmi version and sets client accordingly. Also sets the authentication type for V1.5
        /// </summary>
        public int GetAuthenticationCapabilities()
        {
            try
            {
                ChannelAuthenticationCapabilities response = ipmi.GetAuthenticationCapabilities(PrivilegeLevel.Administrator);

                if (response.CompletionCode == 0x00)
                {
                    if (response.AuxiliaryData != 0x04 && response.AuxiliaryData != 0x05)
                    {
                        Console.WriteLine("Command failed: {0} Completion Code: {1}.  Reason Code: {2}", "Get Channel Authentication Capabilities", 
                            response.CompletionCode,
                            "Auxiliary Data Field does not meet specification");

                        return 0;
                    }

                    Console.WriteLine("Command Passed: {0}", "Get Channel Authentication Capabilities");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get Channel Authentication Capabilities", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }

        }

        #endregion

        #region JBOD

        /// <summary>
        /// Gets the Disk Status of JBODs
        /// </summary>
        public int GetDiskStatus()
        {
            try
            {
                DiskStatusInfo response = ipmi.GetDiskStatus();

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "JBOD Get Disk Status");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "JBOD Get Disk Status", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Gets the Disk Status of JBODs
        /// </summary>
        public int GetDiskInfo()
        {
            try
            {
                DiskInformation response = ipmi.GetDiskInfo();

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "JBOD Get Disk Info");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "JBOD Get Disk Info", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        #endregion

        #region OEM

        /// <summary>
        /// Gets Processor Information
        /// </summary>
        public int GetProcessorInfo()
        {
            try
            {
                ProcessorInfo response = ipmi.GetProcessorInfo((byte)ConfigLoad.ProcessorNo);

                if ((response.CompletionCode == 0x00) &&
                    (response.ProcessorType != ProcessorType.NoCpuPresent) &&
                    (response.ProcessorState == ProcessorState.Present) &&
                    (response.Frequency != 0))
                {
                    Console.WriteLine("Command Passed: {0}", "Get Processor Info");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get Processor Info", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Gets Memory Information
        /// </summary>
        public int GetMemoryInfo()
        {
            try
            {
                MemoryInfo response = ipmi.GetMemoryInfo((byte)ConfigLoad.MemoryNo);

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Get Memory Info");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get Memory Info", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }

        }

        /// <summary>
        /// Gets PCIe Information
        /// </summary>
        public int GetPCIeInfo()
        {
            try
            {
                PCIeInfo response = ipmi.GetPCIeInfo((byte)ConfigLoad.PCIeNo);

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Get PCIe Info");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get PCIe Info", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Gets Nic Information
        /// </summary>
        public int GetNicInfo()
        {
            try
            {
                NicInfo response = ipmi.GetNicInfo((byte)ConfigLoad.NicNo);

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Get Nic Info");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get Nic Info", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        #endregion

        #region Energy Storage Commands

        /// <summary>
        /// Get Energy Storage
        /// </summary>
        /// <returns>Flags indicating the boot device.</returns>
        public int VerifyEnergyStorage(EnergyStorageState state)
        {
            //Get Energy Storage
            EnergyStorage GetEnergyResponse = ipmi.GetEnergyStorage();

            try
            {

                if (GetEnergyResponse.CompletionCode == 0x00 &&
                    GetEnergyResponse.State == state)
                {
                    Console.WriteLine("Command Passed: {0} Completion Code: {1}", "Get Energy Storage", GetEnergyResponse.CompletionCode);
                    if (showDetail)
                        EnumProp.EnumerableObject(GetEnergyResponse);

                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get Energy Storage", GetEnergyResponse.CompletionCode);
                    return 0;
                }


            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Set Energy Storage to Unknown
        /// </summary>
        public int SetEnergyStorageToUnknown()
        {
            EnergyStorageState StorageState = EnergyStorageState.Unknown;
            Byte percentage = 50;
            ushort holdtime = 100;
            bool presence = false;

            try
            {
                bool setEnergyStorageResponse = ipmi.SetEnergyStorage(presence, StorageState, percentage, holdtime, DateTime.Now);
                if (setEnergyStorageResponse == true &&
                    VerifyEnergyStorage(StorageState) == 1)
                {
                    Console.WriteLine("Command Passed: {0} Return value: {1}", "Set Energy Storage To Unknown.", setEnergyStorageResponse);
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Return value: {1}", "Set Energy Storage to Unknown.", setEnergyStorageResponse);
                    return 0;
                }

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Set Energy Storage to Charging
        /// </summary>
        public int SetEnergyStorageToCharging()
        {
            EnergyStorageState StorageState = EnergyStorageState.Charging;
            Byte percentage = 50;
            ushort holdtime = 100;
            bool presence = false;

            try
            {
                bool setEnergyStorageResponse = ipmi.SetEnergyStorage(presence, StorageState, percentage, holdtime, DateTime.Now);
                if (setEnergyStorageResponse == true &&
                    VerifyEnergyStorage(StorageState) == 1)
                {
                    Console.WriteLine("Command Passed: {0} Return value: {1}", "Set Energy Storage to Charging.", setEnergyStorageResponse);
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Return value: {1}", "Set Energy Storage to Charging.", setEnergyStorageResponse);
                    return 0;
                }

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Set Energy Storage to Discharging
        /// </summary>
        public int SetEnergyStorageToDischarging()
        {
            EnergyStorageState StorageState = EnergyStorageState.Discharging;
            Byte percentage = 50;
            ushort holdtime = 100;
            bool presence = false;

            try
            {
                bool setEnergyStorageResponse = ipmi.SetEnergyStorage(presence, StorageState, percentage, holdtime, DateTime.Now);
                if (setEnergyStorageResponse == true &&
                    VerifyEnergyStorage(StorageState) == 1)
                {
                    Console.WriteLine("Command Passed: {0} Return value: {1}", "Set Energy Storage to Discharging.", setEnergyStorageResponse);
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Return value: {1}", "Set Energy Storage to Discharging.", setEnergyStorageResponse);
                    return 0;
                }

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Set Energy Storage to Floating
        /// </summary>
        public int SetEnergyStorageToFloating()
        {
            EnergyStorageState StorageState = EnergyStorageState.Floating;
            Byte percentage = 50;
            ushort holdtime = 100;
            bool presence = false;

            try
            {
                bool setEnergyStorageResponse = ipmi.SetEnergyStorage(presence, StorageState, percentage, holdtime, DateTime.Now);
                if (setEnergyStorageResponse == true &&
                    VerifyEnergyStorage(StorageState) == 1)
                {
                    Console.WriteLine("Command Passed: {0} Return value: {1}", "Set Energy Storage to Floating.", setEnergyStorageResponse);
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Return value: {1}", "Set Energy Storage to Floating.", setEnergyStorageResponse);
                    return 0;
                }

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }
        #endregion

        #region ActivateDeactivate PSU_Alert

        /// <summary>
        /// Verify Get PSU Alert
        /// </summary>
        private int VerifyGetPSUAlert(bool enableAutoProcHot)
        {
            try
            {
                PsuAlert getPsuAlertResponse = ipmi.GetPsuAlert();
                if (getPsuAlertResponse.CompletionCode == 0x00 &&
                    getPsuAlertResponse.AutoProchotEnabled == enableAutoProcHot)
                {
                    Console.WriteLine("Command Passed: {0} Completion Code: {1} Auto ProcHot {2}", "Get PSU Alert", getPsuAlertResponse.CompletionCode, getPsuAlertResponse.AutoProchotEnabled);

                    if (showDetail)
                    {
                        EnumProp.EnumerableObject(getPsuAlertResponse);
                    }

                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1} Auto ProcHot {2}", "Get PSU Alert", getPsuAlertResponse.CompletionCode, getPsuAlertResponse.AutoProchotEnabled);
                    return 0;
                }

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Get PSU Alert
        /// </summary>
        public int GetPSUAlert()
        {
            int test_result = 0;
            try
            {
                PsuAlert getPsuAlertResponse = ipmi.GetPsuAlert();
                if (getPsuAlertResponse.CompletionCode == 0x00)
                {
                    if (checkDefaultValues)
                    {
                        if (getPsuAlertResponse.AutoProchotEnabled == WcsTestUtilDefaults.autoProchotEnabled)
                        {
                            test_result = 1;
                        }
                        else
                        {
                            Console.WriteLine("Wrong default value. {0}: Expected: {1}. Value Read: {2}",
                                "AutoProchotEnabled", WcsTestUtilDefaults.autoProchotEnabled, getPsuAlertResponse.AutoProchotEnabled);
                            test_result = 0;
                        }
                    }
                    else
                    {
                        test_result = 1;
                    }
                }
                else
                {
                    test_result = 0;
                }

                if (test_result == 1)
                {
                    Console.WriteLine("Command Passed: {0}", "GetPSUAlert");
                    if (showDetail)
                        EnumProp.EnumerableObject(getPsuAlertResponse);
                }
                else
                {
                    Console.WriteLine("Command Failed: {0} Completion Code: {1}", "GetPSUAlert", getPsuAlertResponse.CompletionCode);
                }
                return test_result;

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Activate PSU Alert - NoAction
        /// </summary>
        public int SetPSUAlert()
        {
            bool setPSUALertResponse;
            PsuAlert psuAlert;

            try
            {
                //Set PSU Alert to true
                setPSUALertResponse = ipmi.SetPsuAlert(true);
                psuAlert = ipmi.GetPsuAlert();

                if (psuAlert.CompletionCode == 0x00 &&
                    setPSUALertResponse == true &&
                    psuAlert.BmcProchotEnabled == true)
                {
                    Console.WriteLine("Command Passed: {0} Return value: {1}", "Set PSU Alert. BmcProchot Enabled", psuAlert.CompletionCode);
                    // fall through to next step.
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Return value: {1}", "Set PSU Alert.", psuAlert.CompletionCode);
                    return 0;
                }
                //Set PSU Alert to false
                setPSUALertResponse = ipmi.SetPsuAlert(false);
                psuAlert = ipmi.GetPsuAlert();

                if (psuAlert.CompletionCode == 0x00 &&
                    setPSUALertResponse == true &&
                    psuAlert.BmcProchotEnabled == false)
                {
                    Console.WriteLine("Command Passed: {0} Return value: {1}", "Set PSU Alert.BmcProchot Disabled.", psuAlert.CompletionCode);
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Return value: {1}", "Set PSU Alert.", psuAlert.CompletionCode);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Activate PSU Alert - NoAction
        /// </summary>
        public int ActivatePSUAlert_NoAction()
        {
            BmcPsuAlertAction bmcAction = BmcPsuAlertAction.NoAction;
            bool enableAutoProcHot = false;
            bool removeCap = true;

            try
            {
                bool activatePsuAlertResponse = ipmi.ActivatePsuAlert(enableAutoProcHot, bmcAction, removeCap);
                if (activatePsuAlertResponse == true &&
                    VerifyGetPSUAlert(enableAutoProcHot) == 1)
                {
                    Console.WriteLine("Command Passed: {0} Return value: {1}", "Activate PSU Alert.", activatePsuAlertResponse);
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Return value: {1}", "Activate PSU Alert.", activatePsuAlertResponse);
                    return 0;
                }

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }
        /// <summary>
        /// Activate PSU Alert - ProcHotAndDpc
        /// </summary>
        public int ActivatePSUAlert_ProcHotAndDpc()
        {
            BmcPsuAlertAction bmcAction = BmcPsuAlertAction.ProcHotAndDpc;
            bool enableAutoProcHot = true;
            bool removeCap = false;
            try
            {
                bool activatePsuAlertResponse = ipmi.ActivatePsuAlert(enableAutoProcHot, bmcAction, removeCap);
                if (activatePsuAlertResponse == true &&
                    VerifyGetPSUAlert(enableAutoProcHot) == 1)
                {
                    Console.WriteLine("Command Passed: {0} Return value: {1}", "Activate PSU Alert.", activatePsuAlertResponse);

                    if (showDetail)
                        EnumProp.EnumerableObject(activatePsuAlertResponse);

                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Return value: {1}", "Activate PSU Alert.", activatePsuAlertResponse);

                    return 0;
                }

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }
        /// <summary>
        /// Activate PSU Alert - DpcOnly
        /// </summary>
        public int ActivatePSUAlert_DpcOnly()
        {
            BmcPsuAlertAction bmcAction = BmcPsuAlertAction.DpcOnly;
            bool enableAutoProcHot = false;
            bool removeCap = false;
            try
            {
                bool activatePsuAlertResponse = ipmi.ActivatePsuAlert(enableAutoProcHot, bmcAction, removeCap);
                if (activatePsuAlertResponse == true &&
                    VerifyGetPSUAlert(enableAutoProcHot) == 1)
                {
                    Console.WriteLine("Command Passed: {0} Return value: {1}", "Activate PSU Alert.", activatePsuAlertResponse);

                    if (showDetail)
                        EnumProp.EnumerableObject(activatePsuAlertResponse);

                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Return value: {1}", "Activate PSU Alert.", activatePsuAlertResponse);

                    return 0;
                }

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        #endregion

        #region Set/Get Default Power Limit

        /// <summary>
        /// Get Default Power Limit
        /// </summary>
        public int GetDefaultPowerLimit()
        {
            int test_result = 0;
            try
            {
                DefaultPowerLimit response = ipmi.GetDefaultPowerCap();
                if (response.CompletionCode == 0x00)
                {
                    if (checkDefaultValues)
                    {
                        if ((response.DefaultPowerCap == WcsTestUtilDefaults.defaultPowerCap) &&
                            (response.WaitTime == WcsTestUtilDefaults.defaultPowerCapDelay) &&
                            (response.DefaultCapEnabled == WcsTestUtilDefaults.dpcEnabled))
                        {
                            test_result = 1;
                        }
                        else
                        {
                            Console.WriteLine("Wrong default value. {0}: Expected: {1}. Value Read: {2}",
                                "DefaultPowerCap", WcsTestUtilDefaults.defaultPowerCap, response.DefaultPowerCap);
                            Console.WriteLine("Wrong default value. {0}: Expected: {1}. Value Read: {2}",
                                "WaitTime", WcsTestUtilDefaults.defaultPowerCapDelay, response.WaitTime);
                            Console.WriteLine("Wrong default value. {0}: Expected: {1}. Value Read: {2}",
                                "DefaultCapEnabled", WcsTestUtilDefaults.dpcEnabled, response.DefaultCapEnabled);
                            test_result = 0;
                        }
                    }
                    else
                    {
                        test_result = 1;
                    }
                }
                else
                {
                    test_result = 0;
                }

                if (test_result == 1)
                {
                    Console.WriteLine("Command Passed: {0}", "GetDefaultPowerLimit");
                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command Failed: {0} Completion Code: {1}", "GetDefaultPowerLimit", response.CompletionCode);
                }
                return test_result;

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Set/Get Default Power Limit. Do not enable capping
        /// </summary>
        public int SetDefaultPowerLimit_NoCapping()
        {
            ushort defaultPowerCap = 200;
            ushort waitTime = 50;
            bool enableCapping = false;
            DefaultPowerLimit GetDefaultPowerLimit;

            try
            {
                bool setDefaultPowerLimitResponse = ipmi.SetDefaultPowerLimit(defaultPowerCap, waitTime, enableCapping);
                if (setDefaultPowerLimitResponse == true)
                {
                    Console.WriteLine("Command Passed: {0} Return value: {1}", "Set Default Power Limit.", setDefaultPowerLimitResponse);

                }
                else
                {
                    Console.WriteLine("Command failed: {0} Return value: {1}", "Set Default Power Limit.", setDefaultPowerLimitResponse);
                    return 0;
                }

                //Verify Get Default Power Limit

                //Verify the set power cap values.
                GetDefaultPowerLimit = ipmi.GetDefaultPowerCap();

                if (GetDefaultPowerLimit.CompletionCode == 0x00 &&
                    GetDefaultPowerLimit.DefaultPowerCap == defaultPowerCap &&
                    GetDefaultPowerLimit.DefaultCapEnabled == enableCapping)
                {
                    Console.WriteLine("Command Passed: {0} Return value: {1}", "Get Default Power Limit.", GetDefaultPowerLimit.CompletionCode);
                    return 1;

                }
                else
                {
                    Console.WriteLine("Command failed: {0} Return value: {1}", "Get Default Power Limit.", GetDefaultPowerLimit.CompletionCode);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Set Default Power Limit. Enable capping
        /// </summary>
        public int SetDefaultPowerLimit_EnableCapping()
        {
            ushort defaultPowerCap = 210;
            ushort waitTime = 40;
            bool enableCapping = true;
            DefaultPowerLimit GetDefaultPowerLimit;

            try
            {
                bool setDefaultPowerLimitResponse = ipmi.SetDefaultPowerLimit(defaultPowerCap, waitTime, enableCapping);
                if (setDefaultPowerLimitResponse == true)
                {
                    Console.WriteLine("Command Passed: {0} Return value: {1}", "Set Default Power Limit.", setDefaultPowerLimitResponse);

                }
                else
                {
                    Console.WriteLine("Command failed: {0} Return value: {1}", "Activate PSU Alert.", setDefaultPowerLimitResponse);
                    return 0;
                }

                //Verify Get Default Power Limit

                GetDefaultPowerLimit = ipmi.GetDefaultPowerCap();
                if (GetDefaultPowerLimit.CompletionCode == 0x00 &&
                    GetDefaultPowerLimit.DefaultPowerCap == defaultPowerCap &&
                    GetDefaultPowerLimit.DefaultCapEnabled == enableCapping)
                {
                    Console.WriteLine("Command Passed: {0} Return value: {1}", "Get Default Power Limit.", GetDefaultPowerLimit.CompletionCode);
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Return value: {1}", "Get Default Power Limit.", GetDefaultPowerLimit.CompletionCode);
                    return 0;
                }

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        #endregion

        #region Set/GetNVDIMMM_Trigger

        /// <summary>
        /// Verify Get NVDIMM Values
        /// </summary>
        private int VerifyGetNVDIMMM_Trigger(NvDimmTriggerAction nvDimmAction, byte adrCompleteDelay, byte nvdimmPresentPoweroffDelay)
        {
            try
            {
                NvDimmTrigger nvDimmTriggerResponse = ipmi.GetNvDimmTrigger();
                if ((nvDimmTriggerResponse.CompletionCode == 0x00) &&
                    (nvDimmTriggerResponse.AdrTriggerType == nvDimmAction) &&
                    (nvDimmTriggerResponse.AdrCompleteDelay == adrCompleteDelay) &&
                    (nvDimmTriggerResponse.NvdimmPresentPowerOffDelay == nvdimmPresentPoweroffDelay))
                {
                    Console.WriteLine("Command Passed: {0} Completion Code: {1}. AdrTriggerType = {2}. AdrCompleteDelay = {3}. NvdimmPresentPowerOffDelay = {4}", 
                        "Get NVDIMM trigger.", 
                        nvDimmTriggerResponse.CompletionCode, nvDimmTriggerResponse.AdrTriggerType.ToString(),
                        nvDimmTriggerResponse.AdrCompleteDelay, nvDimmTriggerResponse.NvdimmPresentPowerOffDelay);

                    if (showDetail)
                        EnumProp.EnumerableObject(nvDimmTriggerResponse);
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command Failed: {0} Completion Code: {1}. AdrTriggerType = {2}. AdrCompleteDelay = {3}. NvdimmPresentPowerOffDelay = {4}",
                        "Get NVDIMM trigger.",
                        nvDimmTriggerResponse.CompletionCode, nvDimmTriggerResponse.AdrTriggerType.ToString(),
                        nvDimmTriggerResponse.AdrCompleteDelay, nvDimmTriggerResponse.NvdimmPresentPowerOffDelay);
                    return 0;
                }

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Get Nvdimm Trigger Values
        /// </summary>
        public int GetNvDimmTrigger()
        {
            int test_result = 0;

            try
            {
                NvDimmTrigger nvDimmTriggerResponse = ipmi.GetNvDimmTrigger();
                if (nvDimmTriggerResponse.CompletionCode == 0x00)
                {
                    if (checkDefaultValues)
                    {
                        if ((nvDimmTriggerResponse.AdrTriggerType == WcsTestUtilDefaults.nvdimmTriggerAction) &&
                            (nvDimmTriggerResponse.AdrCompleteDelay == WcsTestUtilDefaults.adrCompleteDelay) &&
                            (nvDimmTriggerResponse.NvdimmPresentPowerOffDelay == WcsTestUtilDefaults.nvdimmPresentDelay))
                        {
                            test_result = 1;
                        }
                        else
                        {
                            Console.WriteLine("Wrong default value. {0}: Expected: {1}. Value Read: {2}",
                                "AdrTriggerType", WcsTestUtilDefaults.nvdimmTriggerAction, nvDimmTriggerResponse.AdrTriggerType);
                            Console.WriteLine("Wrong default value. {0}: Expected: {1}. Value Read: {2}",
                                "AdrCompleteDelay", WcsTestUtilDefaults.adrCompleteDelay, nvDimmTriggerResponse.AdrCompleteDelay);
                            Console.WriteLine("Wrong default value. {0}: Expected: {1}. Value Read: {2}",
                                "NvdimmPresentPowerOffDelay", WcsTestUtilDefaults.nvdimmPresentDelay, nvDimmTriggerResponse.NvdimmPresentPowerOffDelay);
                            test_result = 0;
                        }
                    }
                    else
                    {
                        test_result = 1;
                    }
                }
                else
                {
                    test_result = 0;
                }

                if (test_result == 1)
                {
                    Console.WriteLine("Command Passed: {0}", "GetNvDimmTrigger");
                    if (showDetail)
                        EnumProp.EnumerableObject(nvDimmTriggerResponse);
                }
                else
                {
                    Console.WriteLine("Command Failed: {0} Completion Code: {1}", "GetNvDimmTrigger", nvDimmTriggerResponse.CompletionCode);
                }
                return test_result; 
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// SetNvDimmTrigger_Disabled
        /// </summary>
        public int SetNvDimmTrigger_Disabled()
        {
            NvDimmTriggerAction nvDimmAction = NvDimmTriggerAction.Disabled;
            bool assertTrigger = false;
            byte adrCompleteDelay = 10;
            byte nvdimmPresentPoweroffDelay = 120;

            try
            {
                bool setNvDimmTriggerResponse = ipmi.SetNvDimmTrigger(nvDimmAction, assertTrigger, adrCompleteDelay, nvdimmPresentPoweroffDelay);
                if (setNvDimmTriggerResponse == true &&
                    VerifyGetNVDIMMM_Trigger(nvDimmAction, adrCompleteDelay, nvdimmPresentPoweroffDelay) == 1)
                {
                    Console.WriteLine("Command Passed: {0} Return value: {1}", "Set NVDIMM trigger", setNvDimmTriggerResponse);
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Return value: {1}", "Set NVDIMM trigger", setNvDimmTriggerResponse);
                    return 0;
                }

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// SetNvDimmTrigger_PchAdrGpi
        /// </summary>
        public int SetNvDimmTrigger_PchAdrGpi()
        {
            NvDimmTriggerAction nvDimmAction = NvDimmTriggerAction.PchAdrGpi;
            bool assertTrigger = true;
            byte adrCompleteDelay = 10;
            byte nvdimmPresentPoweroffDelay = 120;

            try
            {
                bool setNvDimmTriggerResponse = ipmi.SetNvDimmTrigger(nvDimmAction, assertTrigger, adrCompleteDelay, nvdimmPresentPoweroffDelay);
                if (setNvDimmTriggerResponse == true &&
                    VerifyGetNVDIMMM_Trigger(nvDimmAction, adrCompleteDelay, nvdimmPresentPoweroffDelay) == 1)
                {
                    Console.WriteLine("Command Passed: {0} Return value: {1}", "Set NVDIMM trigger", setNvDimmTriggerResponse);
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Return value: {1}", "Set NVDIMM trigger", setNvDimmTriggerResponse);
                    return 0;
                }

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// SetNvDimmTrigger_PchSmiGpi
        /// </summary>
        public int SetNvDimmTrigger_PchSmiGpi()
        {
            NvDimmTriggerAction nvDimmAction = NvDimmTriggerAction.PchSmiGpi;
            bool assertTrigger = true;
            byte adrCompleteDelay = 10;
            byte nvdimmPresentPoweroffDelay = 120;

            try
            {
                bool setNvDimmTriggerResponse = ipmi.SetNvDimmTrigger(nvDimmAction, assertTrigger, adrCompleteDelay, nvdimmPresentPoweroffDelay);
                if (setNvDimmTriggerResponse == true &&
                    VerifyGetNVDIMMM_Trigger(nvDimmAction, adrCompleteDelay, nvdimmPresentPoweroffDelay) == 1)
                {
                    Console.WriteLine("Command Passed: {0} Return value: {1}", "Set NVDIMM trigger", setNvDimmTriggerResponse);
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Return value: {1}", "Set NVDIMM trigger", setNvDimmTriggerResponse);
                    return 0;
                }

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        #endregion

        #region Get BIOS Code
        /// <summary>
        /// Get BIOS Code
        /// </summary>
        public int GetBIOSCode()
        {
            byte bVersionCurrent = 0;

            try
            {
                BiosCode response = ipmi.GetBiosCode(bVersionCurrent);
                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0} Completion Code: {1}", "Get BIOS Code Current Version", response.CompletionCode);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get BIOS Code Current Version 0", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        #endregion

        #region ThermalControl
        /// <summary>
        /// Get Thermal Control Values
        /// </summary>
        public int GetThermalControl()
        {
            int test_result = 0;

            try
            {
                ThermalControl response = ipmi.ThermalControl(ThermalControlFunction.GetStatus);
                if (response.CompletionCode == 0x00)
                {
                    if (checkDefaultValues)
                    {
                        if (response.Status == WcsTestUtilDefaults.ThermalControlFeatureStatus)
                        {
                            test_result = 1;
                        }
                        else
                        {
                            Console.WriteLine("Wrong default value. {0}: Expected: {1}. Value Read: {2}",
                                "Status", WcsTestUtilDefaults.ThermalControlFeatureStatus, response.Status);
                            test_result = 0;
                        }
                    }
                    else
                    {
                        test_result = 1;
                    }
                }
                else
                {
                    test_result = 0;
                }

                if (test_result == 1)
                {
                    Console.WriteLine("Command Passed: {0}", "GetThermalControl");
                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command Failed: {0} Completion Code: {1}", "GetThermalControl", response.CompletionCode);
                }
                return test_result;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Set Thermal Control Values
        /// </summary>
        public int SetThermalControl()
        {
            try
            {
                ThermalControl response = ipmi.ThermalControl(ThermalControlFunction.Disable);
                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "SetThermalControl");
                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                    
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command Failed: {0} Completion Code: {1}", "SetThermalControl", response.CompletionCode);
                    return 0;
                }

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        #endregion

        #endregion

        #region DCMI Commands

        /// <summary>
        /// DCMI Get Power Limit Command
        /// </summary>
        public int GetPowerLimit()
        {
            try
            {
                PowerLimit response = ipmi.GetPowerLimit();

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: {0}", "Get Power Limit");

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Get Power Limit", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// DCMI Set Power Limit Command
        /// </summary>
        public int SetPowerLimit()
        {
            short watts = ConfigLoad.LimitWatts;
            int correctionTime = ConfigLoad.CorrectionTime;
            byte action = ConfigLoad.Action;
            short samplingPeriod = ConfigLoad.SamplingPeriod;

            try
            {
                ActivePowerLimit response = ipmi.SetPowerLimit(watts, correctionTime, action, samplingPeriod);

                string message = string.Format("Set Power Limit (Watts: {0} CorrectionTime: {1} Action: {2} Sample Period{3}",
                watts.ToString(), correctionTime.ToString(), action.ToString(), samplingPeriod.ToString());
                Console.WriteLine(message);

                if (response.CompletionCode == 0x00)
                {
                    Console.WriteLine("Command Passed: Set Power Limit (Watts: {0} CorrectionTime: {1} Action: {2} Sample Period: {3})",
                        watts.ToString(), correctionTime.ToString(), action.ToString(), samplingPeriod.ToString());

                    if (showDetail)
                        EnumProp.EnumerableObject(response);
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Set Power Limit", response.CompletionCode);
                }

                return response.CompletionCode == 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// DCMI Get Power Reading Command
        /// </summary>
        public int GetAdvancedPowerReading()
        {
            try
            {
                List<PowerReading> response = ipmi.GetAdvancedPowerReading();

                if (response.Count > 0)
                {
                    if (response[0].CompletionCode == 0x00)
                    {
                        Console.WriteLine("Command Passed: {0}", "Get Advanced Power Reading");

                        if (showDetail)
                            EnumProp.EnumerableObject(response[0]);
                    }
                    else
                    {
                        Console.WriteLine("Command failed: {0} ", "Get Advanced Power Reading");
                    }

                    return response[0].CompletionCode == 0 ? 1 : 0;
                }
                else
                {
                    Console.WriteLine("Command failed: {0} ", "Get Advanced Power Reading");
                    return 0;
                }


            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        public int GetPowerReading()
        {
            try
            {

                List<PowerReading> response = ipmi.GetPowerReading();

                if (response.Count > 0)
                {
                    if (response[0].CompletionCode == 0x00)
                    {
                        Console.WriteLine("Command Passed: {0}", "Get Power Reading");

                        if (showDetail)
                            EnumProp.EnumerableObject(response[0]);
                    }
                    else
                    {
                        Console.WriteLine("Command failed: {0} ", "Get Power Reading");
                    }

                    return response[0].CompletionCode == 0 ? 1 : 0;
                }
                else
                {
                    Console.WriteLine("Command failed: {0} ", "Get Power Reading");
                    return 0;
                }


            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        /// <summary>
        /// Activate/Deactivate DCMI power limit
        /// </summary>
        /// <param name="enable">Activate/Deactivate</param>
        public int ActivatePowerLimit()
        {
            try
            {

                if (ipmi.ActivatePowerLimit(true))
                {
                    Console.WriteLine("Command Passed: {0}", "Activate Power Limit");
                    return 1;
                }
                else
                {
                    Console.WriteLine("Command failed: {0}", "Activate Power Limit");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }

        #endregion

        #region NodeManager Commands

        /// <summary>
        /// Ipmi Forwarding Send Message Command
        /// </summary>
        public int SendMessage()
        {
            try
            {

                PolicyControlRequest policy = new PolicyControlRequest(NodeManagerPolicy.GlobalEnablePolicy, NodeManagerDomainId.Platform, 0x01);
                SendNodeMangerMessage ipmiFwdMsg = ipmi.SendNodeManagerRequest<PolicyControlResponse>(0x06, 0x2C, policy);

                if (ipmiFwdMsg.CompletionCode == 0x00)
                {
                    byte req = ipmiFwdMsg.RqSeq;

                    if (ipmiFwdMsg.MessageData != null)
                    {
                        PolicyControlResponse response = (PolicyControlResponse)ipmiFwdMsg.Response;

                        if (ipmiFwdMsg.Response.CompletionCode == 0x00)
                        {
                            Console.WriteLine("Command Passed: {0} Completion Code: {1} Payload {2}",
                                "Send Message",
                                ipmiFwdMsg.CompletionCode,
                                IpmiSharedFunc.ByteArrayToHexString(response.ManufactureId));
                            return 1;
                        }
                        else
                        {
                            Console.WriteLine("Command failed: Send Message. No Manufacture Id.  Confirm Payload was returned");
                            return 0;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Command failed: {0} Error: No payload, although Completion Code: {1}", "Send Message", ipmiFwdMsg.CompletionCode);
                        return 0;
                    }
                }
                else
                {
                    Console.WriteLine("Command failed: {0} Completion Code: {1}", "Send Message", ipmiFwdMsg.CompletionCode);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return WriteException(ex);
            }
        }


        #endregion

    }
}
