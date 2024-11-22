using log4net;
using log4net.Repository.Hierarchy;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

#pragma warning disable CA1416
namespace rEFInd_Automenu.Win32
{
    public static class Win32Utilities
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Win32Utilities));
        private const string _CheckUriString = @"https://sourceforge.net/projects/refind";

        public static bool ProcessHasAdminRigths()
        {
            try
            {
                log.Info("Checking if process has admin rights");
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
            catch (Exception exc)
            {
                log.Error("Failed to get WindowsIdentity while checking process rights", exc);
                return false;
            }
        }

        public static bool IsInternetConnectionAvailable()
        {
            // Checking for any internet devices is active
            if (!NativeMethods.InternetGetConnectedState(out NativeMethods.InternetConnectionState state, 0))
            {
                log.ErrorFormat("No internet devices online, state : {0}", state);
                return false;
            }

            // Checking for server availablity
            if (!NativeMethods.InternetCheckConnectionA(_CheckUriString, 0x00000001, 0))
            {
                int lastError = Marshal.GetLastWin32Error();
                if (lastError == NativeMethods.ERROR_NOT_CONNECTED)
                    log.Error("No internet connection");
                else
                    log.Error("Server unavailable");

                return false;
            }

            return true;
        }

        public static bool CheckProcessDuplication()
        {
            Process MyProcess = Process.GetCurrentProcess();
            foreach (Process OtherProcess in Process.GetProcessesByName(MyProcess.ProcessName))
            {
                if (OtherProcess.Id != MyProcess.Id)
                {
                    log.FatalFormat("Another instance of rEFInd Automenu is running (PID : {0})", OtherProcess.Id);
                    //Environment.Exit(0x235); // ERROR_TOO_MANY_THREADS
                    return true;
                }
            }

            return false;
        }

        private static class NativeMethods
        {
            public const int ERROR_NOT_CONNECTED = 0x8CA;

            [DllImport("wininet.dll", SetLastError = true)]
            public extern static bool InternetGetConnectedState(out InternetConnectionState lpdwFlags, int dwReserved);

            [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Ansi)]
            public extern static bool InternetCheckConnectionA(
                string lpszUrl,
                int dwFlags,
                int dwReserved);

            public enum InternetConnectionState
            {
                CONFIGURED = 0x40,
                LAN = 0x02,
                MODEM = 0x01,
                MODEM_BUSY = 0x08,
                OFFLINE = 0x20,
                PROXY = 0x04
            }
        }
    }
}
