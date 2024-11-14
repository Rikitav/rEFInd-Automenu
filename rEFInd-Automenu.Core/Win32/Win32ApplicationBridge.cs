using log4net;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace rEFInd_Automenu.Win32
{
    public class Win32ApplicationBridge : IDisposable
    {
        /*
        public static readonly Win32ApplicationBridge MountVol = new Win32ApplicationBridge("mountvol.exe");
        public static readonly Win32ApplicationBridge BcdEdit = new Win32ApplicationBridge("bcdedit.exe");
        */

        private bool _IsDisposed;
        private readonly string _ApplicationName;
        private readonly Process _ApplicationProcess;
        private static readonly ILog log = LogManager.GetLogger(typeof(Win32ApplicationBridge));

        public Win32ApplicationBridge(string ApplicationName)
        {
            _ApplicationName = ApplicationName;
            _ApplicationProcess = new Process()
            {
                StartInfo = new ProcessStartInfo(_ApplicationName)
                {
                    CreateNoWindow = true,
                    WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System)
                }
            };
        }

        public string Execute(string args, bool redirectStandartOutput = false, bool throwOnPositiveExitCode = true)
        {
            // 
            _ApplicationProcess.StartInfo.RedirectStandardOutput = redirectStandartOutput;
            _ApplicationProcess.StartInfo.Arguments = args;

            //
            log.InfoFormat("Executing Win32 application \"{1}\" with parameters - \"{0}\"", _ApplicationName, args);
            if (!_ApplicationProcess.Start())
            {
                log.ErrorFormat("Failed to execute {0} Win32 application", _ApplicationName);
                NullifyStartInfoParameters();
                throw new ProcessExecutionException(_ApplicationName);
            }

            //
            _ApplicationProcess.WaitForExit();
            if (throwOnPositiveExitCode && _ApplicationProcess.ExitCode != 0)
            {
                log.ErrorFormat("Failed to execute bcdedit (ExitCode : {0})", _ApplicationProcess.ExitCode);
                NullifyStartInfoParameters();
                throw new ProcessExecutionException(_ApplicationName, _ApplicationProcess.ExitCode);
            }

            //
            if (redirectStandartOutput)
            {
                StringBuilder outputBuilder = new StringBuilder();
                while (!_ApplicationProcess.StandardOutput.EndOfStream)
                    outputBuilder.AppendLine(_ApplicationProcess.StandardOutput.ReadLine());

                NullifyStartInfoParameters();
                return outputBuilder.ToString();
            }

            //
            NullifyStartInfoParameters();
            return string.Empty;
        }

        private void NullifyStartInfoParameters()
        {
            _ApplicationProcess.StartInfo.Arguments = string.Empty;
            _ApplicationProcess.StartInfo.RedirectStandardOutput = false;
        }

        public void Dispose()
        {
            if (_IsDisposed)
                return;

            GC.SuppressFinalize(this);
            _ApplicationProcess.Dispose();
            _IsDisposed = true;
        }
    }

    public class ProcessExecutionException : Win32Exception
    {
        public ProcessExecutionException(string appName)
            : base(string.Format("Failed to execute {0} Win32 application", appName)) { }

        public ProcessExecutionException(string appName, int exitCode)
            : base(exitCode, string.Format("{0} Win32 application exited with {1} exit code", appName, exitCode)) { }
    }
}
