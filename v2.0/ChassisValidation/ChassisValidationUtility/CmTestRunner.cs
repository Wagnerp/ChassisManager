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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using ChassisValidation;

namespace ChassisValidationUtility
{
    /// <summary>
    /// A very simple test engine to run test cases defined in CmRestTest.
    /// </summary>
    public static class CmTestRunner
    {
        #region Fields
        private static CmRestTest cmTest;
        private static List<MethodInfo> testMethods;
        private static List<ILogger> loggers;
        private static string cmurl;
        private static string username;
        private static string password;
        private static string logPath;
        private static string testDomainTestUser;
        private static string testDomainName;

        #endregion

        static CmTestRunner()
        {
            cmurl = ConfigurationManager.AppSettings["CM_URL"];
            username = ConfigurationManager.AppSettings["UserName"];
            password = ConfigurationManager.AppSettings["Password"];

            testDomainTestUser = ConfigurationManager.AppSettings["LabDomainTestUser"];
            testDomainName = ConfigurationManager.AppSettings["LabDomainName"];
       

            logPath = TestLogName.Instance;

            loggers = new List<ILogger> {
                new TxtFileLogger(logPath),
                new ConsoleLogger()
            };

            cmTest = new CmRestTest(cmurl, username, password,testDomainTestUser,testDomainName, loggers);

            // find all public method ended with Test
            testMethods = (
                from test in cmTest.GetType().GetMethods()
                where test.Name.EndsWith("Test")
                select test
            ).ToList();
        }

        /// <summary>
        /// Gets all available test cases.
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllTestCases()
        {
            return testMethods.Select(t => t.Name).ToArray();
        }

        /// <summary>
        /// Run all available test cases.
        /// </summary>
        public static void RunAllTestCases()
        {
            testMethods.ForEach(t => RunTestCase(t.Name));
        }

        /// <summary>
        /// Run a series of test cases; each case will be run once.
        /// </summary>
        /// <param name="names">The names of the test cases.</param>
        public static void RunTestCases(IEnumerable<String> names)
        {
            names.ToList().ForEach(t => RunTestCase(t));
        }

        /// <summary>
        /// Run a single test case one or multiple times.
        /// </summary>
        /// <param name="name">The name of the test case.</param>
        /// <param name="count">Run the test case how many times.</param>
        public static void RunTestCase(string name, uint count = 1)
        {
            var testMethod = testMethods.SingleOrDefault(t => t.Name.EqualsIgnoreCase(name));

            if (testMethod == null)
            {
                Log.Error(name + " is not a valid test case name.");
                return;
            }
            try
            {
                for (var c = 0; c < count; c++) testMethod.Invoke(cmTest, null);
            }
            catch (Exception e)
            {
                Log.Error("The test failed with the following exception: " + e);
            }
        }

        /// <summary>
        /// Run the test cases in a batch file.
        /// </summary>
        /// <param name="filePath">The path of the batch file.</param>
        public static void RunFromBatch(string filePath)
        {
            TestList tests;

            try
            {
                tests = CmTestBatch.ReadTestList(filePath);
            }
            catch (Exception e)
            {
                Log.Error("Failed loading test batch. " + e);
                return;
            }
            if (tests.TestCases.Count == 0)
            {
                Log.Info("No test cases to run");
                return;
            }
            tests.TestCases.ForEach(t => RunTestCase(t.Name, t.Run));
        }

        internal static bool EqualsIgnoreCase(this string source, string target)
        {
            return string.Equals(source, target, StringComparison.InvariantCultureIgnoreCase);
        }
    }

    /// <summary>
    /// Provides operations on test batches.
    /// </summary>
    public class CmTestBatch
    {
        /// <summary>
        /// Read test cases from a batch file.
        /// </summary>
        public static TestList ReadTestList(string filePath)
        {
            var serializer = new XmlSerializer(typeof(TestList));
            using (var stream = File.OpenRead(filePath))
            {
                return (TestList)serializer.Deserialize(stream);
            }
        }
    }

    [Serializable]
    public class TestList
    {
        [XmlElement("TestCase")] public List<TestCase> TestCases;
    }

    [Serializable]
    public class TestCase
    {
        [XmlAttribute("Name")] public string Name;

        [XmlAttribute("Run")] public uint Run;
    }
}
