using CommandLine;
using log4net;
using rEFInd_Automenu.Configuration.GlobalConfiguration;
using rEFInd_Automenu.Configuration;
using rEFInd_Automenu.Configuration.MenuEntry;
using rEFInd_Automenu.Configuration.Parsing;
using rEFInd_Automenu.ConsoleApplication.ConsoleInterface;
using rEFInd_Automenu.ConsoleApplication.WorkerMethodsImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using rEFInd_Automenu.Configuration.Serializing;

namespace rEFInd_Automenu.ConsoleApplication.FunctionalWorkers
{
    public static class EntriesConfigurationArgumentsWorker
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GetArgumentsWorker));

        public static void Execute(EntriesConfigurationArgumentsInfo argumentsInfo)
        {
            // --Create
            if (!string.IsNullOrEmpty(argumentsInfo.CreateEntry))
            {
                CreateMenuEntry(argumentsInfo);
                return;
            }

            // --Remove
            if (!string.IsNullOrEmpty(argumentsInfo.RemoveEntry))
            {
                RemoveMenuEntry(argumentsInfo);
                return;
            }

            // --Edit
            if (!string.IsNullOrEmpty(argumentsInfo.EditEntry))
            {
                EditMenuEntry(argumentsInfo);
                return;
            }

            // --Get
            if (!string.IsNullOrEmpty(argumentsInfo.ShowEntry))
            {
                ShowMenuEntry(argumentsInfo);
                return;
            }
        }

        private static void CreateMenuEntry(EntriesConfigurationArgumentsInfo argumentsInfo)
        {
            // Setting commands
            object SyncLockObject = new object();
            ConsoleControllerCommands commands = ConsoleProgram.GetControllerCommands<ConsoleControllerCommands>(SyncLockObject);
            WorkerMethods methods = new WorkerMethods(commands);

            if (string.IsNullOrEmpty(argumentsInfo.CreateEntry))
                throw new ArgumentNullException(nameof(argumentsInfo.CreateEntry));

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = methods.CheckInstanceExisting();

            // Parsing configuration file
            RefindConfiguration RefindConf = methods.ParseConfigurationFile(EspRefindDir);

            // Checking for already existing entries
            MenuEntryInfo? entryInfo = RefindConf.Entries.FirstOrDefault(x => x.EntryName.Equals(argumentsInfo.CreateEntry));
            if (entryInfo != null)
            {
                log.ErrorFormat("Config file already contain menuentry with name \"{0}\"", argumentsInfo.CreateEntry);
                ConsoleInterfaceWriter.WriteError(string.Format("Config file already contain menuentry with name \"{0}\"", argumentsInfo.CreateEntry));
                return;
            }

            // Modifying configuration
            ConsoleProgram.Interface.Execute("Modifying config file", commands, (ctrl) =>
            {
                // Creating new MenuEntry info instance
                log.InfoFormat("Creating \"{0}\" entry", argumentsInfo.CreateEntry);
                entryInfo = new MenuEntryInfo() { EntryName = argumentsInfo.CreateEntry };
                
                // Applying arguments to info and adding to configuration
                EnumerateAndSetMenuEntryValues(entryInfo, argumentsInfo);
                RefindConf.Entries.Add(entryInfo);

                // Writing updated config file
                log.Info("Writing updated configuration to file");
                ConfigurationFileBuilder confBuilder = new ConfigurationFileBuilder(RefindConf, EspRefindDir.FullName);
                confBuilder.WriteConfigurationToFile("refind.conf");

                // Success
                log.Info("Config file updated successfully");
                ctrl.Success("Created new \"" + argumentsInfo.CreateEntry + "\" menuentry");
            });
        }

        private static void RemoveMenuEntry(EntriesConfigurationArgumentsInfo argumentsInfo)
        {
            // Setting commands
            object SyncLockObject = new object();
            ConsoleControllerCommands commands = ConsoleProgram.GetControllerCommands<ConsoleControllerCommands>(SyncLockObject);
            WorkerMethods methods = new WorkerMethods(commands);

            if (string.IsNullOrEmpty(argumentsInfo.RemoveEntry))
                throw new ArgumentNullException(nameof(argumentsInfo.RemoveEntry));

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = methods.CheckInstanceExisting();

            // Parsing configuration file
            RefindConfiguration RefindConf = methods.ParseConfigurationFile(EspRefindDir);

            // Checking for already existing entries
            MenuEntryInfo? entryInfo = RefindConf.Entries.FirstOrDefault(x => x.EntryName.Equals(argumentsInfo.RemoveEntry));
            if (entryInfo == null)
            {
                log.ErrorFormat("Config file does not contain menuentry with name \"{0}\"", argumentsInfo.RemoveEntry);
                ConsoleInterfaceWriter.WriteError(string.Format("Config file does not contain menuentry with name \"{0}\"", argumentsInfo.RemoveEntry));
                return;
            }

            // Modifying configuration
            ConsoleProgram.Interface.Execute("Modifying config file", commands, (ctrl) =>
            {
                // Creating new MenuEntry info instance
                log.InfoFormat("Removing \"{0}\" entry", argumentsInfo.RemoveEntry);
                RefindConf.Entries.Remove(entryInfo);

                // Writing updated config file
                log.Info("Writing updated configuration to file");
                ConfigurationFileBuilder confBuilder = new ConfigurationFileBuilder(RefindConf, EspRefindDir.FullName);
                confBuilder.WriteConfigurationToFile("refind.conf");

                // Success
                log.Info("Config file updated successfully");
                ctrl.Success("Removed \"" + argumentsInfo.RemoveEntry + "\" menuentry");
            });
        }

        private static void EditMenuEntry(EntriesConfigurationArgumentsInfo argumentsInfo)
        {
            // Setting commands
            object SyncLockObject = new object();
            ConsoleControllerCommands commands = ConsoleProgram.GetControllerCommands<ConsoleControllerCommands>(SyncLockObject);
            WorkerMethods methods = new WorkerMethods(commands);

            if (string.IsNullOrEmpty(argumentsInfo.EditEntry))
                throw new ArgumentNullException(nameof(argumentsInfo.EditEntry));

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = methods.CheckInstanceExisting();

            // Parsing configuration file
            RefindConfiguration RefindConf = methods.ParseConfigurationFile(EspRefindDir);

            // Checking for already existing entries
            MenuEntryInfo? entryInfo = RefindConf.Entries.FirstOrDefault(x => x.EntryName.Equals(argumentsInfo.EditEntry));
            if (entryInfo == null)
            {
                log.ErrorFormat("Config file does not contain menuentry with name \"{0}\"", argumentsInfo.EditEntry);
                ConsoleInterfaceWriter.WriteError(string.Format("Config file does not contain menuentry with name \"{0}\"", argumentsInfo.EditEntry));
                return;
            }

            // Modifying configuration
            ConsoleProgram.Interface.Execute("Modifying config file", commands, (ctrl) =>
            {
                // Editing existing menuentry
                log.InfoFormat("Creating \"{0}\" entry", argumentsInfo.EditEntry);
                EnumerateAndSetMenuEntryValues(entryInfo, argumentsInfo);

                // Writing updated config file
                log.Info("Writing updated configuration to file");
                ConfigurationFileBuilder confBuilder = new ConfigurationFileBuilder(RefindConf, EspRefindDir.FullName);
                confBuilder.WriteConfigurationToFile("refind.conf");

                // Success
                log.Info("Config file updated successfully");
                ctrl.Success("Created new \"" + argumentsInfo.EditEntry + "\" menuentry");
            });
        }

        private static void ShowMenuEntry(EntriesConfigurationArgumentsInfo argumentsInfo)
        {
            // Setting commands
            object SyncLockObject = new object();
            ConsoleControllerCommands commands = ConsoleProgram.GetControllerCommands<ConsoleControllerCommands>(SyncLockObject);
            WorkerMethods methods = new WorkerMethods(commands);

            if (string.IsNullOrEmpty(argumentsInfo.ShowEntry))
                throw new ArgumentNullException(nameof(argumentsInfo.ShowEntry));

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = methods.CheckInstanceExisting();

            // Parsing configuration file
            RefindConfiguration RefindConf = methods.ParseConfigurationFile(EspRefindDir);

            // Checking for already existing entries
            IEnumerable<MenuEntryInfo> entryInfo = Array.Empty<MenuEntryInfo>();
            if (!new string[] { "all", "enum" }.Contains(argumentsInfo.ShowEntry, StringComparer.CurrentCultureIgnoreCase))
            {
                entryInfo = RefindConf.Entries.Where(x => x.EntryName.Equals(argumentsInfo.ShowEntry));
                if (!entryInfo.Any())
                {
                    log.ErrorFormat("Config file does not contain menuentry with name \"{0}\"", argumentsInfo.ShowEntry);
                    ConsoleInterfaceWriter.WriteError(string.Format("Config file does not contain menuentry with name \"{0}\"", argumentsInfo.ShowEntry));
                    return;
                }
            }
            else
            {
                entryInfo = RefindConf.Entries;
            }

            StringWriter MenuEntryWriter = new StringWriter();
            ConfigFileFormatter.BuildConfigEntriesString(MenuEntryWriter, entryInfo);

            ConsoleInterfaceWriter.WriteInformation(entryInfo.Count() > 1 ? "Enumerating entries" : "Reading entry", "\n" + MenuEntryWriter.ToString());
        }

        private static void EnumerateAndSetMenuEntryValues(MenuEntryInfo entryInfo, EntriesConfigurationArgumentsInfo argumentsInfo)
        {
            foreach (PropertyInfo ValProp in argumentsInfo.GetType().GetProperties())
            {
                // Getting option attributes
                OptionAttribute? optionAttribute = ValProp.GetCustomAttribute<OptionAttribute>();
                if (optionAttribute == null)
                    continue;

                // Getting if option is associated with main arguments
                if (!string.IsNullOrEmpty(optionAttribute.Group))
                    continue;

                // Getting property of info associated with this option
                PropertyInfo? entryInfoProperty = typeof(MenuEntryInfo).GetProperty(optionAttribute.LongName);
                if (entryInfoProperty == null)
                    continue;

                // Getting option's value
                string? OptionValue = ValProp.GetValue(argumentsInfo)?.ToString();
                if (string.IsNullOrEmpty(OptionValue))
                    continue;

                // Setting info's value
                object? NewInfoValue = OptionValue == "NULL" ? null : ConfigurationTokenParser.ConvertTokenValue(entryInfoProperty.PropertyType, OptionValue);
                log.InfoFormat("Setting {0} property to {1}", ValProp.Name, NewInfoValue == null ? "<NULL>" : NewInfoValue.ToString());
                entryInfoProperty.SetValue(entryInfo, NewInfoValue);
            }
        }
    }
}
