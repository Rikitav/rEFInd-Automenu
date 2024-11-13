using rEFInd_Automenu.Configuration;
using rEFInd_Automenu.Configuration.MenuEntry;
using rEFInd_Automenu.ConsoleApplication.ConsoleInterface;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Configuration;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Instance;

namespace rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.MenuEntries
{
    public static partial class ProcedureProcessor_MenuEntries
    {
        private static void RemoveMenuEntry(EntriesConfigurationArgumentsInfo argumentsInfo)
        {
            // Working
            log.Info("Started removing existing menuentry in current instances config file (\'MenuEntry --Remove\' flag)");

            if (string.IsNullOrEmpty(argumentsInfo.RemoveEntry))
                throw new ArgumentNullException(nameof(argumentsInfo.RemoveEntry), "\"Configuration menuentry name\" argument is empty");

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = InstanceOperations.CheckInstanceExisting();

            // Parsing configuration file
            RefindConfiguration RefindConf = ConfigurationOperations.ParseConfigurationFile(EspRefindDir);

            // Checking for already existing entries
            MenuEntryInfo? entryInfo = RefindConf.Entries.FirstOrDefault(x => x.EntryName.Equals(argumentsInfo.RemoveEntry));
            if (entryInfo == null)
            {
                log.ErrorFormat("Config file does not contain menuentry with name \"{0}\"", argumentsInfo.RemoveEntry);
                ConsoleInterfaceWriter.WriteError(string.Format("Config file does not contain menuentry with name \"{0}\"", argumentsInfo.RemoveEntry));
                return;
            }

            // Modifying configuration
            ConsoleProgram.Interface.Execute("Modifying config file", ConsoleProgram.CommonCommands, (ctrl) =>
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
    }
}
