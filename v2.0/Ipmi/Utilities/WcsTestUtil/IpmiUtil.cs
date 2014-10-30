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
    using System.Collections.Generic;
    using System.IO.Ports;
    using System.Linq;
    using System.Management;
    using System.Text.RegularExpressions;
    using System.Reflection;
    using System.Threading;
    using System.Diagnostics;
    using Microsoft.GFS.WCS.ChassisManager;
    using Microsoft.GFS.WCS.ChassisManager.Ipmi;

    class IpmiUtil
    {

        #region Private Variables

        /// <summary>
        /// Supported Command List
        /// </summary>
        static SortedDictionary<int, string> commands = new SortedDictionary<int, string>() 
        {
            {1, "GetNextBoot"},
            {2, "SetNextBootBios"},
            {3, "SetNextBootPxe"},
            {4, "SetNextBootNormal"},
            
            {6, "Identify"},
            {7, "GetSystemGuid"},

            {8, "SendMessage"},

            {10, "GetSdr"},
            {11, "GetSdrInfo"},
            {12, "GetSensorReading"},
            {13, "GetSensorReadingFactors"},
            {14, "GetSensorType"},

            {20, "SetPowerOff"},
            {21, "SetPowerOn"},
            {22, "SetPowerReset"},
            {23, "SetPowerOnTime"},
            {24, "GetPowerState"},
            {25, "GetChassisState"},
            {26, "GetPowerRestorePolicy"},
            {27, "SetPowerRestorePolicyOn"},
            {28, "SetPowerRestorePolicyOff"},

            {30, "WriteFruData"},
            {31, "ReadFruData"},
            {32, "GetFruInventoryArea"},
            
            {40, "GetFirmware"},          
            {41, "GetDeviceId"},
            
            {50, "ClearSel"},
            {51, "GetSelInfo"},
            {52, "AddSel"},
            {53, "GetSel"},
            
            {60, "SetSerialMuxSwitch"},
            {61, "ResetSerialMux"},
            {62, "SerialMuxSwitchStress"},
            
            {70, "GetChannelInfo"},
            {71, "GetAuthenticationCapabilities"},
            {72, "GetProcessorInfo"},
            {73, "GetMemoryInfo"},
            {74, "GetPCIeInfo"},
            {75, "GetNicInfo"},
            
            {80, "GetPowerLimit"},    
            {81, "SetPowerLimit"}, 
            {82, "ActivatePowerLimit"},
            {83, "GetPowerReading"},
            
            {90, "SetEnergyStorageToUnknown"},
            {91, "SetEnergyStorageToCharging"},
            {92, "SetEnergyStorageToDischarging"},
            {93, "SetEnergyStorageToFloating"},
            
            {100, "ActivatePSUAlert_NoAction"},
            {101, "ActivatePSUAlert_ProcHotAndDpc"},
            {102, "GetPSUAlert"},
            {103, "SetPSUAlert"},
            {104, "ActivatePSUAlert_DpcOnly"},
            {105, "GetDefaultPowerLimit"},
            {106, "SetDefaultPowerLimit_NoCapping"},
            {107, "SetDefaultPowerLimit_EnableCapping"},
            
            {110, "GetNvDimmTrigger"},
            {111, "SetNvDimmTrigger_Disabled"},
            {112, "SetNvDimmTrigger_PchAdrGpi"},
            {113, "SetNvDimmTrigger_PchSmiGpi"},
            
            {120, "GetBIOSCode"},

            {130, "GetThermalControl"},
            {131, "SetThermalControl"}
        };

        /// <summary>
        /// Commands with Default Values to Check
        /// </summary>
        static SortedDictionary<int, string> commandsDefaultValues = new SortedDictionary<int, string>() 
        {
            {26, "GetPowerRestorePolicy"},
            {102, "GetPSUAlert"},
            {105, "GetDefaultPowerLimit"},
            {110, "GetNvDimmTrigger"},
            {130, "GetThermalControl"}
        };

        /// <summary>
        /// Commands not allowed for Inband vesion.
        /// </summary>
        static Dictionary<int, string> InbandForbidden = new Dictionary<int, string>() 
        { 
            {8, "SendMessage"},
            {20, "SetPowerOff"},
            {21, "SetPowerOn"},
            {22, "SetPowerReset"},
            // Commands not supported by BMC on KCS interface
            {60, "SetSerialMuxSwitch"},
            {61, "ResetSerialMux"},
            {62, "SerialMuxSwitchStress"},
            {71, "GetAuthenticationCapabilities"},
        };

        /// <summary>
        /// Supported Command List for JBOD
        /// </summary>
        static SortedDictionary<int, string> JbodCommands = new SortedDictionary<int, string>() 
        {
            {1, "GetDiskStatus"},
            {2, "GetDiskInfo"},
            {3, "GetAuthenticationCapabilities"},
            {4, "GetSystemGuid"}
        };

        // The main thread uses AutoResetEvent to signal the 
        // registered wait handle, which executes the callback 
        // Receive() method.
        static AutoResetEvent ev = new AutoResetEvent(false);

        /// <summary>
        /// Blade Type
        /// </summary>
        enum BladeType
        {
            Blade = 0,
            JBOD = 1
        }

        /// <summary>
        /// Test Passes
        /// </summary>
        static int passes;

        /// <summary>
        /// COM Port Number (default = 1) 
        /// </summary>
        static int port = 1;

        /// <summary>
        /// BMC Connection Type
        /// </summary>
        static int connectionType = 0;

        /// <summary>
        /// locker object
        /// </summary>
        static readonly object locker = new object();

        /// <summary>
        /// Test Passes
        /// </summary>
        static int Passes
        {
            get
            {
                lock (locker)
                {
                    return passes;
                }
            }
            set
            {
                lock (locker)
                {
                    passes = (value > 100 ? 0 : value);
                }
            }
        }

        /// <summary>
        /// Commands
        /// </summary>
        static string cmd = string.Empty;

        /// <summary>
        /// Enable Debug Output
        /// </summary>
        static bool debugEnabled = false;

        /// <summary>
        /// Flag to signal outputting response
        /// message details.
        /// </summary>
        static bool showDetail = false;

        /// <summary>
        /// Flag to signal check for BMC default response values.
        /// </summary>
        static bool checkDefaultValues = false;

        /// <summary>
        /// Flag to signal switch to serial console
        /// </summary>
        static bool serialConsole = false;

        /// <summary>
        /// Flag to signal serial session functionality.
        /// </summary>
        static bool serialSession = false;

        /// <summary>
        /// Time (delay) between commands
        /// </summary>
        static int throttle = 0;

        /// <summary>
        /// Device Type
        /// </summary>
        static BladeType bladeType = BladeType.Blade;

        /// enterEncodeCRLF: VT100 encoding for Enter key. True: CR+LF for Enter. False: CR for Enter.
        internal static bool enterEncodeCRLF = true;

        #endregion

        /// <summary>
        /// Applicaiton Entry Point
        /// </summary>
        static void Main(string[] args)
        {
            if (CheckSyntax(args))
            {
                if (connectionType == 1) // Wmi Inband
                {
                    Proceed();

                    WmiClient();
                }
                else if (connectionType == 2) // Serial Client
                {
                    Proceed();

                    SerialClient();
                }
                else if (connectionType == 3) // Chassis Manager
                {
                    Proceed();

                    ChassisMgrSim();

                    SimChassisManager.ShowResults();
                }
                else
                {
                    Console.WriteLine("Unknown BMC connection type specified.");
                }
            }
            else
            {
                ShowSyntax();
            }
        }

        /// <summary>
        /// Wmi Client
        /// </summary>
        private static void WmiClient()
        {
            // indicates proceed to processing commands
            bool connected = false;

            // wmi scope for target server
            ManagementScope _scope = new ManagementScope("\\\\" + Environment.MachineName + "\\root\\wmi");
            try
            {
                IpmiWmiClient wmi = new IpmiWmiClient(_scope, debugEnabled);
                connected = true;

                // WMI is throttled by default
                throttle = 200;

                if (connected)
                    ProcessCmd<IpmiWmiClient>(wmi);
                else
                {
                    Console.WriteLine("Error connecting to WMI Ipmi provider");
                    Tracer.WriteError("Error connecting to WMI Ipmi provider");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to WMI Ipmi provider: {0}", ex.ToString());
                Tracer.WriteError("Error connecting to WMI Ipmi provider: {0}", ex.ToString());
            }

        }

        /// <summary>
        /// Serial Client
        /// </summary>
        private static void SerialClient()
        {
            // indicates proceed to processing commands
            bool connected = false;

            IpmiSerialClient sc = new IpmiSerialClient();

            sc.ClientPort = string.Format("COM{0}", port.ToString());
            sc.ClientBaudRate = ConfigLoad.BaudRate;
            sc.ClientDataBits = 8;
            sc.ClientParity = Parity.None;
            sc.ClientStopBits = StopBits.One;


            // disable logon retry.
            sc.OverRideRetry = ConfigLoad.AllowLogonRetry;

            sc.Timeout = ConfigLoad.SerialTimeout;

            Console.WriteLine("Timeout: {0}", sc.Timeout);

            Thread.Sleep(2000);

            if (debugEnabled)
                IpmiSharedFunc.TraceEnabled = true;

            Console.WriteLine("Serial Connection: Port {0} Time Set to: {1}ms", sc.ClientPort, sc.Timeout);
            Tracer.WriteInfo("Serial Connection: Port {0} Time Set to: {1}ms", sc.ClientPort, sc.Timeout);

            try
            {
                sc.Connect();
                // if the above processed proceed.
                connected = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Connecting to Serial Port {0}", ex.ToString());
                Tracer.WriteError("Error Connecting to Serial Port {0}", ex.ToString());
            }

            if (connected)
            {
                DeviceGuid guid = sc.GetSystemGuid(false);

                if (guid.CompletionCode != 0x00)
                {
                    Console.WriteLine("IPMI ERROR: Get Guid outside of session failed with Completion Code:", guid.CompletionCode);
                    Tracer.WriteError("IPMI ERROR: Get Guid outside of session failed with Completion Code:", guid.CompletionCode);
                }

                // logon to the session
                sc.LogOn(ConfigLoad.UserName, ConfigLoad.Password);

                if (throttle > 0)
                    System.Threading.Thread.Sleep(throttle);

                if (serialConsole)
                {
                    GetSerialConsole(sc);
                }
                else if (serialSession)
                {
                    GetSerialSession(sc);
                }
                else
                {
                    ProcessCmd<IpmiSerialClient>(sc);

                    if (cmd.ToUpper() == "A")
                    {
                        Console.WriteLine();
                        Console.WriteLine("IPMI Serial Session Stress:");
                        Console.WriteLine("===========================");
                        Tracer.WriteInfo("IPMI Serial Session Stress:");
                        Tracer.WriteInfo("===========================");

                        int count = 1;
                        while (count < ConfigLoad.IpmiSessionStress)
                        {
                            if (!sc.LogOn(ConfigLoad.UserName, ConfigLoad.Password))
                            {
                                Console.WriteLine("IPMI Session Establishment Error. Iterations {0}.  See traces for Session Challenge, Activate Session and Session Privilege", count);
                                Tracer.WriteError("IPMI Session Establishment Error. Iterations {0}.  See traces for Session Challenge, Activate Session and Session Privilege", count);
                                return;
                            }
                            count++;
                }

                        Console.WriteLine("IPMI Session Establishment. Passed: {0} iterations.", count);
                        Tracer.WriteInfo("IPMI Session Establishment. Passed: {0} iterations.", count);

                        Console.WriteLine();
                        Console.WriteLine("IPMI Serial Session Timeout:");
                        Console.WriteLine("===========================");
                        Tracer.WriteInfo("IPMI Serial Session Timeout:");
                        Tracer.WriteInfo("===========================");

                        count = 1;
                        while (count < ConfigLoad.IpmiSessionTimeout)
                        {
                            DeviceGuid deviceGuid = sc.GetSystemGuid(false);

                            if (deviceGuid.CompletionCode != 0x00)
                            {
                                Console.WriteLine("ERROR: IPMI Serial Session Timeout. Get Guid outside of session failed with Completion Code:", deviceGuid.CompletionCode);
                                Tracer.WriteError("ERROR: IPMI Serial Session Timeout. Get Guid outside of session failed with Completion Code:", deviceGuid.CompletionCode);
                            }

                            if (!sc.LogOn(ConfigLoad.UserName, ConfigLoad.Password))
                            {
                                Console.WriteLine("ERROR: IPMI Serial Session Timeout. Session Establishment Error. Iterations {0}.  See traces for Session Challenge, Activate Session and Session Privilege", count);
                                Tracer.WriteError("ERROR: IPMI Serial Session Timeout. Session Establishment Error. Iterations {0}.  See traces for Session Challenge, Activate Session and Session Privilege", count);
                                return;
                            }

                            Console.Write("Please wait 3 minutes 20 seconds for Session Timeout. Start Time: {0}  ", DateTime.Now.ToLocalTime());
                            Thread.Sleep(TimeSpan.FromSeconds(200));
                            Console.WriteLine("End Time: {0}", DateTime.Now.ToLocalTime());

                            deviceGuid = sc.GetSystemGuid(false);

                            if (deviceGuid.CompletionCode != 0x00)
                            {
                                Console.WriteLine("ERROR: IPMI Serial Session Timeout. Get Guid outside of session failed with Completion Code: {0}", deviceGuid.CompletionCode);
                                Tracer.WriteError("ERROR: IPMI Serial Session Timeout. Get Guid outside of session failed with Completion Code: {0}", deviceGuid.CompletionCode);
                                return;
                            }

                            GetChassisStatusResponse state =
                                (GetChassisStatusResponse)sc.IpmiSendReceive(
                                    new GetChassisStatusRequest(),
                                        typeof(GetChassisStatusResponse), false);

                            if (state.CompletionCode == 0x00)
                            {
                                Console.WriteLine("ERROR: IPMI Serial Session Timeout. Get Chassis Status responded after session timeout. Completion Code: {0}", deviceGuid.CompletionCode);
                                Tracer.WriteError("ERROR: IPMI Serial Session Timeout. Get Chassis Status responded after session timeout. Completion Code: {0}", deviceGuid.CompletionCode);
                                return;
                            }

                            count++;
                        }

                        Console.WriteLine("IPMI Session Timeout. Passed: {0} iterations.", count);
                        Tracer.WriteInfo("IPMI Session Timeout. Passed: {0} iterations.", count);
                        Console.WriteLine();
                    }


                }

                // attempt to close the session
                sc.Close();
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Error connecting to Serial Port Ipmi instance");
            }
        }

        /// <summary>
        /// Chassis Manager Client Test.
        /// </summary>
        private static void ChassisMgrSim()
        {
            if (debugEnabled)
                Tracer.LoadChassisManagerTraces();

            SimChassisManager.Initialize();
        }

        /// <summary>
        /// Uses reflection to construct ProcessCmd and execute methods in the
        /// commands list.
        /// </summary>
        private static void ProcessCmd<T>(T ipmiClient) where T : IpmiClientNodeManager
        {
            string bmcInterface = string.Empty;

            if (connectionType == 1)
            { 
                bmcInterface = "Inband Windows WMI provider"; 
            }
            else if (connectionType == 2)
            { 
                bmcInterface = "Out of Band (OOB) Serial"; 
            }
            else 
            {
                Console.WriteLine("Invalid connection for sending commands");
                return;
            }

            // Swap commands:
            // - Swap Jbod for blade commands if target type is Jbod.  
            // - Remove unsupported commands if inteface is inband on the blade
            // - Swap in commands to check for default values
            SwapCmd();

            // Display list of commands to be processed.
            Console.WriteLine("Processing commands over {0} interface", bmcInterface);
            Console.WriteLine();
            Console.WriteLine("Commands:");
            foreach (KeyValuePair<int, string> rec in commands)
            {
                Console.WriteLine("           " + rec.Value);
            }
            Console.WriteLine();

            string commandName = string.Empty;
            string methodName = string.Empty;

            if (passes == 0)
                passes = 1;

            try
            {
                for (int i = 1; i <= Passes; i++)
                {

                    Console.WriteLine();
                    Console.WriteLine("Test Iteration: {0}", i);
                    Console.WriteLine("===================");
                    Console.WriteLine();

                    Type classType = typeof(ProcCmd);
                    Type[] paramType = new Type[3];

                    paramType[0] = typeof(IpmiClientNodeManager);
                    paramType[1] = typeof(bool);
                    paramType[2] = typeof(bool);

                    // Get the public instance constructor that takes an IpmiClientNodeManager parameter.
                    ConstructorInfo classConstruct = classType.GetConstructor(
                        BindingFlags.Instance | BindingFlags.Public, null,
                        CallingConventions.HasThis, paramType, null);

                    object classInstance = classConstruct.Invoke(new object[3] { ipmiClient, showDetail, checkDefaultValues });

                    int responseCount = 0;

                    List<string> failures = new List<string>();

                    // Get the target method and invoke
                    foreach (KeyValuePair<int, string> cmd in commands)
                    {
                        Console.WriteLine(cmd.Value);
                        commandName = cmd.Value;
                        MethodInfo targetMethod = classType.GetMethod(cmd.Value);

                        methodName = targetMethod.Name.ToString();
                        int response = (int)targetMethod.Invoke(classInstance, null);

                        if (response == 1)
                            responseCount++;
                        else
                            failures.Add(commandName);

                        if (throttle > 0)
                            System.Threading.Thread.Sleep(throttle);

                        commandName = string.Empty;
                        methodName = string.Empty;

                        Console.WriteLine();
                    }

                    Console.WriteLine();
                    Console.WriteLine("Test Result.  Commands: {0}.  Success Count: {1}", commands.Count, responseCount);
                    Console.WriteLine("================================================");

                    if (failures.Count != 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Failed Commands:");
                        Console.WriteLine("================");
                        Console.WriteLine();
                        foreach (string failure in failures)
                        {
                            Console.WriteLine("STATUS FAILED:   Command: {0}", failure);
                        }
                        Console.WriteLine();
                        Console.WriteLine("================");
                    }

                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when processing command: {0} Method: {1} Error: {2}",
                                    commandName,
                                    methodName,
                                    ex.ToString());

                Tracer.WriteError(string.Format("Error when processing command: {0} Method: {1} Error: {2}",
                    commandName,
                    methodName,
                    ex.ToString()));
            }
        }

        /// <summary>
        /// Serial Console
        /// </summary>
        private static void GetSerialConsole(IpmiSerialClient sc)
        {
            byte CompletionCode;

            if (bladeType == BladeType.Blade)
            {
                // Use channel 0x0E for current channel
                SerialMuxSwitch mux = sc.SetSerialMuxSwitch(0x0E);
                CompletionCode = mux.CompletionCode;
            }
            else
            {
                sc.Reader = true;
                CompletionCode = 0x00;
            }

            if (CompletionCode == 0x00)
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine("Switching to Serial Console Mode");
                Thread.Sleep(2000);
                Console.Clear();

                AnsiEscape ansi = new AnsiEscape(sc, SerialConsoleType.Muxed);

                SharedFunc.SetSerialSession(true);

                Thread td = new Thread(ansi.ReadConsoleInput);
                td.Start();

                while (SharedFunc.ActiveSerialSession)
                {
                    ansi.SplitAnsiEscape(sc.SerialRead());

                    Thread.Sleep(150);
                }
            }
            else
            {
                Console.WriteLine("Ipmi error with Serial Mux Switch command. Completion Code: {0} ", SharedFunc.ByteToHexString(CompletionCode));
                Tracer.WriteError("Ipmi error with Serial Mux Switch command. Completion Code: {0} ", SharedFunc.ByteToHexString(CompletionCode));
            }

        }

        /// <summary>
        /// Serial Console
        /// </summary>
        private static void GetSerialSession(IpmiSerialClient sc)
        {

            if (bladeType == BladeType.Blade)
            {
                StartSerialSession session = sc.StartSerialSession(true, 0);

                if (session.CompletionCode == 0x00 || session.CompletionCode == 0xFD)
                {
                    Console.Clear();
                    Thread.Sleep(500);
                    // unload the Console Trace
                    Tracer.DebugUnload();
                    Console.Clear();

                    AnsiEscape ansi = new AnsiEscape(sc, SerialConsoleType.Session);

                    SharedFunc.SetSerialSession(true);

                    Thread td1 = new Thread(UnpackIpmiReceive);
                    td1.Start((object)ansi);

                    Thread td2 = new Thread(ansi.ReadConsoleInput);
                    td2.Start();


                    while (SharedFunc.ActiveSerialSession)
                    {

                        Thread.Sleep(500);
                    }

                    ansi.RevertConsole();

                    Tracer.DebugReload(debugEnabled);

                }
                else
                {
                    Console.WriteLine("Ipmi error with Start Serial Session. Completion Code: {0} ", SharedFunc.ByteToHexString(session.CompletionCode));
                    Tracer.WriteError("Ipmi error with Start Serial Session. Completion Code: {0} ", SharedFunc.ByteToHexString(session.CompletionCode));
                }
            }
            else
            {
                Console.WriteLine("Command Not Supported for Device: {0} ", bladeType.ToString());
                Console.WriteLine("Command Not Supported for Device: {0} ", bladeType.ToString());
            }

        }

        
        /// <summary>
        /// Receive for WCS Serial Console Receive Data.
        /// </summary>
        /// <param name="param">AnsiEscape wrapped as object</param>
        private static void UnpackIpmiReceive(object param)
        {
            AnsiEscape ansi = (AnsiEscape)param;

            while (SharedFunc.ActiveSerialSession)
            {
                lock (SharedFunc.Locker)
                {
                    ReceiveSerialData data = ansi.IpmiClient.ReceiveSerialData();

                    if (data.CompletionCode == 0x00)
                    {
                        ansi.SplitAnsiEscape(data.Payload);
                    }
                    else
                    {
                        Console.WriteLine("Ipmi error with Receive Serial Session Data. Completion Code: {0} ", SharedFunc.ByteToHexString(data.CompletionCode));
                        Tracer.WriteError("Ipmi error with Receive Serial Session Data. Completion Code: {0} ", SharedFunc.ByteToHexString(data.CompletionCode));

                        // End the Serial Session
                        StopSerialSession stopSession = ansi.IpmiClient.StopSerialSession();

                        if (stopSession.CompletionCode != 0)
                        {
                            Console.WriteLine("Ipmi error with Stop Serial Session Data. Completion Code: {0} ", SharedFunc.ByteToHexString(data.CompletionCode));
                            Tracer.WriteError("Ipmi error with Stop Serial Session Data. Completion Code: {0} ", SharedFunc.ByteToHexString(data.CompletionCode));
                        }

                        SharedFunc.SetSerialSession(false);
                    }
                }

                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Serial IPMI Power Off
        /// </summary>
        private static bool PowerOff(IpmiSerialClient sc)
        {
            if (sc.SetPowerState(IpmiPowerState.Off) == 0x00)
            {
                Thread.Sleep(3000);

                SystemStatus status = sc.GetChassisState();

                if (status.CompletionCode == 0x00)
                {
                    Console.WriteLine("Power State: {0}", status.PowerState.ToString());
                    return true;
                }
                else
                {
                    Console.WriteLine("Power State: {0}", "Unknown");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Power Off: {0}", "Failed");
                return false;
            }
        }

        /// <summary>
        /// Serial IPMI Power ON
        /// </summary>
        private static bool PowerOn(IpmiSerialClient sc)
        {
            if (sc.SetPowerState(IpmiPowerState.On) == 0x00)
            {
                Thread.Sleep(3000);

                SystemStatus status = sc.GetChassisState();

                if (status.CompletionCode == 0x00)
                {
                    Console.WriteLine("Power State: {0}", status.PowerState.ToString());
                    return true;
                }
                else
                {
                    Console.WriteLine("Power State: {0}", "Unknown");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Power On: {0}", "Failed");
                return false;
            }
        }

        /// <summary>
        /// Swap Blade for JBOD commands, if the connection is Serial
        /// and the target device type is JBOD.
        /// </summary>
        private static void SwapCmd()
        {
            if (checkDefaultValues)
            {
                // Check default values only
                if (cmd.ToUpper() == "A")
                {
                    commands.Clear();
                    foreach (KeyValuePair<int, string> defaultCmd in commandsDefaultValues)
                    {
                        commands.Add(defaultCmd.Key, defaultCmd.Value);
                    }

                }
                else
                {
                    foreach (KeyValuePair<int, string> defaultCmd in commandsDefaultValues)
                    {
                        if (!commands.ContainsKey(defaultCmd.Key))
                        {
                            commands.Remove(defaultCmd.Key);
                        }
                    }
                }
            }
            else
            {
                if (connectionType == 2 && bladeType == BladeType.JBOD)
                {
                    // clear the existing blade command list
                    commands.Clear();

                    // add each Jbod command to the cleared command list
                    foreach (KeyValuePair<int, string> cmd in JbodCommands)
                    {
                        commands.Add(cmd.Key, cmd.Value);
                    }
                }
                // remove unsupported commands from inband connection.
                else if (connectionType == 1)
                {
                    foreach (KeyValuePair<int, string> cmd in InbandForbidden)
                    {
                        commands.Remove(cmd.Key);
                    }
                }
            }
        }

        #region Syntax

        /// <summary>
        /// Checks the console arguments
        /// </summary>
        private static bool CheckSyntax(string[] args)
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

            // argument parameter string
            string param;

            // argument value string
            string value;

            // connection type
            string conn = string.Empty;

            // device type.  Blade / JBOD
            string deviceType = string.Empty;

            // input regex
            string regex = @"(?<=-|/)(?<arg>\w+):(?<value>[a-zA-Z0-9_-]+)";

            // number of passes
            int pass = 0;

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
                        // set the parameter
                        param = match.Groups["arg"].Value;
                        // set the argument value
                        value = match.Groups["value"].Value;

                        // switch upper case parameter
                        // and set variables
                        switch (param.ToUpper())
                        {
                            case "CONN":
                                conn = value.ToString().Replace(" ", "");
                                break;
                            case "CMD":
                                cmd = value.ToString().Replace(" ", "");
                                break;
                            case "COM":
                                if (Int32.TryParse(value, out port))
                                {
                                    if (port <= 0)
                                        port = 1;

                                };
                                break;
                            case "PASS":
                                if (Int32.TryParse(value, out pass))
                                {
                                    if (pass < 0)
                                        pass = 0;

                                    Passes = pass;

                                };
                                break;
                            case "TRTL":
                                if (Int32.TryParse(value, out throttle))
                                {
                                    if (throttle < 0)
                                        throttle = 0;
                                };
                                break;
                            case "DTL":
                                bool.TryParse(value, out showDetail);
                                break;
                            case "DBG":
                                bool.TryParse(value, out debugEnabled);
                                break;
                            case "CHKDEF":
                                bool.TryParse(value, out checkDefaultValues);
                                break;
                            case "TYPE":
                                deviceType = value.ToString().Replace(" ", "");
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            List<string> commandList = commands.Values.ToList<string>();

            if ((conn.ToUpper() == "IB" || conn.ToUpper() == "OOB") &&
                (cmd.ToUpper() == "A" || commandList.Contains(cmd, StringComparer.OrdinalIgnoreCase))
                || (conn.ToUpper() == "OOB" && (cmd.ToUpper() == "C" || cmd.ToUpper() == "S")))
            {
                if (conn.ToUpper() == "IB")
                    connectionType = 1;
                else
                    connectionType = 2;

                if (connectionType == 2 && cmd.ToUpper() == "C")
                {
                    commands.Clear();

                    serialSession = false;
                    serialConsole = true;
                }

                if (connectionType == 2 && cmd.ToUpper() == "S")
                {
                    commands.Clear();

                    serialConsole = false;
                    serialSession = true;
                }

                if (cmd.ToUpper() != "A" && cmd.ToUpper() != "C"
                    && cmd.ToUpper() != "S"
                    && commandList.Contains(cmd, StringComparer.OrdinalIgnoreCase))
                {
                    int index = commandList.FindIndex(a => string.Equals(a, cmd, StringComparison.OrdinalIgnoreCase));
                    string command = commandList[index];
                    commands.Clear();
                    commands.Add(1, command);
                }

                if (deviceType.ToUpper() == "JBOD")
                {
                    bladeType = BladeType.JBOD;
                    // JBOD uses CR only for Enter key encoding
                    IpmiUtil.enterEncodeCRLF = false;
                }

                if (port > 9)
                    return false;
                else
                    return true;

            }
            else if (conn.ToUpper() == "WCS")
            {
                connectionType = 3;
                return true;
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Error: Invalid parameters or parameter sequence");
                Console.WriteLine();
                return false;
            }
        }

        /// <summary>
        /// Show Syntax
        /// </summary>
        private static void ShowSyntax()
        {
            Console.WriteLine();
            Console.WriteLine("There are 3 types of verification test available through this conformance utility:");
            Console.WriteLine(" 1.	Windows Server native IPMI driver (WMI).  This test must be performed");
            Console.WriteLine("     from within blade Windows Operating System environment.");
            Console.WriteLine(" 2.	Serial Port (OOB).  The test must be performed over a serial port connected");
            Console.WriteLine("     directly to the blades WCS compliant BMC.");
            Console.WriteLine(" 3.	WCS Chassis Manager (WCS).  The blade must be inserted into slot 1 of a WCS Chassis.");
            Console.WriteLine();
            Console.WriteLine("Syntax:");
            Console.WriteLine("=======");
            Console.WriteLine();
            Console.WriteLine("/Conn:   Connection Types:   IB = In Band");
            Console.WriteLine("                             OOB = Out of Band (Serial)");
            Console.WriteLine("                             WCS = Chassis Manager (WCS - PDB)");
            Console.WriteLine();
            Console.WriteLine("/Com:    Serial Port Number:   1-9. (Default = 1)");
            Console.WriteLine();
            Console.WriteLine("/Pass:   Passes: 0-100");
            Console.WriteLine();
            Console.WriteLine("/Trtl:   Throttle in milliseconds.  This increases the interval");
            Console.WriteLine("         between commands being sent to the BMC.  The deault is");
            Console.WriteLine("         zero.  Any value other than zero will not simulate WCS");
            Console.WriteLine("         Chassis Manager functionality");
            Console.WriteLine();
            Console.WriteLine("/Dtl:    True/False. Optional parameter to output command response values");
            Console.WriteLine();
            Console.WriteLine("/Dbg:    True/False. Optional parameter to enable debug Traces");
            Console.WriteLine();
            Console.WriteLine("/ChkDef: True/False. Optional parameter to check for default values in blade BMC responses.");
            Console.WriteLine("         This test must be run before other tests as other tests will change the field values in the BMC.");
            Console.WriteLine();
            Console.WriteLine("/Type:   Identifies the target system as Blade or JBOD.  This command");
            Console.WriteLine("         is used in conjunction with /Conn:OOB. Acceptable values are:");
            Console.WriteLine("         JBOD / Blade.  Default if unspecified is blade.");
            Console.WriteLine();
            Console.WriteLine("/Cmd:    Blade commands: Specify individual command name, or A = All WCS Commands, C = Serial Console (muxed)");
            Console.WriteLine("         S = Serial Console Session (unmuxed)");
            foreach (KeyValuePair<int, string> command in commands)
            {
                Console.WriteLine("                             {0}", command.Value);
            }
            Console.WriteLine();
            Console.WriteLine("/Cmd:    JBOD commands:   A = All WCS Commands (used with /Type:JBOD)");
            foreach (KeyValuePair<int, string> command in JbodCommands)
            {
                Console.WriteLine("                             {0}", command.Value);
            }
            Console.WriteLine();
            Console.WriteLine("Command Examples:");
            Console.WriteLine("=================");
            Console.WriteLine("WcsTestUtil.exe /Conn:IB /Cmd:A /Pass:1");
            Console.WriteLine();
            Console.WriteLine("WcsTestUtil.exe /Conn:OOB /Com:1 /Cmd:A /Pass:1");
            Console.WriteLine();
            Console.WriteLine("WcsTestUtil.exe /Conn:OOB /Com:1 /Cmd:GetPowerLimit /Pass:1");
            Console.WriteLine();
            Console.WriteLine("WcsTestUtil.exe /Conn:OOB /Com:1 /Type:JBOD /Cmd:A /Pass:1");
            Console.WriteLine();
            Console.WriteLine("WcsTestUtil.exe /Conn:OOB /Com:1 /Cmd:A /Pass:1 /Dbg:true");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Serial Console Examples:");
            Console.WriteLine("=================");
            Console.WriteLine("WcsTestUtil.exe /Conn:OOB /Com:1 /Cmd:C /Dbg:true");
            Console.WriteLine();
            Console.WriteLine("WcsTestUtil.exe /Conn:OOB /Com:1 /Cmd:S /Dbg:true");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("WcsTestUtil Version: " + GetAssemblyVersion());
            Console.WriteLine();
        }

        private static void Proceed()
        {
            Console.WriteLine();
            Console.WriteLine("Initializing application");
            Console.WriteLine();

            try
            {
                if (Tracer.TraceEnabled.Enabled)
                {
                    Console.WriteLine("Log file path: {0}", Tracer.tracefileName.ToString());
                }
                else
                {
                    Console.WriteLine("Logging has been disabled in the config settings");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating Trace log file: " + ex.ToString());
            }

            Console.WriteLine();

        }

        /// <summary>
        /// Get assembly version
        /// </summary>
        /// <returns></returns>
        private static string GetAssemblyVersion()
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                string version = fileVersionInfo.ProductVersion;
                return version;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion
    }
}
