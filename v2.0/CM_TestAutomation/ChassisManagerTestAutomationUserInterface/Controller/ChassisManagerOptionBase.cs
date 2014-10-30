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

namespace CMTestAutomationInterface.Controller
{
    /// <summary>
    /// Base class for all option classes used by Launcher.
    /// </summary>
    public abstract class ChassisManagerOptionBase
    {
        /// <summary>
        /// The number of arguments being processed.
        /// </summary>
        public int InputProcessed { get; internal set; }
        
        /// <summary>
        /// The number of arguments being processed.
        /// </summary>
        public List<String> CommandOption { get; internal set; }

        /// <summary>
        /// The raw key-value pair parsed from the user input
        /// </summary>
        public Dictionary<String, String> CommandVaue { get; internal set; }

        /// <summary>
        /// The raw key-value pair parsed from the user input
        /// </summary>
        public KeyValuePair<String, String>[] UserInputsArgs { get; internal set; }
    }
}
