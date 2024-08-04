using log4net;
using rEFInd_Automenu.Extensions;
using rEFInd_Automenu.RegistryExplorer;
using Rikitav.IO.ExtensibleFirmware;
using Rikitav.IO.ExtensibleFirmware.SystemPartition;
using System;
using System.IO;
using System.Security.Principal;

#pragma warning disable CA1416
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

        public static bool UseMountvol
        {
            get => Environment.OSVersion.Version.Major <= 6;
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
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    log.Warn("An attempt was made to execute UEFI firmware-related commands in a non-administrator process");
                    throw new UnauthorizedAccessException("The program was launched without administrator rights.");
                }
            }

            // Getting ESP path
            DirectoryInfo? ESP;

            // Desiding which method program should access ESP volume
            if (Environment.OSVersion.Version.Major <= 6 || ProgramRegistry.PreferMountvolEspSearch)
            {
                // Mounting ESP as logical drive with mount point
                log.Info("Mounting ESP to check instance");
                string? EspMountPoint = MountVolBribge.MountEsp();

                // Checking MP value
                if (string.IsNullOrEmpty(EspMountPoint))
                {
                    // Failed to get MP
                    log.Error("Failed to get ESP mount point, value was null");
                    throw new MountVolBribge.MountVolException("Failed to get ESP mount point");
                }

                //
                log.InfoFormat("Efi system partition mount point : {0}", EspMountPoint);
                ESP = new DirectoryInfo(EspMountPoint);
            }
            else
            {
                // Finding ESP's GUID-based volume path
                log.Info("Getting Efi system partition GUID to check instance");
                ESP = EfiPartition.GetDirectoryInfo();
                log.InfoFormat("Efi system partition GUID : {0}", ESP.FullName);
            }

            if (ESP == null)
            {
                // Somehow null?
                log.Error("ESP directory information instance was null");
                throw new ArgumentNullException("ESP directory information instance was null");
            }

            // Trying to get File system access
            try
            {
                log.Info("Getting ESP directory read\\write access");
                ESP.GrantAccessControl();
                log.Info("Access to ESP granted successfully");
                return ESP;
            }
            catch (Exception Exc)
            {
                log.Fatal("Failed to grant access to ESP", Exc);
                throw new UnauthorizedAccessException("Failed to grant access to ESP");
            }
        }
    }
}
