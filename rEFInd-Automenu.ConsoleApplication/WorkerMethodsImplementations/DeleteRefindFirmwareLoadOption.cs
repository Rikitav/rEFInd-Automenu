using Rikitav.IO.ExtensibleFirmware.BootService;

namespace rEFInd_Automenu.ConsoleApplication.WorkerMethodsImplementations
{
    public partial class WorkerMethods
    {
        public void DeleteRefindFirmwareLoadOption() => ConsoleProgram.Interface.Execute("Removing rEFInd boot option", commands, (ctrl) =>
        {
            log.Info("Removing boot option for rEFInd boot manager");

            // Searching
            if (!FindFirmwareRefindBootLoader(out ushort loadOptionIndex))
            {
                // Not found
                ctrl.Error("Boot option was not found");
                return;
            }

            // Option finded
            DeleteRefindFirmwareLoadOption(loadOptionIndex);
            return;
        });

        public void DeleteRefindFirmwareLoadOption(ushort loadOptionIndex)
        {
            // Option finded
            log.Info("Removing old refind load option");
            FirmwareBootService.DeleteLoadOption(loadOptionIndex);
            return;
        }
    }
}
