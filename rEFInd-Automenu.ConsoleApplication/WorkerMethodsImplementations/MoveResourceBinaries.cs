using rEFInd_Automenu.Booting;
using rEFInd_Automenu.Extensions;
using rEFInd_Automenu.Installation;

namespace rEFInd_Automenu.ConsoleApplication.WorkerMethodsImplementations
{
    public partial class WorkerMethods
    {
        public void MoveResourceBinaries(DirectoryInfo BinArchiveDir, DirectoryInfo RefindInstallationDir, EnvironmentArchitecture Arch, bool Usb) => ConsoleProgram.Interface.Execute("Installing binaries", commands, (ctrl) =>
        {
            // Creating such directory and moving binaries
            log.Info("Copying rEFInd binaries to installation directory");
            RefindInstallationDir.Create();
            log.Info("Installation directory created");
            InstallManager.CopyResourceArchive(BinArchiveDir.GetSubDirectory("refind"), RefindInstallationDir, Arch, Usb ? "boot{0}.efi" : "refind_{0}.efi");

            // Trying to not litter %Temp% directory
            BinArchiveDir.Delete(true);
            log.Info("Temporary resource directory was deleted");

            // Creating info file for current installed rEFInd instance
            log.Info("Creating loader information file");
            string RefindBinVersion = BinArchiveDir.Name.Substring("refind-bin-".Length);

            RefindInstanceInfo info = new RefindInstanceInfo(Version.Parse(RefindBinVersion));
            RefindInstanceInfo.Write(info, RefindInstallationDir.FullName);

            ctrl.Success(Arch == EnvironmentArchitecture.None ? "AUTO" : Arch.ToString());
        });
    }
}
