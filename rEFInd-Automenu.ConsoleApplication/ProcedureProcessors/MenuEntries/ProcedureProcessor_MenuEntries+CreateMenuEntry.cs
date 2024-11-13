using rEFInd_Automenu.Configuration;
using rEFInd_Automenu.Configuration.MenuEntry;
using rEFInd_Automenu.ConsoleApplication.ConsoleInterface;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Configuration;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Instance;

namespace rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.MenuEntries
{
    public static partial class ProcedureProcessor_MenuEntries
    {
        private static void CreateMenuEntry(EntriesConfigurationArgumentsInfo argumentsInfo)
        {
            // Working
            log.Info("Started creating new menuentry in current instances config file (\'MenuEntry --Create\' flag)");

            if (string.IsNullOrEmpty(argumentsInfo.CreateEntry))
                throw new ArgumentNullException(nameof(argumentsInfo.CreateEntry), "\"Configuration menuentry name\" argument is empty");

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = InstanceOperations.CheckInstanceExisting();

            // Parsing configuration file
            RefindConfiguration RefindConf = ConfigurationOperations.ParseConfigurationFile(EspRefindDir);

            // Checking for already existing entries
            MenuEntryInfo? entryInfo = RefindConf.Entries.FirstOrDefault(x => x.EntryName.Equals(argumentsInfo.CreateEntry));
            if (entryInfo != null)
            {
                log.ErrorFormat("Config file already contain menuentry with name \"{0}\"", argumentsInfo.CreateEntry);
                ConsoleInterfaceWriter.WriteError(string.Format("Config file already contain menuentry with name \"{0}\"", argumentsInfo.CreateEntry));
                return;
            }

            // Modifying configuration
            ConsoleProgram.Interface.Execute("Modifying config file", ConsoleProgram.CommonCommands, (ctrl) =>
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
    }
}
