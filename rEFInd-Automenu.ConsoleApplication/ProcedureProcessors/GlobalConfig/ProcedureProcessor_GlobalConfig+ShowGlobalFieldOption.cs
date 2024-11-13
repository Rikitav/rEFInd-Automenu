using rEFInd_Automenu.Configuration;
using rEFInd_Automenu.Configuration.GlobalConfiguration;
using rEFInd_Automenu.ConsoleApplication.ConsoleInterface;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Configuration;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Instance;
using System.Reflection;

namespace rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.GlobalConfig
{
    public static partial class ProcedureProcessor_GlobalConfig
    {
        private static void ShowGlobalFieldOption(GlobalConfigurationArgumentsInfo argumentsInfo)
        {
            // Working
            log.InfoFormat("Started showing global config value {0} (\'Config --Show\' flag)", argumentsInfo.ShowGlobalFieldValue);

            if (string.IsNullOrEmpty(argumentsInfo.ShowGlobalFieldValue))
                throw new ArgumentNullException(nameof(argumentsInfo.ShowGlobalFieldValue), "\"Configuration field name\" argument is empty");

            // Checking setting value existing
            PropertyInfo? GlobalOptionProperty = FindConfigurationInformationProperty(typeof(RefindGlobalConfigurationInfo), argumentsInfo.ShowGlobalFieldValue);
            if (GlobalOptionProperty == null)
            {
                log.ErrorFormat("The field named \"{0}\" does not exist in the current configuration version", argumentsInfo.ShowGlobalFieldValue);
                ConsoleInterfaceWriter.WriteError(string.Format("The field named \"{0}\" does not exist in the current configuration version. Check the spelling of the option", argumentsInfo.ShowGlobalFieldValue));
                return;
            }

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = InstanceOperations.CheckInstanceExisting();

            // Parsing configuration file
            RefindConfiguration RefindConf = ConfigurationOperations.ParseConfigurationFile(EspRefindDir);

            // Getting value
            object? PropValue = GlobalOptionProperty.GetValue(RefindConf.Global);
            ConsoleInterfaceWriter.WriteInformation(argumentsInfo.ShowGlobalFieldValue, (PropValue ?? "not set and has null value").ToString());
        }
    }
}
