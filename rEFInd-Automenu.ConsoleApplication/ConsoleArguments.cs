using CommandLine;
using rEFInd_Automenu.Booting;

namespace rEFInd_Automenu.ConsoleApplication
{
    [Verb("Install", HelpText = "Contains options for installing rEFInd on a computer or extracting it to a USB flash drive")]
    public class InstallArgumentsInfo
    {
        [Option("Computer", Group = "Install Destination", HelpText = "Specifies that rEFInd should be installed on the current computer. High parsing priority")]
        public bool OnComputer { get; set; }

        [Option("FlashDrive", Group = "Install Destination", HelpText = "Specifies that rEFInd should be installed on removable storage as fallback loader. The parameter must contain a drive letter (for example \"C\") or a path to the root directory (for example \"C:\\\"). Medium parsing priority")]
        public DriveInfo? OnFlashDrive { get; set; }

        [Option('L', "Latest", HelpText = "Download latest version of rEFInd from SourceForge.com before installation")]
        public bool DownloadLatestBin { get; set; }

        [Option('T', "Theme", HelpText = "Set path to your Theme folder. The parameter must contain the path to the directory (For example, \"C:\\rEFInd\\Bin\"), and target directory must contain theme configuration file named \"Theme.conf\"")]
        public DirectoryInfo? Theme { get; set; }

        [Option('A', "Arch", Default = EnvironmentArchitecture.None, HelpText = "Force set installation arcitecture \nPermissible values : \"AMD64, ARM64, x86\"")]
        public EnvironmentArchitecture Architecture { get; set; }

        [Option('F', "Format", HelpText = "Indicates that the flash drive must be formatted before installation if it has an invalid file system")]
        public bool FormatDrive { get; set; }

        [Option('X', "Force", HelpText = "Trying to fix some errors during installation, such as trying to remove an existing rEFInd instance on the computer")]
        public bool ForceWork { get; set; }
    }

    [Verb("Instance", HelpText = "Working with an instance of rEFInd already installed on your computer")]
    public class InstanceArgumentsInfo
    {
        [Option('R', "Remove", Group = "Instance command", HelpText = "Remove rEFInd from current Computer")]
        public bool RemoveBin { get; set; }

        [Option('U', "Update", Group = "Instance command", HelpText = "If rEFInd already installed on computer, reinstall rEFInd to newest version and regenerate config file\nElse just install rEFInd with clear configuration")]
        public bool UpdateBin { get; set; }

        [Option('C', "Config", Group = "Instance command", HelpText = "If rEFInd already installed on computer, this parametr programm will open \"refind.conf\"")]
        public bool OpenConfig { get; set; }

        [Option("RegenConf", Group = "Instance command", HelpText = "If your config file was corrupted, you can regenerate it")]
        public bool RegenConf { get; set; }

        [Option("RegenBoot", Group = "Instance command", HelpText = "If rEFInd won't load and you think it's because BootMgr has redrawn, you can rewrite it to load rEFInd again")]
        public bool RegenBoot { get; set; }
    }

    [Verb("Get", HelpText = "Options related to obtaining rEFInd resources")]
    public class GetArgumentsInfo
    {
        [Option('E', "Embedded", Group = "Get commands", HelpText = "Extract ZIP archive with rEFInd from program resources")]
        public bool ExtractResourceArchive { get; set; }

        [Option('A', "Archive", Group = "Get commands", HelpText = "Downloads the rEFInd archive from SourceForge. You must specify the archive version (For example, \"0.14.0\"), or \"latest\" to download the latest version")]
        public string? DownloadSourceArchive { get; set; }

        [Option('S', "SysPar", Group = "Get commands", HelpText = "Mounts the efi system partition to a visible explorer, or unmount if its already have mount point")]
        public bool MountEsp { get; set; }
    }
}
