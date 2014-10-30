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
    using System.IO.Ports;
    using System.Threading;
    using System.Diagnostics;
    using System.Collections.Generic;

    class IpmiPerfUtil : Indexer
    {
        /// <summary>
        /// Serial Port Instance
        /// </summary>
        static SerialPort sPort = new SerialPort();

        static Stopwatch stpWatch = new Stopwatch();
           
        static void Main(string[] args)
        {
            if (Func.CheckSyntax(args))
            {
                OpenSerialPort();

                if (Func.TestPass == 1 || Func.TestPass == 2)
                {
                    stpWatch.Start();

                    SetProcess(true);

                    // Capture Ctrl+C as key command.
                    Console.TreatControlCAsInput = true;

                    Thread readInput = new Thread(ReadConsole);
                    readInput.Start();

                    if (Func.TestPass == 1)
                    {
                        Test1();
                    }
                    else if (Func.TestPass == 2)
                    {
                        Test2();
                    }

                    stpWatch.Stop();

                    // Wait for the session to close.
                    readInput.Join(800);

                    Console.WriteLine();
                    Console.WriteLine("Execution Time: {0}:{1}:{2}:{3}", stpWatch.Elapsed.Hours, stpWatch.Elapsed.Minutes, stpWatch.Elapsed.Seconds, (stpWatch.Elapsed.Milliseconds / 10));
                    Console.WriteLine();

                    if (sPort.IsOpen)
                        sPort.Close();

                    sPort.Dispose();

                }
                else
                {
                    Console.WriteLine("Provide a test type in Syntax");
                }

            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Valid Input:");
                Console.WriteLine("Command Syntax: IpmiPerfTest /P:COM1 /T:1");
                Console.WriteLine();
                Console.WriteLine("If BMC Authentication is required:");
                Console.WriteLine("Command Syntax: IpmiPerfTest /P:COM1 /T:1 /L:1");
                Console.WriteLine();
                Console.WriteLine("If BMC Authentication Validation required:");
                Console.WriteLine("Command Syntax: IpmiPerfTest /P:COM1 /T:1 /L:1 /V:1");
                Console.WriteLine();
                Console.WriteLine(" T Options:");
                Console.WriteLine("         1 = Test without throttling");
                Console.WriteLine("         2 = Test with 50ms throttling");
                Console.WriteLine(" L Options:");
                Console.WriteLine("         0 = No Session Established");
                Console.WriteLine("         1 = Session Established");
                Console.WriteLine(" V Options:");
                Console.WriteLine("         0 = No Session Validation");
                Console.WriteLine("         1 = Session Validation, may cause Completion Code 0x81");
                Console.WriteLine();
            }
        }

        static void Test1()
        {
            if (Func.Logon > 0)
                Logon();

            while (Process)
                GetChannelAuthenticationCapabilitiesRequest();
        }

        static void Test2()
        {

            sPort.ReadTimeout = 50;

            if (Func.Logon > 0)
                Logon();

            while (Process)
            {
                GetChannelAuthenticationCapabilitiesRequest();
                System.Threading.Thread.Sleep(50);
            }
        }

        static void Logon()
        {
            bool validate = false;

            if (Func.ValidateSession == 1)
            { validate = true;  }

            foreach (byte[] payload in Func.LogonRequests)
            {
                sPort.Write(payload, 0, payload.Length);

                if (!ReceiveData(validate))
                {
                    Console.WriteLine("Error: BMC Logon Failed");
                    Environment.Exit(0);
                }
            }
        }

        static void CloseSession()
        {
            // Ipmi.CloseSessionRequest Seq: 5
            byte[] payload = new byte[13] { 0xA0, 0x20, 0x18, 0xC8, 0x81, 0x14, 0x3C, 0xFB, 0x2D, 0xB5, 0xFF, 0x53, 0xA5 };
            
            Console.WriteLine();
            Console.WriteLine("Closing Session:");
            
            WriteData(payload);
        }

        static void OpenSerialPort()
        {
            if (Func.ComPort.Length == 4)
            {
                try 
	            {	        	
                    // COM port paramaters
                    sPort.PortName = Func.ComPort;
                    sPort.BaudRate = 115200;
                    sPort.Parity = Parity.None;
                    sPort.DataBits = 8;
                    sPort.StopBits = StopBits.One;
                    sPort.RtsEnable = true;
                    sPort.Handshake = Handshake.None;

                    // set the read-timeout.
                    sPort.ReadTimeout = 100;

                    // attempt to open port
                    sPort.Open();
                
                }
	            catch (Exception ex)
	            {
                    Console.WriteLine("Error opening COM Port: {0}", ex.ToString());
                    Environment.Exit(0);
	            }
            }
            else
            { 
                Console.WriteLine("Unknown COM Port" );
            }

        }
       
        static void GetChannelAuthenticationCapabilitiesRequest()
        {
            WriteData(Func.ChannelAuthRequests[ChannelAuthIndex++]);
        }

        static void WriteData(byte[] payload)
        {
            try
            {
                if (sPort.IsOpen)
                {
                    Console.WriteLine("Sending:  {0} ", Func.ByteArrayToHexString(payload));
                    sPort.Write(payload, 0, payload.Length);

                    ReceiveData();
                }
                else
                {
                    Console.WriteLine("Error with Serial port state");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Serial Port IO Exception: {0}", ex.ToString());
            }
        
        }

        static bool ReceiveData(bool checkResponse = false)
        {
            Console.Write("Received: ");
            List<byte> response = new List<byte>();
            bool payloadReceived = false;

            while (true)
            {
                try
                {
                    // read from serial buffer
                    int receivedData = sPort.ReadByte();
                    // display response
                    Console.Write(Func.ByteToHexString((byte)receivedData));
                    // add to collection
                    response.Add((byte)receivedData);

                    // end of buffer
                    if (receivedData == -1 || receivedData == 165)
                    {
                        payloadReceived = true;
                        Console.WriteLine();
                        break;
                    }
                }
                catch (TimeoutException ex)
                {
                    payloadReceived = false;
                    Console.WriteLine("Serial Timeout Exception {0}", ex.ToString());

                    Environment.Exit(0);

                    break;
                }
            }

            if (checkResponse)
            {
                if (response.Count > 8)
                { 
                    if(response[8] == 0x00)
                        payloadReceived = true;
                    else
                    {
                        payloadReceived = false;
                        Console.WriteLine("Error Ipmi Completion Code: {0}", Func.ByteArrayToHexString(response.ToArray()));
                    }
                }
                else
                {
                    payloadReceived = false;
                    Console.WriteLine("Error Ipmi malformed packet");
                }
                                
            }

            return payloadReceived;

        }

        static void ReadConsole()
        {
            // Engage Console.
            Console.Write(ConsoleKey.Enter);

            //ConsoleKeyInfo keyInf;
            while (Process)
            {
                // read key and intercept
                ConsoleKeyInfo keyInf = Console.ReadKey(true);

                // check for Cntrl + C.
                if (IsFunctionKey(keyInf))
                {
                    // Gracefull Terminate.
                    Terminate();
                }
            }
        }

        /// <summary>
        /// Check ConcoleKeyInfo for Function Key
        /// </summary>
        static bool IsFunctionKey(ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.C &&
                 keyInfo.Modifiers == ConsoleModifiers.Control // Ctrl + C
                )
            {
                return true;
            }
            else
                return false;
        }

        static void Terminate()
        {
            // Issue a graceful terminate.
            SetProcess(false);

            // Allow Read Loops to end.
            Thread.Sleep(200);

            // if the COM port is opened and logon was required
            // attempt to gracefully close the session
            if (sPort.IsOpen && Func.Logon == 1 &&
                (Func.TestPass == 1 || Func.TestPass == 2))
                CloseSession();

            // Capture Ctrl+C as key command.
            Console.TreatControlCAsInput = false;
        }

    }

    abstract class Indexer
    {
        private static int _index;

        /// <summary>
        /// locker object
        /// </summary>
        private static readonly object locker = new object();

        /// <summary>
        /// List Indexer
        /// </summary>
        protected static int ChannelAuthIndex
        {
            get
            {
                lock (locker)
                {
                    return _index;
                }
            }
            set
            {
                lock (locker)
                {
                    _index = (value > (byte)0x1E ? (byte)0x00 : value);
                }
            }
        }

        private static volatile bool process = false;

        protected static bool Process
        {
            get { return process; }
            private set { process = value; }
        }

        protected static void SetProcess(bool enabled)
        {
            Process = enabled;
        }

    }
}
