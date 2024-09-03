using Rikitav.IO.ExtensibleFirmware;
using Rikitav.IO.ExtensibleFirmware.BootService;
using Rikitav.IO.ExtensibleFirmware.BootService.LoadOption;

namespace rEFInd_Automenu.ConsoleApplication.WorkerMethodsImplementations
{
    public partial class WorkerMethods
    {
        public void FindDeleteRefindFirmwareLoadOption() => ConsoleProgram.Interface.Execute("Removing rEFInd boot option", commands, (ctrl) =>
        {
            byte[] optionalData = CreateRefindBootOptionOptionalData();
            log.Info("Removing boot option for rEFInd boot manager");

            log.Info("Search for an rEFInd boot option");
            log.InfoFormat("Optional data is : {0}", string.Join(", ", optionalData));
            foreach (ushort loadOptionIndex in FirmwareGlobalEnvironment.BootOrder)
            {
                // Reading variable optional data
                FirmwareBootOption checkBootOption = FirmwareBootService.ReadLoadOption(loadOptionIndex);

                // Checking
                if (checkBootOption.OptionalData.SequenceEqual(optionalData))
                {
                    // Option finded
                    log.Info("rEFInd boot option finded. Removing");
                    FirmwareBootService.DeleteLoadOption(loadOptionIndex);
                    return;
                }
            }

            // Not find
            log.Error("Boot option was not found");
        });
    }
}
