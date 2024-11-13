using rEFInd_Automenu.Booting;
using rEFInd_Automenu.Configuration;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Configuration;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Installing;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Resources;
using rEFInd_Automenu.TypesExtensions;
using System.Management;

namespace rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.Install
{
    public static partial class ProcedureProcessor_Install
    {
        public static void OnFlashDrive(InstallArgumentsInfo installArguments)
        {
            // Working
            log.InfoFormat("Started installation on Flash drive {0} (\'Install --FlashDrive\' flag)", installArguments.OnFlashDrive?.Name ?? "<NULL>");

            if (installArguments.OnFlashDrive == null)
                throw new ArgumentNullException(nameof(installArguments.OnFlashDrive), "\"Target flash drive\" argument is null or empty");

            if (!installArguments.OnFlashDrive.IsReady)
                throw new ArgumentNullException(nameof(installArguments.OnFlashDrive), "\"Target flash drive\" is not ready");

            // Checking force architecture param
            if (installArguments.Architecture != FirmwareExecutableArchitecture.None)
            {
                // Using a specific architecture
                log.InfoFormat("Using a specific architecture : {0}", installArguments.Architecture);
            }

            // Checking drive info
            ConsoleProgram.Interface.Execute("Checking drive information", ConsoleProgram.CommonCommands, (ctrl) =>
            {
                // Recomended working only on removable flash drive
                if (installArguments.OnFlashDrive.DriveType != DriveType.Removable)
                {
                    log.Error("Working drive is not removable");
                    log.Warn("Recomended stop work to prevent loss of data");

                    if (!installArguments.ForceWork)
                    {
                        ctrl.Error("Working drive is not removable", true);
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
                ConsoleProgram.Interface.Execute("Formating drive FAT32", ConsoleProgram.CommonCommands, (ctrl) =>
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
            DirectoryInfo EfiBootDir = ConsoleProgram.Interface.ExecuteAndReturn<DirectoryInfo>("Checking EFI-Boot directory", ConsoleProgram.CommonCommands, (ctrl) =>
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
                        return ctrl.Error("Boot directory already has files", true);
                    }
                }

                return ctrl.Success("Exist", EfiBootDir);
            });

            ConfigurationFileBuilder configurationBuilder = ConfigurationOperations.GetConfigurationFileBuilder(
                EfiBootDir, // refindDir
                true);      // IsDynamicPlatform

            // Getting resource archive
            ResourcesOperations.ObtainResourceArchive(
                installArguments.DownloadLatestBin, // DownloadLatest
                new Version(0, 0, 0));              // InstalledVersion

            // Resource stream containig an archive that extracting into temp directory
            DirectoryInfo BinArchiveDir = ResourcesOperations.ExtractLatestResourceArchive();

            // Moving binaries (loader and tools) to "refind" directory on ESP
            InstallingOperations.MoveResourceBinaries(
                BinArchiveDir,                 // BinArchiveDir
                EfiBootDir,                    // RefindInstallationDir
                installArguments.Architecture, // Arch
                true);                         // USB

            // Installing formaliztion theme
            DirectoryInfo? formalizationThemePath = null;
            if (installArguments.Theme != null)
            {
                formalizationThemePath = InstallingOperations.InstallFormalizationThemeDirectory(
                    installArguments.Theme, // ThemeDir
                    EfiBootDir);            // RefindInstallationDir
            }

            // Generating configuration file
            ConsoleProgram.Interface.Execute("Generating config file", ConsoleProgram.CommonCommands, (ctrl) =>
            {
                ConfigurationFileBuilder configurationBuilder = new ConfigurationFileBuilder(EfiBootDir.FullName, formalizationThemePath);
                configurationBuilder.ConfigureDynamicPlatform();
                configurationBuilder.WriteConfigurationToFile("refind.conf");
                log.Info("Config file succesfully generated");
            });
        }
    }
}
