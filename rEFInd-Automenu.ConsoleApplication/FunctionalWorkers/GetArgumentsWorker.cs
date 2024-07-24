using log4net;
using rEFInd_Automenu.Booting;
using rEFInd_Automenu.Configuration;
using rEFInd_Automenu.Configuration.LoaderParsers;
using rEFInd_Automenu.ConsoleApplication.ConsoleInterface;
using rEFInd_Automenu.ConsoleApplication.WorkerMethodsImplementations;
using rEFInd_Automenu.Resources;
using Rikitav.IO.ExtensibleFirmware.SystemPartition;
using System.Runtime.InteropServices;
using System.Text;

namespace rEFInd_Automenu.ConsoleApplication.FunctionalWorkers
{
    public class GetArgumentsWorker
    {
        private static ILog log = LogManager.GetLogger(typeof(GetArgumentsWorker));

        public static void Execute(GetArgumentsInfo argumentsInfo)
        {
            // --Resource
            if (argumentsInfo.ExtractResourceArchive)
            {
                ExtractResourceArchive();
                return;
            }

            // --Archive
            if (argumentsInfo.DownloadSourceArchive != null)
            {
                DownloadResourceArchive(argumentsInfo.DownloadSourceArchive);
                return;
            }

            // --MountESP
            if (argumentsInfo.MountEsp)
            {
                MountEsp();
                return;
            }
        }

        /*
        private static void GenerateConfigFile()
        {
            // Setting commands
            ConsoleControllerCommands commands = ConsoleProgram.GetControllerCommands<ConsoleControllerCommands>();

            // Working
            log.Info("Generating config file (-c flag)");

            // Generating configuration file
            ConsoleProgram.Interface.Execute("Generating config file", commands, (ctrl) =>
            {
                RefindConfiguration configuration = new RefindConfiguration();
                configuration.ConfigureStaticPlatform();
                configuration.ParseConfigurationEntries(new FwBootmgrLoaderScanner(), ArchitectureInfo.Current);
                configuration.WriteConfigurationToFile(Path.Combine(Environment.CurrentDirectory, "refind.conf"));
                log.Info("Config file succesfully generated");
            });

        }
        */

        private static void ExtractResourceArchive()
        {
            // Setting commands
            ConsoleControllerCommands commands = ConsoleProgram.GetControllerCommands<ConsoleControllerCommands>();

            // Setting variables
            string FileName = string.Format("rEFInd-bin-{0}.zip", EmbeddedResourceManager.rEFIndBin_VersionInResources.ToString());
            string OutputFile = Path.Combine(Environment.CurrentDirectory, FileName);

            // Working
            log.Info("Extracting resource archive (-r flag)");

            // Extracting embedded resource archive
            ConsoleProgram.Interface.Execute("Extracting resource archive", commands, (ctrl) =>
            {
                if (File.Exists(OutputFile))
                {
                    // Archive with this version or name already exists
                    ctrl.Warning("Archive already exists");
                    log.Warn("Archive already exists");
                    return;
                }

                // Opening resource stream and cretaing new file to write
                using Stream ResArchiveStream = EmbeddedResourceManager.GetArchiveStream().Result;
                if (ResArchiveStream == null)
                {
                    // Archive with this version or name already exists
                    ctrl.Error("ResArchiveStream is null");
                    log.Warn("ResArchiveStream is null");
                    return;
                }

                using FileStream file = File.Create(OutputFile);
                ResArchiveStream.CopyTo(file);

                log.Info("Resource archive succesfully extracted");
                ctrl.Success(FileName);
            });
        }

