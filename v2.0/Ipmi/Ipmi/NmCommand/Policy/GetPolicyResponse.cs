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
    /// Represents the Node Manager 'Get Policy' response message.
    /// </summary>
    [NodeManagerMessageResponse(NodeManagerFunctions.NodeManager, NodeManagerCommand.GetPolicy)]
    public class GetPolicyResponse : NodeManagerResponse
    {
        /// <summary>
        /// Intel Manufacture Id
        /// </summary>
        private byte[] manufactureId;

        /// <summary>
        /// Domain Id
        /// [0:3] Domain Id
        /// [4]   Policy enabled.
        /// [5:7] Reserved. Write as 00.
        /// </summary>
        private byte domainId;

        /// <summary>
        /// Policy Type [0:3] Action [4] Correction [5:6] Persistence [7]
        /// </summary>
        private byte policyType;


        /// <summary>
        /// Node Manager Exception Action.
        /// </summary>
        private byte exceptionAction;


        /// <summary>
        /// Target limit depends on Policy Type [0:3]
        ///     0, 1, 3:    Power Limt in Watts
        ///     2:          Thorttle level of platform in % [whereby 100% is maximum throttling]
        ///     4:          Power Profile during boot mode.       
        /// </summary>
        private ushort targetLimit;

        /// <summary>
        /// Correction Time Limit:  the maximum time in ms that node manager must take corrective
        /// action to bring the platform back to the specified power limit before taking the
        /// policy exception action.
        /// 
        /// If the policy type defines Boot Time Policy the correction limit will be overridden 
        /// to zero.
        /// </summary>
        private uint correctionTime;

        /// <summary>
        /// Trigger Limit depends on Policy Type [0:3]
        ///     0: Trigger value will be ignored
        ///     1: Trigger value should defined temperature in Celsius.
        ///     2: Trigger should define time in 1/10th of a second.
        ///     3: Trigger should be in 1/10th of second after reset or startup.
        ///     4: Trigger Limit is not applicable for boot time and will be overridden to zero.  
        /// </summary>
        private ushort triggerLimit;

        /// <summary>
        ///  Statistics Reporting Period in seconds. The number of seconds that the measured power 
        ///  will be averaged over for the purpose of reporting statistics. Note that this value 
        ///  is different from the period that Node Manager uses for maintaining an average for the 
        ///  purpose of power control.
        /// </summary>
        private ushort statisticReporting;

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
        /// Domain Id
        /// </summary>
        [NodeManagerMessageData(4)]
        public byte DomainId
        {
            get { return (byte)(this.domainId & 0x0f); }
            set { this.domainId = value; }
        }

        /// <summary>
        /// Policy Enabled
        /// </summary>
        public bool PolicyEnabled
        {
            get { if((byte)(this.domainId & 0x10) == 0x10)
                    return true;
                  else 
                    return false;
            }
        }

        /// <summary>
        /// Per domain policy enabled
        /// </summary>
        public bool PerDomainPolicyEnabled
        {
            get
            {
                if ((byte)(this.domainId & 0x20) == 0x20)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Global Policy control enabled
        /// </summary>
        public bool GlobalPolicyControlEnabled
        {
            get
            {
                if ((byte)(this.domainId & 0x40) == 0x40)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Dcmi Policy control enabled
        /// </summary>
        public bool DcmiPolicyControlEnabled
        {
            get
            {
                if ((byte)(this.domainId & 0x80) == 0x80)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Policy Type
        /// </summary>
        [NodeManagerMessageData(5)]
        public byte PolicyType
        {
            get { return (byte)(this.policyType & 0x0f); }
            set { this.policyType = value; }
        }

        /// <summary>
        /// CPU Power Correction Aggression
        /// </summary>
        public byte PowerCorrection
        {
            get
            {
                return (byte)((this.policyType >> 6) & 0x03);
            }
        }

        /// <summary>
        ///  Policy Persistence
        /// </summary>
        public bool Persistent
        {
            get
            {
                if ((byte)(this.policyType & 0x80) == 0x80)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Node Manager Exception Action.
        /// </summary>
        [NodeManagerMessageData(6)]
        public byte ExceptionAction
        {
            get { return (byte)(this.exceptionAction & 0x03); }
            set { this.exceptionAction = value; }
        }

        /// <summary>
        /// Target limit depends on Policy Type [0:3]
        ///     0, 1, 3:    Power Limt in Watts
        ///     2:          Thorttle level of platform in % [whereby 100% is maximum throttling]
        ///     4:          Power Profile during boot mode.       
        /// </summary>
        [NodeManagerMessageData(7)]
        public ushort PowerLimit
        {
            get { return this.targetLimit; }
            set { this.targetLimit = value; }
        }

        /// <summary>
        /// Correction Time Limit:  the maximum time in ms that node manager must take corrective
        /// action to bring the platform back to the specified power limit before taking the
        /// policy exception action.
        /// 
        /// If the policy type defines Boot Time Policy the correction limit will be overridden 
        /// to zero.
        /// </summary>
        [NodeManagerMessageData(9)]
        public uint CorrectionTime
        {
            get { return this.correctionTime; }
            set { this.correctionTime = value; }
        }

        /// <summary>
        /// Trigger Limit depends on Policy Type [0:3]
        ///     0: Trigger value will be ignored
        ///     1: Trigger value should defined temperature in Celsius.
        ///     2: Trigger should define time in 1/10th of a second.
        ///     3: Trigger should be in 1/10th of second after reset or startup.
        ///     4: Trigger Limit is not applicable for boot time and will be overridden to zero.  
        /// </summary>
        [NodeManagerMessageData(13)]
        public ushort TriggerLimit
        {
            get { return this.triggerLimit; }
            set { this.triggerLimit = value; }
        }

        /// <summary>
        ///  Statistics Reporting Period in seconds. The number of seconds that the measured power 
        ///  will be averaged over for the purpose of reporting statistics. Note that this value 
        ///  is different from the period that Node Manager uses for maintaining an average for the 
        ///  purpose of power control.
        /// </summary>
        [NodeManagerMessageData(15)]
        public ushort StatisticReporting
        {
            get { return this.statisticReporting; }
            set { this.statisticReporting = value; }
        }

    }
}
