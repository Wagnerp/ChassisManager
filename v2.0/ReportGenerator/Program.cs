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

namespace Microsoft.GFS.WCS.Test.ReportGenerator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Parse client call xml for 03 - https - 10 threads - 1 hour
            string clientCallXml = @"C:\Users\sbijay\Desktop\WCS Perf\Perf run results\03 - https - 10 threads - 1 hour\https___DVTCM03_8000-TrialPerfRunBatch.Results.xml";
            int testDurationInSec = 3600;
            string targetFolder = @"C:\Users\sbijay\Desktop\WCS Perf\Perf run results\03 - https - 10 threads - 1 hour\";
            ClientReport clientReport = new ClientReport();
            clientReport.GenerateClientReport(clientCallXml, testDurationInSec, targetFolder, "03 - https - 10 threads - 1 hour - ");

            //Parse client call xml for 03 - https - 1 thread - 1 hour
            clientCallXml = @"C:\Users\sbijay\Desktop\WCS Perf\Perf run results\03 - https - 1 thread - 1 hour\https___DVTCM03_8000-TrialPerfRunBatch.Results.xml";
            testDurationInSec = 3600;
            targetFolder = @"C:\Users\sbijay\Desktop\WCS Perf\Perf run results\03 - https - 1 thread - 1 hour\";
            clientReport = new ClientReport();
            clientReport.GenerateClientReport(clientCallXml, testDurationInSec, targetFolder, "03 - https - 1 thread - 1 hour - ");

            //string fileName2 = "ImagingTestInProduction - " + "test" + ".txt";
            //string logFilePath2 = @"C:\Temp\" + fileName2;
            //CreateLogFile(logFilePath2);
            //StreamWriter sw2 = new StreamWriter(logFilePath2);
            CreateReport createReport = new CreateReport();
            //ch.ExceptionTest();

            //Parse perflog file to generate latency data for 03 - https - 10 threads - 1 hour
            string dataFile = @"C:\Users\sbijay\Documents\Visual Studio 2012\Projects\WCS\Mt Rainier\Manageability\Developement\ReportGenerator\LatencyPerfCounters.xml";
            string perfLogFilePath = @"C:\Users\sbijay\Desktop\WCS Perf\Perf run results\03 - https - 10 threads - 1 hour\Perflogs\Server\DVTCM03_20121120-000002\Performance Counter - Copy.csv";
            string latencyResultFilePath = @"C:\Users\sbijay\Desktop\WCS Perf\Perf run results\03 - https - 10 threads - 1 hour\03 - https - 10 threads - 1 hour - LatencyData.html";
            string tableTitle = "WCF Service APIs - Server End - Latency (Sec)";
            createReport.GetPerfNumbersTableHtml(dataFile, perfLogFilePath, tableTitle, latencyResultFilePath);

            //Parse perflog file to generate throughput data for 03 - https - 10 threads - 1 hour
            dataFile = @"C:\Users\sbijay\Documents\Visual Studio 2012\Projects\WCS\Mt Rainier\Manageability\Developement\ReportGenerator\ThroughputPerfCounters.xml";
            tableTitle = "WCF Service APIs - Server End - Throughput (Calls / sec)";
            string throughputResultFilePath = @"C:\Users\sbijay\Desktop\WCS Perf\Perf run results\03 - https - 10 threads - 1 hour\03 - https - 10 threads - 1 hour - ThroughputCounters.html";
            createReport.GetPerfNumbersTableHtml(dataFile, perfLogFilePath, tableTitle, throughputResultFilePath);

            //Parse perflog file to generate Calls data for 03 - https - 10 threads - 1 hour            
            string callsDataFile = @"C:\Users\sbijay\Documents\Visual Studio 2012\Projects\WCS\Mt Rainier\Manageability\Developement\ReportGenerator\TotalCallsPerfCounters.xml";
            string failedCallsDataFile = @"C:\Users\sbijay\Documents\Visual Studio 2012\Projects\WCS\Mt Rainier\Manageability\Developement\ReportGenerator\FailedCallsPerfCounters.xml";
            tableTitle = "WCF Service APIs - Server End - Calls";
            string callsResultFilePath = @"C:\Users\sbijay\Desktop\WCS Perf\Perf run results\03 - https - 10 threads - 1 hour\03 - https - 10 threads - 1 hour - CallsData.html";
            createReport.GetCallsAndFailedCallsTableHtml(callsDataFile, failedCallsDataFile, perfLogFilePath, tableTitle, callsResultFilePath);

            //Parse perflog file to generate latency data for 03 - https - 1 thread - 1 hour
            dataFile = @"C:\Users\sbijay\Documents\Visual Studio 2012\Projects\WCS\Mt Rainier\Manageability\Developement\ReportGenerator\LatencyPerfCounters.xml";
            perfLogFilePath = @"C:\Users\sbijay\Desktop\WCS Perf\Perf run results\03 - https - 1 thread - 1 hour\PerfLogs\Server\DVTCM03_20121120-000003\Performance Counter.csv";
            latencyResultFilePath = @"C:\Users\sbijay\Desktop\WCS Perf\Perf run results\03 - https - 1 thread - 1 hour\03 - https - 1 thread - 1 hour - LatencyData.html";
            tableTitle = "WCF Service APIs - Server End - Latency (Sec)";
            createReport.GetPerfNumbersTableHtml(dataFile, perfLogFilePath, tableTitle, latencyResultFilePath);

            //Parse perflog file to generate throughput data for 03 - https - 1 thread - 1 hour
            dataFile = @"C:\Users\sbijay\Documents\Visual Studio 2012\Projects\WCS\Mt Rainier\Manageability\Developement\ReportGenerator\ThroughputPerfCounters.xml";
            tableTitle = "WCF Service APIs - Server End - Throughput (Calls / sec)";
            throughputResultFilePath = @"C:\Users\sbijay\Desktop\WCS Perf\Perf run results\03 - https - 1 thread - 1 hour\03 - https - 1 thread - 1 hour - ThroughputCounters.html";
            createReport.GetPerfNumbersTableHtml(dataFile, perfLogFilePath, tableTitle, throughputResultFilePath);

            //Parse perflog file to generate Calls data for 03 - https - 1 thread - 1 hour            
            callsDataFile = @"C:\Users\sbijay\Documents\Visual Studio 2012\Projects\WCS\Mt Rainier\Manageability\Developement\ReportGenerator\TotalCallsPerfCounters.xml";
            failedCallsDataFile = @"C:\Users\sbijay\Documents\Visual Studio 2012\Projects\WCS\Mt Rainier\Manageability\Developement\ReportGenerator\FailedCallsPerfCounters.xml";
            tableTitle = "WCF Service APIs - Server End - Calls";
            callsResultFilePath = @"C:\Users\sbijay\Desktop\WCS Perf\Perf run results\03 - https - 1 thread - 1 hour\03 - https - 1 thread - 1 hour - CallsData.html";
            createReport.GetCallsAndFailedCallsTableHtml(callsDataFile, failedCallsDataFile, perfLogFilePath, tableTitle, callsResultFilePath);
        }
    }
}
