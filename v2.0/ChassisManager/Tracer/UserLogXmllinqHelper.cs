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

namespace Microsoft.GFS.WCS.ChassisManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.ServiceModel;
    using Microsoft.GFS.WCS.Contracts;
    using System.IO;
    using System.Globalization;

    public static class UserLogXmllinqHelper
    {
        /// <summary>
        /// Unified API for querying user chassis audit logs - both timestamp and maxEntries used as input
        /// </summary>
        /// <param name="filterStartTime"></param>
        /// <param name="filterEndTime"></param>
        /// <param name="maxEntries"></param>
        /// <returns>Returns list of user log when success else returns null.</returns>
        public static List<LogEntry> GetFilteredLogEntries(DateTime filterStartTime, DateTime filterEndTime, int maxEntries)
        {
            if (Tracer.GetCurrentUserLogFilePath() == null)
                return null;

            try
            {
                List<LogEntry> timestampFilteredEntries = new List<LogEntry>();

                using (FileStream fileStream = new FileStream(
                                Tracer.GetCurrentUserLogFilePath(), FileMode.Open,
                                FileAccess.Read, FileShare.ReadWrite))
                using (BufferedStream bstream = new BufferedStream(fileStream))
                using (StreamReader reader = new StreamReader(bstream))
                {
                    int index = 0;
                    int count = 2048; // Reading 2K characters at a time to alleviate memory pressure
                    // Splitting file with arbitrary chunks size may result in chopped 'partial' XML data which is saved in this variable
                    // This data will be merged with the subsequent (consecutive) XML data 
                    string prevEntry = null; 
                                        
                    while (!reader.EndOfStream)
                    {
                        char[] localbuffer = new char[count];
                        reader.Read(localbuffer, index, count);
                        string myData = new string(localbuffer);
                        myData = prevEntry + myData;
                        string[] subStrings = System.Text.RegularExpressions.Regex.Split(myData, @"ApplicationData>");
                        if (subStrings.Length < 1)
                            break;
                        prevEntry = subStrings[subStrings.Length - 1];

                        for (int i = 0; i < subStrings.Length - 1; i++)
                        {
                            string str = subStrings[i];
                            if (str.Length > 2 && str.Trim().EndsWith("</"))
                            {
                                string currentEntry = (str.Remove(str.Length - 2));
                                string[] tokens = currentEntry.Trim().Split(new char[] { ',' });
                                LogEntry timestampFilteredEntry = new LogEntry();
                                if (DateTime.TryParse(tokens[0], out timestampFilteredEntry.eventTime))
                                {
                                    timestampFilteredEntry.eventTime = DateTime.ParseExact(tokens[0], "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                                    timestampFilteredEntry.eventDescription = currentEntry.Replace(tokens[0] + ",", "");
                                    timestampFilteredEntries.Add(timestampFilteredEntry);
                                }
                                else
                                {
                                    Tracer.WriteWarning("GetFilteredLogEntries(): Reading Chassis user log - ignoring entry '({0})' due to unparse-able date ", tokens[0]);
                                    // Skipping the entry since date is not parse-able
                                }
                            }
                        }
                        prevEntry = subStrings[subStrings.Length - 1];
                    }
                }
                timestampFilteredEntries.Reverse();
                return timestampFilteredEntries.Take(maxEntries).ToList();
            }
            catch (Exception e)
            {
                Tracer.WriteError("GetFilteredLogEntries(): Reading Chassis user log exception " + e.Message);
                return null;
            }
        }
    }
}
