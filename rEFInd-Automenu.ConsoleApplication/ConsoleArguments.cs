using CommandLine;
using rEFInd_Automenu.Booting;
using rEFInd_Automenu.Configuration.MenuEntry;

namespace rEFInd_Automenu.ConsoleApplication
{
    [Verb("Install", aliases: new string[] { "Make", "Setup" }, HelpText = "Installing a boot manager on a computer or flash drive")]
    public class InstallArgumentsInfo
    {
        [Option("Computer", Group = "Install Destination", HelpText = "Installation option. rEFInd is installing on the current computer as the default bootloader. The argument accepts nothing")]
        public bool OnComputer { get; set; }

        [Option("FlashDrive", Group = "Install Destination", HelpText = "Installation option. rEFInd is unpacking onto the specified flash drive. The argument accepts the drive letter (for example \"F\") or the root directory (for example \"F:\\\") where you want to unpack the bootloader")]
        public DriveInfo? OnFlashDrive { get; set; }

        [Option('L', "Latest", HelpText = "Installation option. Before installation, need to download the latest version of the bootloader and install it instead of the saved one. Note: If the saved version is latest, the saved version will be installed. The argument accepts nothing")]
        public bool DownloadLatestBin { get; set; }

        [Option('T', "Theme", HelpText = "Installation option. The theme will be installed along with the loader. The argument takes the path to the directory containing resources and theme config (Example \"D:\\RefindThemes\\Regular\"). Note: The theme must have a \"theme.conf\" file or similar")]
        public DirectoryInfo? Theme { get; set; }

        [Option('A', "Arch", Default = EnvironmentArchitecture.None, HelpText = "Installation option. Instead of automatically detecting the professor's architecture, the specified one will be used, the argument takes the name of the architecture (For example, \"ARM64\"). Possible values: AMD64, X86, ARM64")]
        public EnvironmentArchitecture Architecture { get; set; }

        [Option('F', "Format", HelpText = "Installation option. Only applicable during installation on a flash drive. Indicates that the target flash drive must be formatted in FAT32 before installation, if it is different")]
        public bool FormatDrive { get; set; }

        [Option('X', "Force", HelpText = "Installation option. If conflicts arise during installation and this argument is specified, an attempt will be made to resolve them. Note: Not all conflicts can be resolved with this argument.\r\n\r\n")]
        public bool ForceWork { get; set; }
    }

    [Verb("Instance", aliases: new string[] { "Present", "Existent" }, HelpText = "Working with an instance of rEFInd already installed on your computer")]
    public class InstanceArgumentsInfo
    {
        [Option('I', "ShowInfo", Group = "Instance command", HelpText = "Action. Displays all known information about the installed rEFInd bootloader")]
        public bool ShowInfo { get; set; }

        [Option('R', "Remove", Group = "Instance command", HelpText = "Action. Uninstalls the current rEFInd bootloader and restores the Windows bootloader")]
        public bool RemoveBin { get; set; }

        [Option('U', "Update", Group = "Instance command", HelpText = "Action. Updates the installed rEFInd bootloader to the latest version")]
        public bool UpdateBin { get; set; }

        [Option('C', "OpenConfig", Group = "Instance command", HelpText = "Action. Opens the config file of the currently installed rEFInd bootloader")]
        public bool OpenConfig { get; set; }

        [Option('T', "InstallTheme", Group = "Instance command", HelpText = "Action. Sets the theme for the currently installed rEFInd bootloader. The argument takes the path to the directory containing resources and theme config (Example \"D:\\RefindThemes\\Regular\"). Note: The theme must have a \"theme.conf\" file or similar")]
        public DirectoryInfo? InstallTheme { get; set; }

        [Option("RegenConf", Group = "Instance command", HelpText = "Action. Regenerates the configuration file for the currently installed rEFInd bootloader")]
        public bool RegenConf { get; set; }

        [Option("RegenBoot", Group = "Instance command", HelpText = "Action. Reconfigures Bootmgr to load the currently installed rEFInd bootloader")]
        public bool RegenBoot { get; set; }
    }

    [Verb("Obtain", aliases: new string[] { "Get", "Gain" }, HelpText = "Obtaining rEFInd resources and related sources")]
    public class GetArgumentsInfo
    {
        [Option('E', "Embedded", Group = "Get commands", HelpText = "Action. Retrieves a prepared instance of the rEFInd installation image from program resources")]
        public bool ExtractResourceArchive { get; set; }

