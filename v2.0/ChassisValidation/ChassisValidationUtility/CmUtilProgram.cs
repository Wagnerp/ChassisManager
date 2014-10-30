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
using System.Linq;

namespace ChassisValidationUtility
{
    class CmUtilOption : CmdOptionBase
    {
        [CmdOption(LongName = "RunAll", ShortName = 'a',
        Description = "Run all test cases.")]
        public bool RunAll { get; set; }

        [CmdOption(LongName = "RunTests", ShortName = 't',
        Description = "Run single or multiple test cases.")]
        public string[] RunTests { get; set; }

        [CmdOption(LongName = "RunBatch", ShortName = 'b',
        Description = "Run test cases in a batch file.")]
        public string RunBatch { get; set; }

        [CmdOption(LongName = "Help", ShortName = 'h',
        Description = "Show command help.")]
        public bool Help { get; set; }
    }

    class CmUtilProgram
    {
        private static readonly string appName = AppDomain.CurrentDomain.FriendlyName;

        static void Main()
        {
            try
            {
                Run(Environment.CommandLine);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception happened: {0}", e);
            }
            // Console.ReadKey();
        }

        static void Run(string commandLine)
        {
            CmUtilOption option = null;

            try
            {
                option = new CmdLineParser<CmUtilOption>().Parse(commandLine);
            }
            catch (CommandLineParsingException e)
            {
                Console.WriteLine(e.Message);
                ShowUsage();
                return;
            }

            if (option.Parsed == 0)
            {
                ShowUsage();
                return;
            }

            if (option.Parsed != 1)
            {
                Console.WriteLine("Only one option is allowed.");
                ShowUsage();
                return;
            }

            if (option.Help) ShowUsage();
            else if (option.RunAll) CmTestRunner.RunAllTestCases();
            else if (option.RunBatch != null) CmTestRunner.RunFromBatch(option.RunBatch);
            else if (option.RunTests != null) CmTestRunner.RunTestCases(option.RunTests);
        }

        static void ShowUsage()
        {
            Console.WriteLine("\n\rNAME\n\r{0} - Chassis validation utility.", appName);
            Console.WriteLine("\n\rSYNTAX\n\r{0} [-RunAll] [-RunTests] [-RunBatch] [-Help]", appName);
            Console.WriteLine("\n\rOPTIONS");
            Console.WriteLine(string.Join(Environment.NewLine, (
                from prop in typeof(CmUtilOption).GetProperties()
                let attr = prop.GetCustomAttributes(typeof(CmdOptionAttribute), true)
                               .Cast<CmdOptionAttribute>().FirstOrDefault()
                where attr != null
                select string.Format("-{0} -{1}  {2}",
                    attr.LongName.PadRight(10), attr.ShortName, attr.Description)
                )));
            Console.WriteLine("\n\rREMARKS\n\rAll available test cases: ");
            Console.WriteLine(string.Join(Environment.NewLine, CmTestRunner.GetAllTestCases()));
        }
    }
}
