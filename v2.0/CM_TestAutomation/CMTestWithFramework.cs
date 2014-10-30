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

namespace Microsoft.GFS.WCS.Test
{
    using System;
    using System.IO;
    using System.Linq;
    using Microsoft.GFS.WCS.Test.Framework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary> This is a helper class used to kick off the Test Framework. </summary>
    //[TestClass]
    public class CmTestWithFramework
    { 
        /// <summary>
        /// TestMethod to kick off all batches defined for Test Framework.
        /// </summary>
        //[TestMethod, DeploymentItem("TestBatches")]
        public void RunAllFrameworkBatches(string userName, string userPassword)
        {
            this.RunAllFrameworkBatches(@".", TestConfigLoaded.CM_URL, userName, userPassword);
        }

        /// <summary> Kicks off all batches specified. </summary>
        /// <param name="batchDirectory"> A directory to load Test batches from. </param>
        /// <param name="chassisManagerEndPoint"> Chassis Manager endpoint uri. </param>
        public void RunAllFrameworkBatches(string batchDirectory, string chassisManagerEndPoint, string userName, string userPassword)
        {
            var globalParameters = Parameters.GetSampleParameters();
            string exceptionMessage = null;
            Assert.IsTrue(
                Directory.GetFiles(batchDirectory ?? @".", "*Batch.xml").Any(),
                String.Format("No batch found matching *Batch.xml in directory '{0}'", batchDirectory));
            foreach (var batchDefinitionFile in Directory.GetFiles(batchDirectory ?? @".", "*Batch.xml"))
            {
                try
                {
                    this.RunFrameworkBatch(
                        TestBatch.LoadBatch(batchDefinitionFile), chassisManagerEndPoint, globalParameters, userName, userPassword);
                }
                catch (Exception ex)
                {
                    exceptionMessage = ex.Message;
                }
            }

            Assert.IsTrue(
                string.IsNullOrEmpty(exceptionMessage),
                string.Format("At least one Batch failed to Load/Run;\n{0}", exceptionMessage));
        }

        /// <summary> Kicks off batch specified. </summary>
        /// <param name="batchDefinitionFile"> A Test batch objectame of batch file to load and run. </param>
        /// <param name="chassisManagerEndPoint"> Chassis Manager endpoint uri. </param>
        public void RunFrameworkBatch(string batchDefinitionFile, string chassisManagerEndPoint, string userName, string userPassword)
        {
            var batch = TestBatch.LoadBatch(batchDefinitionFile);
            this.RunFrameworkBatch(batch, chassisManagerEndPoint, Parameters.GetSampleParameters(), userName, userPassword);
        }

        /// <summary> kicks off batch specified. </summary>
        /// <param name="batch"> A Test batch objectame of batch file to load and run. </param>
        /// <param name="chassisManagerEndPoint"> Chassis Manager endpoint uri. </param>
        /// <param name="globalParameters"> Global Parameters. </param>
        public void RunFrameworkBatch(TestBatch batch, string chassisManagerEndPoint, Parameters globalParameters, string userName, string userPassword)
        {
            var batchResults = new ResultOfTestBatch(batch.Name, chassisManagerEndPoint);

            try
            {
                batch.Run(globalParameters, batchResults, userName, userPassword);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\n********** Batch has Ended.\n");
            }
            finally
            {
                batchResults.Save();
            }
        }

