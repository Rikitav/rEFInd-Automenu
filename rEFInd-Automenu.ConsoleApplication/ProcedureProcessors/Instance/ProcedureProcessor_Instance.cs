using log4net;

namespace rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.Instance
{
    public static partial class ProcedureProcessor_Instance
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProcedureProcessor_Instance));

        public static void Execute(InstanceArgumentsInfo argumentsInfo)
        {
            // --ShowInfo
            if (argumentsInfo.ShowInfo)
            {
                ShowInstanceInfo();
                return;
            }

            // --Remove
            if (argumentsInfo.RemoveBin)
            {
                RemoveInstance();
                return;
            }

            // --Update
            if (argumentsInfo.UpdateBin)
            {
                UpdateInstanceBin();
                return;
            }

            // --OpenConfig
            if (argumentsInfo.OpenConfig)
            {
                OpenInstanceConfig();
                return;
            }

            // --InstallTheme
            if (argumentsInfo.InstallTheme != null)
            {
                InstallFormalizationTheme(argumentsInfo);
                return;
            }

            // --RegenBoot
            if (argumentsInfo.RegenBoot)
            {
                RegenerateBootEntry();
                return;
            }

            // --RegenConf
            if (argumentsInfo.RegenConf)
            {
                RegenerateConfigFile();
                return;
            }
        }
    }
}
