using log4net;
using rEFInd_Automenu.Booting;
using rEFInd_Automenu.Configuration;
using rEFInd_Automenu.Configuration.LoaderParsers;
using rEFInd_Automenu.ConsoleApplication.ConsoleInterface;
using rEFInd_Automenu.ConsoleApplication.WorkerMethodsImplementations;
using rEFInd_Automenu.Extensions;
using rEFInd_Automenu.RuntimeConfiguration;
using System.Management;
using System.Runtime.InteropServices;

namespace rEFInd_Automenu.ConsoleApplication.FunctionalWorkers
{
    public static class InstallArgumentsWorker
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(InstallArgumentsWorker));

        public static void Execute(InstallArgumentsInfo argumentsInfo)
        {
            // --Computer
            if (argumentsInfo.OnComputer)
            {
                InstallOnComputer(argumentsInfo);
                return;
            }

            // --FlashDrive
            if (argumentsInfo.OnFlashDrive != null)
            {
                InstallToFlashDrive(argumentsInfo);
                return;
            }
        }

        private static void InstallOnComputer(InstallArgumentsInfo installArguments)
        {
            // Working
            object SyncLockObject = new object();
            ConsoleControllerCommands commands = ConsoleProgram.GetControllerCommands<ConsoleControllerCommands>(SyncLockObject);
            WorkerMethods methods = new WorkerMethods(commands);
            log.Info("Started installation on current computer");

            // Setting processor architecture
            FirmwareExecutableArchitecture Arch = FirmwareExecutableArchitecture.None;
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
            DirectoryInfo EspRefindDir = methods.CheckInstanceNonExisting(
                installArguments.ForceWork); // ForceWork

            // Getting resource archive
            methods.ObtainResourceArchive(
                installArguments.DownloadLatestBin, // DownloadLatest
                new Version(0, 0, 0));              // InstalledVersion

            // Resource stream containig an archive that extracting into temp directory
            DirectoryInfo BinArchiveDir = methods.ExtractLatestResourceArchive();

            // Moving binaries (loader and tools) to "refind" directory on ESP
            methods.MoveResourceBinaries(
                BinArchiveDir, // BinArchiveDir
                EspRefindDir,  // RefindInstallationDir
                Arch,          // Arch
                false);        // USB

            // Installing formaliztion theme
            string? formalizationThemePath = null;
            if (installArguments.Theme != null)
            {
                formalizationThemePath = methods.InstallFormalizationThemeDirectory(
                    installArguments.Theme, // ThemeDir
                    EspRefindDir);          // RefindInstallationDir
            }

            // Generating configuration file
            ConsoleProgram.Interface.Execute("Generating config file", commands, (ctrl) =>
            {
                ConfigurationFileBuilder configurationBuilder = new ConfigurationFileBuilder(EspRefindDir.FullName, formalizationThemePath);
                configurationBuilder.ConfigureStaticPlatform();

                configurationBuilder.AddWindowsMenuEntry();
                configurationBuilder.ParseConfigurationEntries(GetScanner(), Arch); // ProgramRegistry.PreferLoadersEspScan ? new EfiSystemPartitionLoaderScanner() : new FwBootmgrLoaderScanner()
                configurationBuilder.AssignLoaderIcons();

                configurationBuilder.WriteConfigurationToFile("refind.conf");
                log.Info("Config file succesfully generated");
            });

            // Configuring boot loader for rEFInd boot manager
            if (!ProgramConfiguration.Instance.PreferBootmgrBooting)
            {
                // Creating rEFInd boot option
                methods.CreateRefindFirmwareLoadOption(
                    false, // vverrideExisting
                    true,  // addFirst
                    Arch); // Arch
            }
            else
            {
                // Configuring bootmagr to loading rEFInd
                methods.ConfigureBootmgrBootEntry(
                    Arch); // Arch
            }
        }

        private static void InstallToFlashDrive(InstallArgumentsInfo installArguments)
        {
            object SyncLockObject = new object();
            ConsoleControllerCommands commands = ConsoleProgram.GetControllerCommands<ConsoleControllerCommands>(SyncLockObject);
            WorkerMethods methods = new WorkerMethods(commands);

            if (installArguments.OnFlashDrive == null)
                throw new ArgumentNullException(nameof(installArguments.OnFlashDrive));

            log.InfoFormat("Started installation on Flash drive {0}", installArguments.OnFlashDrive.Name);

            // Checking force architecture param
            if (installArguments.Architecture != FirmwareExecutableArchitecture.None)
            {
                // Using a specific architecture
                log.InfoFormat("Using a specific architecture : {0}", installArguments.Architecture);
            }

            // Checking drive info
            ConsoleProgram.Interface.Execute("Checking drive information", commands, (ctrl) =>
            {
                // Recomended working only on removable flash drive
                if (installArguments.OnFlashDrive.DriveType != DriveType.Removable)
                {
                    log.Error("Working drive is not removable");
                    log.Warn("Recomended stop work to prevent loss of data");

                    if (!installArguments.ForceWork)
                    {
                        ctrl.Error("Working drive is not removable");
                        log.Error("No force work. Stoping installation");
                    }
                    else
                    {
                        // Force work
                        log.Warn("Force working. Continue installation on drive");
                    }
                }

                // Checking drives file system, almost all UEFI based systems only working eith FAT32 partitions
                if (installArguments.OnFlashDrive.DriveFormat != "FAT32" && !installArguments.FormatDrive)
                {
                    log.Info("Drive has invalid file system. UEFI does not recognize non FAT32 fs partitions");
                    ctrl.Error("Invalid file system. Format required", true);
                }
            });

            if (installArguments.FormatDrive && installArguments.OnFlashDrive.DriveFormat != "FAT32")
            {
                ConsoleProgram.Interface.Execute("Formating drive FAT32", commands, (ctrl) =>
                {
                    // Formating drive to FAT32
                    log.Info("Formatting drive - fs FAT32, Cluster 8192, Fast");
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher(string.Concat("select * from Win32_Volume WHERE DriveLetter = \'", installArguments.OnFlashDrive.Name.AsSpan(0, 2), "\'"));
                    foreach (ManagementObject vi in searcher.Get().Cast<ManagementObject>())
                    {
                        vi.InvokeMethod("Format", new object[]
                        {
                            "FAT32",
                            true,
                            8192,
                            installArguments.OnFlashDrive.VolumeLabel ?? "bootable",
                            false
                        });
                    }

                    ctrl.Success("Formatted");
                });
            }

            // Checking EFI-Boot directory existing? or creating it
            DirectoryInfo EfiBootDir = ConsoleProgram.Interface.ExecuteAndReturn<DirectoryInfo>("Checking EFI-Boot directory", commands, (ctrl) =>
            {
                DirectoryInfo EfiBootDir = installArguments.OnFlashDrive.RootDirectory.GetSubDirectory("EFI\\Boot");
                if (!EfiBootDir.Exists)
                {
                    EfiBootDir.Create();
                    log.InfoFormat("EFI boot directory was created {0}", EfiBootDir.FullName);
                    return ctrl.Success("Created", EfiBootDir);
                }

                if (EfiBootDir.EnumerateFileSystemInfos().Any())
                {
                    if (installArguments.ForceWork)
                    {
                        log.Warn("Boot directory already has files");
                        log.Info("Trying to empty directory");
                        EfiBootDir.Empty();

                        log.Info("EFI-Boot directory is empty now");
                        return ctrl.Success("Cleaned", EfiBootDir);
                    }
                    else
                    {
                        log.Error("Boot directory already has files");
                        return ctrl.Error("Boot directory already has files");
                    }
                }

                return ctrl.Success("Exist", EfiBootDir);
            });

            // Getting resource archive
            methods.ObtainResourceArchive(
                installArguments.DownloadLatestBin, // DownloadLatest
                new Version(0, 0, 0));              // InstalledVersion

            // Resource stream containig an archive that extracting into temp directory
            DirectoryInfo BinArchiveDir = methods.ExtractLatestResourceArchive();

            // Moving binaries (loader and tools) to "refind" directory on ESP
            methods.MoveResourceBinaries(
                BinArchiveDir,                 // BinArchiveDir
                EfiBootDir,                    // RefindInstallationDir
                installArguments.Architecture, // Arch
                true);                         // USB

            // Installing formaliztion theme
            string? formalizationThemePath = null;
            if (installArguments.Theme != null)
            {
                formalizationThemePath = methods.InstallFormalizationThemeDirectory(
                    installArguments.Theme, // ThemeDir
                    EfiBootDir);          // RefindInstallationDir
            }

            // Generating configuration file
            ConsoleProgram.Interface.Execute("Generating config file", commands, (ctrl) =>
            {
                ConfigurationFileBuilder configurationBuilder = new ConfigurationFileBuilder(EfiBootDir.FullName, formalizationThemePath);
                configurationBuilder.ConfigureDynamicPlatform();
                configurationBuilder.WriteConfigurationToFile("refind.conf");
                log.Info("Config file succesfully generated");
            });
        }

        private static ILoadersScanner GetScanner() => ProgramConfiguration.Instance.LoaderScannerType switch
        {
            LoaderScannerType.NvramLoadOptionReader => new FirmwareLoadOptionsScanner(),
            LoaderScannerType.EspDirectoryEnumerator => new EfiSystemPartitionLoaderScanner(),
            LoaderScannerType.FwBootmgrRecordParser => new FwBootmgrLoaderScanner(),
            _ => new FirmwareLoadOptionsScanner()
        };
    }
}
