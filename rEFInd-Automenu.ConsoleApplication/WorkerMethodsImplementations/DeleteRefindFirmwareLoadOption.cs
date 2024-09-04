using Rikitav.IO.ExtensibleFirmware.BootService;

namespace rEFInd_Automenu.ConsoleApplication.WorkerMethodsImplementations
{
    public partial class WorkerMethods
    {
        public void DeleteRefindFirmwareLoadOption() => ConsoleProgram.Interface.Execute("Removing rEFInd boot option", commands, (ctrl) =>
        {
            byte[] optionalData = CreateRefindBootOptionOptionalData();
            log.Info("Removing boot option for rEFInd boot manager");

            // Searching
            if (!FindFirmwareRefindBootLoader(optionalData, out ushort loadOptionIndex))
            {
                // Not found
                log.Error("Boot option was not found");
                ctrl.Error("Boot option was not found");
                return;
            }

            // Option finded
            log.Info("rEFInd boot option finded. Removing");
            FirmwareBootService.DeleteLoadOption(loadOptionIndex);
            return;
        });
    }
}
