using rEFInd_Automenu.Installation;
using rEFInd_Automenu.Resources;

namespace rEFInd_Automenu.ConsoleApplication.WorkerMethodsImplementations
{
    public partial class WorkerMethods
    {
        public DirectoryInfo ExtractLatestResourceArchive() => ConsoleProgram.Interface.ExecuteAndReturn<DirectoryInfo>("Extracting resource files", commands, (ctrl) =>
        {
            log.Info("Extracting rEFInd binaries from resource archive");

            // Extracting archive
            using (Stream ResStream = LocalResourceManager.GetLatestBinArchiveStream())
            {
                DirectoryInfo TmpArchiveDir = InstallManager.ExtractBinArchive(ResStream);
                log.InfoFormat("Archive extracted on TEMP folder : {0}", TmpArchiveDir.FullName);
                return TmpArchiveDir;
            }
        });
    }
}
