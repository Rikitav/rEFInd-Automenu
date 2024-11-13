using rEFInd_Automenu.ConsoleApplication.ConsoleInterface;
using rEFInd_Automenu.Resources;

namespace rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.Obtain
{
    public static partial class ProcedureProcessor_Obtain
    {
        private static void DownloadResourceArchive(string VersionString)
        {
            // Working
            log.Info("Started downloading resource archive (Obtain --Download flag)");

            // getting latest available version
            Version LatestVerInfo = WebResourceManager.GetLatestVersion().Result;
            log.InfoFormat("Latest posible version is {0}", LatestVerInfo);

            Version TargetVersion = ConsoleProgram.Interface.ExecuteAndReturn<Version>("Checking version availablity", ConsoleProgram.CommonCommands, (ctrl) =>
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

                    string TestUri = string.Format(@"https://sourceforge.net/projects/refind/files/{0}", ParsedVersion);
                    if (!WebResourceManager.CheckWebpageAvailablity(TestUri).Result)
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

            using Stream ArchiveStream = ConsoleProgram.Interface.ExecuteAndReturn<Stream>("Downloading archive", ConsoleProgram.ProgressCommands, (ctrl) =>
            {
                ConsoleControllerProgressBarCommands progressCommands = (ConsoleControllerProgressBarCommands)ctrl.ExecutingCommand;

                try
                {
                    // Downloading by version
                    log.InfoFormat("Downloading resource archive of vresion : {0}", TargetVersion);
                    ResourceObtaining.GetMethodForVersion(TargetVersion);
                    return WebResourceManager.DownloadArchiveByVersionWithProgress(TargetVersion, progressCommands.KernelProgressBar, length => progressCommands.KernelProgressBar.MaxValue = (int)length).Result;
                }
                catch (Exception exc)
                {
                    log.Error("WebResourceManager.DownloadArchiveByVersion FAILED", exc);
                    return ctrl.Error(exc, true);
                }
            });

            ConsoleProgram.Interface.Execute("Flushing data to file", ConsoleProgram.CommonCommands, (ctrl) =>
            {
                if (ArchiveStream == null)
                {
                    // ArchiveStream eas null or empty
                    log.Error("The received data is empty");
                    ctrl.Error("The received data is empty", true);
                    return;
                }

                // Flushing data
                string OutputFile = LocalResourceManager.SaveBinArchive(TargetVersion, ArchiveStream).Result;

                log.Info("Resource archive succesfully created");
                ctrl.Success(string.Format("rEFInd-bin-{0}.zip", TargetVersion));
            });

        }
    }
}
