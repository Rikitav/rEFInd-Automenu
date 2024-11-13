using rEFInd_Automenu.ConsoleApplication.ConsoleInterface;
using rEFInd_Automenu.Installation;
using rEFInd_Automenu.Resources;
using System.Diagnostics.CodeAnalysis;

namespace rEFInd_Automenu.ConsoleApplication.OperationsManager.Resources
{
    public static partial class ResourcesOperations
    {
        public static DirectoryInfo ExtractLatestResourceArchive() => ConsoleProgram.Interface.ExecuteAndReturn<DirectoryInfo>("Extracting resource files", ConsoleProgram.CommonCommands, (ctrl) =>
        {
            ConsoleControllerCommands commands = (ConsoleControllerCommands)ctrl.ExecutingCommand;
            log.Info("Extracting rEFInd binaries from resource archive");
            int attempts = 0;

            // Extracting archive
            while (true)
            {
                // Breaking loop on 11th attempt
                if (++attempts > 10)
                    break;

                using (Stream ResStream = LocalResourceManager.GetLatestBinArchiveStream(out Version? version))
                {
                    if (TryExtract(ResStream, out DirectoryInfo? archiveDirectory))
                    {
                        log.InfoFormat("Archive extracted on TEMP folder : {0}", archiveDirectory.FullName);
                        if (attempts > 1)
                        {
                            return ctrl.Warning(string.Format("Archive version {0} unpacked", version), archiveDirectory, false);
                        }

                        return archiveDirectory;
                    }
                }
            }

            return ctrl.Error("Failed to TryHard-Extract", true);
        });

        private static bool TryExtract(Stream ResStream, [NotNullWhen(true)] out DirectoryInfo? archiveDir)
        {
            try
            {
                DirectoryInfo TmpArchiveDir = InstallManager.ExtractBinArchive(ResStream);
                log.InfoFormat("Archive extracted on TEMP folder : {0}", TmpArchiveDir.FullName);

                archiveDir = TmpArchiveDir;
                return true;
            }
            catch (InvalidDataException)
            {
                ResStream.Dispose();
                string brokenArchiveName = LocalResourceManager.GetLatestBinArchiveFullPath(out Version? version);
                log.InfoFormat("Failed to extract archive of version {0}", version);

                File.Delete(brokenArchiveName);
                log.InfoFormat("The corrupted archive ({0}) has been deleted", version);

                log.Info("An attempt will be made to unpack the archive with the version below");
                archiveDir = null;
                return false;
            }
        }
    }
}
