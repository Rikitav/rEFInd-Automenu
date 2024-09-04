using rEFInd_Automenu.Booting;
using Rikitav.IO.ExtensibleFirmware;
using Rikitav.IO.ExtensibleFirmware.BootService;
using Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols;
using Rikitav.IO.ExtensibleFirmware.BootService.LoadOption;
using Rikitav.IO.ExtensibleFirmware.BootService.Win32Native;
using Rikitav.IO.ExtensibleFirmware.MediaDevicePathProtocols;
using Rikitav.IO.ExtensibleFirmware.SystemPartition;
using System.Text;

namespace rEFInd_Automenu.ConsoleApplication.WorkerMethodsImplementations
{
    public partial class WorkerMethods
    {
        public ushort CreateRefindFirmwareLoadOption(bool overrideExisting, bool addFirst, EnvironmentArchitecture Arch) => ConsoleProgram.Interface.ExecuteAndReturn<ushort>("Creating rEFInd boot option", commands, (ctrl) =>
        {
            byte[] optionalData = CreateRefindBootOptionOptionalData();
            log.Info("Creating new boot option for rEFInd boot manager");

            if (FindFirmwareRefindBootLoader(optionalData, out ushort loadOptionIndex))
            {
                if (!overrideExisting)
                {
                    // Option already exist
                    log.Info("Existing boot entry found, no need to create");
                    return ctrl.Success("Already exist", loadOptionIndex);
                }

                // Option finded
                log.Info("Existing boot entry found");

                // Updating existing boot option variable
                log.InfoFormat("Updating {0} boot option variable", loadOptionIndex);
                FirmwareBootService.UpdateLoadOption(new FirmwareRefindBootOption(Arch, optionalData), loadOptionIndex);
                return ctrl.Success("Updated", loadOptionIndex);
            }

            // Not found for update
            if (overrideExisting)
                log.Warn("Boot entry was not found, a new entry will be created");

            // Creating new load option
            log.Info("Creating new boot option variable");
            ushort newLoadOptoinIndex = FirmwareBootService.CreateLoadOption(new FirmwareRefindBootOption(Arch, optionalData), addFirst);

            // Success
            log.InfoFormat("New rEFInd boot option was successfully created");
            return ctrl.Success("Created", newLoadOptoinIndex);
        });

        private static bool FindFirmwareRefindBootLoader(byte[] optionalData, out ushort loadOptionIndex)
        {
            // Logging data
            log.Info("Search for an existing boot entry");
            log.InfoFormat("Optional data is : {0}", string.Join(", ", optionalData));

            // Enumearting existing boot entries
            foreach (ushort loadOption in FirmwareGlobalEnvironment.BootOrder)
            {
                // Reading variable optional data
                EFI_LOAD_OPTION checkBootOption = FirmwareBootService.ReadRawLoadOption(loadOption);
                
                // Checking
                if (checkBootOption.OptionalData.SequenceEqual(optionalData))
                {
                    loadOptionIndex = loadOption;
                    return true;
                }
            }

            loadOptionIndex = 0;
            return false;
        }

        private static void LogFirmwareRefindBootOption(FirmwareRefindBootOption RefindBootOption)
        {
            // Logging option data
            log.InfoFormat("Option attributes - {0}", RefindBootOption.Attributes);
            log.InfoFormat("Option description - {0}", RefindBootOption.Description);
            log.InfoFormat("Option optionalData - {0}", RefindBootOption.OptionalData);
            log.InfoFormat("Protocol HardDriveMediaDevicePath.GptPartitionSignature - {0}", RefindBootOption.HardDriveProtocol.GptPartitionSignature);
            log.InfoFormat("Protocol FilePathMediaDevicePath.PathName - {0}", RefindBootOption.FilePathProtocol.PathName);
        }

        private static byte[] CreateRefindBootOptionOptionalData()
            => Encoding.Unicode.GetBytes("rEFInd Automenu");

        private class FirmwareRefindBootOption : LoadOptionBase
        {
            public HardDriveMediaDevicePath HardDriveProtocol => (HardDriveMediaDevicePath)OptionProtocols[0];
            public FilePathMediaDevicePath FilePathProtocol => (FilePathMediaDevicePath)OptionProtocols[1];
            public byte[] OptionalData => _OptionalData;

            public FirmwareRefindBootOption(EnvironmentArchitecture Arch, byte[] optionalData) : base(LoadOptionAttributes.ACTIVE, "rEFInd Boot manager")
            {
                _OptionalData = optionalData;
                OptionProtocols = new DevicePathProtocolBase[]
                {
                    new HardDriveMediaDevicePath(EfiPartition.Identificator),
                    new FilePathMediaDevicePath(@"\EFI\refind\refind_" + Arch.GetArchPostfixString() + ".efi"),
                    new DevicePathProtocolEnd()
                };

                LogFirmwareRefindBootOption(this);
            }
        }
    }
}
