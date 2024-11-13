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
        private static void RemoveGlobalFieldOption(GlobalConfigurationArgumentsInfo argumentsInfo)
        {
            // Working
            log.InfoFormat("Started removing global config value {0} (\'Config --Remove\' flag)", argumentsInfo.SetGlobalFieldValue);

            if (string.IsNullOrEmpty(argumentsInfo.RemoveGlobalFieldValue))
                throw new ArgumentNullException(nameof(argumentsInfo.RemoveGlobalFieldValue), "\"Configuration field name\" argument is empty");

            // Checking setting value existing
            PropertyInfo? GlobalOptionProperty = FindConfigurationInformationProperty(typeof(RefindGlobalConfigurationInfo), argumentsInfo.RemoveGlobalFieldValue);
            if (GlobalOptionProperty == null)
            {
                log.ErrorFormat("rEFInd configuration does not contain such option as \"{0}\"", argumentsInfo.RemoveGlobalFieldValue);
                ConsoleInterfaceWriter.WriteError(string.Format("rEFInd configuration does not contain such option as \"{0}\". Check the spelling of the option", argumentsInfo.RemoveGlobalFieldValue));
                return;
            }

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = InstanceOperations.CheckInstanceExisting();

            // Parsing configuration file
            RefindConfiguration RefindConf = ConfigurationOperations.ParseConfigurationFile(EspRefindDir);

            // Modifying configuration
            ConsoleProgram.Interface.Execute("Modifying config file", ConsoleProgram.CommonCommands, (ctrl) =>
            {
                // Modifying option
                log.InfoFormat("Removing \"{0}\" option", argumentsInfo.RemoveGlobalFieldValue);
                GlobalOptionProperty.SetValue(RefindConf.Global, null);

                // Writing updated config file
                log.Info("Writing updated configuration to file");
                ConfigurationFileBuilder confBuilder = new ConfigurationFileBuilder(RefindConf, EspRefindDir.FullName);
                confBuilder.WriteConfigurationToFile("refind.conf");

                // Success
                log.Info("Config file updated successfully");
                ctrl.Success("Removed " + argumentsInfo.RemoveGlobalFieldValue);
            });
        }
    }
}
