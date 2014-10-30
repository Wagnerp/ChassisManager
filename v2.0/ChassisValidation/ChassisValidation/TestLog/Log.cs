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
using System.Text;

namespace ChassisValidation
{
    public class Log
    {
        private static LogLevel level = LogLevel.Info;
         
        public static LogLevel Level 
        {
            get
            {
                return level;
            }
            set
            {
                level = value;
            }
        }

        private static readonly List<ILogger> loggers = new List<ILogger>();

        public static ICollection<ILogger> Loggers 
        {
            get
            {
                return loggers;
            }
        }

        public static void Error(string message, Exception ex = null)
        {
            if (level <= LogLevel.Error)
            {
                DoLog(LogLevel.Error, null, message, ex);
            }
        }

        public static void Error(string category, string message, Exception ex = null)
        {
            if (level <= LogLevel.Error)
            {
                DoLog(LogLevel.Error, category, message, ex);
            }
        }

        public static void Notice(string category, string message, Exception ex = null)
        {
            if (level <= LogLevel.Notice)
            {
                DoLog(LogLevel.Notice, category, message, ex);
            }
        }

        public static void Info(string message, Exception ex = null) 
        {
            if (level <= LogLevel.Info)
            {
                DoLog(LogLevel.Info, null, message, ex);
            }
        }

        public static void Info(string category, string message, Exception ex = null) 
        {
            if (level <= LogLevel.Info)
            {
                DoLog(LogLevel.Info, category, message, ex);
            }
        }

        public static void Verbose(string category, string message, Exception ex = null) 
        {
            if (level <= LogLevel.Verbose)
            {
                DoLog(LogLevel.Verbose, category, message, ex);
            }
        }

        public static void Debug(string category, string message, Exception ex = null) 
        {
            if (level <= LogLevel.Debug)
            {
                DoLog(LogLevel.Debug, category, message, ex);
            }
        }

        protected static void DoLog(LogLevel logLevel, String category, String message, Exception exception)
        {
            const int exceptionLeftPadding = 9;

            var builder = new StringBuilder();

            builder.AppendFormat("{0}  ", DateTime.Now.ToString("T"));

            if (!string.IsNullOrWhiteSpace(category))
            {
                builder.AppendFormat("[{0}] ", category);
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                builder.Append(message);
            }

            if (exception != null)
            {
                builder.Append(Environment.NewLine);
                builder.Append(' ', exceptionLeftPadding);
                builder.Append(exception.ToString());
            }

            var stringLine = builder.ToString();

            // write log to each logger
            loggers.ForEach(logger =>
            {
                if (logger != null)
                {
                    logger.WriteLine(logLevel, stringLine);
                }
            });
        }
    }
}
