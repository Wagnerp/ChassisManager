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
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.IO;

    internal static class Tracer
    {
        /// <summary>
        /// Changed in the App config.  Signal for checking if tracing is enabled.  Application usage:
        ///     Tracer.Trace.WriteLineIf(traceEnabled.Enabled, "Content to trace");
        /// </summary>
        public static BooleanSwitch TraceEnabled = new BooleanSwitch("TraceEnabled", "On/Off signal for trace checking");

        /// <summary>
        /// Changed in the App config.  Signal level/depth of tracing.  Application usage:
        ///    Trace.WriteLineIf(TraceLevel.TraceError, "App Config: TraceLevel.TraceError");
        ///    Trace.WriteLineIf(TraceLevel.TraceWarning, "App Config: TraceLevel.TraceWarning");
        ///    Trace.WriteLineIf(TraceLevel.TraceInfo, "App Config: TraceLevel.TraceInfo");
        ///    Trace.WriteLineIf(TraceLevel.TraceVerbose, "App Config: TraceLevel.TraceVerbose");
        /// </summary>
        public static TraceSwitch LogLevel = new TraceSwitch("TraceLevel", "Trace severity level switch");

        //private static string _tracefileName = @"C:\WcsTestUtil.txt";
        internal static string tracefileName = ConfigLoad.ReportLogFilePath;
        private static FileStream _traceFile;

        // Constant strings 
        internal const string targetSiteEmpty = "Exception targetSite Is Empty ";
        internal const string stackTraceEmpty = "Exception Stack Trace Is Empty ";
        internal const string exceptionMessageEmpty = "Exception Message Is Empty ";

        // Temparary StreamWriter.
        private static MemoryStream debugOutput;

        private static StreamReader outputStream;

        private static TextWriterTraceListener SuspendedListener;

        private static TextWriterTraceListener DefaultTraceFile;

        // Console Tracer.
        private static TextWriterTraceListener ConsoleListener = new TextWriterTraceListener(Console.Out);

        internal static void DebugUnload()
        {
            try
            {
                // create memory stream
                debugOutput = new MemoryStream();

                // create stream reader
                outputStream = new StreamReader(debugOutput);

                // create background trace listener
                SuspendedListener = new TextWriterTraceListener(debugOutput);

                if (!Debug.Listeners.Contains(SuspendedListener))
                    Debug.Listeners.Add(SuspendedListener);

                if (Debug.Listeners.Contains(ConsoleListener))
                    Debug.Listeners.Remove(ConsoleListener);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to suspend Trace Listener: {0}", ex.ToString());
            }
        }

        internal static void DebugReload(bool debugEnabled)
        {
            try
            {
                if (!Debug.Listeners.Contains(ConsoleListener))
                {
                    if (ConsoleListener == null)
                    {
                        ConsoleListener = new TextWriterTraceListener(Console.Out);
                    }

                     Debug.Listeners.Add(ConsoleListener);
                }

                if (debugEnabled)
                    ReplayTraces();

                if (Debug.Listeners.Contains(SuspendedListener))
                    Debug.Listeners.Remove(SuspendedListener);
                
                // SuspendedListener
                if (SuspendedListener != null)
                    SuspendedListener.Close();

                if (outputStream != null)
                    outputStream.Close();

                if (debugOutput != null)
                    debugOutput.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to resume Trace Listener: {0}", ex.ToString());
            }
        }

        private static void ReplayTraces()
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("Replaying captured traces");
                Console.WriteLine("========================");
                debugOutput.Position = 0;
                System.Threading.Thread.Sleep(1000);
                
                Console.Write(outputStream.ReadToEnd());

                System.Threading.Thread.Sleep(1000);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to resume Trace Listener: {0}", ex.ToString());
            }
        }

        internal static void LoadChassisManagerTraces()
        {
            if(DefaultTraceFile != null)
            {
                try
                {
                    Microsoft.GFS.WCS.ChassisManager.Tracer.DebugSourceSwitch = new SourceSwitch("Information");
                    Microsoft.GFS.WCS.ChassisManager.Tracer.AddDebugTraceListener(DefaultTraceFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: Unable to load Chassis Manager Traces: {0}", ex.ToString());
                    WriteError("ERROR: Unable to load Chassis Manager Traces: {0}", ex.ToString());
                }

            }
        }

        /// <summary>
        /// Constructor for initialization
        /// </summary>
        static Tracer()
        {
           
            try
            {
                _traceFile = new System.IO.FileStream(tracefileName, System.IO.FileMode.Append);
                DefaultTraceFile = new TextWriterTraceListener(_traceFile);
                Debug.Listeners.Add(DefaultTraceFile);
                Debug.Listeners.Add(ConsoleListener);
                Trace.AutoFlush = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Trace Logging cannot be done. Exception: " + ex.ToString());
            }
        }

        /// <summary>
        /// System Trace Write Output if log level is enabled in the app config.
        /// </summary>
        public static void WriteError(Exception ex)
        {
            try
            {
                if (LogLevel.TraceError)
                {
                    WriteTrace(ex.TargetSite != null ? ex.TargetSite.ToString() : targetSiteEmpty, "Error", true);
                    WriteTrace(ex.StackTrace != null ? ex.StackTrace : stackTraceEmpty, "Error", true);
                    WriteTrace(ex.Message != null ? ex.Message.ToString() : exceptionMessageEmpty, "Error", true);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Trace Logging cannot be done. Exception: " + exception.ToString());
            }
        }

        /// <summary>
        /// System Trace Write Output if log level is enabled in the app config.
        /// </summary>
        public static void Write(string message, object obj1 = null, object obj2 = null, object obj3 = null)
        {
            try
            {
                 WriteTrace(string.Format(message, obj1, obj2, obj3), "", false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Trace Logging cannot be done. Exception: " + ex.ToString());
            }
        }

        public static void WriteInfo(string message, object obj1 = null, object obj2 = null, object obj3 = null)
        {
            try
            {
                // Assume highest verbosity level since not specified.. and always log this content
                if (LogLevel.TraceInfo)
                    WriteTrace(String.Format(message, obj1, obj2, obj3), "Info", true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Trace Logging cannot be done. Exception: " + ex.ToString());
            }
        }

        /// <summary>
        /// System Trace Write Output if log level is enabled in the app config.
        /// </summary>
        public static void WriteWarning(string message, object obj1 = null, object obj2 = null, object obj3 = null)
        {
            try
            {
                if (LogLevel.TraceWarning)
                    WriteTrace(String.Format(message, obj1, obj2, obj3), "Warning", true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Trace Logging cannot be done. Exception: " + ex.ToString());
            }
        }

        /// <summary>
        /// System Trace Write Output if log level is enabled in the app config.
        /// </summary>
        public static void WriteError(string message, object obj1 = null, object obj2 = null, object obj3 = null)
        {
            try
            {
                if (LogLevel.TraceError)
                    WriteTrace(String.Format(message, obj1, obj2, obj3), "Error", true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Trace Logging cannot be done. Exception: " + ex.ToString());
            }
        }

        /// <summary>
        /// System Trace Write Output if log level is enabled in the app config.
        /// </summary>
        private static void WriteTrace(string message, string type, bool timeStamp)
        {
            try
            {
                if (timeStamp)
                    Trace.WriteLine(string.Format("{0},{1},{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), type, message));
                else
                    Trace.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Trace Logging cannot be done. Exception: " + ex.ToString());
            }
        }
    }
}
