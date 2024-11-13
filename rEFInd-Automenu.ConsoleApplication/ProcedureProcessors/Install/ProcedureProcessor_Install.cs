using log4net;

namespace rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.Install
{
    public static partial class ProcedureProcessor_Install
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProcedureProcessor_Install));

        public static void Execute(InstallArgumentsInfo installArguments)
        {
            // --Computer
            if (installArguments.OnComputer)
            {
                OnComputer(installArguments);
                return;
            }

            // --FlashDrive
            if (installArguments.OnFlashDrive != null)
            {
                OnFlashDrive(installArguments);
                return;
            }
        }
    }
}
