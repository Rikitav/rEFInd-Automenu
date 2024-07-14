using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rEFInd_Automenu.Configuration.MenuEntry
{
    public static class KnownMenuEntries
    {
        public static readonly MenuEntryInfo Windows = new MenuEntryInfo()
        {
            EntryName = "Windows",
            Loader = "EFI\\Microsoft\\Boot\\bootmgfw.efi",
            OSType = OSType.Windows
        };

        public static readonly MenuEntryInfo Phoenix = new MenuEntryInfo()
        {
            EntryName = "Phoenix OS",
            Loader = "EFI\\PhoenixOS\\kernel",
            InitRD = @"EFI\PhoenixOS\initrd.img",
            Options = "quiet root=/dev/ram0 androidboot.hardware=android_x86 SRC=/PhoenixOS vga=788",
            OSType = OSType.Linux
        };
    }
}
