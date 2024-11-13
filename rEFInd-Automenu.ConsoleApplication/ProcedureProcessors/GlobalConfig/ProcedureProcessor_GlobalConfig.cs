using log4net;
using rEFInd_Automenu.Configuration.Serializing;
using System.Reflection;

namespace rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.GlobalConfig
{
    public static partial class ProcedureProcessor_GlobalConfig
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProcedureProcessor_GlobalConfig));

        public static void Execute(GlobalConfigurationArgumentsInfo argumentsInfo)
        {
            // --Set
            if (!string.IsNullOrEmpty(argumentsInfo.SetGlobalFieldValue))
            {
                SetGlobalFieldValue(argumentsInfo);
                return;
            }

            // --Remove
            if (!string.IsNullOrEmpty(argumentsInfo.RemoveGlobalFieldValue))
            {
                RemoveGlobalFieldOption(argumentsInfo);
                return;
            }

            // --Get
            if (!string.IsNullOrEmpty(argumentsInfo.ShowGlobalFieldValue))
            {
                ShowGlobalFieldOption(argumentsInfo);
                return;
            }
        }

        private static PropertyInfo? FindConfigurationInformationProperty(Type InfoType, string RawName)
        {
            return InfoType
                .GetProperties()
                .FirstOrDefault(p => p.GetCustomAttribute<ConfigFileElementAttribute>()?.ElementName.Equals(RawName, StringComparison.CurrentCultureIgnoreCase) ?? false);
        }
    }
}
