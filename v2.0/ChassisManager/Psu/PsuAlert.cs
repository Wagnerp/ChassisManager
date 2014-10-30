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
    /// <summary>
    /// Chassis PDB PSU_ALERT.  PCA9535C (0x48) Pin 4.
    /// </summary>
    class PsuAlertSignal : ChassisSendReceive
    {

        /// <summary>
        /// Get Psu Alert
        /// </summary>
        internal PsuAlertSignalResponse GetPsuAlertSignal()
        {
            PsuAlertSignalResponse psuAlert = (PsuAlertSignalResponse)this.SendReceive(DeviceType.PsuAlertInput, 0xff,
                new PsuAlertSignalRequest(), typeof(PsuAlertSignalResponse), (byte)PriorityLevel.System);

            return psuAlert;
        }

        /// <summary>
        /// Set Psu Alert On
        /// </summary>
        internal SetPsuAlertOnResponse SetPsuAlertOn()
        {
            SetPsuAlertOnResponse psuAlert = (SetPsuAlertOnResponse)this.SendReceive(DeviceType.PsuAlertOutput, 0xff,
                new SetPsuAlertOnRequest(), typeof(SetPsuAlertOnResponse), (byte)PriorityLevel.System);

            return psuAlert;
        }

        /// <summary>
        /// Set Psu Alert Off
        /// </summary>
        internal SetPsuAlertOffResponse SetPsuAlertOff()
        {
            SetPsuAlertOffResponse psuAlert = (SetPsuAlertOffResponse)this.SendReceive(DeviceType.PsuAlertOutput, 0xff,
                new SetPsuAlertOffRequest(), typeof(SetPsuAlertOffResponse), (byte)PriorityLevel.System);

            return psuAlert;
        }

        /// <summary>
        /// Set Psu Alert Off
        /// </summary>
        internal GetPsuAlertOnOffResponse GetPsuAlertOnOffStatus()
        {
            GetPsuAlertOnOffResponse psuAlert = (GetPsuAlertOnOffResponse)this.SendReceive(DeviceType.PsuAlertOutput, 0xff,
                new GetPsuAlertOnOffRequest(), typeof(GetPsuAlertOnOffResponse), (byte)PriorityLevel.System);

            return psuAlert;
        }

    }

    /// <summary>
    /// Chassis PDB PSU_ALERT. Request Message.
    /// </summary>
    [ChassisMessageRequest(FunctionCode.PsuAlertInput)]
    internal class PsuAlertSignalRequest : ChassisRequest
    {
    }

    /// <summary>
    /// Chassis PDB PSU_ALERT. Response Message.
    /// </summary>
    [ChassisMessageResponse(FunctionCode.PsuAlertInput)]
    internal class PsuAlertSignalResponse : ChassisResponse
    {
        // Psu Alert Status
        private byte status;

        /// <summary>
        /// Psu Alert Status
        /// </summary>
        [ChassisMessageData(0)]
        public byte Status
        {
            get { return this.status; }
            set { this.status = value; }
        }

        /// <summary>
        /// If Completion Code is zero,
        /// this value returns the status
        /// of PSU Alert.
        /// </summary>
        public bool PsuAlertActive
        {
            get { return (status == 0x01 ? true : false); }
        }

    }

    /// <summary>
    /// Chassis PDB Set PSU_ALERT On. Request Message.
    /// </summary>
    [ChassisMessageRequest(FunctionCode.PsuAlertOuputOn)]
    internal class SetPsuAlertOnRequest : ChassisRequest
    {
    }

    /// <summary>
    /// Chassis PDB Set PSU_ALERT On. Response Message.
    /// </summary>
    [ChassisMessageResponse(FunctionCode.PsuAlertOuputOn)]
    internal class SetPsuAlertOnResponse : ChassisResponse
    {
        // Psu Alert Status
        private byte status;

        /// <summary>
        /// Psu Alert Status
        /// </summary>
        [ChassisMessageData(0)]
        public byte Status
        {
            get { return this.status; }
            set { this.status = value; }
        }

    }

    /// <summary>
    /// Chassis PDB Set PSU_ALERT On. Request Message.
    /// </summary>
    [ChassisMessageRequest(FunctionCode.PsuAlertOuputOff)]
    internal class SetPsuAlertOffRequest : ChassisRequest
    {
    }

    /// <summary>
    /// Chassis PDB Set PSU_ALERT Off. Response Message.
    /// </summary>
    [ChassisMessageResponse(FunctionCode.PsuAlertOuputOff)]
    internal class SetPsuAlertOffResponse : ChassisResponse
    {
        // Psu Alert Status
        private byte status;

        /// <summary>
        /// Psu Alert Status
        /// </summary>
        [ChassisMessageData(0)]
        public byte Status
        {
            get { return this.status; }
            set { this.status = value; }
        }

    }

    /// <summary>
    /// Get Chassis PDB SET PSU_ALERT On/Off. Request Message.
    /// </summary>
    [ChassisMessageRequest(FunctionCode.GetPsuAlertOuput)]
    internal class GetPsuAlertOnOffRequest : ChassisRequest
    {
    }

    /// <summary>
    /// Get Chassis PDB SET PSU_ALERT On/Off. Request Message.
    /// </summary>
    [ChassisMessageResponse(FunctionCode.GetPsuAlertOuput)]
    internal class GetPsuAlertOnOffResponse : ChassisResponse
    {
        // Set Psu Alert Status pin
        private byte status;

        /// <summary>
        // Set Psu Alert Status pin
        /// </summary>
        [ChassisMessageData(0)]
        public byte Status
        {
            get { return this.status; }
            set { this.status = value; }
        }

        /// <summary>
        /// If Completion Code is zero,
        /// this value returns the status
        /// of Set PSU Alert pin.
        /// </summary>
        public bool IsPsuAlertSet
        {
            get { return (status == 0x01 ? true : false); }
        }

    }

}
