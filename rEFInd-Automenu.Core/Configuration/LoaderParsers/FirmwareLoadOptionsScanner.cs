using log4net;
using rEFInd_Automenu.Booting;
using rEFInd_Automenu.Configuration.MenuEntry;
using rEFInd_Automenu.Extensions;
using Rikitav.IO.ExtensibleFirmware;
using Rikitav.IO.ExtensibleFirmware.BootService;
using Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols;
using Rikitav.IO.ExtensibleFirmware.BootService.LoadOption;
using Rikitav.IO.ExtensibleFirmware.BootService.Win32Native;
using Rikitav.IO.ExtensibleFirmware.MediaDevicePathProtocols;
using System;
using System.Collections.Generic;

namespace rEFInd_Automenu.Configuration.LoaderParsers
{
    public class FirmwareLoadOptionsScanner : ILoadersScanner
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FirmwareLoadOptionsScanner));

        public IEnumerable<MenuEntryInfo> Parse(EnvironmentArchitecture Arch)
        {
            // Listing firmware load options
            log.Info("Listing Firmware load options from BootOrder");

            foreach (ushort loadOptionIndex in FirmwareBootService.LoadOrder)
            {
                log.InfoFormat("Reading load option {0}", loadOptionIndex.ToString("X").PadLeft(4, '0'));
                FirmwareApplicationBootOption? loadOption = null;

                try
                {
                    // Reading variable
                    loadOption = FirmwareBootService.ReadLoadOption<FirmwareApplicationBootOption>(loadOptionIndex);

                    // Null check
                    if (loadOption == null)
                    {
                        log.Error("Null instance of bootOption");
                        continue;
                    }

                    // Checking option type
                    if (!loadOption.IsApplication())
                    {
                        log.Error("Not an application option");
                        continue;
                    }
                }
                catch (Exception exc)
                {
                    // Exception
                    log.Error("Failed to read option", exc);
                }

                if (loadOption.Description.Contains("windows", StringComparison.CurrentCultureIgnoreCase))
                    continue;

                // Returning new menu entry
                yield return new MenuEntryInfo()
                {
                    EntryName = loadOption.Description.FirstLetterToUpper(),
                    Loader = loadOption.FilePathProtocol.PathName,
                    Volume = loadOption.HardDriveProtocol.GptPartitionSignature
                };
            }
        }

        private class FirmwareApplicationBootOption : LoadOptionBase
        {
            public HardDriveMediaDevicePath HardDriveProtocol => (HardDriveMediaDevicePath)OptionProtocols[0];
            public FilePathMediaDevicePath FilePathProtocol => (FilePathMediaDevicePath)OptionProtocols[1];

            public FirmwareApplicationBootOption(EFI_LOAD_OPTION loadOption)
                : base(loadOption) { }

            public bool IsApplication()
            {
                if (OptionProtocols[0] is not HardDriveMediaDevicePath)
                    return false;

                if (OptionProtocols[1] is not FilePathMediaDevicePath)
                    return false;

                if (OptionProtocols[2] is not DevicePathProtocolEnd)
                    return false;

                return true;
            }
        }
    }
}
