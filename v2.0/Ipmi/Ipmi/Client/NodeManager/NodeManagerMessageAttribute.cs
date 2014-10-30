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

namespace Microsoft.GFS.WCS.ChassisManager.Ipmi.NodeManager
{
    using System;

    /// <summary>
    /// Defines a class as an Node Manager message.
    /// </summary>
    internal abstract class NodeManagerMessageAttribute : Attribute
    {
        /// <summary>
        /// Node Manager message function.
        /// </summary>
        private readonly NodeManagerFunctions function;

        /// <summary>
        /// Node Manager message command within the current function.
        /// </summary>
        private readonly NodeManagerCommand command;

        /// <summary>
        /// Node Manager message lenght within the current function.
        /// </summary>
        private readonly int dataLength;

        /// <summary>
        /// Initializes a new instance of the NodeManagerMessageAttribute class.
        /// </summary>
        /// <param name="function">Node Manager message function.</param>
        /// <param name="command">Node Manager message command.</param>
        protected NodeManagerMessageAttribute(NodeManagerFunctions function, NodeManagerCommand command)
        {
            this.function = function;
            this.command = command;
        }

        /// <summary>
        /// Initializes a new instance of the NodeManagerMessageAttribute class.
        /// </summary>
        /// <param name="function">Node Manager message function.</param>
        /// <param name="command">Node Manager message command.</param>
        /// <param name="dataLength">Node Manager message data length.</param>
        protected NodeManagerMessageAttribute(NodeManagerFunctions function, NodeManagerCommand command, int dataLength)
        {
            this.function = function;
            this.command = command;
            this.dataLength = dataLength;
        }

        /// <summary>
        /// Gets the Node Manager message function.
        /// </summary>
        internal NodeManagerFunctions NodeManagerFunctions
        {
            get { return this.function; }
        }

        /// <summary>
        /// Gets the Node Manager message command.
        /// </summary>
        internal NodeManagerCommand NodeManagerCommand
        {
            get { return this.command; }
        }
    }
}
