using log4net;
using rEFInd_Automenu.Booting;
using rEFInd_Automenu.Configuration.GlobalConfiguration;
using rEFInd_Automenu.Configuration.MenuEntry;
using rEFInd_Automenu.Configuration.Serializing;
using rEFInd_Automenu.TypesExtensions;
using Rikitav.IO.ExtensibleFirmware.SystemPartition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace rEFInd_Automenu.Configuration
{
    public class ConfigurationFileBuilder
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ConfigurationFileBuilder));

        // Settings
        private readonly string _InstallationDirectory;

        // Factory instance
        private string IconsDirectory
        {
            get => ThemeDirectory?.FullName ?? Path.Combine(_InstallationDirectory, "icons");
        }

        public RefindConfiguration ConfigurationInstance
        {
            get;
            private set;
        }

        public DirectoryInfo? ThemeDirectory
        {
            get;
            set;
        }

        public ConfigurationFileBuilder(string installationDirectory)
            : this(new RefindConfiguration(), installationDirectory) { }

        public ConfigurationFileBuilder(string installationDirectory, DirectoryInfo? formalizationThemePath)
            : this(new RefindConfiguration(), installationDirectory, formalizationThemePath) { }

        public ConfigurationFileBuilder(RefindConfiguration configuration, string installationDirectory)
        {
            _InstallationDirectory = installationDirectory ?? throw new ArgumentNullException(nameof(installationDirectory));
            ConfigurationInstance = configuration;
        }

        public ConfigurationFileBuilder(RefindConfiguration configuration, string installationDirectory, DirectoryInfo? formalizationThemeDir)
        {
            _InstallationDirectory = installationDirectory ?? throw new ArgumentNullException(nameof(installationDirectory));
            ThemeDirectory = formalizationThemeDir;
            ConfigurationInstance = configuration;
        }

        public void WriteConfigurationToFile(string FileName)
        {
            // Writing all of data to text file
            log.InfoFormat("Writing builded configuration instance to file - {0}", Path.Combine(_InstallationDirectory, FileName));
            using StreamWriter writer = File.CreateText(Path.Combine(_InstallationDirectory, FileName));
            ConfigFileFormatter.FormatToString(ConfigurationInstance, writer);
        }

        public static void WriteConfigurationToFile(RefindConfiguration configuration, string FullPath)
        {
            // Writing all of data to text file
            log.InfoFormat("Writing configuration instance to file - {0}", FullPath);
            using StreamWriter writer = File.CreateText(FullPath);
            ConfigFileFormatter.FormatToString(configuration, writer);
        }

        public void ConfigureStaticPlatform()
        {
            // Global field initialization
            log.Info("Configuring global information for static platform boot manager");
            ConfigurationInstance.Global ??= new RefindGlobalConfigurationInfo();

            // Setting defaults
            ConfigurationInstance.Global.ShowTools = ShowTools.firmware | ShowTools.bootorder | ShowTools.about | ShowTools.shutdown | ShowTools.exit;
            ConfigurationInstance.Global.Timeout = 30;
            ConfigurationInstance.Global.ShutdownAfterTimeout = false;

            // Setting scan source
            ConfigurationInstance.Global.ScanForSources = ScanFor.manual | ScanFor.external;
            if (DriveInfo.GetDrives().Any(d => d.DriveType == DriveType.CDRom))
                ConfigurationInstance.Global.ScanForSources |= ScanFor.cd | ScanFor.optical;

            // Add formalization theme configuration if has
            if (ThemeDirectory != null)
            {
                log.Info("Added formalization theme configuration");
                ConfigurationInstance.Global.Include = "theme/theme.conf";
            }
        }

        public void ConfigureDynamicPlatform()
        {
            // Global field initialization
            log.Info("Configuring global information for dynamic platform boot manager");
            ConfigurationInstance.Global ??= new RefindGlobalConfigurationInfo();

            // Setting defaults
            ConfigurationInstance.Global.ShowTools = ShowTools.firmware | ShowTools.gdisk | ShowTools.shell | ShowTools.memtest | ShowTools.gptsync | ShowTools.shutdown | ShowTools.exit;
            ConfigurationInstance.Global.Timeout = 0;
            ConfigurationInstance.Global.ShutdownAfterTimeout = false;

            // Setting scan source
            ConfigurationInstance.Global.ScanForSources = ScanFor.manual;

            // Add formalization theme configuration if has
            if (ThemeDirectory != null)
            {
                log.Info("Added formalization theme configuration");
                ConfigurationInstance.Global.Include = "theme/theme.conf";
            }
        }

        public void ParseConfigurationEntries(ILoadersScanner loadersScanner, FirmwareExecutableArchitecture Arch)
        {
            // Init menu entry list
            log.InfoFormat("Scanning for loaders using {0}", loadersScanner.GetType().Name);
            ConfigurationInstance.Entries ??= new List<MenuEntryInfo>();

            // Parsing entries
            foreach (MenuEntryInfo menuEntry in loadersScanner.Parse(Arch))
            {
                log.InfoFormat("Added new MenuEntryInfo - \"{0}\", \"{1}\", \"{2}\"", menuEntry.EntryName, menuEntry.Loader, menuEntry.Icon ?? "noIcon");
                ConfigurationInstance.Entries.Add(menuEntry);
            }
        }

        public void AssignLoaderIcons()
        {
            foreach (MenuEntryInfo menuEntry in ConfigurationInstance.Entries)
            {
                // fetching loader root directory and getting icon
                string LoaderRootName = menuEntry.Loader.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries)[1];
                string IconName = Path.Combine(IconsDirectory, MenuEntryIconsAliases.GetIconName(LoaderRootName.ToLower()));

                // assigning icon
                if (File.Exists(IconName))
                    menuEntry.Icon = IconName.Substring(IconName.IndexOf("EFI") - 1);
            }
        }

        public void AddWindowsMenuEntry()
        {
            // Init
            ConfigurationInstance.Entries ??= new List<MenuEntryInfo>();

            // Getting windows bootefimgr directory
            DirectoryInfo WinBootDir = EspFinder.EspDirectory.GetSubDirectory("EFI\\microsoft\\boot");

            // Creatong boot entry
            ConfigurationInstance.Entries.Add(new MenuEntryInfo()
            {
                EntryName = "Windows",
                Volume = EfiPartition.Identificator,
                Loader = Path.Combine("EFI", "microsoft", "boot", WinBootDir.EnumerateFiles("boot*.efi").First().Name),
                OSType = OSType.Windows
            });
        }
    }
}
