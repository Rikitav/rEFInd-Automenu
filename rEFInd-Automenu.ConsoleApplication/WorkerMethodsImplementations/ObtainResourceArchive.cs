using rEFInd_Automenu.Resources;

namespace rEFInd_Automenu.ConsoleApplication.WorkerMethodsImplementations
{
    public partial class WorkerMethods
    {
        public void ObtainResourceArchive(bool DownloadLatest, Version InstalledVersion) => ConsoleProgram.Interface.Execute("Getting resource archive", commands, (ctrl) =>
        {
            log.Info("Deciding which method to obtain the resource archive");
            log.InfoFormat("Download latest - {0}", DownloadLatest);

            switch (ResourceObtaining.GetResource(DownloadLatest))
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
                            ctrl.Warning("The installed instance is already the latest available version");
                            return;
                        }

                        // Downloading resource data
                        Stream TmpRes = WebResourceManager.DownloadArchiveStream().Result;
                        LocalResourceManager.SaveBinArchive(SourceForgeLatestResourceVersion, TmpRes).Wait();
                        ctrl.Success(SourceForgeLatestResourceVersion.ToString() + "D");
                        return;
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
                            ctrl.Warning("The installed instance is already the latest available version");
                            return;
                        }

                        // Downloading resource data
                        Stream TmpRes = EmbeddedResourceManager.GetArchiveStream().Result;
                        LocalResourceManager.SaveBinArchive(EmbeddedResourceVersion, TmpRes).Wait();
                        ctrl.Success(EmbeddedResourceVersion.ToString() + "E");
                        return;
                    }

                case ResourceObtaining.Method.LocalStorage:
                    {
                        // Resource archive stream will be opened frmo local storage in "%AppData%\rEFIndAutomenu\Versions" directory
                        commands.ChangeLabel("Extracting Archive".PadRight("Opening local resource archive".Length));

                        // Getting resource archive path with latest version saved in local storage
                        string LatestBinPath = LocalResourceManager.GetLatestBinArchiveFullPath(out Version LocalLatestResourceVersion);
                        log.InfoFormat("Bin archive is opening from local storage. Version : {0}", LocalLatestResourceVersion);

                        // No path, no local resources
                        if (string.IsNullOrEmpty(LatestBinPath))
                        {
                            log.Fatal("Local storage does not contain any resource archives");
                            ctrl.Error("Local storage does not contain any resource archives", true);
                            return;
                        }

                        // Checking version differences 
                        if (InstalledVersion >= LocalLatestResourceVersion)
                        {
                            log.Warn("The installed instance is already the latest available version");
                            ctrl.Warning("The installed instance is already the latest available version", true);
                            return;
                        }

                        // Opening for read
                        ctrl.Success(LocalLatestResourceVersion?.ToString() + "L");
                        return;
                    }

                default:
                    {
                        log.Fatal("Failed to get resource archive");
                        ctrl.Error("Failed to get resource archive", true);
                        return;
                    }
            }
        });
    }
}
