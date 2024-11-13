using log4net;
using rEFInd_Automenu.Win32;
using Rikitav.IO.ExtensibleFirmware.SystemPartition;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace rEFInd_Automenu.Booting
{
    public class MountVolBribge : Win32ApplicationBridge
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MountVolBribge));
        private static readonly Version _Win7_CoreVersion = new Version(6, 1);
        private static bool _UnmountingOnExitCreated = false;

        public static DriveInfo? EfiVolumeMountPoint
        {
            get;
            private set;
        }

        public static bool DeleteMountPointAfterExit
        {
            get;
            set;
        }

        public MountVolBribge()
            : base("mountvol.exe") { }

        public DriveInfo? MountEsp()
        {
            log.Info("Trying to mount ESP as logical drive");

            // Checking if ESP mountpoint is already found
            if (KnowMountPoint())
            {
                log.InfoFormat("ESP mount point is already known - {0}", EfiVolumeMountPoint);
                return EfiVolumeMountPoint;
            }

            // Checking if ESP is already mounted
            if (TryFindEspMountPoint(out DriveInfo? findMountPoint))
            {
                EfiVolumeMountPoint = findMountPoint;
                DeleteMountPointAfterExit = false;

                log.InfoFormat("ESP mount point is already mounted - {0}", findMountPoint);
                return findMountPoint;
            }

            // Getting free mount point letter and mounting
            string mountPointLetter = MVB_Helper.GetFreeMountvolLetter();
            DriveInfo mountPoint = new DriveInfo(mountPointLetter);
            log.InfoFormat("Mounting ESP to {0}", mountPoint);

            if (!ExecuteMount(mountPoint) && !KnowMountPoint())
            {
                log.ErrorFormat("Failed to create Efi volume mount point");
                return null;
            }

            // Info set
            EfiVolumeMountPoint = mountPoint;
            DeleteMountPointAfterExit = true;

            // Deleting Efi volume mount point on process exit
            if (!_UnmountingOnExitCreated)
            {
                _UnmountingOnExitCreated = true;
                AppDomain.CurrentDomain.ProcessExit += (_, _) =>
                {
                    log.InfoFormat("Deleting Efi volume mount point (Process exiting)");
                    UnmountEsp();
                };
            }

            // Success
            log.Info("ESP successfully mounted");
            return mountPoint;
        }

        public void FindUnmountEsp()
        {
            // Trying to find and delete Efi volume mount point
            log.InfoFormat("Deleting ESP mount point from {0}", EfiVolumeMountPoint?.Name ?? "<NULL>");

            // Trying to find Efi volume mount point
            if (!TryFindEspMountPoint(out DriveInfo? mountPoint))
            {
                log.Info("ESP is not mounted");
                return;
            }

            // Trying to unmount ESP
            if (!ExecuteUnmount(mountPoint) && KnowMountPoint())
            {
                log.ErrorFormat("Failed to delete ESP mount point");
                return;
            }

            // Info set
            log.Info("ESP unmounted successfully");
            EfiVolumeMountPoint = null;
            DeleteMountPointAfterExit = false;
        }

        public void UnmountEsp()
        {
            // Trying to unmount ESP
            log.InfoFormat("Deleting ESP mount point from {0}", EfiVolumeMountPoint?.Name ?? "<NULL>");

            // No mounted point
            if (EfiVolumeMountPoint == null)
            {
                log.Warn("ESP was not mounted previously");
                return;
            }

            // Checking if dont need to unmount
            if (!DeleteMountPointAfterExit)
            {
                log.Info("Not nned to unmount");
                return;
            }

            // Deleting Efi volume mount point
            if (!ExecuteUnmount(EfiVolumeMountPoint) && KnowMountPoint())
            {
                log.ErrorFormat("Failed to delete ESP mount point");
                return;
            }

            // Success
            log.Info("ESP unmounted successfully");
            EfiVolumeMountPoint = null;
            DeleteMountPointAfterExit = false;
        }

        public static bool TryFindEspMountPoint([NotNullWhen(true)] out DriveInfo? mountPoint)
        {
            log.Info("Searching if ESP is already have mount point");
            string volumePath = EfiPartition.GetFullPath(); // ESP Guid based path looks like this --> \\?\Volume{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}\

            // Checking if ESP mountpoint is already found
            if (KnowMountPoint())
            {
                log.InfoFormat("ESP mount point is already known - {0}", EfiVolumeMountPoint);
                mountPoint = EfiVolumeMountPoint;
                return true;
            }

            // Checking system version
            Version CurrentWindowsVersion = Environment.OSVersion.Version;
            if (CurrentWindowsVersion <= _Win7_CoreVersion)
            {
                // 'GetVolumeNameForVolumeMountPointW' function works different with ESP volume on Windows 7, and give's ERROR_INVALID_PARAMETER
                log.WarnFormat("WinNT version is below 10 ({0}), problems may occur during search", CurrentWindowsVersion);
            }

            // Enumerating logical volumes
            StringBuilder pathBuffer = new StringBuilder(260);
            foreach (DriveInfo logicalDrive in DriveInfo.GetDrives())
            {
                if (!NativeMethods.GetVolumeNameForVolumeMountPointW(logicalDrive.Name, pathBuffer, pathBuffer.Capacity))
                {
                    // Error check!
                    int lastErr = Marshal.GetLastWin32Error();

                    // Checking for error code and NT Core version
                    if (lastErr == NativeMethods.ERROR_INVALID_PARAMETER && CurrentWindowsVersion <= _Win7_CoreVersion)
                    {
                        // Found trough error code check on Win7 system
                        log.InfoFormat("ESP volume mount point finded - {0}", logicalDrive);
                        mountPoint = logicalDrive;
                        EfiVolumeMountPoint = logicalDrive;
                        return true;
                    }

                    // WinVer is over 7
                    log.ErrorFormat("Failed to get volume path name for mount point {0}. Win32Error - {1}", logicalDrive, lastErr);
                    continue;
                }

                // Getting volume path name for comparing with ESP volume path name
                string volumePathName = pathBuffer.ToString();
                if (string.IsNullOrEmpty(volumePathName))
                {
                    log.ErrorFormat("Failed to get volume path name for mount point {0}. Function returned empty buffer", logicalDrive);
                    continue;
                }

                // Comparing 
                if (!volumePath.Equals(volumePathName, StringComparison.CurrentCultureIgnoreCase))
                {
                    // Volume path names are not equal => not ESP
                    continue;
                }

                // Founded
                mountPoint = logicalDrive;
                EfiVolumeMountPoint = logicalDrive;
                log.InfoFormat("ESP volume mount point finded - {0}", mountPoint);
                return true;
            }

            log.ErrorFormat("ESP volume mount point wasn't finded");
            mountPoint = null;
            EfiVolumeMountPoint = null;
            return false;
        }

        [MemberNotNullWhen(true, nameof(EfiVolumeMountPoint))]
        private static bool KnowMountPoint()
        {
            if (EfiVolumeMountPoint != null && EfiVolumeMountPoint.IsReady)
                return true;

            EfiVolumeMountPoint = null;
            return false;
        }

        private bool ExecuteUnmount(DriveInfo mountPoint)
        {
            string args = string.Join(" ", mountPoint.Name, "/d");
            return string.IsNullOrEmpty(Execute(args, true));
        }

        private bool ExecuteMount(DriveInfo mountPoint)
        {
            string args = string.Join(" ", mountPoint.Name, "/s");
            return string.IsNullOrEmpty(Execute(args, true));
        }

        private static class MVB_Helper
        {
            public static string GetFreeMountvolLetter()
            {
                // Preset letters
                string[] PresetLetters = new string[] { "S:\\", "E:\\", "X:\\" }; // Should i comment this? (ᵕ•_•)
                string? SelectPresetLetter = PresetLetters.FirstOrDefault(x => !Directory.Exists(x));

                // Checking for preset letters freedom
                if (!string.IsNullOrWhiteSpace(SelectPresetLetter))
                    return SelectPresetLetter;

                // Trying to find so
                return Enumerable
                    .Range('D', 'Z' - 'D' + 1)
                    .Select(x => (char)x + ":\\")
                    .First(x => !Directory.Exists(x));
            }
        }

        private static class NativeMethods
        {
            public const int ERROR_INVALID_PARAMETER = 0x57;

            [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool GetVolumeNameForVolumeMountPointW(
                string lpszVolumeMountPoint,
                [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpszVolumeName,
                int cchBufferLength);
        }
    }
}
