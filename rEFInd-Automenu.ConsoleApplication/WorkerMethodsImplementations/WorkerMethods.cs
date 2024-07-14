using log4net;
using rEFInd_Automenu.ConsoleApplication.ConsoleInterface;

namespace rEFInd_Automenu.ConsoleApplication.WorkerMethodsImplementations
{
    public partial class WorkerMethods
    {
        private static ILog log = LogManager.GetLogger(typeof(WorkerMethods));
        private IConsoleInterfacenterfaceCommands commands;

        public WorkerMethods(IConsoleInterfacenterfaceCommands commands)
        {
            this.commands = commands;
        }
    }
}
