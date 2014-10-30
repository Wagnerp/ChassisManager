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

namespace ChassisValidation
{
    internal static class CmConstants
    {
        /// <summary>
        /// Represents a server blade.
        /// </summary>
        public const string ServerBladeType = "Server";

        /// <summary>
        /// Represents a JBOD blade.
        /// </summary>
        public const string JbodBladeType = "Jbod";

        /// <summary>
        /// Represents a Empty blade.
        /// </summary>
        public const string EmptyBladeType = "";

        /// <summary>
        /// Represents a Empty blade.
        /// </summary>
        public const string UnKnownBladeType = "Unknown";

        /// <summary>
        /// Indicates a blade is healthy.
        /// </summary>
        public const string HealthyBladeState = "ON";

        /// <summary>
        /// Number of blades in chassis.
        /// </summary>
        public const int Population = 24;

        /// <summary>
        /// Number of power switches (aka AC sockets) in chassis.
        /// </summary>
        public const int NumPowerSwitches = 3;

        /// <summary>
        /// Number of fans in chassis.
        /// </summary>
        public const int NumFans = 6;

        /// <summary>
        /// Number of PSUs in chassis.
        /// </summary>
        public const int NumPsus = 6;

        /// <summary>
        /// Number of batteries in chassis
        /// </summary>
        public const int NumBattery = 6;

        /// <summary>
        /// Timeout value for a serial session in seconds.
        /// </summary>
        public const int SerialTimeoutSeconds = 300;

        /// <summary>
        /// Timeout value for an HTTP request in seconds.
        /// </summary>
        public const int RequestTimeoutSeconds = 300;

        /// <summary>
        /// The time duration needed to power on a blade in seconds.
        /// </summary>
        public const int BladePowerOnSeconds = 60;

        /// <summary>
        /// The time duration needed to power off a blade in seconds.
        /// </summary>
        public const int BladePowerOffSeconds = 30;

        /// <summary>
        /// Invalid blade: Not in between 1-24 range.
        /// </summary>
        public const int InvalidBladeId = 28;

        /// <summary>
        /// Invalid blade: Negative value.
        /// </summary>
        public const int InvalidNegtiveBladeId = -2;

        /// <summary>
        /// Default powerr state value for Empty blade
        /// </summary>
        public const string EmptyDefautState = "NA";

        /// <summary>
        /// Used by connection context id for Domain user
        /// </summary>
        public const int TestConnectionDomainUserId = 10;

        /// <summary>
        /// Used by connection context id for Local Id
        /// </summary>
        public const int TestConnectionLocalUserId = 20;

        /// <summary>
        /// OffTime for Active powercycle
        /// </summary>
        public const int OffTime = 100;

        /// <summary>
        /// Negative off time for active power cycle
        /// </summary>
        public const int ngtveOffTime = -100;

        /// <summary>
        /// More than 255 seconds is invalid offltime.
        /// </summary>
        public const int InvalidOffTime = 260;

        /// <summary>
        /// Offtime value 
        /// </summary>
        public const uint OffTimeSec = 20;

        /// <summary>
        /// The time duration needed to start/stop Chassis Manager service
        /// </summary>
        public const int CmServiceStartStopSeconds = 20;

        /// <summary>
        /// Log count for after clear logs and readchassisLog
        /// </summary>
        public const int LogCount = 4;

        /// <summary>
        /// Number of log entries should be 50 
        /// </summary>
        public const int LogEntries = 50;
   
    }
}
