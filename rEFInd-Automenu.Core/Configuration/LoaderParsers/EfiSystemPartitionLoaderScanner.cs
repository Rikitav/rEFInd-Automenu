using log4net;
using rEFInd_Automenu.Booting;
using rEFInd_Automenu.Configuration.MenuEntry;
using rEFInd_Automenu.Extensions;
using Rikitav.IO.ExtensibleFirmware.SystemPartition;
using System;
using System.Collections.Generic;
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

        public IEnumerable<MenuEntryInfo> Parse(EnvironmentArchitecture Arch)
        {
            log.Info("Started ESP loaders parsing");
            DirectoryInfo ESP = EfiPartition.GetDirectoryInfo();
            if (!ESP.TryGrantAccessControl())
            {
                log.Fatal("Failed to access ESP");
                yield break;
            }

            foreach (string LoaderDir in Directory.EnumerateDirectories(Path.Combine(ESP.FullName, "EFI")))
            {
                string LoaderName = Path.GetFileName(LoaderDir);
                if (_IgnoredDirs.Contains(LoaderName, StringComparer.InvariantCultureIgnoreCase))
                    continue;

                string ShimLoaderName = GetLoaderByNickname(Arch, "shim");    // EFI-Signed GRUB2 loader
                string GrubLoaderName = GetLoaderByNickname(Arch, "grub");    // GRUB2 loader
                string DefaultLoaderPath = GetLoaderByNickname(Arch, "boot"); // Default name loader

                if (File.Exists(Path.Combine(LoaderDir, ShimLoaderName)))
                {
                    yield return new MenuEntryInfo()
                    {
                        EntryName = LoaderName.FirstLetterToUpper(),
                        Loader = Path.Combine("EFI", LoaderName, ShimLoaderName),
                        OSType = OSType.Linux
                    };
                }
                else if (File.Exists(Path.Combine(LoaderDir, GrubLoaderName)))
                {
                    if (SecureBootInfo.IsEnabled)
                    {
                        // No loader
                        log.WarnFormat("EFI-Loader \"{0}\" does not contain known signed loaders. Skipping", LoaderDir);
                        continue;
                    }

                    
                    if (File.Exists(Path.Combine(LoaderDir, GrubLoaderName)))
                    {
                        yield return new MenuEntryInfo()
                        {
                            EntryName = LoaderName.FirstLetterToUpper(),
                            Loader = Path.Combine("EFI", LoaderName, GrubLoaderName),
                            OSType = OSType.Linux
                        };
                    }
                }
                else if (File.Exists(Path.Combine(LoaderDir, DefaultLoaderPath)))
                {
                    yield return new MenuEntryInfo()
                    {
                        EntryName = LoaderName.FirstLetterToUpper(),
                        Loader = Path.Combine("EFI", LoaderName, DefaultLoaderPath),
                    };
                }

                string FallbackLoaderPath = GetLoaderByNickname(Arch, "fb");
                if (File.Exists(Path.Combine(LoaderDir, FallbackLoaderPath)))
                {
                    yield return new MenuEntryInfo()
                    {
                        EntryName = LoaderName.FirstLetterToUpper() + " (Fallback)",
                        Loader = Path.Combine("EFI", LoaderName, FallbackLoaderPath),
                        OSType = OSType.Linux,
                        Disabled = true
                    };
                }
            }
        }

        private static string GetLoaderByNickname(EnvironmentArchitecture Arch, string LoaderNickname)
        {
            string ArchPostfix = Arch.GetArchPostfixString();
            return LoaderNickname + ArchPostfix + ".efi";
        }
    }
}
