using log4net;

namespace rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.Obtain
{
    public static partial class ProcedureProcessor_Obtain
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProcedureProcessor_Obtain));

        public static void Execute(GetArgumentsInfo argumentsInfo)
        {
            // --Embedded
            if (argumentsInfo.ExtractResourceArchive)
            {
                ExtractResourceArchive();
                return;
            }

            // --Archive
            if (argumentsInfo.DownloadSourceArchive != null)
            {
                DownloadResourceArchive(argumentsInfo.DownloadSourceArchive);
                return;
            }

            // --SysPar
            if (argumentsInfo.MountEsp)
            {
                MountEsp();
                return;
            }
        }
    }
}
