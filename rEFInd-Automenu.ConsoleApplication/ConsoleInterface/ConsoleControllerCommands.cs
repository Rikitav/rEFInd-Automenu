using log4net;
using rEFInd_Automenu.RuntimeConfiguration;
using Rikitav.Plasma.Controls.Spinners;

namespace rEFInd_Automenu.ConsoleApplication.ConsoleInterface
{
    public class ConsoleControllerCommands : IConsoleInterfacenterfaceCommands
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ConsoleControllerCommands));
        private static readonly bool LogInterfaceResults = RegistryExplorer.LogInterfaceResults;
        private static readonly bool LogInterfaceExecution = RegistryExplorer.LogInterfaceExecution;
        private object SyncLockObject = new object();

        private readonly Spinner KernelSpinner = new Spinner()
        {
            WorkPattern = "[ {Spinner} ] {WorkText}",
            SpinSequence = new string[] { "/", "-", "\\", "|" },
            MoveToNextLineAtClosing = true,
        };

        public void BeforeExecute(string WorkText)
        {
            lock (SyncLockObject)
            {
                if (LogInterfaceExecution)
                    log.InfoFormat("\nTask entered : {0}", WorkText);

                SetWorkLine(Console.CursorTop);
                KernelSpinner.Display(WorkText);
            }
        }

        public void ChangeLabel(string WorkText)
        {
            KernelSpinner.WorkText = WorkText;
        }

        public void SetWorkLine(int WorkLine)
        {
            KernelSpinner.WorkLine = WorkLine;
        }

        public void SetLockHandle(object SyncLockObject)
        {
            KernelSpinner.SetLockHandle(SyncLockObject);
            this.SyncLockObject = SyncLockObject;
        }

        public void AfterSuccessExecute(string Message)
        {
            lock (SyncLockObject)
            {
                KernelSpinner.Close();
                DebugAwait();
                ConsoleInterfaceWriter.WriteSuccess(KernelSpinner.WorkLine, KernelSpinner.WorkText, Message);

                if (LogInterfaceResults)
                    log.InfoFormat("Task closed (Success) : {0} : {1}", KernelSpinner.WorkText, Message);
                else if (LogInterfaceExecution)
                    log.InfoFormat("Task closed (Success) : {0}", KernelSpinner.WorkText);
            }
        }

        public void AfterErrorExecute(string Message)
        {
            lock (SyncLockObject)
            {
                KernelSpinner.Close();
                DebugAwait();
                ConsoleInterfaceWriter.WriteError(KernelSpinner.WorkLine, KernelSpinner.WorkText, Message);

                if (LogInterfaceResults)
                    log.InfoFormat("Task closed (Error) : {0} : {1}", KernelSpinner.WorkText, Message);
                else if (LogInterfaceExecution)
                    log.InfoFormat("Task closed (Error) : {0}", KernelSpinner.WorkText);
            }
        }

        public void AfterErrorExecute(Exception ErrorInfo)
        {
            lock (SyncLockObject)
            {
                KernelSpinner.Close();
                DebugAwait();
                ConsoleInterfaceWriter.WriteError(KernelSpinner.WorkLine, KernelSpinner.WorkText, string.Format("{0} : {1}", ErrorInfo.GetType().Name, ErrorInfo.Message));

                if (LogInterfaceResults)
                    log.InfoFormat("Task closed (Error) : {0} : {1}", KernelSpinner.WorkText, string.Format("{0} : {1}", ErrorInfo.GetType().Name, ErrorInfo.Message));
                else if (LogInterfaceExecution)
                    log.InfoFormat("Task closed (Error) : {0}", KernelSpinner.WorkText);
            }
        }

        public void AfterErrorExecute(object ErrorObj)
        {
            lock (SyncLockObject)
            {
                KernelSpinner.Close();
                DebugAwait();
                ConsoleInterfaceWriter.WriteError(KernelSpinner.WorkLine, KernelSpinner.WorkText, ErrorObj.ToString());

                if (LogInterfaceResults)
                    log.InfoFormat("Task closed (Error) : {0} : {1}", KernelSpinner.WorkText, ErrorObj.ToString());
                else if (LogInterfaceExecution)
                    log.InfoFormat("Task closed (Error) : {0}", KernelSpinner.WorkText);
            }
        }

        public void AfterWarningExecute(string Message)
        {
            lock (SyncLockObject)
            {
                KernelSpinner.Close();
                DebugAwait();
                ConsoleInterfaceWriter.WriteWarning(KernelSpinner.WorkLine, KernelSpinner.WorkText, Message);

                if (LogInterfaceResults)
                    log.InfoFormat("Task closed (Warn) : {0} : {1}", KernelSpinner.WorkText, Message);
                else if (LogInterfaceExecution)
                    log.InfoFormat("Task closed (Warn) : {0}", KernelSpinner.WorkText);
            }
        }

        public void AfterWarningExecute(Exception WarningInfo)
        {
            lock (SyncLockObject)
            {
                KernelSpinner.Close();
                DebugAwait();
                ConsoleInterfaceWriter.WriteWarning(KernelSpinner.WorkLine, KernelSpinner.WorkText, string.Format("{0} : {1}",WarningInfo.GetType().Name, WarningInfo.Message));

                if (LogInterfaceResults)
                    log.InfoFormat("Task closed (Warn) : {0} : {1}", KernelSpinner.WorkText, string.Format("{0} : {1}", WarningInfo.GetType().Name, WarningInfo.Message));
                else if (LogInterfaceExecution)
                    log.InfoFormat("Task closed (Warn) : {0}", KernelSpinner.WorkText);
            }
        }

        public void AfterWarningExecute(object WarningObj)
        {
            lock (SyncLockObject)
            {
                KernelSpinner.Close();
                DebugAwait();
                ConsoleInterfaceWriter.WriteWarning(KernelSpinner.WorkLine, KernelSpinner.WorkText, WarningObj.ToString());

                if (LogInterfaceResults)
                    log.InfoFormat("Task closed (Warn) : {0} : {1}", KernelSpinner.WorkText, WarningObj.ToString());
                else if (LogInterfaceExecution)
                    log.InfoFormat("Task closed (Warn) : {0}", KernelSpinner.WorkText);
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
