using rEFInd_Automenu.Booting;

namespace rEFInd_Automenu.ConsoleApplication.OperationsManager.Booting
{
    public static partial class BootingOperations
    {
        public static ushort CreateRefindFirmwareLoadOption(bool overrideExisting, bool addFirst, FirmwareExecutableArchitecture Arch) => ConsoleProgram.Interface.ExecuteAndReturn<ushort>("Creating rEFInd boot option", ConsoleProgram.CommonCommands, (ctrl) =>
        {
            log.Info("Creating new boot option for rEFInd boot manager");
            ushort newLoadOptoinIndex = FirmwareBootManagerBridge.CreateRefindBootOption(overrideExisting, addFirst, Arch);
            return ctrl.Success("Created", newLoadOptoinIndex);
        });
    }
}
