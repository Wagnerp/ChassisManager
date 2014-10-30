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
    using System;
    using Microsoft.GFS.WCS.ChassisManager.Ipmi;
    using Microsoft.GFS.WCS.Contracts;
    using System.Collections.Generic;
    using System.Threading;

    static class NvdimmSupport
    {
        # region Internal variables

        /// <summary>
        /// The number of blades in the chassis
        /// </summary>
        private static int numBlades = ConfigLoaded.Population;

        /// <summary>
        /// Indicates for each blade the action that needs to be performed once backup is complete
        /// </summary>
        private static DatasafeActions[] bladeDatasafeAction = new DatasafeActions[numBlades];

        // Per-blade lock to ensure atomic datasafe operation
        // Note that blade IDs start from 1, but this array using 0-based indexing for the blades
        private static readonly object[] bladeActionLock = new object[numBlades];

        // Wait handle to coordinate executing the specified delegate method
        private static ManualResetEvent pendingDatasafeEvent = new ManualResetEvent(false);
        private static RegisteredWaitHandle reg = ThreadPool.RegisterWaitForSingleObject(
            pendingDatasafeEvent, LoopAndProcessPendingDatasafeCommands, null, -1, false);

        #endregion

        /// <summary>
        /// Static constructor to initialize the blade action after backup data structure
        /// </summary>
        static NvdimmSupport()
        {
            for (int bladeId = 1; bladeId <= numBlades; bladeId++)
            {
                lock (bladeActionLock[bladeId - 1])
                {
                    SetDatasafeAction(bladeId, DatasafeActions.DoNothing);
                }
            }
        }

        # region Public methods

        #endregion

        # region Internal methods

        /// <summary>
        /// Processes the datasafe action.
        /// This method will be called by each of DataSafe REST commands in ChassisManager.cs to process the Datasafe commands
        /// </summary>
        /// <param name="bladeId">The blade identifier.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        internal static DatasafeBladeStatus ProcessDatasafeAction(int bladeId, DatasafeActions action)
        {
            DatasafeBladeStatus response = new DatasafeBladeStatus();

            lock (bladeActionLock[bladeId - 1])
            {
                if (GetDatasafeAction(bladeId) == DatasafeActions.DoNothing)
                {
                    // Process datasafe action
                    response = ProcessDatasafeActionHelper(bladeId, action);
                    if (response.status == DatasafeCommandsReturnStatus.CommandDelayed)
                    {
                        // Set datasafe action for this blade and trigger the delegate to process the pending datasafe command
                        SetDatasafeAction(bladeId, action);
                        pendingDatasafeEvent.Set();
                    }
                }
                else
                {
                    // There is already an existing action for this blade. Do not accept new commands. Return failure
                    response.status = DatasafeCommandsReturnStatus.CommandFailed;
                }

                return response;
            }
        }

        /// <summary>
        /// This method is invoked if battery status reaches a certain critical charge level.
        /// It triggers NVDIMM backup for all blades.
        /// </summary>
        /// <param name="stateInfo">The state information.</param>
        internal static void ProcessCriticalBatteryStatus(Object stateInfo)
        {
            // Trigger NvDimm backup for all blades
            for (int bladeId = 1; bladeId <= numBlades; bladeId++)
            {
                lock (bladeActionLock[bladeId - 1])
                {
                    // Check that blade is not already in BatteryLowCapacityPanic state, and
                    // we only need to process compute blades since JBODs do not have datasafe capabilities
                    if ((GetDatasafeAction(bladeId) != DatasafeActions.BatteryLowCapacityPanic) &&
                        (FunctionValidityChecker.CheckBladeTypeValidity((byte)bladeId)))
                    {
                        // Check to see if the blade is hard powered off
                        BladePowerStatePacket response = ChassisState.BladePower[bladeId - 1].GetBladePowerState();
                        if (response.CompletionCode != CompletionCode.Success)
                        {
                            // Log error here, and proceed to check blade state since we still want to check BMC soft power status
                            // even if blade enable read failed for whatever reason
                            Tracer.WriteError("ProcessCriticalBatteryStatus: Blade {0} Power Enable state read failed (Completion Code: {1:X})",
                                bladeId, response.CompletionCode);
                        }
                        else if (response.BladePowerState == (byte)Contracts.PowerState.OFF)
                        {
                            // If blade is hard powered off, no further processing is necessary
                            continue;
                        }

                        // Check blade soft power state
                        Ipmi.SystemStatus powerState = WcsBladeFacade.GetChassisState((byte)bladeId);

                        Tracer.WriteInfo("ProcessCriticalBatteryStatus: GetChassisState completion code: {0}, Blade State: {1}", 
                            powerState.CompletionCode, powerState.PowerState.ToString());

                        if (powerState.CompletionCode != 0)
                        {
                            // Log error here, and proceed to set BatteryLowCapacityPanic action since we cannot determine actual blade state
                            Tracer.WriteError("ProcessCriticalBatteryStatus: GetBladeState failed with completion code {0:X}", powerState.CompletionCode);
                        }
                        else if (powerState.PowerState != Ipmi.IpmiPowerState.On)
                        {
                            // If blade is not powered on, no further processing is necessary
                            continue;
                        }

                        // Set BatteryLowCapacityPanic action for blade. 
                        // This action overrides any existing action since it is a critical state.
                        SetDatasafeAction(bladeId, DatasafeActions.BatteryLowCapacityPanic);
                    }
                }
            }

            // Trigger delegate to process the BatteryLowCapacityPanic commands
            pendingDatasafeEvent.Set();
        }

        /// <summary>
        /// Gets the datasafe blade status.
        /// </summary>
        /// <param name="bladeId">The blade identifier.</param>
        /// <returns>Datasafe blade status.</returns>
        internal static DatasafeBladeStatus GetDatasafeBladeStatus(byte bladeId)
        {
            lock (bladeActionLock[bladeId - 1])
            {
                // Get datasafe blade status by issuing a DoNothing command
                return ProcessDatasafeActionHelper(bladeId, DatasafeActions.DoNothing);
            }
        }

        # endregion

        # region Private Methods

        /// <summary>
        /// Sets the datasafe action.
        /// </summary>
        /// <param name="bladeId">The blade identifier.</param>
        /// <param name="action">The datasafe action.</param>
        private static void SetDatasafeAction(int bladeId, DatasafeActions action)
        {
            // Set datasafe action for this blade
            bladeDatasafeAction[bladeId - 1] = action;
        }

        /// <summary>
        /// Gets the datasafe action.
        /// </summary>
        /// <param name="bladeId">The blade identifier.</param>
        /// <returns>The current datasafe action for the blade.</returns>
        private static DatasafeActions GetDatasafeAction(int bladeId)
        {
            // Returns datasafe action for this blade
            return bladeDatasafeAction[bladeId - 1];
        }

        /// <summary>
        /// Single point entry method for all datasafe command execution and ADR triggers via CM (including battery panic)
        /// Per-blade actions are executed atomically
        /// </summary>
        /// <param name="bladeId">target bladeId</param>
        /// <param name="command">Command that triggered ADR</param>
        /// <param name="command">Use BatteryLowCapacityPanic when battery discharging and critical capacity</param>
        /// <param name="command">Use DoNothing for default and when battery transitions from discharging to charging</param>
        /// Return DatasafeCommandsReturnStatus.CommandExecuted if command successfully executed 
        /// Return DatasafeCommandsReturnStatus.CommandDelayed if command successfully delayed
        /// Return DatasafeCommandsReturnStatus.CommandFailed if command execution failed
        private static DatasafeBladeStatus ProcessDatasafeActionHelper(int bladeId, DatasafeActions command)
        {
            DatasafeBladeStatus response = new DatasafeBladeStatus();
            response.status = DatasafeCommandsReturnStatus.CommandFailed;

            // Backup status variables
            NvdimmBackupStatus backupStatus = NvdimmBackupStatus.Unknown;
            int nvdimmPresentTimeRemaining;
            byte adrCompleteDelay;
            byte nvdimmPresentPowerOffDelay;

            // Get Backup status
            bool getBackupStatusOK = GetBackupStatus(bladeId, out backupStatus, out nvdimmPresentTimeRemaining,
                out adrCompleteDelay, out nvdimmPresentPowerOffDelay);
            if (getBackupStatusOK)
            {
                response.isBackupPending = backupStatus == NvdimmBackupStatus.Pending ? true : false;
                response.remainingBackupDuration = nvdimmPresentTimeRemaining;
            }

            // DoNothing command will be used to get status of datasafe backup operation 
            if (command == DatasafeActions.DoNothing)
            {
                response.status = getBackupStatusOK == true ?
                    DatasafeCommandsReturnStatus.CommandExecuted : DatasafeCommandsReturnStatus.CommandFailed;
                return response;
            }

            switch (command)
            {
                case DatasafeActions.PowerCycle:
                case DatasafeActions.PowerOff:
                case DatasafeActions.BladeOff:
                case DatasafeActions.BatteryLowCapacityPanic:

                    // Backup is NOT pending OR Not sure if backup is pending
                    if (backupStatus == NvdimmBackupStatus.NotPending || backupStatus == NvdimmBackupStatus.Unknown)
                    {
                        // Kill any active serial session - use the force kill option with invalid blade id since we do not know
                        // what blade currently has open serial session and the session token is also not known to us
                        Contracts.ChassisResponse killSessionResponse = BladeSerialSessionMetadata.StopBladeSerialSession(0, null, true);
                        if ((killSessionResponse.completionCode != Contracts.CompletionCode.NoActiveSerialSession) &&
                            (killSessionResponse.completionCode != Contracts.CompletionCode.Success))
                        {
                            Tracer.WriteError("Before SetNvDimmTrigger Datasafe Operation (BladeId-{0}): Cannot kill the serial session.", bladeId);
                            // It makes sense to proceed further as a best effort even if we cannot kill the serial session.. so continue..
                        }

                        // Send ADR trigger
                        if (WcsBladeFacade.SetNvDimmTrigger((byte)bladeId, Ipmi.NvDimmTriggerAction.PchAdrGpi, true,
                            adrCompleteDelay, nvdimmPresentPowerOffDelay))
                        {
                            response.status = DatasafeCommandsReturnStatus.CommandDelayed;
                        }
                        else
                        {
                            response.status = DatasafeCommandsReturnStatus.CommandFailed;
                        }
                    }
                    else if (backupStatus == NvdimmBackupStatus.Complete)
                    {
                        // We need to process all these above commands once the timer expires
                        if (ExecuteBladePowerCommand(bladeId, command))
                        {
                            response.status = DatasafeCommandsReturnStatus.CommandExecuted;
                        }
                        else
                        {
                            response.status = DatasafeCommandsReturnStatus.CommandFailed;
                        }
                    }
                    else if (backupStatus == NvdimmBackupStatus.Pending)
                    {
                        response.status = DatasafeCommandsReturnStatus.CommandDelayed;
                    }

                    break;

                case DatasafeActions.PowerOn:
                case DatasafeActions.BladeOn:

                    // Just execute command for PowerOn and BladeOn. 
                    if (ExecuteBladePowerCommand(bladeId, command))
                    {
                        response.status = DatasafeCommandsReturnStatus.CommandExecuted;
                    }
                    else
                    {
                        response.status = DatasafeCommandsReturnStatus.CommandFailed;
                    }
                    break;

                default:
                    break;
            }

            return response;
        }

        /// <summary>
        /// Gets the backup status.
        /// </summary>
        /// <param name="bladeId">The blade identifier.</param>
        /// <param name="backupStatus">The backup status.</param>
        /// <param name="nvdimmPresentTimeRemaining">The NVDIMM present time remaining.</param>
        /// <param name="adrCompleteDelay">The ADR complete delay.</param>
        /// <param name="nvdimmPresentPowerOffDelay">The NVDIMM present power off delay.</param>
        /// <returns>true: Backup status obtained successfully from blade.
        ///          false: Failed to obtain backup status from blade</returns>
        private static bool GetBackupStatus(int bladeId, out NvdimmBackupStatus backupStatus,
            out int nvdimmPresentTimeRemaining, out byte adrCompleteDelay, out byte nvdimmPresentPowerOffDelay)
        {
            // Kill any active serial session - use the force kill option with invalid blade id since we do not know
            // what blade currently has open serial session and the session token is also not known to us
            Contracts.ChassisResponse killSessionResponse = BladeSerialSessionMetadata.StopBladeSerialSession(0, null, true);
            if ((killSessionResponse.completionCode != Contracts.CompletionCode.NoActiveSerialSession) &&
                (killSessionResponse.completionCode != Contracts.CompletionCode.Success))
            {
                Tracer.WriteError("Before GetNvDimmTrigger Datasafe Operation (BladeId-{0}): Cannot kill the serial session.", bladeId);
                // It makes sense to proceed further as a best effort even if we cannot kill the serial session.. so continue..
            }

            // Get ADR trigger status. SendReceive() in SerialPortManager will disable safe mode
            // for NVDIMM commands so this command will not be blocked by any existing serial sessions
            NvDimmTrigger getTrigger = WcsBladeFacade.GetNvDimmTrigger((byte)bladeId);

            // Command executed successfully
            if (getTrigger.CompletionCode == 0x00)
            {
                // Determine backup status
                if (getTrigger.AdrComplete == 0x01)
                {
                    if (getTrigger.NvdimmPresentTimeRemaining == 0)
                    {
                        backupStatus = NvdimmBackupStatus.Complete;
                    }
                    else
                    {
                        backupStatus = NvdimmBackupStatus.Pending;
                    }
                }
                else
                {
                    backupStatus = NvdimmBackupStatus.NotPending;
                }
                nvdimmPresentTimeRemaining = getTrigger.NvdimmPresentTimeRemaining;
                adrCompleteDelay = (byte)getTrigger.AdrCompleteDelay;
                nvdimmPresentPowerOffDelay = (byte)getTrigger.NvdimmPresentPowerOffDelay;

                return true;
            }
            else
            {
                backupStatus = NvdimmBackupStatus.Unknown;
                nvdimmPresentTimeRemaining = ConfigLoaded.NvDimmPresentPowerOffDelay;
                adrCompleteDelay = (byte)ConfigLoaded.AdrCompleteDelay;
                nvdimmPresentPowerOffDelay = (byte)ConfigLoaded.NvDimmPresentPowerOffDelay;

                return false;
            }
        }

        /// <summary>
        /// Executes the blade power command for power control
        /// </summary>
        /// <param name="bladeId">The blade identifier.</param>
        /// <param name="command">AdrTrigger command</param>
        /// <returns></returns>
        private static bool ExecuteBladePowerCommand(int bladeId, DatasafeActions command)
        {
            switch (command)
            {
                case DatasafeActions.BatteryLowCapacityPanic:
                    return BladePowerCommands.PowerOff(bladeId);
                case DatasafeActions.PowerOff:
                    return BladePowerCommands.PowerOff(bladeId);
                case DatasafeActions.PowerOn:
                    return BladePowerCommands.PowerOn(bladeId);
                case DatasafeActions.PowerCycle:
                    return BladePowerCommands.PowerCycle(bladeId, 0);
                case DatasafeActions.BladeOff:
                    return BladePowerCommands.BladeOff(bladeId);
                case DatasafeActions.BladeOn:
                    return BladePowerCommands.BladeOn(bladeId);
                default:
                    break;
            }
            return false;   
        }

        /// <summary>
        /// This method loops to process pending DataSafe commands until
        /// all pending commands are processed.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="timedOut"></param>
        private static void LoopAndProcessPendingDatasafeCommands(Object obj, bool timedOut)
        {
            bool moreAction = true;
            
            // Loop and process pending datasafe commands while there are remaining actions (datasafe commands)
            while (moreAction)
            {
                moreAction = false;
                for (int bladeId = 1; bladeId <= numBlades; bladeId++)
                {
                    lock (bladeActionLock[bladeId - 1])
                    {
                        DatasafeActions currentBladeAction = GetDatasafeAction(bladeId);
                        if (currentBladeAction != DatasafeActions.DoNothing)
                        {
                            // Execute datasafe action for the blade
                            DatasafeBladeStatus response = ProcessDatasafeActionHelper(bladeId, currentBladeAction);

                            if (response.status == DatasafeCommandsReturnStatus.CommandDelayed)
                            {
                                // Indicate that there are more actions to process
                                moreAction = true;
                            }
                            else if (response.status == DatasafeCommandsReturnStatus.CommandExecuted)
                            {
                                // Command executed successfully. Reset action for the blade
                                SetDatasafeAction(bladeId, DatasafeActions.DoNothing);
                                Tracer.WriteInfo("LoopAndProcessPendingDatasafeCommands: Action {0} executed for bladeId {1}",
                                    currentBladeAction, bladeId);
                            }
                            else if (response.status == DatasafeCommandsReturnStatus.CommandFailed)
                            {
                                // If command failed, something has gone wrong. Clear action for the blade and log error
                                SetDatasafeAction(bladeId, DatasafeActions.DoNothing);
                                Tracer.WriteError("LoopAndProcessPendingDatasafeCommands: Action {0} failed for bladeId {1}", 
                                    currentBladeAction, bladeId);
                            }
                        }
                    }
                }

                Thread.Sleep(1000);
            }
        }
        
        # endregion
    }

    /// <summary>
    /// Datasafe commands return status
    /// </summary>
    internal enum DatasafeCommandsReturnStatus : byte
    {
        CommandExecuted = 0x0,
        CommandDelayed = 0x1,
        CommandFailed = 0x2,
    }

    /// <summary>
    /// Datasafe command processing status
    /// </summary>
    internal class DatasafeBladeStatus
    {
        internal DatasafeCommandsReturnStatus status = DatasafeCommandsReturnStatus.CommandFailed;
        internal bool isBackupPending = false;
        internal int remainingBackupDuration = 0;
    }

    /// <summary>
    /// Datasafe Actions
    /// </summary>
    internal enum DatasafeActions
    {
        /// <summary>
        /// Default do-nothing command 
        /// </summary>
        DoNothing = 0,

        /// <summary>
        /// Trigger ADR due to low battery capacity
        /// </summary>
        BatteryLowCapacityPanic = 1,

        /// <summary>
        /// Hard power OFF (Blade Enable)
        /// </summary>
        PowerOff = 2,

        /// <summary>
        /// Hard power On (Blade Enable)
        /// </summary>
        PowerOn = 3,

        /// <summary>
        /// IPMI Power cycle
        /// </summary>
        PowerCycle = 4,

        /// <summary>
        /// IPMI Soft Power Off
        /// </summary>
        BladeOff = 5,

        /// <summary>
        /// IPMI Soft Power On
        /// </summary>
        BladeOn = 6,
    }

    /// <summary>
    /// NvDimm backup status
    /// </summary>
    internal enum NvdimmBackupStatus
    {
        /// <summary>
        /// Uknown if backup is pending
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// Backup not pending
        /// </summary>
        NotPending = 0,

        /// <summary>
        /// Backup pending
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Backup complete
        /// </summary>
        Complete = 2
    }

}