        private static void DownloadResourceArchive(string VersionString)
        {
            // Setting commands
            object SyncLockObject = new object();
            ConsoleControllerProgressBarCommands progressCommands = ConsoleProgram.GetControllerCommands<ConsoleControllerProgressBarCommands>(SyncLockObject);
            ConsoleControllerCommands commands = ConsoleProgram.GetControllerCommands<ConsoleControllerCommands>(SyncLockObject);

            // Working
            log.Info("Downloading resource archive (-a flag)");

            // getting latest available version
            Version LatestVerInfo = WebResourceManager.GetLatestVersion().Result;
            log.InfoFormat("Latest posible version is {0}", LatestVerInfo);

            Version TargetVersion = ConsoleProgram.Interface.ExecuteAndReturn<Version>("Checking version availablity", commands, (ctrl) =>
            {
                if (string.IsNullOrWhiteSpace(VersionString) || VersionString.Equals("latest", StringComparison.CurrentCultureIgnoreCase))
                {
                    log.Info("Will be downloaded latest posible version");
                    return ctrl.Success(LatestVerInfo.ToString(), LatestVerInfo);
                }
                else if (Version.TryParse(VersionString, out Version? ParsedVersion))
                {
                    log.InfoFormat("Argumented version is {0}", ParsedVersion);
                    if (ParsedVersion > LatestVerInfo)
                    {
                        // Passed version is higher than available
                        log.Error("The specified version number is higher than available");
                        return ctrl.Error("The specified version number is higher than available. Latest version is " + LatestVerInfo, true);
                    }

                    using HttpClient TesterClient = new HttpClient();
                    string TestUri = string.Format(@"https://sourceforge.net/projects/refind/files/{0}", ParsedVersion);
                    if (!TesterClient.GetAsync(TestUri).Result.IsSuccessStatusCode)
                    {
                        // Passed version is higher than available
                        log.Error("The specified version number was not found in the repository");
                        return ctrl.Error("The specified version number was not found in the repository", true);
                    }

                    return ctrl.Success(ParsedVersion.ToString(), ParsedVersion);
                }
                else
                {
                    log.Error("Failed to parse version passed in arguments");
                    return ctrl.Error("Failed to parse version passed in arguments", true);
                }
            });

            string FileName = string.Format("rEFInd-bin-{0}.zip", TargetVersion);
            string OutputFile = Path.Combine(Environment.CurrentDirectory, FileName);

            using Stream ArchiveStream = ConsoleProgram.Interface.ExecuteAndReturn<Stream>("Downloading archive", progressCommands, (ctrl) =>
            {
                if (File.Exists(OutputFile))
                {
                    // Archive with this version or name already exists
                    log.Warn("Archive already exists");
                    return ctrl.Error("Archive already exists", true);
                }

                try
                {
                    // Downloading by version
                    log.InfoFormat("Downloading resource archive of vresion : {0}", TargetVersion);
                    return WebResourceManager.DownloadArchiveByVersion(TargetVersion).Result;
                }
                catch (Exception exc)
                {
                    log.Error("WebResourceManager.DownloadArchiveByVersion FAILED", exc);
                    return ctrl.Error(exc, true);
                }
            });

            ConsoleProgram.Interface.Execute("Flushing data to file", commands, (ctrl) =>
            {
                if (ArchiveStream == null)
                {
                    // ArchiveStream eas null or empty
                    log.Error("The received data is empty");
                    ctrl.Error("The received data is empty", true);
                    return;
                }

                // Flushing data
                using FileStream file = File.Create(OutputFile);
                ArchiveStream.CopyTo(file);

                log.Info("Resource archive succesfully created");
                ctrl.Success(FileName);
            });
        }

        private static void MountEsp()
        {
            // Working
            ConsoleControllerCommands commands = ConsoleProgram.GetControllerCommands<ConsoleControllerCommands>();
            WorkerMethods methods = new WorkerMethods(commands);

            ConsoleProgram.Interface.Execute("Mounting ESP", commands, (ctrl) =>
            {
                // Getting ESP path
                string EspGuid = EfiPartition.GetFullPath();
                log.InfoFormat("Efi system partition - {0}", EspGuid);

                // Starting
                log.Info("Mounting EFI System Partition");
                if (NativeMethods.SetVolumeMountPointW("S:\\", EspGuid))
                {
                    log.Info("Successfully mounted ESP to S:\\");
                    ctrl.Success("Mounted");
                }

                // Failed to mount
                int lastError = Marshal.GetLastWin32Error();
                log.Info("Error was occured at volume point mounting");

                if (lastError != 0x91) // ERROR_DIR_NOT_EMPTY
                {
                    // Unknown error
                    log.ErrorFormat("Failed to mount EFI system partition. Win32 error {0}", lastError);
                    ctrl.Error("Failed to mount EFI system partition");
                }

                // Drive already have volume name 
                log.Info("LastError was equal to ERROR_DIR_NOT_EMPTY. Already have link");

                // Trying to get VolumeName for mounted S:\
                StringBuilder GuidBuilder = new StringBuilder(50);
                if (!NativeMethods.GetVolumeNameForVolumeMountPointW("S:\\", GuidBuilder, GuidBuilder.Capacity))
                {
                    // Failed get VolumeName
                    lastError = Marshal.GetLastWin32Error();
                    log.ErrorFormat("Failed to get volume GUID for mount point S:\\. Win32 error {0}", lastError);
                    ctrl.Error("Failed to mount EFI system partition.");
                }

                // Comparing ESP guid and S:\ guid
                if (EspGuid != GuidBuilder.ToString())
                {
                    // Not ESP's guid. Fault
                    log.ErrorFormat("\"S:\\\" drive already have another VolumeName - {0}", GuidBuilder.ToString());
                    ctrl.Error("S:\\ drive already attached as another partition");
                }
                else
                {
                    // ESP's guid. Unmounting
                    commands.ChangeLabel("Unmounting ESP");
                    NativeMethods.DeleteVolumeMountPointW("S:\\");
                    log.Info("Successfully unmounted ESP");
                    ctrl.Success("Unmounted");
                }
            });
        }

        private static class NativeMethods
        {
            [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool SetVolumeMountPointW(string lpszVolumeMountPoint, string lpszVolumeName);

            [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool DeleteVolumeMountPointW(string lpszVolumeMountPoint);

            [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern bool GetVolumeNameForVolumeMountPointW(string lpszVolumeMountPoint, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpszVolumeName, int cchBufferLength);
        }
    }
}
