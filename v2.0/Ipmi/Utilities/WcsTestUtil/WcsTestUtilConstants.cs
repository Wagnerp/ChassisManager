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

namespace WcsTestUtil
{

    using Microsoft.GFS.WCS.ChassisManager.Ipmi;

    // Class for all constants 
    internal static class WcsTestUtilDefaults
    {
     
        #region IPMI Command Default Response Values

        // Get Power Restore Policy
        public const PowerRestoreOption powerRestorePolicy = PowerRestoreOption.AlwaysPowerUp;

        // Get PSU Alert
        public const bool autoProchotEnabled = false;

        // Get Default Power Limit
        public const ushort defaultPowerCap = 250;
        public const ushort defaultPowerCapDelay = 20;
        public const bool dpcEnabled = false;

        // Get NVDIMM Trigger
        public const NvDimmTriggerAction nvdimmTriggerAction = NvDimmTriggerAction.Disabled;
        public const byte adrCompleteDelay = 5;
        public const byte nvdimmPresentDelay = 160;

        // Get Thermal Control
        public const byte ThermalControlFeatureStatus = 0;
        
        #endregion
    }
}


