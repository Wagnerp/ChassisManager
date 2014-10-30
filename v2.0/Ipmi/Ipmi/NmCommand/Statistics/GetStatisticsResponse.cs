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

    /// <summary>
    /// Represents the Node Manager 'Get Statistics' response message.
    /// </summary>
    [NodeManagerMessageResponse(NodeManagerFunctions.NodeManager, NodeManagerCommand.GetStatistics)]
    public class GetStatisticsResponse : NodeManagerResponse
    {
        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        private byte[] manufactureId;

        /// <summary>
        /// Crrent value
        /// </summary>
        private ushort currentVal;

        /// <summary>
        /// Minimum value
        /// </summary>
        private ushort minimumVal;

        /// <summary>
        /// Maximum value
        /// </summary>
        private ushort maximumVal;

        /// <summary>
        /// Average value
        /// </summary>
        private ushort averageVal;

        /// <summary>
        /// Time Stamp
        /// </summary>
        private uint timestamp;

        /// <summary>
        /// Statistics Reporting period.
        /// </summary>
        private uint statisticsReporting;

        /// <summary>
        /// Domain Id | Policy State
        /// </summary>
        private byte domainId;

        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        [NodeManagerMessageData(0, 3)]
        public byte[] ManufactureId
        {
            get { return this.manufactureId; }
            set { this.manufactureId = value; }
        }

        /// <summary>
        /// Current Value
        /// </summary>
        [NodeManagerMessageData(3)]
        public ushort CurrentValue
        {
            get { return this.currentVal; }
            set { this.currentVal = value; }
        }

        /// <summary>
        /// Minimum Value
        /// </summary>
        [NodeManagerMessageData(5)]
        public ushort MinimumValue
        {
            get { return this.minimumVal; }
            set { this.minimumVal = value; }
        }

        /// <summary>
        /// Maximum Value
        /// </summary>
        [NodeManagerMessageData(7)]
        public ushort MaximumValue
        {
            get { return this.maximumVal; }
            set { this.maximumVal = value; }
        }

        /// <summary>
        /// Average Value
        /// </summary>
        [NodeManagerMessageData(9)]
        public ushort AverageValue
        {
            get { return this.averageVal; }
            set { this.averageVal = value; }
        }

        /// <summary>
        /// Maximum Value
        /// </summary>
        [NodeManagerMessageData(11)]
        public uint TimeStamp
        {
            get { return this.timestamp; }
            set { this.timestamp = value; }
        }

        /// <summary>
        /// Statistics Reporting
        /// </summary>
        [NodeManagerMessageData(15)]
        public uint StatisticsReporting
        {
            get { return this.statisticsReporting; }
            set { this.statisticsReporting = value; }
        }

        /// <summary>
        /// Domain Id | Policy State
        /// </summary>
        [NodeManagerMessageData(19)]
        public byte DomainIdPolicyState
        {
            get { return this.domainId; }
            set { this.domainId = value; }
        }

        /// <summary>
        /// Domain Id
        /// </summary>
        public byte DomainId
        {
            get { return (byte)(this.domainId & 0x0f); }
        }

        /// <summary>
        /// Policy State
        /// </summary>
        public byte PolicyState
        {
            get { return (byte)((this.domainId >> 4 )& 0x01); }
        }

        /// <summary>
        /// Policy Operational State
        /// </summary>
        public byte PolicyOperational
        {
            get { return (byte)((this.domainId >> 5) & 0x01); }
        }

        /// <summary>
        /// Policy Measurement State
        /// </summary>
        public byte MeasurementState
        {
            get { return (byte)((this.domainId >> 6) & 0x01); }
        }

        /// <summary>
        /// Policy Activation State
        /// </summary>
        public byte PolicyActivationState
        {
            get { return (byte)((this.domainId >> 7) & 0x01); }
        }


    }
}
