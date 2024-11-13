using log4net;
using Rikitav.IO.ExtensibleFirmware;
using Rikitav.IO.ExtensibleFirmware.BootService;
using Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols;
using Rikitav.IO.ExtensibleFirmware.BootService.LoadOption;
using Rikitav.IO.ExtensibleFirmware.BootService.UefiNative;
using Rikitav.IO.ExtensibleFirmware.MediaDevicePathProtocols;
using Rikitav.IO.ExtensibleFirmware.SystemPartition;
using System;
using System.Linq;
using System.Text;

namespace rEFInd_Automenu.Booting
{
    public static class FirmwareBootManagerBridge
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FirmwareBootManagerBridge));

        public static ushort CreateRefindBootOption(bool overrideExisting, bool addFirst, FirmwareExecutableArchitecture Arch)
        {
            log.InfoFormat("{0} rEFInd boot option", overrideExisting ? "Overriding existing" : "Creating new");

            if (overrideExisting)
            {
                if (!FindRefindBootOption(out ushort loadOptionIndex))
                {
                    // Not found for update
                    log.Warn("Boot entry was not found, a new entry will be created");
                }
                else
                {
                    // Option finded
                    log.Info("Existing boot entry found");

                    try
                    {
                        // Updating existing boot option variable
                        log.InfoFormat("Updating {0} boot option variable", loadOptionIndex);
                        FirmwareBootService.UpdateLoadOption(new FirmwareRefindBootOption(Arch), loadOptionIndex);
                        return loadOptionIndex;
                    }
                    catch (Exception exc)
                    {
                        log.Error("Failed to override existing rEFInd boot option", exc);
                        throw new BootOptionCreationFailedException(exc);
                    }
                }
            }

            try
            {
                // Creating new load option
                log.Info("Creating new boot option variable");
                ushort newLoadOptoinIndex = FirmwareBootService.CreateLoadOption(new FirmwareRefindBootOption(Arch), addFirst);

                // Success
                log.InfoFormat("New rEFInd boot option was successfully created. Load optiond index : {0}", newLoadOptoinIndex);
                return newLoadOptoinIndex;
            }
            catch (Exception exc)
            {
                log.Error("Failed to create new rEFInd boot option", exc);
                throw new BootOptionCreationFailedException(exc);
            }
        }

        public static bool FindRefindBootOption(out ushort loadOptionIndex)
        {
            // Logging data
            log.Info("Search for an existing boot entry by optional data");
            log.InfoFormat("Optional data Unicode origin is : \"{0}\"", FirmwareRefindBootOption.OptionalDataInstanceOrigin);

            // Enumearting existing boot entries
            foreach (ushort loadOption in FirmwareGlobalEnvironment.BootOrder)
            {
                try
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
                catch
                {
                    log.WarnFormat("Failed to read boot optiona at {0}", loadOption);
                }
            }

            log.Error("Boot option was not found");
            loadOptionIndex = 0;
            return false;
        }

        public static void DeleteRefindBootOption()
        {
            // Searching
            log.Info("Removing boot option for rEFInd boot manager");
            if (!FindRefindBootOption(out ushort loadOptionIndex))
                throw new BootOptionNotFoundException();

            // Option finded
            FirmwareBootService.DeleteLoadOption(loadOptionIndex);
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

                // Logging option data
                log.Info("Created new rEFInd loadoption info instance");
                log.InfoFormat("HardDriveMediaDevicePath.GptPartitionSignature - {0}", HardDriveProtocol);
                log.InfoFormat("FilePathMediaDevicePath.PathName - {0}", FilePathProtocol);
            }
        }
    }

    public class BootOptionNotFoundException : Exception
    {
        public BootOptionNotFoundException()
            : base("rEInd boot option was not found") { }
    }

    public class BootOptionCreationFailedException : Exception
    {
        public BootOptionCreationFailedException(Exception inner)
            : base("Failed to create new rEFInd load option", inner) { }
    }
}