        public void RunFunctionalBvt(string chassisManagerEndPoint, string skuDefinistionXml, string userName, string userPassword)
        {
            CM_FunctionalTests cMFunctionalExecution = new CM_FunctionalTests(chassisManagerEndPoint,userName,userPassword);

            TestsResultResponse chassisInfoPass;
            TestsResultResponse bladesInfoPass;
            TestsResultResponse chassisHealthPass;
            TestsResultResponse bladesHealthPass;
            
            Console.WriteLine("/n Starting Functional BVT tests");

            //Verify Chassis Health
            chassisInfoPass = cMFunctionalExecution.CheckChassisInfo(skuDefinistionXml);

            if (chassisInfoPass.result != ExecutionResult.Passed)
            {
                Console.WriteLine("\n------- Chassis Info verification Finished with errors. Please fix the listed failures and try again.");
            }
            else
            {
                Console.WriteLine("\n+++++++ Chassis Info verification was successfully verified.");
            }

            //Verify Blades Info
            bladesInfoPass = cMFunctionalExecution.VerifyBladesInfo(skuDefinistionXml);
            if (bladesInfoPass.result != ExecutionResult.Passed)
            {
                Console.WriteLine("\n------- Blades information specifications Finished with errors. Please fix the listed failures and try again.");
                Console.WriteLine(bladesInfoPass.ResultDescription);
            }
            else
            {
                Console.WriteLine("\n+++++++ Blades information specifications was successfully verified.");
            }

            //Verify Chassis Health
            chassisHealthPass = cMFunctionalExecution.CheckChassisHealth();

            if (chassisHealthPass.result != ExecutionResult.Passed)
            {
                Console.WriteLine("\n------- Chassis Health Finished with errors. Please fix the listed failures and try again.");
            }
            else
            {
                Console.WriteLine("\n+++++++ Chassis Health was successfully verified.");
            }

            //Verify all Blades Health

            bladesHealthPass = cMFunctionalExecution.VerifyBladesHealth(skuDefinistionXml);
            if (bladesHealthPass.result != ExecutionResult.Passed)
            {
                Console.WriteLine("\n------- Blades Health check Finished with errors. Please fix the listed failures and try again.");
            }
            else
            {
                Console.WriteLine("\n+++++++ All blades were verified healthy.");
            }
        }

        public void VerifyChassisSpec(string chassisManagerEndPoint, string skuDefinitionXmlFileName, string userName, string userPassword)
        {
            CM_FunctionalTests cMFunctionalExecution = new CM_FunctionalTests(chassisManagerEndPoint, userName, userPassword);

            TestsResultResponse chassisInfoPass;
            TestsResultResponse bladesInfoPass;
            TestsResultResponse chassisHealthPass;
            TestsResultResponse bladesHealthPass;

            Console.WriteLine("/n Starting Functional BVT tests");

            //Verify Chassis Health
            chassisInfoPass = cMFunctionalExecution.CheckChassisInfo(skuDefinitionXmlFileName);

            if (chassisInfoPass.result != ExecutionResult.Passed)
            {
                Console.WriteLine("\n------- Chassis Info verification Finished with errors. Please fix the listed failures and try again.");
            }
            else
            {
                Console.WriteLine("\n+++++++ Chassis Info verification was successfully verified.");
            }

            //Verify Blades Info
            bladesInfoPass = cMFunctionalExecution.VerifyBladesInfo(skuDefinitionXmlFileName);
            if (bladesInfoPass.result != ExecutionResult.Passed)
            {
                Console.WriteLine("\n------- Blades information specifications Finished with errors. Please fix the listed failures and try again.");
                Console.WriteLine(bladesInfoPass.ResultDescription);
            }
            else
            {
                Console.WriteLine("\n+++++++ Blades information specifications was successfully verified.");
            }

            //Verify Chassis Health
            chassisHealthPass = cMFunctionalExecution.CheckChassisHealth();

            if (chassisHealthPass.result != ExecutionResult.Passed)
            {
                Console.WriteLine("\n------- Chassis Health Finished with errors. Please fix the listed failures and try again.");
            }
            else
            {
                Console.WriteLine("\n+++++++ Chassis Health was successfully verified.");
            }

            //Verify all Blades Health

            bladesHealthPass = cMFunctionalExecution.VerifyBladesHealth(skuDefinitionXmlFileName);
            if (bladesHealthPass.result != ExecutionResult.Passed)
            {
                Console.WriteLine("\n------- Blades Health check Finished with errors. Please fix the listed failures and try again.");
            }
            else
            {
                Console.WriteLine("\n+++++++ All blades were verified healthy.");
            }
        }
    }
}
