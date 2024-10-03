using rEFInd_Automenu.Booting;
using Rikitav.IO.ExtensibleFirmware;
using Rikitav.IO.ExtensibleFirmware.BootService;
using Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols;
using Rikitav.IO.ExtensibleFirmware.BootService.LoadOption;
using Rikitav.IO.ExtensibleFirmware.BootService.Win32Native;
using Rikitav.IO.ExtensibleFirmware.MediaDevicePathProtocols;
using Rikitav.IO.ExtensibleFirmware.SystemPartition;
using System.Linq;
using System.Text;

namespace rEFInd_Automenu.ConsoleApplication.WorkerMethodsImplementations
{
    public partial class WorkerMethods
    {
        public ushort CreateRefindFirmwareLoadOption(bool overrideExisting, bool addFirst, FirmwareExecutableArchitecture Arch) => ConsoleProgram.Interface.ExecuteAndReturn<ushort>("Creating rEFInd boot option", commands, (ctrl) =>
        {
            log.Info("Creating new boot option for rEFInd boot manager");

            if (FindFirmwareRefindBootLoader(out ushort loadOptionIndex))
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
                FirmwareBootService.UpdateLoadOption(new FirmwareRefindBootOption(Arch), loadOptionIndex);
                return ctrl.Success("Updated", loadOptionIndex);
            }

            // Not found for update
            if (overrideExisting)
                log.Warn("Boot entry was not found, a new entry will be created");

            // Creating new load option
            log.Info("Creating new boot option variable");
            ushort newLoadOptoinIndex = FirmwareBootService.CreateLoadOption(new FirmwareRefindBootOption(Arch), addFirst);

            // Success
            log.InfoFormat("New rEFInd boot option was successfully created");
            return ctrl.Success("Created", newLoadOptoinIndex);
        });

        public bool FindFirmwareRefindBootLoader(out ushort loadOptionIndex)
        {
            // Logging data
            log.Info("Search for an existing boot entry");
            log.InfoFormat("Optional data origin is : \"{0}\"", FirmwareRefindBootOption.OptionalDataInstanceOrigin);

            // Enumearting existing boot entries
            foreach (ushort loadOption in FirmwareGlobalEnvironment.BootOrder)
            {
                // Reading variable optional data
                EFI_LOAD_OPTION checkBootOption = FirmwareBootService.ReadRawLoadOption(loadOption);
                
                // Checking
                if (checkBootOption.OptionalData.SequenceEqual(FirmwareRefindBootOption.OptionalDataInstance))
                {
                    log.InfoFormat("rEFInd Boot option found. option index - {0}", loadOption);
                    loadOptionIndex = loadOption;
                    return true;
                }
            }

            log.Error("Boot option was not found");
            loadOptionIndex = 0;
            return false;
        }

        private static void LogFirmwareRefindBootOption(FirmwareRefindBootOption RefindBootOption)
        {
            // Logging option data
            log.InfoFormat("rEFInd load option - attributes - {0}", RefindBootOption.Attributes);
            log.InfoFormat("rEFInd load option - description - {0}", RefindBootOption.Description);
            log.InfoFormat("rEFInd load option - optionalData - {0}", string.Join("; ", FirmwareRefindBootOption.OptionalDataInstance));
            log.InfoFormat("rEFInd load option - HardDriveMediaDevicePath.GptPartitionSignature - {0}", RefindBootOption.HardDriveProtocol);
            log.InfoFormat("rEFInd load option - FilePathMediaDevicePath.PathName - {0}", RefindBootOption.FilePathProtocol);
        }

        private class FirmwareRefindBootOption : LoadOptionBase
        {
            public HardDriveMediaDevicePath HardDriveProtocol => (HardDriveMediaDevicePath)OptionProtocols[0];
            public FilePathMediaDevicePath FilePathProtocol => (FilePathMediaDevicePath)OptionProtocols[1];

            public static string OptionalDataInstanceOrigin = "rEFInd Automenu";
            public static byte[] OptionalDataInstance = Encoding.Unicode.GetBytes(OptionalDataInstanceOrigin);

            public FirmwareRefindBootOption(FirmwareExecutableArchitecture Arch) : base(LoadOptionAttributes.ACTIVE, "rEFInd Boot manager")
            {
                _OptionalData = OptionalDataInstance;
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
