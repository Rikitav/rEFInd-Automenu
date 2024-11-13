using rEFInd_Automenu.Configuration;
using rEFInd_Automenu.Configuration.LoaderParsers;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Configuration;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Instance;
using rEFInd_Automenu.RuntimeConfiguration;
using rEFInd_Automenu.TypesExtensions;

namespace rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.Instance
{
    public static partial class ProcedureProcessor_Instance
    {
        private static void RegenerateConfigFile()
        {
            // Working
            log.Info("Regenerating current rEFInd instance configuration file (\'Instance -RegenConf\' flag)");

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = InstanceOperations.CheckInstanceExisting();

            // Getting configuration file builder
            ConfigurationFileBuilder configurationBuilder = ConfigurationOperations.GetConfigurationFileBuilder(
                EspRefindDir,
                false);

            // Scanning loaders
            ILoadersScanner loadersScanner = ProgramConfiguration.Instance.LoaderScannerType switch
            {
                LoaderScannerType.NvramLoadOptionReader => new FirmwareLoadOptionsScanner(),
                LoaderScannerType.EspDirectoryEnumerator => new EfiSystemPartitionLoaderScanner(),
                LoaderScannerType.FwBootmgrRecordParser => new FwBootmgrLoaderScanner(),
                _ => new FirmwareLoadOptionsScanner()
            };

            // Generating configuration file
            ConfigurationOperations.GenerateConfigFile(
                configurationBuilder, // ThemeDir
                false,                //
                loadersScanner);      // loadersScanner

            // Writing config file
            ConfigurationOperations.WriteConfigFile(
                EspRefindDir,          // refindDir
                configurationBuilder); // configurationBuilder
                                       // loadersScanner
        }
    }
}
