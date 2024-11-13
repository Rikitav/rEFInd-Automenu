using rEFInd_Automenu.Configuration;
using rEFInd_Automenu.Configuration.GlobalConfiguration;
using rEFInd_Automenu.Configuration.Parsing;
using rEFInd_Automenu.ConsoleApplication.ConsoleInterface;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Configuration;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Instance;
using System.Reflection;

namespace rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.GlobalConfig
{
    public static partial class ProcedureProcessor_GlobalConfig
    {
        private static void SetGlobalFieldValue(GlobalConfigurationArgumentsInfo argumentsInfo)
        {
            // Working
            log.InfoFormat("Started editing global config value {0} (\'Config --Set\' flag)", argumentsInfo.SetGlobalFieldValue);

            if (string.IsNullOrEmpty(argumentsInfo.SetGlobalFieldValue))
                throw new ArgumentNullException(nameof(argumentsInfo.SetGlobalFieldValue), "\"Configuration field name\" argument is empty");

            // Checking seting value
            if (string.IsNullOrEmpty(argumentsInfo.ValueToSet))
            {
                log.ErrorFormat("Value for settings \"{0}\" global option is null", argumentsInfo.ValueToSet);
                ConsoleInterfaceWriter.WriteError(string.Format("Value for settings \"{0}\" global option is null. Pass \"--Value\" argument to set new value", argumentsInfo.ValueToSet));
                return;
            }

            // Checking setting value existing
            PropertyInfo? GlobalOptionProperty = FindConfigurationInformationProperty(typeof(RefindGlobalConfigurationInfo), argumentsInfo.SetGlobalFieldValue);
            if (GlobalOptionProperty == null)
            {
                log.ErrorFormat("rEFInd configuration does not contain such option as \"{0}\"", argumentsInfo.SetGlobalFieldValue);
                ConsoleInterfaceWriter.WriteError(string.Format("rEFInd configuration does not contain such option as \"{0}\". Check the spelling of the option", argumentsInfo.SetGlobalFieldValue));
                return;
            }

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = InstanceOperations.CheckInstanceExisting();

            // Parsing configuration file
            RefindConfiguration RefindConf = ConfigurationOperations.ParseConfigurationFile(EspRefindDir);

            // Modifying configuration
            ConsoleProgram.Interface.Execute("Modifying config file", ConsoleProgram.CommonCommands, (ctrl) =>
            {
                // Getting new value
                object? NewPropValue = ConfigurationTokenParser.ConvertTokenValue(GlobalOptionProperty.PropertyType, argumentsInfo.ValueToSet);
                if (NewPropValue == null)
                {
                    log.Error("Failed to parse new value");
                    ctrl.Error("Failed to parse new value");
                    return;
                }

                // Modifying option
                bool IsAdded = GlobalOptionProperty.GetValue(RefindConf.Global) == null;
                GlobalOptionProperty.SetValue(RefindConf.Global, NewPropValue);

                // Writing updated config file
                log.Info("Writing updated configuration to file");
                ConfigurationFileBuilder confBuilder = new ConfigurationFileBuilder(RefindConf, EspRefindDir.FullName);
                confBuilder.WriteConfigurationToFile("refind.conf");

                // Success
                log.Info("Config file updated successfully");
                ctrl.Success((IsAdded ? "Added " : "Changed ") + argumentsInfo.SetGlobalFieldValue);
            });
        }
    }
}
