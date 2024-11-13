using rEFInd_Automenu.Booting;
using rEFInd_Automenu.Configuration;

namespace rEFInd_Automenu.ConsoleApplication.OperationsManager.Configuration
{
    public static partial class ConfigurationOperations
    {
        public static void GenerateConfigFile(ConfigurationFileBuilder configurationBuilder, bool AddWindowsEntry, ILoadersScanner loadersScanner) => ConsoleProgram.Interface.Execute("Generating config file", ConsoleProgram.CommonCommands, (ctrl) =>
        {
            // Adding windows menu entry
            if (AddWindowsEntry)
                configurationBuilder.AddWindowsMenuEntry();

            // Configuring entries
            configurationBuilder.ParseConfigurationEntries(loadersScanner, ArchitectureInfo.Current);
            configurationBuilder.AssignLoaderIcons();

            // Writing
            log.Info("Config file succesfully regenerated");
            ctrl.Success(loadersScanner.GetType().Name ?? string.Empty);
            return;
        });
    }
}
