using rEFInd_Automenu.Configuration.MenuEntry;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace rEFInd_Automenu.Configuration.Parsing
{
    public static class MenuEntryParser
    {
        private static readonly Dictionary<string, PropertyInfo> _MenuEntryInfoNamesHashMap = ConfigurationFileParser.InitInfoNamesHashMap(typeof(MenuEntryInfo));

        public static void ParseMenuEntryStrcture(StreamReader fileReader, string HeaderLine, List<MenuEntryInfo> configuration)
        {
            // Entry instance
            MenuEntryInfo entryInfo = new MenuEntryInfo();

            // Parsing header
            Match headerMatch = Regex.Match(HeaderLine, "menuentry (.+){");
            if (!headerMatch.Success)
                return; // FATAL

            // Setting entry name
            entryInfo.EntryName = headerMatch.Groups[1].Value.Trim(' ', '\"');

            // Parsing menuentry strcture 
            while (!fileReader.EndOfStream)
            {
                string? EntryLine = fileReader.ReadLine();
                
                // Empty line
                if (string.IsNullOrEmpty(EntryLine))
                    continue; // SKIP

                // Comment
                EntryLine = EntryLine.TrimStart('\t');
                if (EntryLine.StartsWith(MenuEntryStructureTokens.CommentingSymbol))
                    continue; // SKIP

                // Strcture closed
                if (EntryLine.StartsWith(MenuEntryStructureTokens.StructureEnd))
                    break; // STOP

                // Another structure
                if (EntryLine.StartsWith("menuentry"))
                {
                    ParseMenuEntryStrcture(fileReader, EntryLine, configuration);
                    break; // STOP
                }

                // Settings token
                ConfigurationTokenParser.ProcessLineOfConfig(EntryLine, entryInfo, _MenuEntryInfoNamesHashMap);
            }

            // Adding new menuEntry info
            configuration.Add(entryInfo);
        }

        private static class MenuEntryStructureTokens
        {
            public const string Menuentry = "menuentry";
            public const string SubMenuentry = "submenuentry";
            public const string StructureEnd = "}";
            public const string CommentingSymbol = "#";
        }
    }
}
