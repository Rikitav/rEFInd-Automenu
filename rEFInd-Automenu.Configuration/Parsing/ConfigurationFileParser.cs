using rEFInd_Automenu.Configuration.GlobalConfiguration;
using rEFInd_Automenu.Configuration.MenuEntry;
using rEFInd_Automenu.Configuration.Serializing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace rEFInd_Automenu.Configuration.Parsing
{
    public static class ConfigurationFileParser
    {
        private static readonly Dictionary<string, PropertyInfo> _GlobalConfigurationNamesHashMap = InitInfoNamesHashMap(typeof(RefindGlobalConfigurationInfo));

        public static RefindConfiguration FromFile(string fullFileName)
        {
            // Creating configuration info instance
            RefindConfiguration configuration = new RefindConfiguration()
            {
                Global = new RefindGlobalConfigurationInfo(),
                Entries = new List<MenuEntryInfo>()
            };

            // Opening file reader
            using FileStream stream = File.Open(fullFileName, FileMode.Open);
            using StreamReader fileReader = new StreamReader(stream);
            StreamReadConfig(fileReader, configuration);
            return configuration;
        }

        public static RefindConfiguration FromStream(Stream stream)
        {
            // Creating configuration info instance
            RefindConfiguration configuration = new RefindConfiguration()
            {
                Global = new RefindGlobalConfigurationInfo(),
                Entries = new List<MenuEntryInfo>()
            };

            // Opening file reader
            using StreamReader fileReader = new StreamReader(stream);
            StreamReadConfig(fileReader, configuration);
            return configuration;
        }

        private static void StreamReadConfig(StreamReader fileReader, RefindConfiguration configuration)
        {
            // Reading
            while (!fileReader.EndOfStream)
            {
                // Reading
                string? ConfigLine = fileReader.ReadLine();

                // Empty line checking
                if (string.IsNullOrEmpty(ConfigLine))
                    continue;

                // Comment line checking
                ConfigLine = ConfigLine.Trim();
                if (ConfigLine.StartsWith('#'))
                    continue;

                // Processing line
                if (ConfigLine.StartsWith("menuentry", StringComparison.CurrentCultureIgnoreCase))
                {
                    // Parsing menuentry structure
                    MenuEntryParser.ParseMenuEntryStrcture(fileReader, ConfigLine, configuration.Entries);
                }
                else
                {
                    // Parsing global setting
                    ConfigurationTokenParser.ProcessLineOfConfig(ConfigLine, configuration.Global, _GlobalConfigurationNamesHashMap);
                }
            }
        }

        public static Dictionary<string, PropertyInfo> InitInfoNamesHashMap(Type InfoType)
        {
            return InfoType
                .GetProperties() // Get properties
                .Select(prop => new KeyValuePair<string, PropertyInfo>(prop.GetCustomAttribute<ConfigFileElementAttribute>()?.ElementName ?? string.Empty, prop)) // Make pairs
                .Where(p => !string.IsNullOrEmpty(p.Key)) // Clean nulls
                .ToDictionary(prop => prop.Key, prop => prop.Value); // Comnvert to HashMap
        }
    }
}
