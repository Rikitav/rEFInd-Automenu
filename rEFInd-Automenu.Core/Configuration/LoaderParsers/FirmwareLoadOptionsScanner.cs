using log4net;
using rEFInd_Automenu.Booting;
using rEFInd_Automenu.Configuration.MenuEntry;
using rEFInd_Automenu.TypesExtensions;
using Rikitav.IO.ExtensibleFirmware.BootService;
using Rikitav.IO.ExtensibleFirmware.BootService.DevicePathProtocols;
using Rikitav.IO.ExtensibleFirmware.BootService.LoadOption;
using Rikitav.IO.ExtensibleFirmware.BootService.UefiNative;
using Rikitav.IO.ExtensibleFirmware.MediaDevicePathProtocols;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rEFInd_Automenu.Configuration.LoaderParsers
{
    public class FirmwareLoadOptionsScanner : ILoadersScanner
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FirmwareLoadOptionsScanner));

        private static readonly string[] _IgnoreLoaders = new string[]
        {
            "windows",
            "refind"
        };


        public IEnumerable<MenuEntryInfo> Parse(FirmwareExecutableArchitecture Arch)
        {
            // Listing firmware load options
            log.Info("Listing Firmware load options from BootOrder");

            foreach (ushort loadOptionIndex in FirmwareBootService.LoadOrder)
            {
                FirmwareApplicationBootOption? loadOption = null;

                try
                {
                    // Reading variable
                    log.InfoFormat("Reading load option {0}", loadOptionIndex.ToString("X").PadLeft(4, '0'));
                    loadOption = FirmwareBootService.ReadLoadOption<FirmwareApplicationBootOption>(loadOptionIndex);
                }
                catch (Exception exc)
                {
                    // Exception
                    log.Error("Failed to read option", exc);
                }

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

                // Checking if loader contained in _IgnoreLoaders list
                if (_IgnoreLoaders.Any(l => loadOption.Description.Contains(l, StringComparison.CurrentCultureIgnoreCase)))
                {
                    //log.Error("Not an application option");
                    continue;
                }

                // Returning new menu entry
                yield return new MenuEntryInfo()
                {
                    OSType = OSType.Linux, // Most likely, it will be a linux system, so...
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
                return OptionProtocols[0] is HardDriveMediaDevicePath
                    && OptionProtocols[1] is FilePathMediaDevicePath
                    && OptionProtocols[2] is DevicePathProtocolEnd;
            }
        }
    }
}
