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
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security.Cryptography;
    using System.Text;
    using Microsoft.GFS.WCS.Contracts;

    /// <summary>
    /// This static class has helper methods used by various parts for Test Framework.
    /// </summary>
    public static class Helper
    {
        /// <summary> List of APIs and respective parameters. </summary>
        private static readonly Dictionary<string, Tuple<Type, List<string>>> Apis =
            new Dictionary<string, Tuple<Type, List<string>>>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary> Gets APIs and its parameters. </summary>
        /// <returns> List of APIs </returns>
        public static List<string> GetChassisManagerApiList()
        {
            if (!Apis.Any())
            {
                GetApiListAndParameters(typeof(IChassisManager));
            }

            return Apis.Keys.ToList();
        }

        /// <summary> Gets APIs return Type. </summary>
        /// <param name="api"> Name of API for which to get parameters of. </param>
        /// <returns> Type of return value of given API. </returns>
        public static Type GetChassisManagerApiReturnType(string api)
        {
            if (GetChassisManagerApiList().Contains(api, StringComparer.InvariantCultureIgnoreCase))
            {
                return Apis[api].Item1;
            }

            throw new ArgumentException(string.Format("api '{0}' Not found", api));
        }

        /// <summary> Gets APIs and its parameters. </summary>
        /// <param name="api"> Name of API for which to get parameters of. </param>
        /// <returns> List of parameters for given API. </returns>
        public static List<string> GetChassisManagerApiParameterList(string api)
        {
            if (GetChassisManagerApiList().Contains(api, StringComparer.InvariantCultureIgnoreCase))
            {
                return Apis[api].Item2;
            }

            throw new ArgumentException(string.Format("api '{0}' Not found", api));
        }

        /// <summary> Dumps all APIs and respective parameters of IChassisManager interface. </summary>
        /// <returns> A string having all API and parameters listed. </returns>
        public static string DumpAllChassisManagerApi()
        {
            var dump = new StringBuilder();
            GetChassisManagerApiList().ForEach(api => dump.Append(DumpChassisManagerApi(api)));
            return dump.ToString();
        }

        /// <summary> Dumps APIs and respective parameters of IChassisManager interface. </summary>
        /// <param name="api"> Name of api to get info of. </param>
        /// <returns> A string having all API and parameters listed. </returns>
        public static string DumpChassisManagerApi(string api)
        {
            return string.Format(
                "{0} {1}({2})\n",
                Helper.GetChassisManagerApiReturnType(api),
                api,
                string.Join(",", Helper.GetChassisManagerApiParameterList(api)));
        }

        /// <summary> Saves TestBatch object to a file. </summary>
        /// <param name="theObj"> The object instance to be serialized into fileName. </param>
        /// <param name="fileName"> Name of file to save TestBatch to. </param>
        public static void SaveToFile(object theObj, string fileName)
        {
            try
            {
                Console.WriteLine("Details of [{0}] being saved to {1}", theObj, fileName);
                var serializer = new DataContractSerializer(theObj.GetType());
                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    serializer.WriteObject(stream, theObj);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to Save {0} to {1}\n{2}", theObj.GetType(), fileName, ex);
            }
        }

        /// <summary> Loads TestBatch object from a file. </summary>
        /// <typeparam name="TDeserializeType"> Type of object to Deserialize from file. </typeparam>
        /// <param name="fileName"> Name of file to load TestBatch from. </param>
        /// <returns> An object of type T. </returns>
        public static TDeserializeType LoadFromFile<TDeserializeType>(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                var serializer = new DataContractSerializer(typeof(TDeserializeType));
                var retObject = (TDeserializeType)serializer.ReadObject(stream);
                // Console.WriteLine("Loaded Object {0} from file {1}", typeof(TDeserializeType), fileName);
                return retObject;
            }
        }

        /// <summary>
        /// Shuffles a list in random order.
        /// </summary>
        /// <typeparam name="T"> Type of List. </typeparam>
        /// <param name="list"> List of values to be randomly ordered. </param>
        public static void Shuffle<T>(this IList<T> list)
        {
            var provider = new RNGCryptoServiceProvider();
            var n = list.Count;
            while (n > 1)
            {
                var box = new byte[1];
                do
                {
                    provider.GetBytes(box);
                }
                while (!(box[0] < n * (Byte.MaxValue / n)));
                var k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary> Gets APIs and its parameters. </summary>
        /// <param name="contract"> The type having the contrtactual APIs. </param>        
        private static void GetApiListAndParameters(Type contract)
        {
            var contractMethodNames = contract.GetMethods().Select(m => m.Name);
            lock (Apis)
            {
                contract.GetMethods().Where(m => contractMethodNames.Contains(m.Name)).ToList().ForEach(
                    mi =>
                    { 
                        var pis = mi.GetParameters();
                        Apis[mi.Name] = new Tuple<Type, List<string>>(
                            mi.ReturnType,
                            pis.Any() ? pis.Select(pi => pi.Name).ToList() : new List<string>());
                    });
            }
        }
    }
}
