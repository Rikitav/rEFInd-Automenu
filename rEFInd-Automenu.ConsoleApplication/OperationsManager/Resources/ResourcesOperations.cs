using log4net;

namespace rEFInd_Automenu.ConsoleApplication.OperationsManager.Resources
{
    public static partial class ResourcesOperations
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ResourcesOperations));

        public static void ExitWait(this Task task)
        {
            AppDomain.CurrentDomain.ProcessExit += (_, _) => task.Wait();
        }
    }
}
