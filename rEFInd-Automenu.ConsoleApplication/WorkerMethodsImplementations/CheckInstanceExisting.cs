using rEFInd_Automenu.Extensions;
using Rikitav.IO.ExtensibleFirmware;
using Rikitav.IO.ExtensibleFirmware.SystemPartition;
using System.IO;

namespace rEFInd_Automenu.ConsoleApplication.WorkerMethodsImplementations
{
    public partial class WorkerMethods
    {
        public DirectoryInfo CheckInstanceExisting() => ConsoleProgram.Interface.ExecuteAndReturn<DirectoryInfo>("Checking Instance", commands, (ctrl) =>
        {
            // Getting UEFI availablity
            log.Info("Checking firware interface availablity");
            if (!FirmwareInterface.Available)
            {
                log.Warn("An attempt was made to execute commands related to working with an installed instance of rEFInd on a system that does not support UEFI");
                ctrl.Error("Firmware interface is not available on current system");
            }

            // Getting ESP path
            log.Info("Getting Efi system partition GUID");
            DirectoryInfo ESP = EfiPartition.GetDirectoryInfo();
            log.InfoFormat("Efi system partition GUID : {0}", ESP.FullName);

            // Trying to get File system access
            try
            {
                log.Info("Getting ESP directory read\\write access");
                ESP.GrantAccessControl();
                log.Info("Access to ESP granted successfully");
            }
            catch (Exception Exc)
            {
                log.Fatal("Failed to grant access to ESP", Exc);
                return ctrl.Error("Failed to grant access to ESP", true);
            }

            // Searching for "refind" directory on ESP
            DirectoryInfo EspRefindDir = ESP.GetSubDirectory("EFI\\refind");
            if (!EspRefindDir.Exists && !EspRefindDir.EnumerateFiles("refind_*.efi").Any())
            {
                // Should existing
                log.Fatal("rEFInd is not installed on this computer");
                return ctrl.Error("rEFInd is not installed on this computer", true);
            }

            return ctrl.Success("Installed", EspRefindDir);
        });

        public DirectoryInfo CheckInstanceExisting(bool Force = false) => ConsoleProgram.Interface.ExecuteAndReturn<DirectoryInfo>("Checking Instance", commands, (ctrl) =>
        {
            // Getting UEFI availablity
            log.Info("Checking firware interface availablity");
            if (!FirmwareInterface.Available)
            {
                log.Warn("An attempt was made to execute commands related to working with an installed instance of rEFInd on a system that does not support UEFI");
                ctrl.Error("Firmware interface is not available on current system");
            }

            // Getting ESP path
            log.Info("Getting Efi system partition GUID");
            DirectoryInfo ESP = EfiPartition.GetDirectoryInfo();
            log.InfoFormat("Efi system partition GUID : {0}", ESP.FullName);

            // Trying to get File system access
            try
            {
                log.Info("Getting ESP directory read\\write access");
                ESP.GrantAccessControl();
                log.Info("Access to ESP granted successfully");
            }
            catch (Exception Exc)
            {
                log.Fatal("Failed to grant access to ESP", Exc);
                return ctrl.Error("Failed to grant access to ESP", true);
            }

            // Searching for "refind" directory on ESP
            DirectoryInfo EspRefindDir = ESP.GetSubDirectory("EFI\\refind");
            if (EspRefindDir.Exists)
            {
                // Shouldnt existing
                if (Force) // <-- indicates that we should try to delete instance
                {
                    log.Warn("rEFInd is already installed on this computer");
                    try
                    {
                        // Trying to delete instance
                        log.Info("Force work is enabled, trying to remove instance");
                        EspRefindDir.Delete(true);
                        log.Info("rEFInd instance is removed");
                    }
                    catch (Exception exc)
                    {
                        // Failed to delete
                        log.Fatal("Failed to remove existing rEFInd instance from computer", exc);
                        return ctrl.Error("Failed to remove existing rEFInd instance from computer", true);
                    }

                    return ctrl.Success("Force cleaned", EspRefindDir);
                }
                else
                {
                    // Cannot continue
                    log.Fatal("rEFInd is already installed on this computer");
                    return ctrl.Error("rEFInd is already installed on this computer", true);
                }
            }

            return ctrl.Success(string.Empty, EspRefindDir);
        });
    }
}
