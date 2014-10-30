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

namespace CMTestAutomationInterface.Controller
{
    internal class ChassisManagerUserOptions : ChassisManagerOptionBase
    {
        [ChassisManagerOption(LongOptionName = "Help", ShortOptionName = 'h', OptionDescription = "Show command help.")]
        public bool Help { get; set; }

        [ChassisManagerOption(LongOptionName = "RunBatches", ShortOptionName = 'a', OptionDescription = "Run all test cases.")]
        public bool RunBatches { get; set; }

        [ChassisManagerOption(LongOptionName = "RunBatch", ShortOptionName = 'b', OptionDescription = "Run single or multiple test cases.")]
        public bool RunBatch { get; set; }

        [ChassisManagerOption(LongOptionName = "RunFuncBvt", ShortOptionName = 't', OptionDescription = "Run functional Bvt test.")]
        public bool RunFuncBvt { get; set; }

        [ChassisManagerOption(LongOptionName = "VerifyCSpec", ShortOptionName = 'v', OptionDescription = "Verify Chassis specification.")]
        public bool VerifyCSpec { get; set; }

        [ChassisManagerOption(LongOptionName = "GetCmApiInfo", ShortOptionName = 'c', OptionDescription = "Get Chassis Manager Api Info.")]
        public bool GetCmApiInfo { get; set; }

        [ChassisManagerOption(LongOptionName = "FixCounterNames", ShortOptionName = 'n', OptionDescription = "Fix Counter Names.")]
        public bool FixCounterNames { get; set; }

        [ChassisManagerOption(LongOptionName = "SummaryReports", ShortOptionName = 'r', OptionDescription = "Create Summary Report From Batch Results.")]
        public bool SummaryReports { get; set; }

        [ChassisManagerOption(LongOptionName = "SampleBatchFile", ShortOptionName = 's', OptionDescription = "Run Create sample Batch File.")]
        public bool SampleBatchFile { get; set; }
    }
}
