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
    /// Represents the IPMI 'SetEnergyStorage' OEM request message.
    /// </summary>
    [IpmiMessageRequest(IpmiFunctions.OemGroup, IpmiCommand.SetEnergyStorage)]
    internal class SetEnergyStorageRequest : IpmiRequest
    {
        /// <summary>
        /// Energy Storage State
        /// </summary>
        private readonly byte state;

        /// <summary>
        /// Percentage Charge
        /// </summary>
        private readonly byte percentCharge;

        /// <summary>
        /// Hold Time
        /// </summary>
        private readonly ushort holdTime;

        /// <summary>
        /// Time Stamp
        /// </summary>
        private readonly uint timestamp;

        /// <summary>
        /// Set Energy Storage State
        /// </summary>
        internal SetEnergyStorageRequest(EnergyStorageState state, bool presence, byte charge, ushort holdTime, DateTime timestamp)
        { 
            if(presence)
                this.state = 0x01;

            this.state = (byte)(this.state | ((byte)state << 2));

            this.percentCharge = charge;

            this.holdTime = holdTime;

            this.timestamp = Convert.ToUInt32(IpmiSharedFunc.SecondsFromUnixOffset(timestamp));
        }


        /// <summary>
        /// Energy Storage State
        /// </summary>       
        [IpmiMessageData(0)]
        public byte State
        {
            get { return this.state; }
        }

        /// <summary>
        /// Percentage Charge Remaining
        /// </summary>       
        [IpmiMessageData(1)]
        public byte PercentCharge
        {
            get { return this.percentCharge; }
        }

        /// <summary>
        /// Energy Storage Hold Time
        /// </summary>       
        [IpmiMessageData(2)]
        public ushort HoldTime
        {
            get { return this.holdTime; }
        }

        /// <summary>
        /// Energy Storage update time stamp
        /// </summary>       
        [IpmiMessageData(4)]
        public uint Timestamp
        {
            get { return this.timestamp; }
        }
    }
}
