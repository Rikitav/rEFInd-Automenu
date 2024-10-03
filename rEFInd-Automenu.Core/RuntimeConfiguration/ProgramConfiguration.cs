using log4net;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace rEFInd_Automenu.RuntimeConfiguration
{
    public class ProgramConfiguration
    {
        // Private fields
        private static ProgramConfiguration? _Instance;
        private static string _ConfFilePath = Path.Combine(Environment.CurrentDirectory, "refind.cfg");
        private static readonly ILog log = LogManager.GetLogger(typeof(ProgramConfiguration));
        private readonly int currentParsingLineIndex = 0;

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
            log.InfoFormat("Reading program's config file on \"{0}\"", _ConfFilePath);

            if (!File.Exists(_ConfFilePath))
            {
                // Config file doesnt exist
                log.Error("Program's config file doesnt exist");
                return;
            }

            currentParsingLineIndex = 0;
            foreach (string ConfLine in File.ReadLines(_ConfFilePath))
            {
                // line index
                currentParsingLineIndex += 1;

                // Empty line
                if (string.IsNullOrWhiteSpace(ConfLine))
                    continue;

                // Comment
                if (ConfLine.StartsWith("#"))
                    continue;

                // Parsing line
                if (!ParsePropertyLine(ConfLine, out PropertyInfo? associatedProp, out string? propValue))
                    continue;

                // Try assign property value
                AssignPropertyValue(associatedProp, propValue);
            }
        }

        private bool ParsePropertyLine(string configLine, [NotNullWhen(true)] out PropertyInfo? associatedProp, [NotNullWhen(true)] out string? propValue)
        {
            // Seting default values
            associatedProp = null;
            propValue = null;

            // Parsing line
            string[] lineSplit = configLine.Split('=', StringSplitOptions.RemoveEmptyEntries);
            if (lineSplit.Length < 2)
            {
                log.WarnFormat("Failed to parse config property on line #{0}. Invalid property formatting. Skipping", currentParsingLineIndex);
                return false;
            }

            // Getting associated property
            associatedProp = GetType().GetProperty(lineSplit[0], BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (associatedProp == null)
            {
                log.WarnFormat("Fialed to parse config property on line #{0}. Property called \"{1}\" does not exist in the config. Skipping", currentParsingLineIndex, lineSplit[0]);
                return false;
            }

            // Success
            propValue = lineSplit[1].Trim();
            return true;
        }

        private bool AssignPropertyValue(PropertyInfo associatedProp, string stringValue)
        {
            object? Value = null;
            switch (associatedProp.PropertyType.Name)
            {
                case nameof(String):
                    {
                        if (string.IsNullOrWhiteSpace(stringValue))
                        {
                            // Failed to format
                            log.WarnFormat("Invalid property value on line #{0}", currentParsingLineIndex);
                            return false;
                        }

                        Value = stringValue;
                        break;
                    }

                case nameof(Boolean):
                    {
                        if (!bool.TryParse(stringValue, out bool boolValue))
                        {
                            log.WarnFormat("Invalid property value on line #{0}. Boolean expected", currentParsingLineIndex);
                            return false;
                        }

                        Value = boolValue;
                        break;
                    }

                case var _ when associatedProp.PropertyType.IsEnum:
                    {
                        if (!Enum.TryParse(associatedProp.PropertyType, stringValue, out object? enumValue))
                        {
                            log.WarnFormat("Invalid property value on line #{0}. One of values was expceted {1}", currentParsingLineIndex, string.Join(" / ", Enum.GetNames(associatedProp.PropertyType)));
                            return false;
                        }

                        Value = enumValue;
                        break;
                    }
            }

            if (Value == null)
            {
                log.WarnFormat("Failed to assign property value. Value was null");
                return false;
            }

            associatedProp.SetValue(this, Value);
            log.InfoFormat("Property config \"{0}\" was set value \"{1}\", line #{2}", associatedProp.PropertyType.Name, Value, currentParsingLineIndex);
            return true;
        }
    }
}
