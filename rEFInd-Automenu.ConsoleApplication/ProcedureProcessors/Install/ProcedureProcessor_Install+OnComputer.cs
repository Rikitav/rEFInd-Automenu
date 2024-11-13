using rEFInd_Automenu.Booting;
using rEFInd_Automenu.Configuration;
using rEFInd_Automenu.Configuration.LoaderParsers;
using rEFInd_Automenu.ConsoleApplication.ConsoleInterface;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Booting;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Configuration;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Installing;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Instance;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Resources;
using rEFInd_Automenu.RuntimeConfiguration;
using System.Runtime.InteropServices;

namespace rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.Install
{
    public static partial class ProcedureProcessor_Install
    {
        public static void OnComputer(InstallArgumentsInfo installArguments)
        {
            // Working
            log.Info("Started installation on current computer (\'Install --\' flag)");

            // Setting processor architecture
            FirmwareExecutableArchitecture Arch;
            if (installArguments.Architecture == FirmwareExecutableArchitecture.None)
            {
                // Getting current architecture
                Arch = ArchitectureInfo.Current;
                if (!ArchitectureInfo.IsCurrentPlatformSupported())
                {
                    log.Error(RuntimeInformation.OSArchitecture + " Architecture is not capable for rEFInd installation");
                    ConsoleInterfaceWriter.WriteWarning(RuntimeInformation.OSArchitecture + " Architecture is not capable for rEFInd installation");
                    return;
                }

                log.InfoFormat("Using current processor architecture : {0}", Arch);
            }
            else
            {
                // Follow by user
                Arch = installArguments.Architecture;
                log.InfoFormat("Using different processor architecture : {0}", Arch);
            }

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = InstanceOperations.CheckInstanceNonExisting(
                installArguments.ForceWork); // ForceWork

            ConfigurationFileBuilder configurationBuilder = ConfigurationOperations.GetConfigurationFileBuilder(
                EspRefindDir,
                false);

            // Getting resource archive
            ResourcesOperations.ObtainResourceArchive(
                installArguments.DownloadLatestBin, // DownloadLatest
                new Version(0, 0, 0));              // InstalledVersion

            // Resource stream containig an archive that extracting into temp directory
            DirectoryInfo BinArchiveDir = ResourcesOperations.ExtractLatestResourceArchive();

            // Moving binaries (loader and tools) to "refind" directory on ESP
            InstallingOperations.MoveResourceBinaries(
                BinArchiveDir, // BinArchiveDir
                EspRefindDir,  // RefindInstallationDir
                Arch,          // Arch
                false);        // USB

            // Installing formaliztion theme
            if (installArguments.Theme != null)
            {
                configurationBuilder.ThemeDirectory = InstallingOperations.InstallFormalizationThemeDirectory(
                    installArguments.Theme, // ThemeDir
                    EspRefindDir);          // RefindInstallationDir
            }

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
                configurationBuilder,        // ThemeDir
                installArguments.OnComputer, //
                loadersScanner);             // loadersScanner

            // Writing config file
            ConfigurationOperations.WriteConfigFile(
                EspRefindDir,          // refindDir
                configurationBuilder); // configurationBuilder

            // Configuring boot loader for rEFInd boot manager
            if (!ProgramConfiguration.Instance.PreferBootmgrBooting)
            {
                // Creating rEFInd boot option
                BootingOperations.CreateRefindFirmwareLoadOption(
                    false, // overrideExisting
                    true,  // addFirst
                    Arch); // Arch
            }
            else
            {
                // Configuring bootmagr to loading rEFInd
                BootingOperations.ConfigureBootmgrBootEntry(
                    Arch); // Arch
            }
        }
    }
}
