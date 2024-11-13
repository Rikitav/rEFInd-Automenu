using rEFInd_Automenu.Configuration;
using rEFInd_Automenu.Configuration.MenuEntry;
using rEFInd_Automenu.Configuration.Serializing;
using rEFInd_Automenu.ConsoleApplication.ConsoleInterface;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Configuration;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Instance;

namespace rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.MenuEntries
{
    public static partial class ProcedureProcessor_MenuEntries
    {
        private static readonly string[] _EnumAllCommands = new string[]
        {
            "all",
            "enum",
            "every",
            "any"
        };

        private static void ShowMenuEntry(EntriesConfigurationArgumentsInfo argumentsInfo)
        {
            // Working
            log.Info("Started reading existing menuentry in current instances config file (\'MenuEntry --Get\' flag)");

            if (string.IsNullOrEmpty(argumentsInfo.ShowEntry))
                throw new ArgumentNullException(nameof(argumentsInfo.ShowEntry), "\"Configuration menuentry name\" argument is empty");

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = InstanceOperations.CheckInstanceExisting();

            // Parsing configuration file
            RefindConfiguration RefindConf = ConfigurationOperations.ParseConfigurationFile(EspRefindDir);

            // Checking for already existing entries
            IEnumerable<MenuEntryInfo> entryInfo = !_EnumAllCommands.Contains(argumentsInfo.ShowEntry, StringComparer.CurrentCultureIgnoreCase)
                ? RefindConf.Entries.Where(x => x.EntryName.Equals(argumentsInfo.ShowEntry))
                : entryInfo = RefindConf.Entries;

            if (!entryInfo.Any())
            {
                log.ErrorFormat("Config file does not contain menuentry with name \"{0}\"", argumentsInfo.ShowEntry);
                ConsoleInterfaceWriter.WriteError(string.Format("Config file does not contain menuentry with name \"{0}\"", argumentsInfo.ShowEntry));
                return;
            }

            StringWriter MenuEntryWriter = new StringWriter();
            ConfigFileFormatter.BuildConfigEntriesString(MenuEntryWriter, entryInfo);
            ConsoleInterfaceWriter.WriteInformation(entryInfo.Count() > 1 ? "Enumerating entries" : "Reading entry", "\n" + MenuEntryWriter.ToString());
        }
    }
}
