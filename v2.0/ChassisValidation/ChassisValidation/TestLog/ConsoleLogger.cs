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

namespace ChassisValidation
{
    public class ConsoleLogger : ILogger
    {
        public void WriteLine(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Error :
                    this.WriteConsole(message, ConsoleColor.Red) ;
                    break;
                case LogLevel.Notice :
                    this.WriteConsole(message, ConsoleColor.Cyan) ;
                    break;
                case LogLevel.Verbose :
                    this.WriteConsole(message, ConsoleColor.Gray) ;
                    break;
                case LogLevel.Debug :
                    this.WriteConsole(message, ConsoleColor.Gray) ;
                    break;
                default :
                    this.WriteConsole(message, ConsoleColor.White) ;
                    break;
            }
        }

        private void WriteConsole(string message, ConsoleColor color) 
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
