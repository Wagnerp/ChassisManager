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
using System.Runtime.CompilerServices;

namespace ChassisValidation
{
    internal class CmTestLog : Log
    {
        public static void Start(string message = null, [CallerMemberName]
                                 string testName = null) 
        {
            Log.Notice(testName, message ?? "TEST STARTED");
        }

        public static void End(string message = null, [CallerMemberName]
                               string testName = null) 
        {
            Log.Notice(testName, string.Format("{0}{1}", message ?? "TEST COMPLETED", Environment.NewLine));
        }

        public static void End(bool passed, [CallerMemberName]
                               string testName = null)
        {
            if (passed)
            {
                Log.Notice(testName, string.Format("TEST PASSED{0}", Environment.NewLine));
            }
            else
            {
                Log.Error(testName, string.Format("TEST FAILED{0}", Environment.NewLine));
            }
        }

        public static void Info(string message, [CallerMemberName]
                                string testName = null)
        {
            Log.Info(testName, message);
        }

        public static void Failure(string message, [CallerMemberName]
                                   string testName = null)
        {
            Log.Error(testName, string.Format("Failure: {0}", message));
        }

        public static void Success(string message, [CallerMemberName]
                                   string testName = null) 
        {
            Log.Info(testName, string.Format("Success: {0}", message));
        }

        public static void Exception(Exception exception, [CallerMemberName]
                                     string testName = null)
        {
            Log.Error(testName, "Exception happened", exception);
        }

        public static void Verbose(string message, [CallerMemberName]
                                   string testName = null) 
        {
            Log.Verbose(testName, string.Format("Verbose: {0}", message));
        }
    }
    // This class will create one log file for each execution
}
