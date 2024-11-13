using log4net;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text;

namespace rEFInd_Automenu.RuntimeConfiguration
{
    public class ProgramConfiguration
    {
        /// <summary>
        /// Specifies that the program should log the start and end of tasks
        /// </summary>
        [Description("Specifies that the program should log the start and end of tasks")]
        public bool LogInterfaceExecution { get; set; } = false;

        /// <summary>
        /// Specifies that the program should log the return value of tasks upon completion.
        /// Will log the completion of tasks even if the `LogInterfaceExecution` key is not specified
        /// </summary>
        [Description("Specifies that the program should log the return value of tasks upon completion.\nWill log the completion of tasks even if the `LogInterfaceExecution` key is not specified")]
        public bool LogInterfaceResults { get; set; } = false;

        /// <summary>
        /// If the __WinNT kernel version is greater than 6__, to work with an instance of the boot manager and install it on the computer,
        /// the program will look for the Volume identifier of the EFI system partition, otherwise the program will use MountVol.
        /// This key specifies that the program should use Mountvol ESP search even if the kernel version is greater than 6
        /// </summary>
        [Description(
            "If the __WinNT kernel version is greater than 6__, to work with an instance of the boot manager and install it on the computer,\n" +
            "the program will look for the Volume identifier of the EFI system partition, otherwise the program will use MountVol.\n" +
            "This key specifies that the program should use Mountvol ESP search even if the kernel version is greater than 6")]
        public bool PreferMountvolEspSearch { get; set; } = false;

        /// <summary>
        /// This option specifies that the program should overwrite the current system's Bootmgr instead of creating its own boot entry.
        /// During the Bootmgr census process, its backup copy will be created, the identifier of which will be recorded in the `BackupedBootmgrIdentificator` registry key
        /// </summary>
        [Description(
            "This option specifies that the program should overwrite the current system's Bootmgr instead of creating its own boot entry.\n" +
            "During the Bootmgr census process, its backup copy will be created, the identifier of which will be recorded in the `BackupedBootmgrIdentificator` registry key")]
        public bool PreferBootmgrBooting { get; set; } = false;

        /// <summary>
        /// <para>Description - LoaderScannerType.EspDirectoryEnumerator - 
        /// This key specifies that the program should search for bootloaders on the ESP, going through the directories located on it.
        /// This option can help if, for example, Windows ***accidentally*** killed the bootloader of a Linux Distribution.</para>
        /// 
        /// <para>Description - LoaderScannerType.FwBootmgrRecordParser - 
        /// This key specifies that the program should look for bootloaders in FwBootmgr, parsing values from the output of the BcdEdit program.
        /// This option should be used if LoaderScannerType.NvramLoadOptionReader does not work correctly.</para>
        /// 
        /// <para>Description - LoaderScannerType.NvramLoadOptionReader - 
        /// This key specifies that the program should search for bootloaders in the Nvram of the UEFI, using an enumeration of all `Boot####` boot entries.
        /// Recommended scanner, as it works directly with the firmware. Used by default</para>
        /// 
        /// <para>This parameter specifies which bootloader scanning method will be used during configuration file generation.
        /// If this parameter is not specified, the program will use the most convenient scanner.</para>
        /// </summary>
        [Description(
            "Description - LoaderScannerType.EspDirectoryEnumerator\n" +
            "This key specifies that the program should search for bootloaders on the ESP, going through the directories located on it.\n" +
            "This option can help if, for example, Windows ***accidentally*** killed the bootloader of a Linux Distribution.\n\n" +

            "Description - LoaderScannerType.FwBootmgrRecordParser\n" +
            "This key specifies that the program should look for bootloaders in FwBootmgr, parsing values from the output of the BcdEdit program.\n" +
            "This option should be used if LoaderScannerType.NvramLoadOptionReader does not work correctly.\n\n" +

            "Description - LoaderScannerType.NvramLoadOptionReader\n" +
            "This key specifies that the program should search for bootloaders in the Nvram of the UEFI, using an enumeration of all `Boot####` boot entries.\n" +
            "Recommended scanner, as it works directly with the firmware. Used by default\n\n" +

            "This parameter specifies which bootloader scanning method will be used during configuration file generation.\n" +
            "If this parameter is not specified, the program will use the most convenient scanner.")]
        public LoaderScannerType LoaderScannerType { get; set; } = LoaderScannerType.NvramLoadOptionReader;

