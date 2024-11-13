using rEFInd_Automenu.ConsoleApplication.OperationsManager.Booting;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Instance;
using rEFInd_Automenu.RuntimeConfiguration;

namespace rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.Instance
{
    public static partial class ProcedureProcessor_Instance
    {
        private static void RemoveInstance()
        {
            // Working
            log.Info("Removing current rEFInd instance (\'Instance --Remove\' flag)");

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = InstanceOperations.CheckInstanceExisting();

            // Deleting "refind" directory on ESP
            ConsoleProgram.Interface.Execute("Removing binaries", ConsoleProgram.CommonCommands, (ctrl) =>
            {
                EspRefindDir.Delete(true);
                log.Info("rEFInd was removed from this computer");
            });

            // Removing boot option
            if (!ProgramConfiguration.Instance.PreferBootmgrBooting)
                BootingOperations.FindDeleteRefindFirmwareLoadOption();
        }
    }
}
