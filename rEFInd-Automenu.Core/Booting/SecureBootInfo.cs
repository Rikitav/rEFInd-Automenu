using log4net;
using rEFInd_Automenu.Booting;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

#pragma warning disable CS8629
namespace rEFInd_Automenu.Booting
{
    public static class SecureBootInfo
    {
        /// <summary>
        /// Does the system support SecureBoot technology
        /// </summary>
        public static bool IsCapable
        {
            get
            {
                if (_SecureBootInformation == null)
                    InitSecureBootInfo();

                return _SecureBootInformation.Value.SecureBootCapable;
            }
        }

        /// <summary>
        /// Is SecureBoot technology enabled
        /// </summary>
        public static bool IsEnabled
        {
            get
            {
                if (_SecureBootInformation == null)
                    InitSecureBootInfo();

                return _SecureBootInformation.Value.SecureBootCapable;
            }
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(SecureBootInfo));
        private static NativeMethods.SYSTEM_SECUREBOOT_INFORMATION? _SecureBootInformation;

        /// <summary>
        /// receives SecureBoot technology status
        /// </summary>
        /// <exception cref="Win32Exception"></exception>
        public static void InitSecureBootInfo()
        {
            if (_SecureBootInformation != null)
                return;

            log.Info("Getting SecureBoot information");
            _SecureBootInformation = null;

            // trying to obtain that data
            IntPtr StructPtr = IntPtr.Zero;
            try
            {
                // Initilazing structure pointer
                int StructSize = Marshal.SizeOf<NativeMethods.SYSTEM_SECUREBOOT_INFORMATION>();
                StructPtr = Marshal.AllocHGlobal(StructSize);

                // Executing NtQuery
                NativeMethods.NtQuerySystemInformation(0x91, StructPtr, (uint)StructSize, out _);
                int lastError = Marshal.GetLastWin32Error();
                if (lastError != 0)
                {
                    // Last error is not happy
                    log.ErrorFormat("Failed to obtain SecureBoot information because of Win32 error - {0}", lastError);
                    throw new Win32Exception(lastError, "Failed to obtain SecureBoot information");
                }

                // Data unpacking
                _SecureBootInformation = (NativeMethods.SYSTEM_SECUREBOOT_INFORMATION?)Marshal.PtrToStructure(
                    StructPtr,
                    typeof(NativeMethods.SYSTEM_SECUREBOOT_INFORMATION));

                // Alright
                log.InfoFormat("{0} was obtained successfully", nameof(NativeMethods.SYSTEM_SECUREBOOT_INFORMATION));
                log.InfoFormat("SecureBoot IsCapable - {0}, IsEnabled - {1}", _SecureBootInformation.Value.SecureBootCapable, _SecureBootInformation.Value.SecureBootEnabled);
            }
            catch (Exception exc) when (exc is not Win32Exception)
            {
                // Some of data manipulation error
                log.Error("Failed to obtain SecureBoot information because of exception", exc);
                _SecureBootInformation = null;
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            finally
            {
                // Resources freeing
                Marshal.FreeHGlobal(StructPtr);
            }
        }

        private static class NativeMethods
        {
            [DllImport("ntdll.dll", PreserveSig = false)]
            public static extern void NtQuerySystemInformation(uint SystemInformationClass, IntPtr SystemInformation, uint SystemInformationLength, out uint ReturnLength);

            [StructLayout(LayoutKind.Sequential)]
            public struct SYSTEM_SECUREBOOT_INFORMATION
            {
                [MarshalAs(UnmanagedType.U1)] public bool SecureBootEnabled;
                [MarshalAs(UnmanagedType.U1)] public bool SecureBootCapable;
            }
        }
    }
}
