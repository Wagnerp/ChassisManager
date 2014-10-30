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
    /// Represents the IPMI 'Get Energy Storage' application response message.
    /// </summary>
    [IpmiMessageResponse(IpmiFunctions.OemGroup, IpmiCommand.SetEnergyStorage)]
    internal class GetEnergyStorageResponse : IpmiResponse
    {

        /// <summary>
        /// Energy Storage Presence
        /// </summary>
        private byte presence;

        /// <summary>
        /// Energy Storage State
        /// </summary>
        private byte state;

        /// <summary>
        /// Percentage Charge
        /// </summary>
        private byte percentCharge;

        /// <summary>
        /// Hold Time
        /// </summary>
        private ushort holdTime;

        /// <summary>
        /// Time Stamp
        /// </summary>
        private uint timestamp;

        /// <summary>
        /// Energy Storage Presence
        /// </summary>       
        [IpmiMessageData(0)]
        public byte Precence
        {
            get { return this.presence; }
            set { this.presence = (byte)(value & 0x03); }
        }

        /// <summary>
        /// Energy Storage State
        /// </summary>       
        [IpmiMessageData(0)]
        public byte EnergyState
        {
            get { return this.state; }
            set { this.state = (byte)((value & 0x1C) >> 2); }
        }

        /// <summary>
        /// Energy Storage Percentage Charge
        /// </summary>       
        [IpmiMessageData(1)]
        public byte PercentageCharge
        {
            get { return this.percentCharge; }
            set { this.percentCharge = value; }
        }

        /// <summary>
        /// Energy Storage HoldTime
        /// </summary>       
        [IpmiMessageData(2)]
        public ushort HoldTime
        {
            get { return this.holdTime; }
            set { this.holdTime = value; }
        }

        /// <summary>
        /// Energy Storage Raw Seconds off Set TimeStamp
        /// </summary>       
        [IpmiMessageData(4)]
        public uint RawTimeStamp
        {
            get { return this.timestamp; }
            set { this.timestamp = value; }
        }

        /// <summary>
        /// Local Energy Storage TimeStamp
        /// </summary>
        public DateTime TimeStamp
        {
            get { return IpmiSharedFunc.SecondsOffSet(this.timestamp); }
        }

    }
}
