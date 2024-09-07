using log4net;
using log4net.Config;
using log4net.Core;
using rEFInd_Automenu.Booting;
using rEFInd_Automenu.Resources;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace rEFInd_Automenu.RuntimeLogging
{
    public static class CoreLogging
    {
        //private static readonly string LogsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "rEFInd Automenu", "Logs");
        private static readonly string _LoggerConfigManifestPath = typeof(CoreLogging).Namespace + "." + "lognet.config";
        private static readonly ILog log = LogManager.GetLogger(typeof(CoreLogging));

        public static void InitLogFile()
        {
            // Create log file
            XmlConfigurator.Configure(Assembly.GetExecutingAssembly().GetManifestResourceStream(_LoggerConfigManifestPath));
            AppDomain.CurrentDomain.ProcessExit += (_, _) => LoggerManager.Shutdown();

            // Write log header
            log.InfoFormat("\"rEFInd Automenu {0}\" by Rikitav (C) 2024", Assembly.GetCallingAssembly().GetName().Version ?? new Version(0, 0));
            log.InfoFormat("\"rEFInd Boot Manager {0}\" by Roderick W. Smith (C) 2024", EmbeddedResourceManager.rEFIndBin_VersionInResources);
            log.InfoFormat("Processor   : {0}", ArchitectureInfo.Current);
            log.InfoFormat("Environment : {0}", NativeMethods.GetParentProcess()?.ProcessName ?? "<UNKNOWN>");
#if DEBUG
            log.Info("DEBUG VERSION");
#endif
        }

        private static class NativeMethods
        {
            private struct PROCESS_BASIC_INFORMATION
            {
                public IntPtr Reserved1;
                public IntPtr PebBaseAddress;
                public IntPtr Reserved2_0;
                public IntPtr Reserved2_1;
                public IntPtr UniqueProcessId;
                public IntPtr InheritedFromUniqueProcessId;
            }

            [DllImport("ntdll.dll")]
            private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref PROCESS_BASIC_INFORMATION processInformation, int processInformationLength, out int returnLength);

            public static Process? GetParentProcess()
            {
                PROCESS_BASIC_INFORMATION pbi = new PROCESS_BASIC_INFORMATION();
                IntPtr CurrProcHandle = Process.GetCurrentProcess().Handle;

                int status = NtQueryInformationProcess(CurrProcHandle, 0, ref pbi, Marshal.SizeOf(pbi), out _);
                if (status != 0)
                    return null;

                try
                {
                    return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
                }
                catch (ArgumentException)
                {
                    // not found
                    return null;
                }
            }
        }
    }
}