        /// <summary>
        /// Allows the program to create log files necessary for debugging
        /// </summary>
        [Description("Allows the program to create log files necessary for debugging")]
        public bool AllowCreateLogFiles { get; set; } = false;

        /// <summary>
        /// Allows the program to create local loader resource files
        /// </summary>
        [Description("Allows the program to create local loader resource files")]
        public bool AllowCreateLocalResource { get; set; } = false;

        /// <summary>
        /// Allows a program to read and create Windows register values ​​in its branch
        /// </summary>
        [Description("Allows a program to read and create Windows register values ​​in its branch")]
        public bool AllowWindowsRegistryReadWrite { get; set; } = false;

        /* Maybe...
        public int LogLevel { get; set; } = 0;
        */

        // Private fields
        private static ProgramConfiguration? _Instance;
        private static readonly string _ConfFilePath = Path.Combine(Environment.CurrentDirectory, "refind.cfg");
        private static readonly ILog log = LogManager.GetLogger(typeof(ProgramConfiguration));
        private readonly int currentParsingLineIndex = 0;

        // Instance singleton
        public static ProgramConfiguration Instance
        { 
            get => _Instance ??= new ProgramConfiguration();
        }

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
                // line index, this property not for parsing but for config editing
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

        public void RewriteConfigFile(bool AddDescription)
        {
            // Overriding configs data
            log.Info("Rewriting program's config file");
            using StreamWriter writer = new StreamWriter(_ConfFilePath, false, Encoding.UTF8);
            StringBuilder propertyBuilder = new StringBuilder();
            
            // Enumerating config properties
            foreach (PropertyInfo prop in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                // Getting started
                propertyBuilder.Clear();

                // Getting description of property
                string propDescription = AddDescription
                    ? prop.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "{! Failed to get description !}"
                    : string.Empty;

                // Getting property name
                string propName = prop.Name;

                // Getting current property value
                string propValue = GetConfigDefaultValue(prop.PropertyType, prop.GetValue(this));

                // Building property section
                if (AddDescription)
                {
                    propertyBuilder.AppendLine("\n#");
                    propertyBuilder.AppendLine("# " + propDescription);
                    propertyBuilder.AppendLine("#");
                }

                propertyBuilder.AppendLine(string.Format("{0} = {1}\n", propName, propValue));

                // Writing
                log.InfoFormat("Writing \'{0}\' property with value \'{1}\'", propName, propValue);
                writer.Write(propertyBuilder.ToString());
            }
        }

#pragma warning disable 8603
        private string GetConfigDefaultValue(Type propType, object? propValue)
        {
            try
            {
                switch (propType.Name)
                {
                    case nameof(Int32):
                        {
                            if (propValue == null)
                                return 0.ToString();

                            return propValue.ToString();
                        }

                    case nameof(Boolean):
                        {
                            if (propValue == null)
                                return false.ToString();

                            return propValue.ToString();
                        }

                    case var _ when propType.IsEnum:
                        {
                            if (propValue == null)
                                return "<undefined>";

                            return propValue.ToString();
                        }
                }
            }
            catch
            {
                return "<NULL>";
            }

            return "<NULL>";
        }
#pragma warning restore

        private bool ParsePropertyLine(string configLine, [NotNullWhen(true)] out PropertyInfo? associatedProp, [NotNullWhen(true)] out string? propValue)
        {
            // Seting default values
            associatedProp = null;
            propValue = null;

            // Parsing line
            string[] lineSplit = configLine.Split('=', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
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
            propValue = lineSplit[1];
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
