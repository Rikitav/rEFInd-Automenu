using rEFInd_Automenu.ConsoleApplication.ConsoleInterface;
using rEFInd_Automenu.Resources;

namespace rEFInd_Automenu.ConsoleApplication.OperationsManager.Resources
{
    public static partial class ResourcesOperations
    {
        public static Stream ObtainResourceArchive(bool DownloadLatest, Version InstalledVersion) => ConsoleProgram.Interface.ExecuteAndReturn<Stream>("Getting resource archive", ConsoleProgram.ProgressCommands, (ctrl) =>
        {
            ConsoleControllerProgressBarCommands commands = (ConsoleControllerProgressBarCommands)ctrl.ExecutingCommand;
            log.Info("Deciding which method to obtain the resource archive");
            log.InfoFormat("Download latest - {0}", DownloadLatest);

            switch (ResourceObtaining.GetMethod(DownloadLatest, out ResourceObtaining.Warnings warnings))
            {
                case ResourceObtaining.Method.DownloadLatest:
                    {
                        // Downloading resource archive from Repository on SourceForge.com
                        commands.ChangeLabel("Downloading Archive".PadRight("Downloading resource archive".Length));

                        // Getting latest available version info
                        Version SourceForgeLatestResourceVersion = WebResourceManager.GetLatestVersion().Result;
                        log.InfoFormat("Bin archive is downloading from repository on SourceForge. Version : {0}", SourceForgeLatestResourceVersion);

                        // Checking version differences 
                        if (InstalledVersion >= SourceForgeLatestResourceVersion)
                        {
                            log.Warn("The installed instance is already the latest available version");
                            return ctrl.Warning("The installed instance is already the latest available version", Stream.Null, true);
                        }

                        // Downloading resource data
                        Stream TmpRes = WebResourceManager.DownloadArchiveByVersionWithProgress(SourceForgeLatestResourceVersion, commands.KernelProgressBar, length => commands.KernelProgressBar.MaxValue = (int)length).Result;
                        LocalResourceManager.SaveBinArchive(SourceForgeLatestResourceVersion, TmpRes).ExitWait();
                        
                        commands.KernelProgressBar.Value = 100;
                        if (warnings == ResourceObtaining.Warnings.None)
                        {
                            string SuccessMessage = string.Format("{0} downloaded", SourceForgeLatestResourceVersion.ToString());
                            return ctrl.Success(SuccessMessage, TmpRes);
                        }
                        else
                        {
                            string warningMessage = string.Format("{0} downloaded (reason : {1})", SourceForgeLatestResourceVersion.ToString(), warnings);
                            return ctrl.Warning(warningMessage, TmpRes);
                        }
                    }

                case ResourceObtaining.Method.ExtractEmbedded:
                    {
                        // Extraction resource archive from embedded resources of this applicaation
                        commands.ChangeLabel("Extracting Archive".PadRight("Getting embedded resource archive".Length));

                        // Getting latest available version info
                        Version EmbeddedResourceVersion = EmbeddedResourceManager.rEFIndBin_VersionInResources;
                        log.InfoFormat("Bin archive will be extracted from embedded resources. Version : {0}", EmbeddedResourceVersion);

                        // Checking version differences 
                        if (InstalledVersion >= EmbeddedResourceVersion)
                        {
                            log.Warn("The installed instance is already the latest available version");
                            return ctrl.Warning("The installed instance is already the latest available version", true);
                        }

                        // Downloading resource data
                        Stream TmpRes = EmbeddedResourceManager.GetArchiveStream().Result;
                        LocalResourceManager.SaveBinArchive(EmbeddedResourceVersion, TmpRes).ExitWait();

                        commands.KernelProgressBar.Value = 100;
                        if (warnings == ResourceObtaining.Warnings.None)
                        {
                            string SuccessMessage = string.Format("{0} from emdebbed", EmbeddedResourceVersion.ToString());
                            return ctrl.Success(SuccessMessage, TmpRes);
                        }
                        else
                        {
                            string warningMessage = string.Format("{0} from emdebbed (reason : {1})", EmbeddedResourceVersion.ToString(), warnings);
                            return ctrl.Warning(warningMessage, TmpRes);
                        }
                    }

                case ResourceObtaining.Method.LocalStorage:
                    {
                        // Resource archive stream will be opened frmo local storage in "%AppData%\rEFIndAutomenu\Versions" directory
                        commands.ChangeLabel("Extracting Archive".PadRight("Opening local resource archive".Length));

                        // Getting resource archive path with latest version saved in local storage
                        string LatestBinPath = LocalResourceManager.GetLatestBinArchiveFullPath(out Version? LocalLatestResourceVersion);
                        LocalLatestResourceVersion ??= new Version(0, 0);
                        log.InfoFormat("Bin archive is opening from local storage. Version : {0}", LocalLatestResourceVersion);

                        // No path, no local resources
                        if (string.IsNullOrEmpty(LatestBinPath))
                        {
                            log.Fatal("Local storage does not contain any resource archives");
                            return ctrl.Error("Local storage does not contain any resource archives", Stream.Null, true);
                        }

                        // Checking version differences 
                        if (InstalledVersion >= LocalLatestResourceVersion)
                        {
                            log.Warn("The installed instance is already the latest available version");
                            return ctrl.Warning("The installed instance is already the latest available version", Stream.Null, true);
                        }

                        // Opening for read
                        Stream TmpRes = LocalResourceManager.GetLatestBinArchiveStream(out _);

                        commands.KernelProgressBar.Value = 100;
                        if (warnings == ResourceObtaining.Warnings.None)
                        {
                            string SuccessMessage = string.Format("{0} from local", LocalLatestResourceVersion.ToString());
                            return ctrl.Success(SuccessMessage, TmpRes);
                        }
                        else
                        {
                            string warningMessage = string.Format("{0} from local (reason : {1})", LocalLatestResourceVersion.ToString(), warnings);
                            return ctrl.Warning(warningMessage, TmpRes);
                        }
                    }

                default:
                    {
                        log.Fatal("Failed to get resource archive");
                        return ctrl.Error("Failed to get resource archive", true);
                    }
            }
        });
    }
}
