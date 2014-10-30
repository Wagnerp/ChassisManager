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

    /// <summary>
    /// Represents the IPMI 'Activate/Deastivate Psu Alert' OEM request message.
    /// </summary>
    [IpmiMessageRequest(IpmiFunctions.OemGroup, IpmiCommand.ActivatePsuAlert)]
    internal class ActivePsuAlertRequest : IpmiRequest
    {
        /// <summary>
        /// Drives GPIOF7
        /// [1:0]
        ///     0 = GPIOF7, disables BLADE_EN2 signal
        ///     1 = GPIOF7, enabling BLADE_EN2 signal
        /// [5:2]
        ///     0 = No Action upon BLADE_EN2 Signal
        ///     1 = Asserts GPIO31 on the PCH and FAST_PROCHOT 
        ///         when BLADE_EN2 GPIOH6 on the BMC is asserted.
        ///         Then Implements default power cap.
        ///     2 = No PROCHOT, just set default power cap.
        /// </summary>
        private readonly byte autoProcHot;

        /// <summary>
        /// 00 = Leave current Default Power limit if set.
        /// 01 = Remove current Default Power limit. 
        ///      Forces re-arm, unmasking the BLADE_EN2 GPIOH6 on the BMC.
        /// </summary>
        private readonly byte removeDpc;


        /// <summary>
        /// Initialize instance of the class.
        /// </summary>
        /// <param name="enableAutoProcHot">Enables Automatic ProcHot</param>
        /// <param name="bmcAction">Bmc Action on PSU_Alert GPI</param>
        /// <param name="removeCap">Removes Default Power Cap</param>
        internal ActivePsuAlertRequest(bool enableAutoProcHot, BmcPsuAlertAction bmcAction, bool removeCap)
        {
            if (enableAutoProcHot)
                autoProcHot = 0x01;
            
            // Bmc Action is bit shifed in the enum
            autoProcHot = (byte)(autoProcHot | (byte)bmcAction);

            if (removeCap)
                removeDpc = 0x01;
        }

        /// <summary>
        /// Byte representations of bitmask for Automatic PSU_Alert
        /// Fast PROCHOT function.
        /// </summary>       
        [IpmiMessageData(0)]
        public byte AutoProcHot
        {
            get { return this.autoProcHot; }

        }

        /// <summary>
        ///  Modify Default Power Cap
        /// </summary>       
        [IpmiMessageData(1)]
        public byte RemoveDpc
        {
            get { return this.removeDpc; }

        }

    }
}
