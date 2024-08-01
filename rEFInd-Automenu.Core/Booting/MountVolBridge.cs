using log4net;
using Rikitav.IO.ExtensibleFirmware.SystemPartition;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace rEFInd_Automenu.Booting
{
    public static class MountVolBribge
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MountVolBribge));
        private static string? _EspMountedPoint = null;
        private static bool _NeedToUnmount = false;

        public static bool ExecuteMountvol(string Params)
        {
            using Process MountVolProc = new Process()
            {
                StartInfo = new ProcessStartInfo("mountvol.exe")
                {
                    Arguments = Params,
                    CreateNoWindow = true,
                    WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System)
                }
            };

            log.InfoFormat("Mountvol is executing with parameters - {0}", Params);
            MountVolProc.Start();
            MountVolProc.WaitForExit();
            return MountVolProc.ExitCode == 0;
        }

        public static string GetFreeMountvolLetter()
        {
            // Preset letters
            string[] PresetLetters = new string[] { "S:\\", "E:\\", "X:\\" }; // this is not an accident...
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

        public static string? MountEsp()
        {
            log.Info("Trying to mount ESP as logical drive...");

            // Checking if ESP mountpoint is already found
            if (_EspMountedPoint != null)
            {
                log.InfoFormat("ESP mount point is already known - {0}", _EspMountedPoint);
                return _EspMountedPoint;
            }

            // Checking if ESP is already mounted
            string? tmpMountPoint = IsEspMounted();
            if (!string.IsNullOrEmpty(tmpMountPoint))
            {
                log.InfoFormat("ESP mount point is already mounted - {0}", tmpMountPoint);
                _EspMountedPoint = tmpMountPoint;
                return _EspMountedPoint;
            }

            // Getting free mount point letter and mounting
            tmpMountPoint = GetFreeMountvolLetter();
            log.InfoFormat("Mounting ESP to {0}", tmpMountPoint);
            if (!ExecuteMountvol(tmpMountPoint + " /s"))
            {
                log.Error("Failed to mount ESP (Mountvol error)");
                return null;
            }

            // Info set
            _EspMountedPoint = tmpMountPoint;
            _NeedToUnmount = true;

            // success
            log.Info("ESP successfully mounted");
            return _EspMountedPoint;
        }

        public static void FindUnmountEsp()
        {
            throw new NotImplementedException();
        }

        public static void UnmountEsp()
        {
            // Trying to unmount ESP
            log.InfoFormat("Trying to unmount ESP from {0} mount point", _EspMountedPoint);

            // No mounted point
            if (_EspMountedPoint == null)
            {
                log.Warn("No saved mount point");
                return;
            }

            if (!_NeedToUnmount)
            {
                log.Warn("Not nned to unmount");
                return;
            }

            if (!ExecuteMountvol(_EspMountedPoint + " /d"))
            {
                log.WarnFormat("Failed to unmount ESP", _EspMountedPoint);
                //return;
            }
            else
            {
                log.Info("ESP unmounted successfully");
            }

            _EspMountedPoint = null;
            _NeedToUnmount = false;
        }

        public static string? IsEspMounted()
        {
            log.Info("Searching for mounted ESP...");
            string volumePath = EfiPartition.GetFullPath(); // ESP --> \\?\Volume{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}\

            // Getting windows NT core version
            int NtVer = Environment.OSVersion.Version.Major;
            if (NtVer <= 6) // If version is below or equals 6, 'GetVolumeNameForVolumeMountPointW' will work different with ESP volume
            {
                log.WarnFormat("WinNT version is below 10 ({0}), problems may occur during search", NtVer);
            }

            // Enumerating logical volumes
            foreach (string drive in Environment.GetLogicalDrives())
            {
                // Getting GUID-based volume path for logical drive
                StringBuilder buffer = new StringBuilder(260);
                if (!NativeMethods.GetVolumeNameForVolumeMountPointW(drive, buffer, buffer.Capacity))
                {
                    // ERROR!
                    int lastErr = Marshal.GetLastWin32Error();

                    // If NT core version is over 6, then it fatal
                    if (NtVer > 6)
                    {
                        log.ErrorFormat("Failed to get Volume information. Win32Error - {0}", lastErr);
                        //throw new Win32Exception(lastErr);
                        return null;
                    }

                    // 87 code if fine if NT core version equal 6 on ESP volume
                    if (lastErr == 87)
                    {
                        string EfiDirPath = Path.Combine(drive, "EFI", "microsoft");
                        if (!Directory.Exists(EfiDirPath))
                        {
                            log.WarnFormat("Failed to get volume guid path name. Win32Error - {0}", lastErr);
                            continue;
                        }

                        log.InfoFormat("ESP volume mount point finded - {0} (NT6-Err87)", drive);
                        return drive;
                    }
                    else // and this is not fine
                    {
                        log.WarnFormat("Failed to get volume guid path name. Win32Error - {0}", lastErr);
                        continue;
                    }
                }

                // Checking 
                if (volumePath.Equals(buffer.ToString(), StringComparison.CurrentCultureIgnoreCase))
                {
                    // Founded
                    log.InfoFormat("ESP volume mount point finded - {0}", drive);
                    return drive;
                }
                else
                {
                    // Guid path name not equal
                    //log.Info("");
                    continue;
                }
            }

            // Not found
            log.Error("Enumeration end, ESP was not founded");
            return null;
        }

        public class MountVolException : Exception
        {
            public MountVolException(string message)
                : base(message) { }

            public MountVolException(Exception inner)
                : base("Failed to execute mountvol", inner) { }

            public MountVolException(string message, Exception inner)
                : base(message, inner) { }
        }

        private static class NativeMethods
        {
            [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool GetVolumeNameForVolumeMountPointW(
                string lpszVolumeMountPoint,
                [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpszVolumeName,
                int cchBufferLength);
        }
    }
}
