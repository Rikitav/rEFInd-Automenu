using rEFInd_Automenu.Booting;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Installing;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Instance;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Resources;
using rEFInd_Automenu.Installation;

namespace rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.Instance
{
    public static partial class ProcedureProcessor_Instance
    {
        private static void UpdateInstanceBin()
        {
            // Working
            log.Info("Updating current rEFInd instatnce (\'Instance --Update\' flag)");
            FirmwareExecutableArchitecture Arch = ArchitectureInfo.Current;

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = InstanceOperations.CheckInstanceExisting();

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
            ResourcesOperations.ObtainResourceArchive(
                true,              // DownloadLatest
                InstalledVersion); // InstalledVersion

            // Resource stream containig an archive that extracting into temp directory
            DirectoryInfo BinArchiveDir = ResourcesOperations.ExtractLatestResourceArchive();

            // Moving binaries (loader and tools) to "refind" directory on ESP
            InstallingOperations.MoveResourceBinaries(
                BinArchiveDir, // BinArchiveDir
                EspRefindDir,  // RefindInstallationDir
                Arch,          // Arch
                false);        // USB
        }
    }
}
