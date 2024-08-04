using log4net;
using rEFInd_Automenu.RegistryExplorer;
using Rikitav.Plasma.Controls.Spinners;

namespace rEFInd_Automenu.ConsoleApplication.ConsoleInterface
{
    public class ConsoleControllerProgressBarCommands : IConsoleInterfacenterfaceCommands
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ConsoleControllerProgressBarCommands));
        public static readonly bool LogControllerExecution = ProgramRegistry.LogInterfaceExecution;

        private object SyncLockObject = new object();
        private readonly Spinner KernelSpinner = new Spinner()
        {
            WorkPattern = "[ {Spinner} ] {WorkText}",
            SpinSequence = new string[] { "/", "-", "\\", "|" }
        };

        public readonly ProgressBar KernelProgressBar = new ProgressBar()
        {
            WorkPattern = "{Percent}% [{Bar}]",
            WorkingLeftOffset = 30
        };

        public void BeforeExecute(string WorkText)
        {
            lock (SyncLockObject)
            {
                SetWorkLine(Console.CursorTop);
                KernelSpinner.Display(WorkText);
                KernelProgressBar.Display(string.Empty);
            }
        }

        public void ChangeLabel(string WorkText)
        {
            KernelSpinner.WorkText = WorkText;
        }

        public void SetWorkLine(int WorkLine)
        {
            KernelSpinner.WorkLine = WorkLine;
            KernelProgressBar.WorkLine = WorkLine;
        }

        public void SetLockHandle(object SyncLockObject)
        {
            KernelSpinner.SetLockHandle(SyncLockObject);
            KernelProgressBar.SetLockHandle(SyncLockObject);
            this.SyncLockObject = SyncLockObject;
        }

        public void AfterSuccessExecute(string Message)
        {
            lock (SyncLockObject)
            {
                KernelSpinner.Close();
                KernelProgressBar.Close();
                DebugAwait();
                ConsoleInterfaceWriter.WriteSuccess(KernelSpinner.WorkLine, KernelSpinner.WorkText, Message);

                if (LogControllerExecution)
                    log.InfoFormat("Task closed : {0}", KernelSpinner.WorkText);
            }
        }

        public void AfterErrorExecute(string Message)
        {
            lock (SyncLockObject)
            {
                KernelSpinner.Close();
                KernelProgressBar.Close();
                DebugAwait();
                ConsoleInterfaceWriter.WriteError(KernelSpinner.WorkLine, KernelSpinner.WorkText, Message);

                if (LogControllerExecution)
                    log.InfoFormat("Task closed : {0}", KernelSpinner.WorkText);
            }
        }

        public void AfterErrorExecute(Exception ErrorInfo)
        {
            lock (SyncLockObject)
            {
                KernelSpinner.Close();
                KernelProgressBar.Close();
                DebugAwait();
                ConsoleInterfaceWriter.WriteError(KernelSpinner.WorkLine, KernelSpinner.WorkText, ErrorInfo.Message);

                if (LogControllerExecution)
                    log.InfoFormat("Task closed : {0}", KernelSpinner.WorkText);
            }
        }

        public void AfterErrorExecute(object ErrorObj)
        {
            lock (SyncLockObject)
            {
                KernelSpinner.Close();
                KernelProgressBar.Close();
                DebugAwait();
                ConsoleInterfaceWriter.WriteError(KernelSpinner.WorkLine, KernelSpinner.WorkText, ErrorObj.ToString());

                if (LogControllerExecution)
                    log.InfoFormat("Task closed : {0}", KernelSpinner.WorkText);
            }
        }

        public void AfterWarningExecute(string Message)
        {
            lock (SyncLockObject)
            {
                KernelSpinner.Close();
                KernelProgressBar.Close();
                DebugAwait();
                ConsoleInterfaceWriter.WriteWarning(KernelSpinner.WorkLine, KernelSpinner.WorkText, Message);

                if (LogControllerExecution)
                    log.InfoFormat("Task closed : {0}", KernelSpinner.WorkText);
            }
        }

        public void AfterWarningExecute(Exception WarningInfo)
        {
            lock (SyncLockObject)
            {
                KernelSpinner.Close();
                KernelProgressBar.Close();
                DebugAwait();
                ConsoleInterfaceWriter.WriteWarning(KernelSpinner.WorkLine, KernelSpinner.WorkText, WarningInfo.Message);

                if (LogControllerExecution)
                    log.InfoFormat("Task closed : {0}", KernelSpinner.WorkText);
            }
        }

        public void AfterWarningExecute(object WarningObj)
        {
            lock (SyncLockObject)
            {
                KernelSpinner.Close();
                KernelProgressBar.Close();
                DebugAwait();
                ConsoleInterfaceWriter.WriteWarning(KernelSpinner.WorkLine, KernelSpinner.WorkText, WarningObj.ToString());

                if (LogControllerExecution)
                    log.InfoFormat("Task closed : {0}", KernelSpinner.WorkText);
            }
        }

        private static void DebugAwait()
        {
            //#if DEBUG
            int Await = 150;
            Thread.Sleep(Await);
            //#endif
        }
    }
}
