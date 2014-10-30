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
using System.Reflection;
using CMTestAutomationInterface.Controller;

namespace CMTestAutomationInterface
{
    /// <summary>
    /// A very simple test engine to run test cases.
    /// </summary>
    public static class CommandFetcher
    { 
        public static void ExecuteUserCommand(string command, Dictionary<string, string> userInputsArgs)
        {
            ChassisManagerLauncher launcher = new ChassisManagerLauncher();
            
            List<KeyValuePair<string, string>> methodParam = new List<KeyValuePair<string, string>>();

            foreach (KeyValuePair<string, string> parm in userInputsArgs)
            {
                if (parm.Key != command && parm.Key != string.Empty)
                {
                    methodParam.Add(new KeyValuePair<string, string>(CorrectParameterName(command,parm.Key) , parm.Value));
                }
            }

            try
            {
                launcher.GetType().InvokeMember(command, BindingFlags.InvokeMethod, null, launcher,
                    methodParam.Select(d => d.Value).ToArray(),
                    null, null, methodParam.Select(d => d.Key).ToArray());
            }
            catch (TargetInvocationException ex)
            {
                ex.InnerException.Data["OriginalStackTrace"] = ex.InnerException.StackTrace;
                throw ex.InnerException;
            }
        }

        /// <summary>
        ///  this will take care of lower case sentivity
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        private static string CorrectParameterName(string methodName, string parameterName)
        {

            // get all public static methods of ChassisManagerLauncher type
            MethodInfo[] methodInfos = typeof(ChassisManagerLauncher).GetMethods(BindingFlags.Public |
                                                                                 BindingFlags.Static);

            // sort methods by name
            methodInfos = Array.FindAll(methodInfos, i => i.Name == methodName);
                        
            // Loop each method names Find first match
            foreach (MethodInfo methodInfo in methodInfos)
            {
                foreach (ParameterInfo param in methodInfo.GetParameters()) // name and parametertype
                {
                    if (string.Equals(parameterName, param.Name, StringComparison.InvariantCultureIgnoreCase))//  EqualsIgnoreCase(param.Name, p.Key))
                    {
                        return param.Name;
                    }
                }
            }


            return parameterName;
        }
    }
}
