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
using Microsoft.GFS.WCS.Contracts;

namespace Microsoft.GFS.WCS.ChassisManager
{

    /// <summary>
    /// Response class for Blade Power Statue (Blade_EN)
    /// </summary>
    public class BladePowerStatePacket
    {
        public CompletionCode CompletionCode;
        public byte BladePowerState;
        public byte DecompressionTime;

        /// <summary>
        /// Returns Power State
        /// </summary>
        public PowerState PowerStatus
        {
            get{
                   if(Enum.IsDefined(typeof(PowerState), BladePowerState))
                   {
                        return (PowerState)BladePowerState;
                   }
                   else
                   {
                        return PowerState.NA;
                   }
            
                }
        }
    }

    /// <summary>
    /// Blade Power Switch (Blade_EN) class
    /// </summary>
    public class BladePowerSwitch : ChassisSendReceive
    {
        private DeviceType deviceType;
        private byte deviceId;

        public BladePowerSwitch(byte deviceId)
        {
            this.deviceId = deviceId;
            deviceType = DeviceType.Power;
        }

        internal DeviceType DeviceType
        {
            get
            {
                return deviceType;
            }
        }

        internal byte DeviceId
        {
            get
            {
                return deviceId;
            }
        }

        /// <summary>
        /// Returns the state of the blade power switch (blade enable)
        /// </summary>
        internal BladePowerStatePacket GetBladePowerState()
        {
            return GetBladePowerState(this.DeviceId);
        }

        /// <summary>
        /// Returns the state of the blade power switch (blade enable)
        /// From Cached.  Preferred for state management and API calls
        /// as no command to the GPIO needs to occur.
        /// </summary>
        internal BladePowerStatePacket GetCachedBladePowerState()
        {
            return GetCachedBladePowerState(this.DeviceId);
        }

        private BladePowerStatePacket GetBladePowerState(byte deviceId)
        {
            // Initialize return packet 
            BladePowerStatePacket returnPacket = new BladePowerStatePacket();
            returnPacket.CompletionCode = CompletionCode.UnspecifiedError;
            returnPacket.BladePowerState = 3;

            try
            {
                BladePowerStateResponse stateResponse = (BladePowerStateResponse)this.SendReceive(this.DeviceType, deviceId, new BladePowerStateRequest(), typeof(BladePowerStateResponse));
                if (stateResponse.CompletionCode != 0)
                {
                    returnPacket.CompletionCode = (CompletionCode)stateResponse.CompletionCode;
                }
                else
                {
                    returnPacket.CompletionCode = CompletionCode.Success;
                    returnPacket.BladePowerState = stateResponse.BladePowerState;
                    returnPacket.DecompressionTime = stateResponse.DecompressionTime;
                }
            }
            catch (System.Exception ex)
            {
                returnPacket.CompletionCode = CompletionCode.UnspecifiedError;
                returnPacket.BladePowerState = 2;
                Tracer.WriteError(this.DeviceId, this.DeviceType, ex);
            }
            return returnPacket;
        }

        private BladePowerStatePacket GetCachedBladePowerState(byte deviceId)
        {
            // Initialize return packet 
            BladePowerStatePacket returnPacket = new BladePowerStatePacket();
            returnPacket.CompletionCode = CompletionCode.UnspecifiedError;
            returnPacket.BladePowerState = 3;

            try
            {
                BladeCachedPowerStateResponse stateResponse = (BladeCachedPowerStateResponse)this.SendReceive(this.DeviceType, deviceId, new BladeCachedPowerStateRequest(), typeof(BladeCachedPowerStateResponse));
                if (stateResponse.CompletionCode != 0)
                {
                    returnPacket.CompletionCode = (CompletionCode)stateResponse.CompletionCode;
                }
                else
                {
                    returnPacket.CompletionCode = CompletionCode.Success;
                    returnPacket.BladePowerState = stateResponse.BladePowerState;
                    returnPacket.DecompressionTime = stateResponse.DecompressionTime;
                }
            }
            catch (System.Exception ex)
            {
                returnPacket.CompletionCode = CompletionCode.UnspecifiedError;
                returnPacket.BladePowerState = 3;
                Tracer.WriteError(this.DeviceId, DeviceType.Power, ex);
            }
            return returnPacket;
        }

        internal BladePowerStatePacket SetBladePowerState(byte state)
        {
            return SetBladePowerState(this.DeviceId, state);
        }

