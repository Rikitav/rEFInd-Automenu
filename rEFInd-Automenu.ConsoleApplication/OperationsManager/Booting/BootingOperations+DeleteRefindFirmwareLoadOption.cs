using rEFInd_Automenu.Booting;

namespace rEFInd_Automenu.ConsoleApplication.OperationsManager.Booting
{
    public static partial class BootingOperations
    {
        public static void FindDeleteRefindFirmwareLoadOption() => ConsoleProgram.Interface.Execute("Removing rEFInd boot option", ConsoleProgram.CommonCommands, (ctrl) =>
        {
            log.Info("Removing boot option for rEFInd boot manager");
            FirmwareBootManagerBridge.DeleteRefindBootOption();
            return;
        });
    }
}
