using rEFInd_Automenu.Booting;
using rEFInd_Automenu.TypesExtensions;

namespace rEFInd_Automenu.ConsoleApplication.OperationsManager.Instance
{
    public static partial class InstanceOperations
    {
        public static DirectoryInfo CheckInstanceExisting() => ConsoleProgram.Interface.ExecuteAndReturn<DirectoryInfo>("Checking Instance", ConsoleProgram.CommonCommands, (ctrl) =>
        {
            // Searching for "refind" directory on ESP
            DirectoryInfo ESP = EspFinder.EspDirectory;
            DirectoryInfo EspRefindDir = ESP.GetSubDirectory("EFI\\refind");

            if (!InstanceExist(EspRefindDir))
            {
                log.Fatal("rEFInd is not installed on this computer");
                return ctrl.Error("rEFInd is not installed on this computer", true);
            }

            return ctrl.Success("Installed", EspRefindDir);
        });

        public static DirectoryInfo CheckInstanceNonExisting(bool Force = false) => ConsoleProgram.Interface.ExecuteAndReturn<DirectoryInfo>("Checking Instance", ConsoleProgram.CommonCommands, (ctrl) =>
        {
            // Searching for "refind" directory on ESP
            DirectoryInfo ESP = EspFinder.EspDirectory;
            DirectoryInfo EspRefindDir = ESP.GetSubDirectory("EFI\\refind");

            // All ok
            if (!InstanceExist(EspRefindDir))
                return ctrl.Success(string.Empty, EspRefindDir);

            // Shouldnt existing
            if (Force) // <-- indicates that we should try to delete instance
            {
                log.Warn("rEFInd is already installed on this computer");
                try
                {
                    // Trying to delete instance
                    log.Info("Force work is enabled, trying to remove instance");
                    EspRefindDir.Delete(true);
                    log.Info("rEFInd instance is removed");
                }
                catch (Exception exc)
                {
                    // Failed to delete
                    log.Fatal("Failed to remove existing rEFInd instance from computer", exc);
                    return ctrl.Error("Failed to remove existing rEFInd instance from computer", true);
                }

                return ctrl.Success("Force cleaned", EspRefindDir);
            }
            else
            {
                // Cannot continue
                log.Fatal("rEFInd is already installed on this computer");
                return ctrl.Error("rEFInd is already installed on this computer", true);
            }
        });

        private static bool InstanceExist(DirectoryInfo EspRefindDir)
        {
            if (!EspRefindDir.Exists)
                return false;

            return EspRefindDir.EnumerateFiles("refind_*.efi").Any();
        }
    }
}
