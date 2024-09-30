using log4net;
using rEFInd_Automenu.Booting;
using rEFInd_Automenu.Configuration.MenuEntry;
using rEFInd_Automenu.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace rEFInd_Automenu.Configuration.LoaderParsers
{
    public class FwBootmgrLoaderScanner : ILoadersScanner
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FwBootmgrLoaderScanner));

        public IEnumerable<MenuEntryInfo> Parse(FirmwareExecutableArchitecture Arch)
        {
            // Getting FwBootmgr data via bcdedit execution
            log.Info("Started FwBootmgr parsing (BcdEdit)");
            string? EnumData = BcdEditBridge.ExecuteBcdedit("/enum {fwbootmgr}", true);
            if (EnumData == null)
            {
                log.Error("Failed to enumrate FwBootmgr. Bcdedit returned empty data");
                yield break;
            }

            // FwBootmgr contains bootorder which we are parsing
            foreach (Match entry in Regex.Matches(EnumData, @"{(\S+)}", RegexOptions.Multiline).Cast<Match>())
            {
                // Parsing values from enumerating each GUID from bootorder
                if (Guid.TryParse(entry.Groups[1].Value, out _))
                {
                    // No null
                    log.InfoFormat("Parsing {0} entry", entry.Groups[1].Value);
                    MenuEntryInfo? info = ParseMenuEntry(entry.Value);
                    if (info == null)
                        continue;

                    // no MS loaders
                    if (info.Loader.StartsWith(@"EFI\Microsoft", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    // no rEFInd loaders
                    if (info.Loader.StartsWith(@"EFI\refind", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    yield return info;
                }
            }
        }

        private static MenuEntryInfo? ParseMenuEntry(string EntryGuid)
        {
            // Enumerating entry data via bcdedit execution
            string? EntryData = BcdEditBridge.ExecuteBcdedit("/enum " + EntryGuid, true);
            if (string.IsNullOrWhiteSpace(EntryData))
            {
                // No empties
                log.Error("Failed to parse menuentry. Bcdedit returned empty data");
                return null;
            }

            Match DescriptionMatch = Regex.Match(EntryData, @"description\s+(.+)"); // Loader name
            Match PathMatch = Regex.Match(EntryData, @"path\s+(.+)");               // Loader path

            // No empties
            if (!DescriptionMatch.Success || !PathMatch.Success)
                return null;

            // Creating new menuentry from parsed data
            return new MenuEntryInfo()
            {
                //OSType = OSType.Linux, // Most likely, it will be a linux system, so...
                EntryName = DescriptionMatch.Groups[1].Value.Trim().FirstLetterToUpper(), // Some workaround for loaders name ;)
                Loader = PathMatch.Groups[1].Value.Trim().TrimStart('\\', '/') // Loader path
            };
        }
    }
}
