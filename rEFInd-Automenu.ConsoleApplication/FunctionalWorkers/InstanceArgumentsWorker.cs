using log4net;
using rEFInd_Automenu.Booting;
using rEFInd_Automenu.Configuration;
using rEFInd_Automenu.Configuration.LoaderParsers;
using rEFInd_Automenu.Configuration.Parsing;
using rEFInd_Automenu.ConsoleApplication.ConsoleInterface;
using rEFInd_Automenu.ConsoleApplication.WorkerMethodsImplementations;
using rEFInd_Automenu.Extensions;
using rEFInd_Automenu.Installation;
using rEFInd_Automenu.RuntimeConfiguration;
using Rikitav.IO.ExtensibleFirmware.SystemPartition;
using System.Diagnostics;

namespace rEFInd_Automenu.ConsoleApplication.FunctionalWorkers
{
    public static class InstanceArgumentsWorker
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(InstanceArgumentsWorker));

        public static void Execute(InstanceArgumentsInfo argumentsInfo)
        {
            // --ShowInfo
            if (argumentsInfo.ShowInfo)
            {
                ShowInstanceInfo();
                return;
            }

            // --Remove
            if (argumentsInfo.RemoveBin)
            {
                RemoveInstance();
                return;
            }

            // --Update
            if (argumentsInfo.UpdateBin)
            {
                UpdateInstanceBin();
                return;
            }

            // --OpenConfig
            if (argumentsInfo.OpenConfig)
            {
                OpenInstanceConfig();
                return;
            }

            // --InstallTheme
            if (argumentsInfo.InstallTheme != null)
            {
                InstallFormalizationTheme(argumentsInfo);
                return;
            }

            // --RegenBoot
            if (argumentsInfo.RegenBoot)
            {
                RegenerateBootEntry();
                return;
            }

            // --RegenConf
            if (argumentsInfo.RegenConf)
            {
                RegenerateConfigFile();
                return;
            }
        }

        public static void InstallFormalizationTheme(InstanceArgumentsInfo argumentsInfo)
        {
            // Setting commands
            object SyncLockObject = new object();
            ConsoleControllerCommands commands = ConsoleProgram.GetControllerCommands<ConsoleControllerCommands>(SyncLockObject);
            WorkerMethods methods = new WorkerMethods(commands);

            if (argumentsInfo.InstallTheme == null)
                throw new ArgumentNullException(nameof(argumentsInfo.InstallTheme));

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = methods.CheckInstanceExisting();

            // Removing theme if exist
            if (EspRefindDir.GetSubDirectory("theme").Exists)
            {
                ConsoleProgram.Interface.Execute("Removing old theme", commands, (ctrl) =>
                {
                    try
                    {
                        log.Warn("Theme directory is already exist. Directory will be deleted");
                        EspRefindDir.GetSubDirectory("theme").Delete(true);
                        log.Info("OLD Theme directory was deleted");
                    }
                    catch (Exception exc)
                    {
                        log.Error("Failed to delete old formalization theme directory", exc);
                        ctrl.Error("Failed to delete old formalization theme directory");
                    }
                });
            }

            // Installing formaliztion theme
            string formalizationThemePath = methods.InstallFormalizationThemeDirectory(
                argumentsInfo.InstallTheme, // ThemeDir, it CAN'T be null
                EspRefindDir);              // RefindInstallationDir

            // Correcting config file to have\add 'include' keyword
            ConsoleProgram.Interface.Execute("Config correction", commands, (ctrl) =>
            {
                // Getting config file path
                string configFilePath = Path.Combine(EspRefindDir.FullName, "refind.conf");

                // Parsing configuration information from file
                log.Info("Parsing config file");
                RefindConfiguration conf = ConfigurationFileParser.FromFile(configFilePath);

                // Modifying config
                log.Info("Modifying configuration information");
                conf.Global.Include = @"theme/theme.conf";

                // reassigning menu netry icons
                ConfigurationFileBuilder configurationBuilder = new ConfigurationFileBuilder(conf, EspRefindDir.FullName, formalizationThemePath);
                configurationBuilder.AssignLoaderIcons();

                // Writing updated config file
                log.Info("Writing updated configuration to file");
                configurationBuilder.WriteConfigurationToFile("refind.conf");
            });
        }

        private static void ShowInstanceInfo()
        {
            // Setting commands
            object SyncLockObject = new object();
            ConsoleControllerCommands commands = ConsoleProgram.GetControllerCommands<ConsoleControllerCommands>(SyncLockObject);
            WorkerMethods methods = new WorkerMethods(commands);

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = methods.CheckInstanceExisting();

            string[] values = new string[4];
            ConsoleProgram.Interface.Execute("Getting instance information", commands, (ctrl) =>
            {
                // Getting efi system partition path
                values[0] = EspFinder.EspDirectory.FullName;

                // Getting loader version
                RefindInstanceInfo? instanceInfo = RefindInstanceInfo.Read(EspRefindDir.FullName);
                values[1] = instanceInfo == null
                    ? "<NULL>" // null value
                    : instanceInfo.Value.LoaderVersion.ToString();

                // Getting formalization theme existing
                bool themeExisting = EspRefindDir.GetSubDirectory("theme").Exists;
                values[2] = themeExisting.ToString();

                // Getting loader architecture
                string loaderFileName = EspRefindDir.EnumerateFiles("refind_*.efi").First().Name;
                values[3] = new EfiExecutableInfo("refind", loaderFileName).Architecture.ToString();
            });

            Console.WriteLine();
            ConsoleInterfaceWriter.MessageOffset = "[ INFO ] Loader architecture".Length;

            ConsoleInterfaceWriter.WriteInformation("ESP path", " - " + values[0]);
            ConsoleInterfaceWriter.WriteInformation("Loader version", " - " + values[1]);
            ConsoleInterfaceWriter.WriteInformation("Theme existing", " - " + values[2]);
            ConsoleInterfaceWriter.WriteInformation("Loader architecture", " - " + values[3]);

            ConsoleInterfaceWriter.ResetOffset();
        }

        private static void UpdateInstanceBin()
        {
            // Setting commands
            object SyncLockObject = new object();
            ConsoleControllerCommands commands = ConsoleProgram.GetControllerCommands<ConsoleControllerCommands>(SyncLockObject);
            WorkerMethods methods = new WorkerMethods(commands);

            // Working
            EnvironmentArchitecture Arch = ArchitectureInfo.Current;
            log.Info("Updating instance bin (-u flag)");

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = methods.CheckInstanceExisting();

            // Get version of installed instance if info file exists
            Version InstalledVersion = new Version(0, 0, 0);
            string InfoJsonPath = Path.Combine(EspRefindDir.FullName, "info.json");
            if (File.Exists(InfoJsonPath))
            {
                object? TmpObj = RefindInstanceInfo.Read(EspRefindDir.FullName)?.LoaderVersion;
                if (TmpObj != null)
                    InstalledVersion = (Version)TmpObj;
            }

            // Getting resource archive
            methods.ObtainResourceArchive(
                true,              // DownloadLatest
                InstalledVersion); // InstalledVersion

            // Resource stream containig an archive that extracting into temp directory
            DirectoryInfo BinArchiveDir = methods.ExtractLatestResourceArchive();

            // Moving binaries (loader and tools) to "refind" directory on ESP
            methods.MoveResourceBinaries(
                BinArchiveDir, // BinArchiveDir
                EspRefindDir,  // RefindInstallationDir
                Arch,          // Arch
                false);        // USB
        }

        private static void RemoveInstance()
        {
            // Setting commands
            object SyncLockObject = new object();
            ConsoleControllerCommands commands = ConsoleProgram.GetControllerCommands<ConsoleControllerCommands>(SyncLockObject);
            WorkerMethods methods = new WorkerMethods(commands);

            // Working
            log.Info("Removing rEFInd (-r flag)");

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = methods.CheckInstanceExisting();

            // Deleting "refind" directory on ESP
            ConsoleProgram.Interface.Execute("Removing binaries", commands, (ctrl) =>
            {
                EspRefindDir.Delete(true);
                log.Info("rEFInd was removed from this computer");
            });

            // Removing boot option
            if (!RegistryExplorer.PreferBootmgrBooting)
                methods.DeleteRefindFirmwareLoadOption();
        }

        private static void OpenInstanceConfig()
        {
            // Setting commands
            object SyncLockObject = new object();
            ConsoleControllerCommands commands = ConsoleProgram.GetControllerCommands<ConsoleControllerCommands>(SyncLockObject);
            WorkerMethods methods = new WorkerMethods(commands);

            // Working
            log.Info("Opening current instance config file (-c flag)");

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = methods.CheckInstanceExisting();

            // Opening config file "refind.conf" on ESP directory "refind"
            ConsoleProgram.Interface.Execute("Opening config file", commands, (ctrl) =>
            {
                string RefindConf = Path.Combine(EspRefindDir.FullName, "refind.conf");
                if (!File.Exists(RefindConf))
                {
                    log.Fatal("rEFInd.conf is not found");
                    ctrl.Error("rEFInd.conf is not found", true);
                    return;
                }

                Process.Start(new ProcessStartInfo()
                {
                    FileName = RefindConf,
                    UseShellExecute = true
                });
            });
        }

        private static void RegenerateBootEntry()
        {
            // Setting commands
            object SyncLockObject = new object();
            ConsoleControllerCommands commands = ConsoleProgram.GetControllerCommands<ConsoleControllerCommands>(SyncLockObject);
            WorkerMethods methods = new WorkerMethods(commands);

            // Working
            log.Info("Regenerating current rEFInd instance booting entry (--regenboot flag)");

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = methods.CheckInstanceExisting();

            // Setting new loader information
            string LoaderPath = EspRefindDir.EnumerateFiles("refind_*.efi").First().FullName;
            if (!File.Exists(LoaderPath))
            {
                log.Error("rEFInd bootloader not found");
                ConsoleInterfaceWriter.WriteError(Console.CursorTop, nameof(RegenerateBootEntry), "rEFInd bootloader not found, please reinstall boot manager");
                return;
            }

            // Getting Arch
            EnvironmentArchitecture Arch = ArchitectureInfo.FromPostfix(Path.GetFileNameWithoutExtension(LoaderPath).Substring("refind_".Length));
            if (Arch == EnvironmentArchitecture.None)
            {
                log.Warn("Could not find out the bootloader architecture");
                ConsoleInterfaceWriter.WriteError(Console.CursorTop, nameof(RegenerateBootEntry), "Could not find out the bootloader architecture");
                return;
            }

            // Configuring boot loader for rEFInd boot manager
            if (!RegistryExplorer.PreferBootmgrBooting)
            {
                // Creating rEFInd boot option
                methods.CreateRefindFirmwareLoadOption(
                    false, // overrideExisting
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

        private static void RegenerateConfigFile()
        {
            // Setting commands
            object SyncLockObject = new object();
            ConsoleControllerCommands commands = ConsoleProgram.GetControllerCommands<ConsoleControllerCommands>(SyncLockObject);
            WorkerMethods methods = new WorkerMethods(commands);

            // Working
            log.Info("Regenerating current rEFInd instance configuration file (-regenconf flag)");

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = methods.CheckInstanceExisting();

            // Generating configuration file
            ConsoleProgram.Interface.Execute("Generating config file", commands, (ctrl) =>
            {
                string EfiConfFile = Path.Combine(EspRefindDir.FullName, "refind.conf");
                if (File.Exists(EfiConfFile))
                    File.Delete(EfiConfFile);

                DirectoryInfo EspThemeDir = EspRefindDir.GetSubDirectory("theme");
                ConfigurationFileBuilder configurationBuilder = new ConfigurationFileBuilder(
                    EspRefindDir.FullName,
                    EspThemeDir.Exists ? EspThemeDir.FullName : string.Empty);

                // Configuring global info
                configurationBuilder.ConfigureStaticPlatform();

                // Configuring menu entries
                configurationBuilder.AddWindowsMenuEntry();
                configurationBuilder.ParseConfigurationEntries(new FwBootmgrLoaderScanner(), ArchitectureInfo.Current);
                configurationBuilder.AssignLoaderIcons();

                // Writing
                configurationBuilder.WriteConfigurationToFile(EfiConfFile);
                log.Info("Config file succesfully regenerated");
            });
        }
    }
}