        private BladePowerStatePacket SetBladePowerState(byte deviceId, byte state)
        {
            // Initialize return packet 
            BladePowerStatePacket returnPacket = new BladePowerStatePacket();
            returnPacket.CompletionCode = CompletionCode.UnspecifiedError;
            returnPacket.BladePowerState = 3;

            Tracer.WriteInfo("SetSledPowerState Switch id: " + deviceId);

            if (state == (byte)Contracts.PowerState.ON)
            {
                try
                {
                    BladePowerStateOnResponse response = (BladePowerStateOnResponse)this.SendReceive(this.DeviceType, deviceId, new BladePowerStateOnRequest(state), typeof(BladePowerStateOnResponse));
                    if (response.CompletionCode != 0)
                    {
                        returnPacket.CompletionCode = (CompletionCode)response.CompletionCode;
                    }
                    else
                    {
                        returnPacket.CompletionCode = CompletionCode.Success;
                        returnPacket.BladePowerState = 1; //NOTE: response is not actually returned.
                        returnPacket.DecompressionTime = response.DecompressionTime;
                    }
                }
                catch (System.Exception ex)
                {
                    returnPacket.CompletionCode = CompletionCode.UnspecifiedError;
                    returnPacket.BladePowerState = 3;
                    Tracer.WriteError(this.DeviceId, DeviceType.Power, ex);
                }
            }
            else
            {
                try
                {
                    BladePowerStateOffResponse response = (BladePowerStateOffResponse)this.SendReceive(this.DeviceType, deviceId, new BladePowerStateOffRequest(state), typeof(BladePowerStateOffResponse));
                    if (response.CompletionCode != 0)
                    {
                        returnPacket.CompletionCode = (CompletionCode)response.CompletionCode;
                    }
                    else
                    {
                        returnPacket.CompletionCode = CompletionCode.Success;
                        returnPacket.BladePowerState = 0;  // blade is off
                        returnPacket.DecompressionTime = 0; // blade is off.
                    }
                }
                catch (System.Exception ex)
                {
                    returnPacket.CompletionCode = CompletionCode.UnspecifiedError;
                    returnPacket.BladePowerState = 3;
                    Tracer.WriteError(this.DeviceId, DeviceType.Power, ex);
                }
            }

            return returnPacket;
        }
    }

    #region Blade Power Request Structures

    /// <summary>
    /// Gets the state of the blade power switch (Blade_EN)
    /// </summary>
    [ChassisMessageRequest(FunctionCode.GetServerPowerStatus)]
    internal class BladePowerStateRequest : ChassisRequest
    {
    }

    /// <summary>
    /// Gets the state of the cached blade power switch (Blade_EN)
    /// </summary>
    [ChassisMessageRequest(FunctionCode.GetCachedPowerStatus)]
    internal class BladeCachedPowerStateRequest : ChassisRequest
    {
    }

    /// <summary>
    /// Turns the Blade Power Switch On (Blade_EN = On)
    /// </summary>
    [ChassisMessageRequest(FunctionCode.TurnOnServer)]
    internal class BladePowerStateOnRequest : ChassisRequest
    {
        public BladePowerStateOnRequest(byte state)
        {
            this.PowerState = state;
        }

        [ChassisMessageData(0)]
        public byte PowerState
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Turns the Blade Power Switch Off (Blade_EN = Off)
    /// </summary>
    [ChassisMessageRequest(FunctionCode.TurnOffServer)]
    internal class BladePowerStateOffRequest : ChassisRequest
    {
        public BladePowerStateOffRequest(byte state)
        {
            this.PowerState = state;
        }

        [ChassisMessageData(0)]
        public byte PowerState
        {
            get;
            set;
        }
    }

    #endregion

    #region Blade Power Response Structures

    /// <summary>
    /// Turns the Blade Power Switch Off (Blade_EN = Off)
    /// </summary>
    [ChassisMessageRequest(FunctionCode.TurnOffServer)]
    internal class BladePowerStateOffResponse : ChassisResponse
    {
        private byte decompressionTime;

        [ChassisMessageData(0)]
        public byte DecompressionTime
        {
            get { return this.decompressionTime; }
            set { this.decompressionTime = value; }
        }
    }

    /// <summary>
    /// Turns the Blade Power Switch Off (Blade_EN = On)
    /// </summary>
    [ChassisMessageRequest(FunctionCode.TurnOnServer)]
    internal class BladePowerStateOnResponse : ChassisResponse
    {
        private byte decompressionTime;

        [ChassisMessageData(0)]
        public byte DecompressionTime
        {
            get { return this.decompressionTime; }
            set { this.decompressionTime = value; }
        }
    }

    [ChassisMessageResponse(FunctionCode.GetCachedPowerStatus)]
    internal class BladeCachedPowerStateResponse : ChassisResponse
    {
        private byte bladePowerState;
        private byte decompressionTime;

        [ChassisMessageData(0)]
        public byte BladePowerState
        {
            get { return this.bladePowerState; }
            set { this.bladePowerState = value; }
        }

        [ChassisMessageData(1)]
        public byte DecompressionTime
        {
            get { return this.decompressionTime; }
            set { this.decompressionTime = value; }
        }
    }

    [ChassisMessageResponse(FunctionCode.GetServerPowerStatus)]
    internal class BladePowerStateResponse : ChassisResponse
    {
        private byte bladePowerState;

        private byte decompressionTime;

        [ChassisMessageData(0)]
        public byte BladePowerState
        {
            get { return this.bladePowerState; }
            set { this.bladePowerState = value; }
        }

        [ChassisMessageData(1)]
        public byte DecompressionTime
        {
            get { return this.decompressionTime; }
            set { this.decompressionTime = value; }
        }
    }

    #endregion

}
