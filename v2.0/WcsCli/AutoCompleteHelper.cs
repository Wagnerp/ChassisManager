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

namespace Microsoft.GFS.WCS.WcsCli
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Policy;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.GFS.WCS.WcsCli;

    /// <summary>
    /// Class implements the logic for tab autocompletion.
    /// Stores all the commands and returs the matching string for a prefix.
    /// </summary>
    public static class AutoCompleteHelper
    {
        private static string previousPrefix = string.Empty;
        private static int tabCount = 0;
        public static AutoComplete.Completion HandleAutoComplete(string currentPrefix)
        {
            if (string.Compare(currentPrefix, previousPrefix) == 0)
            {
                tabCount++;
            }
            else
            {
                previousPrefix = currentPrefix;
                tabCount = 0;
            }

            string prefix = currentPrefix;
            var stringArray = new string[]
            {
                "wcscli", 
                // Infrastructure commands
                "-getserviceversion", "-getchassisinfo", "-getchassishealth", "-getbladeinfo", "-getbladehealth", 
                "-updatepsufw", "-getpsufwstatus",
                
                // Blade Management commands
                 "-getpowerstate", "-setpoweron", "-setpoweroff", "-getbladestate", "-setbladeon", "-setbladeoff", "-setbladedefaultpowerstate",
                 "-getbladedefaultpowerstate", "-setbladeactivepowercycle", "-getnextboot", "-setnextboot", "-setbladeattentionledon",
                 "-setbladeattentionledoff", "-readbladelog", "-clearbladelog", "-getbladepowerreading", "-getbladepowerlimit",
                 "-setbladepowerlimit", "-setbladepowerlimiton", "-setbladepowerlimitoff", "-setdatasafebladeon", "-setdatasafepoweron",
                 "-setdatasafebladeoff", "-setdatasafepoweroff", "-setbladedatasafeactivepowercycle", "-getbladedatasafepowerstate",
                 "-getbladebiospostcode", "-setbladepsualertdpc", "-getbladepsualertdpc", "-getbladepsualert", "-activatedeactivatebladepsualert",
                 "-getbladeassetinfo", "-setbladeassetinfo",

               // Chassis Management commands
                 "-getchassisattentionledstatus", "-setchassisattentionledon", "-setchassisattentionledoff", "-readchassislog", "-clearchassislog",
                 "-getacsocketpowerstate", "-setacsocketpowerstateon", "-setacsocketpowerstateoff", "-getchassismanagerassetinfo",
                 "-getpdbassetinfo", "-setchassismanagerassetinfo", "-setpdbassetinfo",

               // Local commands, CLI serial mode only
                 "-setnic", "-getnic", "-clear",

               // Chassis Manager service configuration commands 
                 "-startchassismanager", "-stopchassismanager", "-getchassismanagerstatus", "-enablechassismanagerssl", "-disablechassismanagerssl",

               // User Management Commands
                 "-adduser", "-changeuserrole", "-changeuserpwd", "-removeuser",

               // Serial Session Commands
                 "-startbladeserialsession", "-stopbladeserialsession", "-startportserialsession", "-stopportserialsession", "-establishcmconnection",
                 "-terminatecmconnection"
            };

            var suffixes = stringArray.ToList().Where(a => a.StartsWith(prefix)).Select(s => s.Substring(prefix.Length)).ToArray();
            Array.Sort(suffixes, StringComparer.InvariantCultureIgnoreCase);
            return new AutoComplete.Completion(prefix, suffixes);
        }
    }
}
