using log4net;
using rEFInd_Automenu.RuntimeConfiguration;
using rEFInd_Automenu.TypesExtensions;
using rEFInd_Automenu.Win32;
using Rikitav.IO.ExtensibleFirmware;
using Rikitav.IO.ExtensibleFirmware.SystemPartition;
using System;
using System.IO;

namespace rEFInd_Automenu.Booting
{
    public static class EspFinder
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EspFinder));
        private static DirectoryInfo? _EspDirectory;

        public static DirectoryInfo EspDirectory
        {
            get => _EspDirectory ??= InitDirInfo();
        }

        private static DirectoryInfo InitDirInfo()
        {
            // Getting UEFI availablity
            log.Info("Checking firware interface availablity before refind instance checking");
            if (!FirmwareInterface.Available)
            {
                log.Warn("An attempt was made to execute commands related to working with an installed instance of rEFInd on a system that does not support UEFI");
                throw new PlatformNotSupportedException("Firmware interface is not available on current system");
            }

            // Checking administrator rights
            if (!Win32Utilities.ProcessHasAdminRigths())
            {
                log.Warn("An attempt was made to execute UEFI firmware-related commands in a non-administrator process");
                throw new UnauthorizedAccessException("The program was launched without administrator rights.");
            }

            // Desiding which method program should access ESP volume
            DirectoryInfo ESP = GetEspDirectoryInfo();

            // Trying to get ESP's filesystem access
            try
            {
                // Trying to get File system access
                log.Info("Getting ESP directory read\\write access");
                ESP.GrantAccessControl();

                // Granted successfully
                log.Info("Access to ESP granted successfully");
                return ESP;
            }
            catch (Exception Exc)
            {
                // Failed to grant fs access
                log.Fatal("Failed to grant access to ESP", Exc);
                throw new UnauthorizedAccessException("Failed to grant access to ESP");
            }
        }

        private static DirectoryInfo GetEspDirectoryInfo()
        {
            // Getting if ESP is already mounted as logical drive
            if (MountVolBribge.TryFindEspMountPoint(out DriveInfo? mountPoint))
            {
                // Returing 
                log.InfoFormat("Efi system partition mount point : {0}", mountPoint.Name);
                return mountPoint.RootDirectory;
            }
            else
            {
                if (Environment.OSVersion.Version <= new Version(6, 1) | ProgramConfiguration.Instance.PreferMountvolEspSearch)
                {
                    // Failed to get mount point
                    log.Error("Failed to get ESP mount point, value was null");
                    throw new DriveNotFoundException("Failed to get ESP mount point");
                }

                // Finding ESP's GUID-based volume path
                log.Info("Getting Efi system partition Guid path");
                string EfiPathName = EfiPartition.GetFullPath();

                // ESP Finded
                log.InfoFormat("Efi system partition GUID : {0}", EfiPathName);
                return new DirectoryInfo(EfiPathName);
            }
        }
    }
}
