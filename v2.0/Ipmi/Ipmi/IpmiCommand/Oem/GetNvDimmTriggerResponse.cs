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

namespace Microsoft.GFS.WCS.ChassisManager.Ipmi
{

    using System;

    /// <summary>
    /// Represents the IPMI 'Get NVDIMM Trigger' application response message.
    /// </summary>
    [IpmiMessageResponse(IpmiFunctions.OemGroup, IpmiCommand.GetNvDimmTrigger)]
    internal class GetNvDimmTriggerResponse : IpmiResponse
    {
        /// <summary>
        /// [1:0] ADR_TRIGGER (GPIO37) on PCH.
        ///     00h = Disabled
        ///     01h = ADR_TRIGGER - GPIO37.  Switch connected.
        ///     02h = SMI Trigger - GPIO06.  Manual BMC trigger Enabled.
        /// </summary>
        private byte adrTrigger;

        /// <summary>
        /// [1:0] Set Trigger
        ///     00 = Deassert Trigger
        ///     01 = Assert Trigger
        /// </summary>
        private byte trigger;

        /// <summary>
        /// Delay between ADR_COMPLETE and blade power off (seconds)
        /// Range: 00h – FFh
        /// </summary>
        private byte adrCompleteDelay;

        /// <summary>
        /// [7:0] NVDIMM Present Power-off Delay
        /// Range: 00h - FFh
        /// </summary>
        private byte nvdimmPresentPowerOffDelay;

        /// <summary>
        /// ADR_COMPLETE Status
        /// 00h = ADR_COMPLETE deasserted
        /// 01h = ADR_COMPLETE asserted
        /// </summary>
        private byte adrComplete;

        /// <summary>
        /// ADR_COMPLETE Power-off Delay Remaining Time
        /// Time remaining before power is turned off (seconds).
        /// </summary>
        private byte adrCompleteTimeRemaining;
        
        /// <summary>
        /// NVDIMM Present Power-off Delay Remaining Time
        /// Countdown timer value that starts from the value specified in the NVDIMM Present Power-off Delay field.
        /// </summary>
        private byte nvdimmPresentTimeRemaining;

        /// <summary>
        /// NVDIMM Trigger Mechanism.
        /// 0 =  Do nothing upon NVDIMM Trigger. HW switch on 
        ///      the mainboard will be disabled.  Signal loss
        ///      from the HSC comparator will not result in 
        ///      ADR trigger on the PCH
        /// 1 =  Automatically assert PCH GPI37 upon signal loss.
        ///      HW switch on the mainboard will be enabled.  
        ///      Signal loss from the HSC comparator will result 
        ///      in ADR trigger.
        /// 2 =  Bmc will trigger SMI routine in the BIOS.  The
        ///      command must be executed manually with this bit
        ///      set for the manual SMI.
        /// </summary>       
        [IpmiMessageData(0)]
        public byte AdrTriggerMechanism
        {
            get { return this.adrTrigger; }
            set { this.adrTrigger = value; }
        }

        /// <summary>
        /// Manual Trigger
        ///     0 = Deassert Trigger
        ///     1 = Assert Trigger
        /// </summary>
        [IpmiMessageData(1)]
        public byte ManualTrigger
        {
            get { return this.trigger; }
            set { this.trigger = value; }
        }

        /// <summary>
        /// Delay between ADR_COMPLETE and blade power off (seconds)
        /// Range: 00h – FFh
        /// </summary>
        [IpmiMessageData(2)]
        public byte AdrCompletePowerOffDelay
        {
            get { return this.adrCompleteDelay; }
            set { this.adrCompleteDelay = value; }
        }

        /// <summary>
        /// [7:0] NVDIMM Present Power-off Delay
        /// Range: 00h - FFh
        /// </summary>
        [IpmiMessageData(3)]
        public byte NvdimmPresentPowerOffDelay
        {
            get { return this.nvdimmPresentPowerOffDelay;  }
            set { this.nvdimmPresentPowerOffDelay = value; }
        }

        /// <summary>
        /// ADR_COMPLETE Status
        /// 00h = ADR_COMPLETE deasserted
        /// 01h = ADR_COMPLETE asserted
        /// </summary>
        [IpmiMessageData(4)]
        public byte AdrComplete
        {
            get { return this.adrComplete; }
            set { this.adrComplete = value; }
        }

        /// <summary>
        /// Power-off Delay Remaining Time
        /// Time remaining before power is turned off (seconds).
        /// </summary>
        [IpmiMessageData(5)]
        public byte AdrCompleteTimeRemaining
        {
            get { return this.adrCompleteTimeRemaining; }
            set { this.adrCompleteTimeRemaining = value; }
        }

        /// <summary>
        /// NVDIMM Present Power-off Delay Remaining Time
        /// Countdown timer value that starts from the value specified in the NVDIMM Present Power-off Delay field.
        /// </summary>
        [IpmiMessageData(6)]
        public byte NvdimmPresentTimeRemaining
        {
            get { return this.nvdimmPresentTimeRemaining; }
            set { this.nvdimmPresentTimeRemaining = value; }
        }

        /// <summary>
        /// NVDIMM Trigger Asserted
        /// </summary>
        public bool ManualTriggerAsserted
        {
            get {
                if ((trigger & 0x01) == 0x01)
                        return true;
                    else 
                        return false;
                }
        }

       
        /// <summary>
        /// Adr Trigger 
        /// </summary>
        public NvDimmTriggerAction AdrTriggerType
        {
            get
            {
                if(Enum.IsDefined(typeof(NvDimmTriggerAction), adrTrigger))
                    return (NvDimmTriggerAction)adrTrigger;
                else
                    return NvDimmTriggerAction.Unknown;
            }
        }
    }
}
