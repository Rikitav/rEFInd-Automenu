using log4net;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace rEFInd_Automenu.RuntimeConfiguration
{
    public class ProgramConfiguration
    {
        // Private fields
        private static ProgramConfiguration? _Instance;
        private static string _ConfFilePath = Path.Combine(Environment.CurrentDirectory, "refind.cfg");
        private static readonly ILog log = LogManager.GetLogger(typeof(ProgramConfiguration));

        // Instance singleton
        public static ProgramConfiguration Instance
        { 
            get => _Instance ??= new ProgramConfiguration();
        }

        // Configuration properties
        public bool LogInterfaceExecution { get; set; } = false;
        public bool LogInterfaceResults { get; set; } = false;
        public bool PreferMountvolEspSearch { get; set; } = false;
        public bool PreferBootmgrBooting { get; set; } = false;
        public LoaderScannerType LoaderScannerType { get; set; } = LoaderScannerType.NvramLoadOptionReader;

        private ProgramConfiguration()
        {
            Type thisType = GetType();
            Regex lineRegex = new Regex(@"(\S+)\s?=\s?(\S+)");
            log.InfoFormat("Reading program's config file on \"{0}\"", _ConfFilePath);

            if (!File.Exists(_ConfFilePath))
            {
                // Config file doesnt exist
                log.Error("Program's config file doesnt exist");
                return;
            }

            int lineIndex = 1;
            foreach (string ConfLine in File.ReadLines(_ConfFilePath))
            {
                // Empty
                if (string.IsNullOrEmpty(ConfLine))
                {
                    // Empty line
                    continue;
                }

                // Comment
                if (ConfLine.StartsWith("#"))
                {
                    // comment line
                    continue;
                }

                // Parsing line
                Match lineMatch = lineRegex.Match(ConfLine);
                if (!lineMatch.Success)
                {
                    // failed to parse
                    log.WarnFormat("Failed to parse line on {0}. Skipping", lineIndex);
                    continue;
                }

                // Getting associated property
                PropertyInfo? confProp = thisType.GetProperty(lineMatch.Groups[1].Value, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if (confProp == null)
                {
                    // Property not found
                    log.WarnFormat("Program's config property \"{0}\" doesnt exist. Skipping", lineMatch.Groups[1].Value);
                    continue;
                }

                // Try format value
                string confLineValue = lineMatch.Groups[2].Value;
                log.InfoFormat("Setting {0} config property value of type {1}", confLineValue, confProp.PropertyType.Name);
                switch (confProp.PropertyType.Name)
                {
                    case "String":
                        {
                            if (string.IsNullOrEmpty(confLineValue))
                            {
                                // Failed to format
                                log.WarnFormat("Invalid property value on line {0}", lineIndex);
                                continue;
                            }

                            confProp.SetValue(this, confLineValue);
                            break;
                        }

                    case "Boolean":
                        {
                            if (!bool.TryParse(confLineValue, out bool value))
                            {
                                // Failed to format
                                log.WarnFormat("Invalid property value on line {0}. Boolean expected", lineIndex);
                                continue;
                            }

                            confProp.SetValue(this, value);
                            break;
                        }

                    case var _ when confProp.PropertyType.IsEnum:
                        {
                            if (!Enum.TryParse(confProp.PropertyType, confLineValue, out object? value))
                            {
                                // Failed to format
                                log.WarnFormat("Invalid property value on line {0}. Expceted {1}", lineIndex, string.Join(" / ", Enum.GetNames(confProp.PropertyType)));
                                continue;
                            }

                            confProp.SetValue(this, value);
                            break;
                        }
                }

                // line index
                lineIndex++;
            }
        }
    }
}
