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
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.GFS.WCS.Test.ReportGenerator
{
    public class PerfCounterListParser
    {
        public static IEnumerable<Counter> ParseXml(string xml)
        {
            XDocument xDocument = XDocument.Parse(xml);
            IEnumerable<Counter> result = from counter in xDocument.Descendants("Counter")
                                          select new Counter()
                                          {
                                              PerfCounterName = counter.Element("PerfCounterName").SafeElementValue(),
                                              FriendlyName = counter.Element("FriendlyName").SafeElementValue()
                                          };
            return result;
        }
    }

    public class Counter
    {
        public string PerfCounterName { get; set; }

        public string FriendlyName { get; set; }

        public string ToString(int counterIndex = 0)
        {
            string result = string.Format("{0}{1}{2}", string.Format("CounterIndex:{0}\n\n", counterIndex), string.Format("PerfCounterName:{0}\n", this.PerfCounterName), string.Format("FriendlyName:{0}\n", this.FriendlyName));

            return result;
        }
    }

    internal static class XmlElementExtension
    {
        public static string SafeElementValue(this XElement element)
        {
            if (element != null)
            {
                return element.Value;
            }

            return String.Empty;
        }
    }
}
