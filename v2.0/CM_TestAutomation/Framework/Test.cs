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

namespace Microsoft.GFS.WCS.Test.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Cache;
    using System.Net.Security;
    using System.Runtime.Serialization;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Timers;
    using Timer = System.Timers.Timer;

    /// <summary>
    /// This class object represents a batch of tests to run against a Chassis Manager.
    /// </summary>
    [DataContract(Name = "Test")]
    public class Test
    {
        /// <summary> Default time framework awaits for any given API to return. </summary>
        private const int ApiSlaDefaultInSeconds = 120;

        /// <summary>  Gets or sets Name of API to run. </summary>
        [DataMember(Order = 1)]
        public string Name { get; set; }

        /// <summary> Gets or sets Iterations. </summary>
        [DataMember(Order = 2)]
        public uint Iterations { get; set; }

        /// <summary> Gets or sets DelayBetweenIterationsInMS in one thousandth of a second.</summary>
        [DataMember(Order = 3)]
        public uint DelayBetweenIterationsInMS { get; set; }

        /// <summary> Gets or sets DelayBeforeStartInMS in one thousandth of a second.</summary>
        [DataMember(Order = 4)]
        public uint DelayBeforeStartInMS { get; set; }

        /// <summary> Gets or sets ApiSla.  If an API does not respond within this timeframe a message is displayed. </summary>
        [DataMember]
        public TimeSpan? ApiSla { get; set; }

        /// <summary> Gets or sets a value indicating whether to run or skip the test. </summary>
        [IgnoreDataMember]
        public bool SkipTest { get; set; }

        /// <summary> Sets default values if not specified or out of range. </summary>
        public void SetDefaults()
        {
            this.Iterations = this.Iterations == 0 ? 1 : this.Iterations;
        }

        /// <summary> Runs the given test. </summary>
        /// <param name="endpoint">Chassis Manager Endpoint. </param>
        /// <param name="sequenceName">Sequence Name.</param>
        /// <param name="sequenceInstance">Sequence execution Instance.</param>
        /// <param name="parameters">Number of times to call the API.</param>
        /// <returns>a TestRun object with test results.</returns>
        public ResultOfTest Run(
            string endpoint,
            string sequenceName,
            int sequenceInstance,
            IDictionary<string, string> parameters,
            TimeSpan? apiSlaFromSequence,
            string userName,
            string userPassword)
        {
            Stopwatch stopwatch = null;
            ResultOfTest testRun = null;
            Timer timeoutTimer = null;
            if (!this.ApiSla.HasValue)
            {
                this.ApiSla = apiSlaFromSequence.HasValue ? apiSlaFromSequence : TimeSpan.FromSeconds(ApiSlaDefaultInSeconds);
            }

            HttpWebResponse response = null;
            try
            {
                var restParams =
                    Helper.GetChassisManagerApiParameterList(this.Name).Select(
                        rp => string.Format("{0}={1}", rp, parameters[rp]));
                testRun = new ResultOfTest()
                {
                    Name = this.Name,
                    IterationsExecutedSuccessfully = 0,
                    RestUri =
                        string.Format(
                            "{0}/{1}{2}{3}",
                            endpoint,
                            this.Name,
                            restParams.Any() ? "?" : string.Empty,
                            string.Join("&", restParams)),
                    StartTime = DateTime.UtcNow,
                    State = TestRunState.Running,
                    SequenceName = sequenceName,
                    SequenceInstance = sequenceInstance
                };

                ServicePointManager.ServerCertificateValidationCallback +=
                    delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
                    {
                        return true;
                    };

                Thread.Sleep((int)this.DelayBeforeStartInMS);

                stopwatch = Stopwatch.StartNew();
                while (testRun.IterationsExecutedSuccessfully < this.Iterations &&
                       testRun.State != TestRunState.RunFailed)
                {
                    // Sleep between iterations but not on first iteration.
                    if (testRun.IterationsExecutedSuccessfully != 0)
                    {
                        Thread.Sleep((int)this.DelayBetweenIterationsInMS);
                    }

                    response = null;
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(testRun.RestUri);
                    httpWebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                    httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
                    httpWebRequest.UserAgent = Dns.GetHostName();
                    httpWebRequest.Method = "Get";
                    //if (runAsUserCredential == null)
                    //{
                    //    httpWebRequest.UseDefaultCredentials = true;
                    //    testRun.RunAsUserName = WindowsIdentity.GetCurrent().Name.Split('\\').LastOrDefault();
                    //}
                    //else
                    //{
                    //    testRun.RunAsUserName = runAsUserCredential.UserName;
                    //    httpWebRequest.PreAuthenticate = true;
                    //    httpWebRequest.UseDefaultCredentials = false;
                    //    httpWebRequest.Credentials = new NetworkCredential(@"CMPVT02\dciBuild", "CSIBuild!");
                    //}

                    testRun.RunAsUserName = userName;
                    httpWebRequest.PreAuthenticate = true;
                    httpWebRequest.UseDefaultCredentials = false;
                    httpWebRequest.Credentials = new NetworkCredential(userName, userPassword);

                    timeoutTimer = new Timer(this.ApiSla.Value.TotalMilliseconds);
                    timeoutTimer.Elapsed += delegate(object sender, ElapsedEventArgs args)
                    {
                        Console.WriteLine(
                            "\n!!!Timeout: Request {0} (Iteration:{1}) has not returned in SLA:{2} seconds; Total Lapsed time:{3} seconds\n",
                            httpWebRequest.RequestUri,
                            testRun.IterationsExecutedSuccessfully,
                            this.ApiSla.Value.TotalSeconds,
                            stopwatch.Elapsed.TotalSeconds);
                    };

                    timeoutTimer.Start();
                    // Default is 100,000 = 100 seconds. changing it to 3,600,000 = 1 Hour.
                    // TODO: Make this a parameter value instead of hard coded.
                    httpWebRequest.Timeout = 3600000;
                    stopwatch.Start();
                    response = (HttpWebResponse)httpWebRequest.GetResponse();
                    stopwatch.Stop();
                    testRun.ProcessResponse(response);
                }
            }
            catch (System.Net.WebException ex)
            {
                if (stopwatch.IsRunning)
                {
                    stopwatch.Stop();
                }

                if (ex.Response != null)
                {
                    testRun.ProcessResponse((HttpWebResponse)ex.Response);
                }
                else
                {
                    testRun.State = TestRunState.RunFailed;
                    testRun.ErrorMessage = ex.ToString();
                }
            }
            catch (Exception ex)
            {
                if (stopwatch.IsRunning)
                {
                    stopwatch.Stop();
                }

                testRun.State = TestRunState.RunFailed;
                testRun.ErrorMessage = ex.ToString();
            }
            finally
            {
                if (timeoutTimer != null)
                {
                    timeoutTimer.Close();
                    timeoutTimer.Dispose();
                }

                testRun.TotalExecutionTime = stopwatch.Elapsed;
                testRun.AverageExecutionTime = testRun.IterationsExecutedSuccessfully == 0
                                               ? TimeSpan.Zero
                                               : TimeSpan.FromTicks(
                                                                    testRun.TotalExecutionTime.Ticks /
                                                                    testRun.IterationsExecutedSuccessfully);
            }

            return testRun;
        }
    }
}