        [Option('A', "Archive", Group = "Get commands", HelpText = "Action. Downloads the specified version of the installation image from the official rEFInd repository. The argument accepts the version of the archive being searched (For example: \"0.13.2\") or the keyword \"latest\" to download the latest current version")]
        public string? DownloadSourceArchive { get; set; }

        [Option('S', "SysPar", Group = "Get commands", HelpText = "Action. Finds and attaches the Efi system partition to a free logical partition, or, if the partition is already connected, disconnects it")]
        public bool MountEsp { get; set; }
    }

    [Verb("Configuration", aliases: new string[] { "Config", "Settings" }, HelpText = "Managing global fields of current instance's configuration")]
    public class GlobalConfigurationArgumentsInfo
    {
        [Option('S', "Set", Group = "Config commands", HelpText = "Action. Creates or overrides a global configuration field. The argument takes the name of a global field (For example, \"timeout\"). Note: You need to provide a second parameter \"--Value\" in which you specify the value you want to set.")]
        public string? SetGlobalFieldValue { get; set; }

        [Option('R', "Remove", Group = "Config commands", HelpText = "Action. Removes a global configuration field. The argument takes the name of a global field (For example, \"timeout\")")]
        public string? RemoveGlobalFieldValue { get; set; }

        [Option('G', "Get", Group = "Config commands", HelpText = "Action. Reads and prints the value of the global configuration field. The argument takes the name of a global field (For example, \"timeout\")")]
        public string? ShowGlobalFieldValue { get; set; }

        [Option('V', "Value", HelpText = "Auxiliary parameter. Specifies a new value for the global configuration field")]
        public string? ValueToSet { get; set; }
    }

    [Verb("MenuEntry", aliases: new string[] { "BootOption", "Record" }, HelpText = "Managing boot entries of current instance's configuration")]
    public class EntriesConfigurationArgumentsInfo
    {
        [Option('C', "Create", Group = "Entry commands", HelpText = "Action. Creates a new MenuEntry in the configuration. The argument takes the name of the new MenuEntry")]
        public string? CreateEntry { get; set; }

        [Option('R', "Remove", Group = "Entry commands", HelpText = "Action. Removes an existing MenuEntry from the configuration. The argument accepts a MenuEntry that needs to be removed")]
        public string? RemoveEntry { get; set; }

        [Option('E', "Edit", Group = "Entry commands", HelpText = "Action. Changes MenuEntry values ​​such as Loader or Volume. The argument takes the name of the MenuEntry that needs to be changed. Note: You must specify one or more parameters from the value argument group")]
        public string? EditEntry { get; set; }

        [Option('G', "Get", Group = "Entry commands", HelpText = "Action. Reads from configuration menuentry and prints it. The argument takes the name of the MenuEntry or the keyword \"enum\" to enumerate all entries. Note: You must specify one or more parameters from the value argument group.")]
        public string? ShowEntry { get; set; }

        [Option("EntryName", HelpText = "Argument-Value. Boot entry name")]
        public string? MenuEntrySetEntryName { get; set; }

        [Option("Volume", HelpText = "Argument-Value. GUID identifier of the volume on which the bootloader is located")]
        public Guid? MenuEntrySetVolume { get; set; }

        [Option("Loader", HelpText = "Argument-Value. The path to the bootloader is relative to ESP or volume if specified")]
        public string? MenuEntrySetLoader { get; set; }

        [Option("InitRD", HelpText = "Argument-Value. Path to the temporary filesystem image used by the Linux kernel")]
        public string? MenuEntrySetInitRD { get; set; }

        [Option("FirmwareBootnum", HelpText = "Argument-Value. Boot loader's boot Number\\Index")]
        public short? MenuEntrySetFirmwareBootnum { get; set; }

        [Option("Icon", HelpText = "Argument-Value. Path to the icon displayed in the boot menu")]
        public string? MenuEntrySetIcon { get; set; }

        [Option("OSType", HelpText = "Argument-Value. The type of operating system being loaded. Possible values: MacOS, Linux, ELILO, Windows, XOM")]
        public OSType? MenuEntrySetOSType { get; set; }

        [Option("Graphics", HelpText = "Enables or disables a graphical boot mode")]
        public bool? MenuEntrySetGraphics { get; set; }

        [Option("Options", HelpText = "Argument-Value. List of arguments passed to the kernel before loading")]
        public string? MenuEntrySetOptions { get; set; }

        [Option("Disabled", HelpText = "Argument-Value. Disable an entry")]
        public bool? MenuEntrySetDisabled { get; set; }
    }
}
