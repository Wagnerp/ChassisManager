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
using System.Runtime.CompilerServices;

namespace ChassisValidation
{
    internal static class ChassisManagerTestHelper
    {
        private static readonly Random random = new Random(Guid.NewGuid().GetHashCode());

        /// <summary>
        ///     Checks if two values are equal. The method logs a success if the two values are equal,
        ///     or it logs a failure.
        /// </summary>
        /// <typeparam name="T">The type of the two values to be compared.</typeparam>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="message">The message to be logged.</param>
        /// <returns>True if the two values are equal; false, otherwise.</returns>
        internal static bool AreEqual<T>(T expected, T actual, string message, [CallerMemberName]
                                         string testName = null)
        {
            if (expected.Equals(actual))
            {
                CmTestLog.Success(message, testName);
                return true;
            }
            CmTestLog.Failure(string.Format("{0}{1}", message, string.Format(" (Expected: {0}, Actual: {1}) ", expected, actual)), testName);
            return false;
        }

        /// <summary>
        ///     Checks if a given condition is true. The method logs a success if the condition is true,
        ///     or it logs a failure.
        /// </summary>
        /// <param name="condition">The condition to be checked.</param>
        /// <param name="message">The message to be logged.</param>
        /// <returns>True if the condition holds; false, otherwise.</returns>
        internal static bool IsTrue(bool condition, string message, [CallerMemberName]
                                    string testName = null)
        {
            if (condition)
            {
                CmTestLog.Success(message, testName);
                return true;
            }
            CmTestLog.Failure(message, testName);
            return false;
        }

        /// <summary>
        ///     An extension to an array object. Returns a random element from the array.
        ///     If the array is empty, the default value of the element type will be returned.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <returns>An random element in the array; or the default value of the element type.</returns>
        internal static T RandomOrDefault<T>(this T[] array)
        {
            return array.Length > 0 ? array[random.Next(array.Length)] : default(T);
        }

        /// <summary>
        ///     Generates a random interger between the given range.
        /// </summary>
        internal static int RandomInteger(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }

        internal static bool AreEqual(string propertyValue, int p1, string p2)
        {
            throw new NotImplementedException();
        }
    }
}
