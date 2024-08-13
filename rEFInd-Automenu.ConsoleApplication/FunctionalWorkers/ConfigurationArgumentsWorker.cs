using log4net;
using rEFInd_Automenu.Configuration;
using rEFInd_Automenu.Configuration.GlobalConfiguration;
using rEFInd_Automenu.Configuration.Parsing;
using rEFInd_Automenu.Configuration.Serializing;
using rEFInd_Automenu.ConsoleApplication.ConsoleInterface;
using rEFInd_Automenu.ConsoleApplication.WorkerMethodsImplementations;
using System.Reflection;

namespace rEFInd_Automenu.ConsoleApplication.FunctionalWorkers
{
    public static class GlobalConfigurationArgumentsWorker
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GetArgumentsWorker));

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

        private static void SetGlobalFieldValue(GlobalConfigurationArgumentsInfo argumentsInfo)
        {
            // Setting commands
            object SyncLockObject = new object();
            ConsoleControllerCommands commands = ConsoleProgram.GetControllerCommands<ConsoleControllerCommands>(SyncLockObject);
            WorkerMethods methods = new WorkerMethods(commands);

            if (string.IsNullOrEmpty(argumentsInfo.SetGlobalFieldValue))
                throw new ArgumentNullException(nameof(argumentsInfo.SetGlobalFieldValue));

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
            DirectoryInfo EspRefindDir = methods.CheckInstanceExisting();

            // Parsing configuration file
            RefindConfiguration RefindConf = methods.ParseConfigurationFile(EspRefindDir);

            // Modifying configuration
            ConsoleProgram.Interface.Execute("Modifying config file", commands, (ctrl) =>
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

        private static void RemoveGlobalFieldOption(GlobalConfigurationArgumentsInfo argumentsInfo)
        {
            // Setting commands
            object SyncLockObject = new object();
            ConsoleControllerCommands commands = ConsoleProgram.GetControllerCommands<ConsoleControllerCommands>(SyncLockObject);
            WorkerMethods methods = new WorkerMethods(commands);

            if (string.IsNullOrEmpty(argumentsInfo.RemoveGlobalFieldValue))
                throw new ArgumentNullException(nameof(argumentsInfo.RemoveGlobalFieldValue));

            // Checking setting value existing
            PropertyInfo? GlobalOptionProperty = FindConfigurationInformationProperty(typeof(RefindGlobalConfigurationInfo), argumentsInfo.RemoveGlobalFieldValue);
            if (GlobalOptionProperty == null)
            {
                log.ErrorFormat("rEFInd configuration does not contain such option as \"{0}\"", argumentsInfo.RemoveGlobalFieldValue);
                ConsoleInterfaceWriter.WriteError(string.Format("rEFInd configuration does not contain such option as \"{0}\". Check the spelling of the option", argumentsInfo.RemoveGlobalFieldValue));
                return;
            }

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = methods.CheckInstanceExisting();

            // Parsing configuration file
            RefindConfiguration RefindConf = methods.ParseConfigurationFile(EspRefindDir);

            // Modifying configuration
            ConsoleProgram.Interface.Execute("Modifying config file", commands, (ctrl) =>
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

        private static void ShowGlobalFieldOption(GlobalConfigurationArgumentsInfo argumentsInfo)
        {
            // Setting commands
            object SyncLockObject = new object();
            ConsoleControllerCommands commands = ConsoleProgram.GetControllerCommands<ConsoleControllerCommands>(SyncLockObject);
            WorkerMethods methods = new WorkerMethods(commands);

            if (string.IsNullOrEmpty(argumentsInfo.ShowGlobalFieldValue))
                throw new ArgumentNullException(nameof(argumentsInfo.ShowGlobalFieldValue));

            // Checking setting value existing
            PropertyInfo? GlobalOptionProperty = FindConfigurationInformationProperty(typeof(RefindGlobalConfigurationInfo), argumentsInfo.ShowGlobalFieldValue);
            if (GlobalOptionProperty == null)
            {
                log.ErrorFormat("The field named \"{0}\" does not exist in the current configuration version", argumentsInfo.ShowGlobalFieldValue);
                ConsoleInterfaceWriter.WriteError(string.Format("The field named \"{0}\" does not exist in the current configuration version. Check the spelling of the option", argumentsInfo.ShowGlobalFieldValue));
                return;
            }

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = methods.CheckInstanceExisting();

            // Parsing configuration file
            RefindConfiguration RefindConf = methods.ParseConfigurationFile(EspRefindDir);

            // Getting value
            object? PropValue = GlobalOptionProperty.GetValue(RefindConf.Global);
            ConsoleInterfaceWriter.WriteInformation(argumentsInfo.ShowGlobalFieldValue, (PropValue ?? "not set and has null value").ToString());
        }

        private static PropertyInfo? FindConfigurationInformationProperty(Type InfoType, string RawName)
        {
            return InfoType
                .GetProperties()
                .FirstOrDefault(p => p.GetCustomAttribute<ConfigFileElementAttribute>().ElementName.Equals(RawName, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
