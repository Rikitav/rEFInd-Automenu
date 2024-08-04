using log4net;
using rEFInd_Automenu.Booting;
using rEFInd_Automenu.Configuration;
using rEFInd_Automenu.Configuration.LoaderParsers;
using rEFInd_Automenu.ConsoleApplication.ConsoleInterface;
using rEFInd_Automenu.ConsoleApplication.WorkerMethodsImplementations;
using rEFInd_Automenu.Extensions;
using rEFInd_Automenu.Installation;
using Rikitav.IO.ExtensibleFirmware.SystemPartition;
using System.Diagnostics;

namespace rEFInd_Automenu.ConsoleApplication.FunctionalWorkers
{
    public class InstanceArgumentsWorker
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(InstanceArgumentsWorker));

        public static void Execute(InstanceArgumentsInfo argumentsInfo)
        {
            if (argumentsInfo.ShowInfo)
            {
                ShowInstanceInfo();
                return;
            }

            if (argumentsInfo.RemoveBin)
            {
                RemoveInstance();
                return;
            }

            if (argumentsInfo.UpdateBin)
            {
                UpdateInstanceBin();
                return;
            }

            if (argumentsInfo.OpenConfig)
            {
                OpenInstanceConfig();
                return;
            }

            if (argumentsInfo.RegenBoot)
            {
                RegenerateBootEntry();
                return;
            }

            if (argumentsInfo.RegenConf)
            {
                RegenerateConfigFile();
                return;
            }
        }

        private static void ShowInstanceInfo()
        {
            // Setting commands
            object SyncLockObject = new object();
            ConsoleControllerCommands commands = ConsoleProgram.GetControllerCommands<ConsoleControllerCommands>(SyncLockObject);
            WorkerMethods methods = new WorkerMethods(commands);

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = methods.CheckInstanceExisting();

            string[] values = new string[3];
            ConsoleProgram.Interface.Execute("Getting instance information", commands, (ctrl) =>
            {
                // Getting loader version
                RefindInstanceInfo? instanceInfo = RefindInstanceInfo.Read(EspRefindDir.FullName);
                values[0] = instanceInfo == null
                    ? "<NULL>" // null value
                    : instanceInfo.Value.LoaderVersion.ToString();

                // Getting formalization theme existing
                bool themeExisting = EspRefindDir.GetSubDirectory("theme").Exists;
                values[1] = themeExisting.ToString();

                // Getting loader architecture
                string loaderFileName = EspRefindDir.EnumerateFiles("refind_*.efi").First().Name;
                values[2] = new EfiExecutableInfo("refind", loaderFileName).Architecture.ToString();
            });

            Console.WriteLine();
            ConsoleInterfaceWriter.MessageOffset = "[ INFO ] Loader architecture".Length;

            ConsoleInterfaceWriter.WriteInformation("Loader version", values[0]);
            ConsoleInterfaceWriter.WriteInformation("Theme existing", values[1]);
            ConsoleInterfaceWriter.WriteInformation("Loader architecture", values[2]);

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
            if (File.Exists(LoaderPath))
                File.Delete(LoaderPath);

            methods.ConfigureBootmgrBootEntry(LoaderPath.Substring(LoaderPath.IndexOf("EFI")));
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

                // Writing
                configurationBuilder.WriteConfigurationToFile(EfiConfFile);
                log.Info("Config file succesfully regenerated");
            });
        }
    }
}
