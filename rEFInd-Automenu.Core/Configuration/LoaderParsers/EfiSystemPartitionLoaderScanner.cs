using log4net;
using rEFInd_Automenu.Booting;
using rEFInd_Automenu.Configuration.MenuEntry;
using rEFInd_Automenu.TypesExtensions;
using Rikitav.IO.ExtensibleFirmware.SystemPartition;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace rEFInd_Automenu.Configuration.LoaderParsers
{
    public class EfiSystemPartitionLoaderScanner : ILoadersScanner
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EfiSystemPartitionLoaderScanner));

        private static readonly string[] _IgnoredDirs = new string[]
        {
            "boot",
            "keys",
            "microsoft",
            "refind"
        };

        // Key : loader nickname, Value : SecureBoot capable
        private static readonly Dictionary<string, bool> _KnownLoaderNicknames = new Dictionary<string, bool>()
        {
            { "shim", true },
            { "grub", false },
            { "boot", false }
        };

        public IEnumerable<MenuEntryInfo> Parse(FirmwareExecutableArchitecture Arch)
        {
            // Enumerating directories on Efi system partition
            log.Info("Started ESP loaders parsing");
            foreach (DirectoryInfo loaderDir in ESPLS_Helper.GetEspDirectories(EspFinder.EspDirectory))
            {
                // Checking if directory contained in _IgnoredDirs list
                if (_IgnoredDirs.Contains(loaderDir.Name, StringComparer.InvariantCultureIgnoreCase))
                    continue;

                // Enumerating known loader nicknames for finding first available
                foreach (string loaderNickname in _KnownLoaderNicknames.Keys)
                {
                    if (CreateMenuEntryInfo(out MenuEntryInfo? loaderEntryInfo, loaderDir, loaderNickname, Arch))
                        yield return loaderEntryInfo;
                }

                // Creating fallback loader
                if (CreateMenuEntryInfo(out MenuEntryInfo? fallbackEntryInfo, loaderDir, "fb", Arch))
                    yield return fallbackEntryInfo;
            }
        }

        private static bool CreateMenuEntryInfo([NotNullWhen(true)] out MenuEntryInfo? entryInfo, DirectoryInfo loaderDir, string loaderNickname, FirmwareExecutableArchitecture Arch)
        {
            // Making real Arch-Based loader name
            string loaderName = ESPLS_Helper.MakeLoaderName(Arch, loaderNickname);
            entryInfo = null;

            // Checking if loader is capable with current secure boot settings
            if (!ESPLS_Helper.CheckLoaderCapability(loaderName))
                return false;

            // Checking if loader exists on ESP
            if (!File.Exists(Path.Combine(loaderDir.FullName, loaderName)))
                return false;

            // Creating loader menu entry instance
            entryInfo = new MenuEntryInfo()
            {
                EntryName = loaderName.FirstLetterToUpper(),
                Volume = EfiPartition.Identificator,
                Loader = Path.Combine("EFI", loaderDir.Name, loaderName),
                OSType = OSType.Linux
            };

            return true;
        }

        private static class ESPLS_Helper
        {
            public static IEnumerable<DirectoryInfo> GetEspDirectories(DirectoryInfo ESP)
                => ESP.GetSubDirectory("EFI").EnumerateDirectories();

            public static string MakeLoaderName(FirmwareExecutableArchitecture Arch, string LoaderNickname)
                => string.Concat(LoaderNickname, Arch.GetArchPostfixString(), ".efi");

            public static bool CheckLoaderCapability(string loaderNickname)
            {
                if (SecureBootInfo.IsCapable && SecureBootInfo.IsEnabled)
                {
                    // Is loader secure boot capable
                    return _KnownLoaderNicknames[loaderNickname];
                }

                return true;
            }
        }
    }
}
