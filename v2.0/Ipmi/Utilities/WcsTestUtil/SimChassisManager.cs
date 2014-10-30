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
    using System.Threading;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Text;
    using Microsoft.GFS.WCS.ChassisManager;
    using Microsoft.GFS.WCS.ChassisManager.Ipmi;

    internal static class SimChassisManager
    {
        static private bool haveAllTestsPassed = true;
        const string strFail = "[FAIL] ";
        const string strPass = "[PASS] ";
        const byte success = 0x00;
        const byte failure = 0xff;
        const byte bladeId = 0x01;

        static List<string> failures = new List<string>();

        internal static void Initialize()
        {
            bool isSelfTest = true;
            int numRequesters = 1;

            bool bladeInitialized = false;


            CompletionCode completionCode = CommunicationDevice.Init();

            if (CompletionCodeChecker.Failed(completionCode) == true)
            {
                Console.WriteLine(strFail + "Failed to initialize CommunicationDevice");
                return;
            }

            Console.WriteLine(strPass + "CM CommunicationDevice.Init succeedded");

            if (isSelfTest == true)
            {
                numRequesters = 1;
            }
            else
            {
                numRequesters = 2;
            }

            Thread[] requesterThreads = new Thread[numRequesters];

            for (int i = 0; i < numRequesters; i++)
            {
                if (isSelfTest == true)
                {
                    requesterThreads[i] = new Thread(DoSelfTest);
                }
                requesterThreads[i].Start();
                Thread.Sleep(1000);
            }

            for (int i = 0; i < numRequesters; i++)
            {
                requesterThreads[i].Join();
            }

            if (haveAllTestsPassed == true)
            {
                Console.WriteLine(strPass + "All the Chassis Manager internal tests have passed");

                if (InitializeFacade())
                {
                    // blade was initialized
                    bladeInitialized = true;

                    Console.WriteLine(strPass + "All the Chassis Manager internal tests have passed");
                }
                else
                {
                    Console.WriteLine(strFail + "The WCS Blade in Slot 1 failed to properly initialize");
                }
            }
            else
            {
                Console.WriteLine(strFail + "Some of the Chassis Manager internal tests have failed");
            }

            if (bladeInitialized)
            {
                ExecuteAllBladeCommands();

                WcsBladeFacade.Release();
            }

            CommunicationDevice.Release();
        }

        private static void SendReceiveDevices(PriorityLevel priority, byte deviceType, ref byte[] request, int numDevices)
        {
            byte deviceId;
            byte[] response;

            for (byte i = 1; i <= numDevices; i++)
            {
                response = null;
                deviceId = i;
                CommunicationDevice.SendReceive(priority, deviceType, deviceId, request, out response);
                CheckAndPrintResponsePacket(deviceId, ref response);
            }
        }

        private static void CheckAndPrintResponsePacket(byte deviceId, ref byte[] response)
        {

            if (response == null)
            {
                Console.WriteLine(strFail + "response is null");
            }
            else
            {
                byte completionCode = response[0];
                if (CompletionCodeChecker.Succeeded((CompletionCode)completionCode) == true)
                {
                    Console.Write(strPass);
                }
                else
                {
                    haveAllTestsPassed = false;
                    Console.Write(strFail);
                }
                Console.Write("Response from Device {0}: ", deviceId);
                for (int i = 0; i < response.Length; i++)
                {
                    Console.Write("[{0:x}]", response[i]);
                }
                Console.WriteLine("");
            }
        }

        private static void DoSelfTest()
        {
            byte[] request = null;
            PriorityLevel priority = PriorityLevel.System;
            byte deviceType;
            const int numFansToTest = 6;


            // Set fan speed
            Console.WriteLine("# Testing SetFanSpeed");
            deviceType = (byte)DeviceType.Fan;
            request = new byte[4];
            request[0] = (byte)FunctionCode.SetFanSpeed;
            request[1] = 1;
            request[2] = 0;
            // PWM duty cycle
            request[3] = 100;
            SendReceiveDevices(priority, deviceType, ref request, 1);

            // Get fan speed
            Console.WriteLine("# Testing GetFanSpeed");
            deviceType = (byte)DeviceType.Fan;
            request = null;
            request = new byte[3];
            request[0] = (byte)FunctionCode.GetFanSpeed;
            SendReceiveDevices(priority, deviceType, ref request, numFansToTest);

            // Test blade_enable
            Console.WriteLine("# Testing blade_enable");
            deviceType = (byte)DeviceType.Power;
            request = null;
            request = new byte[3];
            request[0] = (byte)FunctionCode.TurnOnServer;
            SendReceiveDevices(priority, deviceType, ref request, ConfigLoaded.Population);

            Console.WriteLine("# Testing blade_enable status");
            deviceType = (byte)DeviceType.Power;
            request = null;
            request = new byte[3];
            request[0] = (byte)FunctionCode.GetServerPowerStatus;
            SendReceiveDevices(priority, deviceType, ref request, ConfigLoaded.Population);

            // Test watch dog timer
            Console.WriteLine("# Testing WatchDogTimer");
            deviceType = (byte)DeviceType.WatchDogTimer;
            request = null;
            request = new byte[3];
            request[0] = (byte)FunctionCode.EnableWatchDogTimer;
            SendReceiveDevices(priority, deviceType, ref request, 1);

            Console.WriteLine("# Testing ResetWatchDogTimer");
            deviceType = (byte)DeviceType.WatchDogTimer;
            request = null;
            request = new byte[3];
            request[0] = (byte)FunctionCode.ResetWatchDogTimer;
            SendReceiveDevices(priority, deviceType, ref request, 1);

            Console.WriteLine("# Testing StatusLed/TurnOnLed");
            deviceType = (byte)DeviceType.StatusLed;
            request = null;
            request = new byte[3];
            request[0] = (byte)FunctionCode.TurnOnLed;
            SendReceiveDevices(priority, deviceType, ref request, 1);

            Console.WriteLine("# Testing StatusLed/GetLedStatus");
            deviceType = (byte)DeviceType.StatusLed;
            request = null;
            request = new byte[3];
            request[0] = (byte)FunctionCode.GetLedStatus;
            SendReceiveDevices(priority, deviceType, ref request, 1);

            Console.WriteLine("# Testing RearAttentionLed/TurnOnLed");
            deviceType = (byte)DeviceType.RearAttentionLed;
            request = null;
            request = new byte[3];
            request[0] = (byte)FunctionCode.TurnOnLed;
            SendReceiveDevices(priority, deviceType, ref request, 1);

            Console.WriteLine("# Testing StatusLed/GetLedStatus");
            deviceType = (byte)DeviceType.StatusLed;
            request = null;
            request = new byte[3];
            request[0] = (byte)FunctionCode.GetLedStatus;
            SendReceiveDevices(priority, deviceType, ref request, 1);

            Console.WriteLine("# Testing TurnOffPowerSwitch");
            deviceType = (byte)DeviceType.PowerSwitch;
            request = null;
            request = new byte[3];
            request[0] = (byte)FunctionCode.TurnOffPowerSwitch;
            SendReceiveDevices(priority, deviceType, ref request, ConfigLoaded.NumPowerSwitches);

            Console.WriteLine("# Testing GetPowerSwitchStatus");
            request = null;
            request = new byte[3];
            request[0] = (byte)FunctionCode.GetPowerSwitchStatus;
            SendReceiveDevices(priority, deviceType, ref request, ConfigLoaded.NumPowerSwitches);

            Thread.Sleep(3000);

            Console.WriteLine("# Testing TurnOnPowerSwitch");
            deviceType = (byte)DeviceType.PowerSwitch;
            request = null;
            request = new byte[3];
            request[0] = (byte)FunctionCode.TurnOnPowerSwitch;
            SendReceiveDevices(priority, deviceType, ref request, ConfigLoaded.NumPowerSwitches);

            Console.WriteLine("# Testing GetPowerSwitchStatus");
            request = null;
            request = new byte[3];
            request[0] = (byte)FunctionCode.GetPowerSwitchStatus;
            SendReceiveDevices(priority, deviceType, ref request, ConfigLoaded.NumPowerSwitches);

        }

        private static bool InitializeFacade()
        {
            WcsBladeFacade.Initialize();

            return InitializeBlade();
        }

        private static bool InitializeBlade()
        {
            return WcsBladeFacade.InitializeClient(bladeId);
        }

        private static void ExecuteAllBladeCommands()
        {
            Tracer.Write("");
            Tracer.Write("*********************************************************");
            Tracer.Write(string.Format("***** BMC Command Execution: {0} ******", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            Tracer.Write("*********************************************************");
            Tracer.Write("");

            ResponseBase response = new CustomResponse(failure);

            BladePowerSwitch powerPacket = new BladePowerSwitch(bladeId);
            BladePowerStatePacket powerOff = powerPacket.SetBladePowerState(0x00);

            if (powerOff.CompletionCode == CompletionCode.Success)
            {
                Console.WriteLine("Power Blade Off");
            }
            else
            {
                Console.WriteLine("ERROR: Could not power Blade Off");
            }
                        
            BladePowerStatePacket powerOn = powerPacket.SetBladePowerState(0x01);
            Console.WriteLine("Power Blade On");
            if(powerOn.CompletionCode == CompletionCode.Success)
            {
                Console.WriteLine("Waiting for Decompression Time: {0}", powerOn.DecompressionTime);
                Tracer.Write("Waiting for Decompression Time: {0}", powerOn.DecompressionTime);
                Thread.Sleep(TimeSpan.FromSeconds(powerOn.DecompressionTime));
            }
                
            // Get the Power Status of a given device
            response = WcsBladeFacade.GetPowerStatus(bladeId);
            ValidateResponse(response, "Get Power Status");

            // Get Sensor Reading
            response = WcsBladeFacade.GetSensorReading(bladeId, 0x01);
            ValidateResponse(response, "Get Sensor Reading");

            // Get Blade Information
            BladeStatusInfo bladeInfo = WcsBladeFacade.GetBladeInfo(bladeId);
            ValidateResponse(response, "Get Blade Info");

            // Get Fru Device info for Given Device Id
            FruDevice fru = WcsBladeFacade.GetFruDeviceInfo(bladeId);
            ValidateResponse(response, "Get Fru Device");

            // Queries BMC for the currently set boot device.
            response = WcsBladeFacade.GetNextBoot(bladeId);
            ValidateResponse(response, "Get Next Boot");

            // Set next boot device
            response = WcsBladeFacade.SetNextBoot(bladeId, BootType.ForceDefaultHdd, true, false);
            ValidateResponse(response, "Set Next Boot");

            // Physically identify the computer by using a light or sound.
            if (WcsBladeFacade.Identify(bladeId, 255))
            {
                ValidateResponse(new CustomResponse(success), "Set Identify: On");
            }
            else
            {
                ValidateResponse(new CustomResponse(failure), "Set Identify: On");
            }

            // Physically identify the computer by using a light or sound.
            if (WcsBladeFacade.Identify(bladeId, 0))
            {
                ValidateResponse(new CustomResponse(success), "Set Identify: Off");
            }
            else
            {
                ValidateResponse(new CustomResponse(failure), "Set Identify: Off");
            }


            // Set the Power Cycle interval.
            if (WcsBladeFacade.SetPowerCycleInterval(bladeId, 0x08))
            {
                ValidateResponse(new CustomResponse(success), "Set Power Cycle Interval: 8");
            }
            else
            {
                ValidateResponse(new CustomResponse(failure), "Set Power Cycle Interval: 8");
            }

            // Set the computer power state Off
            if (WcsBladeFacade.SetPowerState(bladeId, IpmiPowerState.Off) == 0x00)
            {
                ValidateResponse(new CustomResponse(success), "SetPowerState: Off");
            }
            else
            {
                ValidateResponse(new CustomResponse(failure), "SetPowerState: Off");
            }

            // Set the computer power state On
            if (WcsBladeFacade.SetPowerState(bladeId, IpmiPowerState.On) == 0x00)
            {
                ValidateResponse(new CustomResponse(success), "SetPowerState: On");
            }
            else
            {
                ValidateResponse(new CustomResponse(failure), "SetPowerState: On");
            }

            // Gets BMC firmware revision.  Returns HEX string.
            response = WcsBladeFacade.GetFirmware(bladeId);
            ValidateResponse(response, "Get Firmware");

            // Queries BMC for the GUID of the system.
            response = WcsBladeFacade.GetSystemGuid(bladeId);
            ValidateResponse(response, "Get System Guid");

            // Reset SEL Log
            WcsBladeFacade.ClearSel(bladeId);
            ValidateResponse(response, "Clear Sel");

            // Recursively retrieves System Event Log entries.
            response = WcsBladeFacade.GetSel(bladeId);
            ValidateResponse(response, "Get Sel");

            //  Get System Event Log Information. Returns SEL Info.
            response = WcsBladeFacade.GetSelInfo(bladeId);
            ValidateResponse(response, "Get Sel Info");

            // Get Device Id Command
            response = WcsBladeFacade.GetDeviceId(bladeId);
            ValidateResponse(response, "Get DeviceId");

            // Get/Set Power Policy
            response = WcsBladeFacade.SetPowerRestorePolicy(bladeId, PowerRestoreOption.StayOff);
            ValidateResponse(response, "Set Power Restore Policy: Off");

            response = WcsBladeFacade.SetPowerRestorePolicy(bladeId, PowerRestoreOption.AlwaysPowerUp);
            ValidateResponse(response, "Set Power Restore Policy: Always On");

            // Switches Serial Port Access from BMC to System for Console Redirection
            response = WcsBladeFacade.SetSerialMuxSwitch(bladeId);
            ValidateResponse(response, "Set Serial MuxSwitch");

            // Disable Safe Mode.
            CommunicationDevice.DisableSafeMode();

            // Switches Serial port sharing from System to Bmc
            response = WcsBladeFacade.ResetSerialMux(bladeId);
            ValidateResponse(response, "Reset Serial Mux");

            // Get the current advanced state of the host computer.
            response = WcsBladeFacade.GetChassisState(bladeId);
            ValidateResponse(response, "Get Chassis State");

            // Get Processor Information
            response = WcsBladeFacade.GetProcessorInfo(bladeId, 0x01);
            ValidateResponse(response, "Get Processor Info");

            // Get Memory Information
            response = WcsBladeFacade.GetMemoryInfo(bladeId, 0x01);
            ValidateResponse(response, "Get Memory Info");

            // Get PCIe Info
            response = WcsBladeFacade.GetPCIeInfo(bladeId, 0x01);
            ValidateResponse(response, "Get PCIe Info");

            // Get Nic Info
            response = WcsBladeFacade.GetNicInfo(bladeId, 0x01);
            ValidateResponse(response, "Get Nic Info");

            // Get Hardware Info
            HardwareStatus hwStatus = WcsBladeFacade.GetHardwareInfo(bladeId, true, true,
                        true, true, true, true, true, true, true);

            if (hwStatus.CompletionCode == 0x00)
            { ValidateResponse(new CustomResponse(success), "Hardware Status"); }
            else
            { ValidateResponse(new CustomResponse(failure), "Hardware Status"); }

            // DCMI Get Power Limit Command
            response = WcsBladeFacade.GetPowerLimit(bladeId);
            ValidateResponse(response, "Get Power Limit");

            // DCMI Set Power Limit Command
            response = WcsBladeFacade.SetPowerLimit(bladeId, 220, 6000, 0x00, 0x00);
            ValidateResponse(response, "Set Power Limit");

            // DCMI Get Power Reading Command
            List<PowerReading> pwReadings = WcsBladeFacade.GetPowerReading(bladeId);
            if(pwReadings.Count > 0)
            {
                ValidateResponse(pwReadings[0], "Get Power Reading");
            }
            else
            {
                ValidateResponse(new CustomResponse(failure), "Get Power Reading");
            }

            // Activate/Deactivate DCMI power limit
            if (WcsBladeFacade.ActivatePowerLimit(bladeId, true))
            {
                ValidateResponse(new CustomResponse(success), "Activate Power Limit");
            }
            else
            {
                ValidateResponse(new CustomResponse(failure), "Activate Power Limit");
            }

            if (WcsBladeFacade.ActivatePowerLimit(bladeId, false))
            {
                ValidateResponse(new CustomResponse(success), "Activate Power Limit");
            }
            else
            {
                ValidateResponse(new CustomResponse(failure), "Activate Power Limit");
            }

            StressSession();

        }

        internal static void ShowResults()
        {
            Console.WriteLine();
            Console.WriteLine("IPMI Serial Command Results:");
            Console.WriteLine("===========================");
            Console.WriteLine();

            if (failures.Count > 0)
            {
                Console.WriteLine("Failures: {0}", failures.Count);
                Tracer.Write("Failures: {0}", failures.Count);
                foreach (string fail in failures)
                {
                    Console.WriteLine("Command: {0} Failed", fail);
                    Tracer.Write("Command: {0} Failed", fail);
                }

            }
            else
            {
                Console.WriteLine("All commands Passed");
                Tracer.Write("All commands Passed");
            }
        }

        private static void StressSession()
        {
                Tracer.Write("");
                Console.WriteLine();
                Console.WriteLine("IPMI Serial Session Stress:");
                Console.WriteLine("===========================");
                Tracer.Write("IPMI Serial Session Stress:");
                Tracer.Write("===========================");
                int failCount = 0;

                int count = 1;
                while (count < ConfigLoad.IpmiSessionStress)
                {
                    if (!WcsBladeFacade.LogOn(bladeId))
                    {
                        Console.WriteLine("IPMI Session Establishment Error. Iterations {0}.  See traces for Session Challenge, Activate Session and Session Privilege", count);
                        Tracer.WriteError("IPMI Session Establishment Error. Iterations {0}.  See traces for Session Challenge, Activate Session and Session Privilege", count);
                        failCount++;
                    }

                    count++;
                }

                if (failCount == 0)
                {
                    Console.WriteLine("IPMI Session Establishment. Passed: {0} iterations.", count);
                    Tracer.Write("IPMI Session Establishment. Passed: {0} iterations.", count);
                }
                else
                {
                    Console.WriteLine("IPMI Session Establishment. FAILED: {0} iterations: {1}.",failCount, count);
                    Tracer.Write("IPMI Session Establishment. FAILED: {0} iterations: {1}.", failCount, count);

                    failures.Add(string.Format("Session Establishment: FAILED: {0} iterations", failCount));

                    // Get GUID after sessuin failure test
                    ResponseBase response = WcsBladeFacade.GetSystemGuid(bladeId, false);
                    ValidateResponse(response, "Get DeviceId");

                    // Get Device Id after sessuin failure test
                    response = WcsBladeFacade.GetDeviceId(bladeId);
                    ValidateResponse(response, "Get DeviceId");
                }

                Console.WriteLine();
                Console.WriteLine("IPMI Serial Session Timeout:");
                Console.WriteLine("===========================");
                Tracer.Write("IPMI Serial Session Timeout:");
                Tracer.Write("===========================");
                failCount = 0;
                count = 1;
                while (count < ConfigLoad.IpmiSessionTimeout)
                {
                    DeviceGuid deviceGuid = WcsBladeFacade.GetSystemGuid(bladeId, false);

                    if (deviceGuid.CompletionCode != 0x00)
                    {
                        Console.WriteLine("ERROR: IPMI Serial Session Timeout. Get Guid outside of session failed with Completion Code:", deviceGuid.CompletionCode);
                        Tracer.WriteError("ERROR: IPMI Serial Session Timeout. Get Guid outside of session failed with Completion Code:", deviceGuid.CompletionCode);
                    }

                    if (!WcsBladeFacade.LogOn(bladeId))
                    {
                        Console.WriteLine("ERROR: IPMI Serial Session Timeout. Session Establishment Error. Iterations {0}.  See traces for Session Challenge, Activate Session and Session Privilege", count);
                        Tracer.WriteError("ERROR: IPMI Serial Session Timeout. Session Establishment Error. Iterations {0}.  See traces for Session Challenge, Activate Session and Session Privilege", count);
                    }

                    Console.WriteLine("Please wait 3 minutes 20 seconds for Session Timeout. Start Time: {0}  ", DateTime.Now.ToLocalTime());
                    Thread.Sleep(TimeSpan.FromSeconds(200));
                    Console.WriteLine("End Time: {0}", DateTime.Now.ToLocalTime());

                    deviceGuid = WcsBladeFacade.GetSystemGuid(bladeId, false);

                    if (deviceGuid.CompletionCode != 0x00)
                    {
                        Console.WriteLine("ERROR: IPMI Serial Session Timeout. Get Guid outside of session failed with Completion Code: {0}", deviceGuid.CompletionCode);
                        Tracer.WriteError("ERROR: IPMI Serial Session Timeout. Get Guid outside of session failed with Completion Code: {0}", deviceGuid.CompletionCode);
                        failCount++; 
                    }

                    SystemStatus state = WcsBladeFacade.GetChassisState(bladeId);

                    if (state.CompletionCode == 0x00)
                    {
                        Console.WriteLine("ERROR: IPMI Serial Session Timeout. Get Chassis Status responded after session timeout. Completion Code: {0}", deviceGuid.CompletionCode);
                        Tracer.WriteError("ERROR: IPMI Serial Session Timeout. Get Chassis Status responded after session timeout. Completion Code: {0}", deviceGuid.CompletionCode);
                        failCount++;
                    }

                    count++;
                }

                if (failCount == 0)
                {
                    Console.WriteLine("IPMI Session Timeout. Passed: {0} iterations.", count);
                    Tracer.Write("IPMI Session Timeout. Passed: {0} iterations.", count);
                }
                else
                {
                    Console.WriteLine("IPMI Session Timeout. FAILED: {0} iterations: {1}.", failCount, count);
                    Tracer.Write("IPMI Session Timeout. FAILED: {0} iterations: {1}.", failCount, count);

                    failures.Add(string.Format("IPMI Session Timeout: FAILED: {0} iterations", failCount));

                    // Get GUID after sessuin failure test
                    ResponseBase response = WcsBladeFacade.GetSystemGuid(bladeId, false);
                    ValidateResponse(response, "Get DeviceId");

                    // Get Device Id after sessuin failure test
                    response = WcsBladeFacade.GetDeviceId(bladeId);
                    ValidateResponse(response, "Get DeviceId");

                }

                Tracer.Write("");
                Console.WriteLine();
        }

        private static void ValidateResponse(ResponseBase response, string command)
        {
            if (response.CompletionCode == 0x00)
            {
                Tracer.WriteInfo("  {0} Command {1}", strPass, command);
            }
            else
            {
                Tracer.WriteError(" {0} Command {1} Completion Code: {2}", strFail, command, 
                    string.Format("{0:X2}h", response.CompletionCode));

                failures.Add(command);
            }
        }

        internal class CustomResponse : ResponseBase
        {
            internal CustomResponse(byte completionCode)
            { base.CompletionCode = completionCode; }

            internal override void SetParamaters(byte[] param)
            { }
        }

    }
}
