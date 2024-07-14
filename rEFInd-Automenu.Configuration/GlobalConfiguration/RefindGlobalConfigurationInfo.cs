using rEFInd_Automenu.Configuration.Serializing;

#pragma warning disable CS8618
namespace rEFInd_Automenu.Configuration.GlobalConfiguration
{
    public class RefindGlobalConfigurationInfo
    {
        /// <summary>
        /// Sets the timeout period in seconds. If 0, the timeout is disabled—rEFInd waits indefinitely for user input. If -1, rEFInd will normally boot immediately to the default selection; however, if a shortcut key (for instance, W for Windows) is pressed, that system will boot instead. If any other key is pressed, the menu will show with no timeout.
        /// </summary>
        [ConfigFileElement("timeout")]
        public int? Timeout
        {
            get;
            set;
        }

        /// <summary>
        /// If set to false or one of its synonyms (the default), rEFInd boots the option set via default_selection when the timeout period is reached. When set to true or one of its synonyms, rEFInd attempts to shut down the computer when the timeout period is reached. Many older EFIs lack software shutdown support. On them, setting this option to true will cause a reboot or a hang once the timeout value is reached!
        /// </summary>
        [ConfigFileElement("shutdown_after_timeout")]
        public bool? ShutdownAfterTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the logging level. The default level of 0 performs no logging. Higher values cause rEFInd to log information on its activity to the refind.log file, which resides in the directory from which rEFInd launched; or in the root of the first ESP that rEFInd can locate, if rEFInd's launch directory is read-only (as when rEFInd is launched from an HFS+ volume). The resulting log file is in UTF-16 format, so you'll need to read it with a text editor or other tool that can parse UTF-16. This feature is intended to help with debugging problems; the log level should be kept at 0 in normal operation. Increasing the log level, especially above 1, can degrade rEFInd's performance.
        /// </summary>
        [ConfigFileElement("log_level")]
        public int? LoggingLevel
        {
            get;
            set;
        }

        /// <summary>
        /// If set to true, on, or 1, stores rEFInd-specific variables in NVRAM. If set to false, off, or 0, stores these variables in the vars subdirectory of rEFInd's home directory; in the refind-vars directory on the first ESP that rEFInd can identify, if rEFInd's own directory cannot be modified (for instance, if it's an HFS+ volume); or in NVRAM, if both disk storage options fail. Using NVRAM ties rEFInd's variables to a specific computer and increases wear on the NVRAM, whereas storing them on disk makes the variables move with rEFInd (which is potentially useful on a rEFInd installation on a removable disk). The internal default is true; however, use_nvram false is set in the default refind.conf file, so rEFInd will use disk storage by default on a fresh installation. This option has no effect on storage of non-rEFInd-specific variables, such as LoaderDevicePartUUID (if write_systemd_vars is enabled) or BootNext (used by the loaders that reboot to EFI-defined boot programs).
        /// </summary>
        [ConfigFileElement("use_nvram")]
        public bool? UseNVRam
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the number of seconds of inactivity before the screen blanks to prevent burn-in. The display returns after most keypresses (unfortunately, not including modifiers such as Shift, Control, Alt, or Option). The default is 0, which disables this feature. Setting this token to -1 causes a blank display until the timeout value passes or you press a key.
        /// </summary>
        [ConfigFileElement("screensaver")]
        public int? Screensaver
        {
            get;
            set;
        }

        /// <summary>
        /// Removes the specified user interface features:
        /// <list type="bullet">        
        ///     <item> <term>banner</term> <description>removes the banner graphic or background image</description> </item>
        ///     <item> <term>label</term> <description>removes the text description of each tag and the countdown timer</description> </item>
        ///     <item> <term>singleuser</term> <description>removes the single-user option from the macOS sub-menu</description> </item>
        ///     <item> <term>safemode</term> <description>removes the option to boot to safe mode from the macOS sub-menu</description> </item>
        ///     <item> <term>hwtest</term> <description>removes the Macintosh hardware test option</description> </item>
        ///     <item> <term>arrows</term> <description>removes the arrows to the right or left of the OS tags when rEFInd finds too many OSes to display simultaneously</description> </item>
        ///     <item> <term>hints</term> <description>removes the brief description of what basic keypresses do</description> </item>
        ///     <item> <term>editor</term> <description>disables the options editor</description> </item>
        ///     <item> <term>badges</term> <description>removes the device-type badges from the OS tags</description> </item>
        ///     <item> <term>all</term> <description>removes all of these features.</description> </item>
        /// </list>
        /// You can specify multiple parameters with this option. The default is to set none of these values.
        /// </summary>
        [ConfigFileElement("hideui")]
        public UserInterface? HideUserInterface
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies a directory in which custom icons may be found. This directory should contain files with the same names as the files in the standard icons directory. The directory name is specified relative to the directory in which the rEFInd binary resides. The standard icons directory is searched if an icon can't be found in the one specified by icons_dir, so you can use this location to redefine just some icons. Preferred icon file formats are PNG (*.png) and ICNS (*.icns), because both these formats support transparency. You can use BMP (*.bmp) and JPEG (*.jpg or *.jpeg), but rEFInd does not support transparency with these formats, which is highly desirable in icons. Note that if no icons directory is found (either icons or one specified by icons_dir), rEFInd switches to text-only mode, as if textonly had been specified.
        /// </summary>
        [ConfigFileElement("icons_dir")]
        public string? IconsDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies a custom banner file to replace the rEFInd banner image. The file should be an ICNS, BMP, PNG, or JPEG image with a color depth of 24, 8, 4, or 1 bits. The file path is relative to the directory where the rEFInd binary is stored. Note that some image features, such as JPEG's progressive encoding scheme, are not supported. See the comments on LodePNG and NanoJPEG earlier, in Setting OS Icons.
        /// </summary>
        [ConfigFileElement("banner")]
        public string? Banner
        {
            get;
            set;
        }

        /// <summary>
        /// Tells rEFInd whether to display banner images pixel-for-pixel (noscale) or to scale banner images to fill the screen (fillscreen). The former is the default.
        /// </summary>
        [ConfigFileElement("banner_scale")]
        public BannerScale? BannerScale
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the size of big icons (those used for OSes on the first row). All icons are square, so only one value is specified. If icon files don't contain images of the specified size, the available images are scaled to this size. The disk-type badge size is set indirectly by this token; badges are 1/4 the size of big icons. The default value is 128 on most systems, or 256 when the screen is wider than 1920 pixels.
        /// </summary>
        [ConfigFileElement("big_icon_size")]
        public int? BigIconSize
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the size of small icons (those used for tools on the second row). All icons are square, so only one value is specified. If icon files don't contain images of the specified size, the available images are scaled to this size. The default value is 48 on most systems, or 96 when the screen is wider than 1920 pixels.
        /// </summary>
        [ConfigFileElement("small_icon_size")]
        public int? SmallIconSize
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies a graphics file that can be used to highlight the OS selection icons. This should be a 144x144 image in BMP, PNG, ICNS, or JPEG format, stored in rEFInd's main directory. (Images larger or smaller than 144x144 will be scaled, but the results may be ugly.)
        /// </summary>
        [ConfigFileElement("selection_big")]
        public string? SelectionBig
        {
            get;
            set;
        }

        /// <summary>
        /// Like selection_big, this sets an alternate highlight graphic, but for the smaller utility tags on the second row. This should be a 64x64 image in BMP, PNG, ICNS, or JPEG format, stored in rEFInd's main directory. (Images larger or smaller than 64x64 will be scaled, but the results may be ugly.)
        /// </summary>
        [ConfigFileElement("selection_small")]
        public string? SelectionSmall
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies which tool tags to display on the second row:
        /// <list type="bullet">
        ///     <item> <term>shell</term> <description>launches an EFI shell, if one is found on the disk or available as a built-in EFI feature</description> </item>
        ///     <item> <term>memtest</term> <description>(or memtest86) launches the Memtest86 program</description> </item>
        ///     <item> <term>gdisk</term> <description>launches the partitioning tool of the same name</description> </item>
        ///     <item> <term>gptsync</term> <description>launches a tool that creates a hybrid MBR</description> </item>
        ///     <item> <term>install</term> <description>installs rEFInd from the booted medium to another ESP</description> </item>
        ///     <item> <term>bootorder</term> <description>provides a tool for manipulating the EFI boot order (not the order of items in rEFInd's own menu)</description> </item>
        ///     <item> <term>apple_recovery</term> <description>boots the macOS Recovery HD</description> </item>
        ///     <item> <term>csr_rotate</term> <description>rotates through System Integrity Protection (SIP) values specified by csr_values</description> </item>
        ///     <item> <term>windows_recovery</term> <description>boots a Windows recovery tool</description> </item>
        ///     <item> <term>mok_tool</term> <description>launches a tool to manage Machine Owner Keys (MOKs) on systems with Secure Boot active</description> </item>
        ///     <item> <term>fwupdate</term> <description>launches a firmware-update tool</description> </item>
        ///     <item> <term>netboot</term> <description>launches the network boot tool (iPXE)</description> </item>
        ///     <item> <term>about</term> <description>displays information about rEFInd</description> </item>
        ///     <item> <term>hidden_tags</term> <description>enables you to recover tags you've hidden</description> </item>
        ///     <item> <term>exit</term> <description>terminates rEFInd</description> </item>
        ///     <item> <term>shutdown</term> <description>shuts down the computer (or reboots it, on some UEFI PCs)</description> </item>
        ///     <item> <term>reboot</term> <description>reboots the computer</description> </item>
        ///     <item> <term>firmware</term> <description>reboots the computer into the computer's own setup utility</description> </item>
        /// </list>
        /// The tags appear in the order in which you specify them. The default is shell, memtest, gdisk, apple_recovery, windows_recovery, mok_tool, about, hidden_tags, shutdown, reboot, firmware, fwupdate. Note that the shell, memtest, gdisk, apple_recovery, mok_tool, and fwupdate options all require the presence of programs not included with rEFInd. The gptsync option requires use of a like-named program which, although it ships with rEFInd, is not installed by default except under macOS. See the "Installing Additional Components" section of the Installing rEFInd page for pointers to the shell, Memtest86, gptsync, and gdisk programs. The apple_recovery option will appear only if you've got an Apple Recovery HD partition (which has a boot loader called com.apple.recovery.boot/boot.efi). The firmware option works only on computers that support this option; on other computers, the option is quietly ignored. See the Secure Boot page for information on Secure Boot and MOK management. If you want no tools whatsoever, provide a showtools line with no options.
        /// </summary>
        [ConfigFileElement("showtools")]
        public ShowTools? ShowTools
        {
            get;
            set;
        }

        /// <summary>
        /// You can change the font that rEFInd uses in graphics mode by specifying the font file with this token. The font file should exist in rEFInd's main directory and must be a PNG-format graphics file holding glyphs for all the characters between ASCII 32 (space) through 126 (tilde, ~), plus a glyph used for all characters outside of this range. See the Theming rEFInd page for more details.
        /// </summary>
        [ConfigFileElement("font")]
        public string? FontImage
        {
            get;
            set;
        }

        /// <summary>
        /// rEFInd defaults to a graphical mode; however, if you prefer to do without the flashy graphics, you can run it in text mode by including this option (alone or with true, on, or 1). Passing false, off, or 0 causes graphics mode to be used. (This could be useful if you want to override a text-mode setting in an included secondary configuration file.) Text-only mode is implicitly set if rEFInd cannot find either a subdirectory called icons or a subdirectory named by icons_dir.
        /// </summary>
        [ConfigFileElement("textonly")]
        public bool? TextModeOnly
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the text-mode video resolution to be used in conjunction with textonly or for the line editor and program-launch screens. This option takes a single-digit code. Mode 0 is guaranteed to be present and should be 80x25. Mode 1 is supposed to be either invalid or 80x50, but some systems use this number for something else. Higher values are system-specific. Mode 1024 is a rEFInd-specific code that means to not set any mode at all; rEFInd instead uses whatever mode was set when it launched. If you set this option to an invalid value, rEFInd pauses during startup to tell you of that fact. Note that setting textmode can sometimes force your graphics-mode resolution to a higher value than you specify in resolution. On Linux, the /sys/class/graphics/fb0/modes file holds available modes, but it may not be the same set of modes that EFI provides.
        /// </summary>
        [ConfigFileElement("textmode")]
        public int? TextMode
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the graphical video resolution used by rEFInd; takes a width and a height or a single UEFI video mode number or the string max as options. For instance, resolution 1024 768 sets the resolution to 1024x768. On UEFI systems, resolution 1 sets video mode 1, the resolution of which varies from system to system. The value max sets the highest available resolution, which is often desirable, but with caveats described shortly. If you set a resolution that isn't valid on a UEFI-based system, rEFInd displays a message along with a list of valid modes. On a system built around EFI 1.x (such as a Mac), setting an incorrect resolution fails silently; you'll get the system's default resolution. You'll also get the system's default resolution if you set both resolution values to 0 or if you pass anything but one or two numbers. (Note that passing a resolution with an x, as in 1024x768, will be interpreted as one option and so will cause the default resolution to be used.) If you get a higher resolution than you request, try commenting out or changing the textmode value, since it can force the system to use a higher graphics resolution than you specify with resolution. Also, be aware that it is possible to set a valid resolution for your video card that's invalid for your monitor; this is a real risk with the max option. If you do this, your monitor will go blank until you've booted an OS that resets the video mode.
        /// </summary>
        [ConfigFileElement("resolution")]
        public string? Resolution
        {
            get;
            set;
        }

        /// <summary>
        /// Enables support for touch screens (as on tablets). Note that not all tablets provide the necessary support. If this feature is enabled and the tablet supports it, touching an OS or tool should launch it or enter a submenu. In a submenu, it is currently not possible to select a specific item; any touch will launch the default submenu option. This option is incompatible with enable_mouse. If both are specified, the one appearing later in refind.conf takes precedence. The default is off.
        /// </summary>
        [ConfigFileElement("enable_touch")]
        public bool? EnableTouchpadSupport
        {
            get;
            set;
        }

        /// <summary>
        /// Enables support for mice (and related pointing devices, such as trackballs). Note that not all EFIs provide the necessary support. If this feature is enabled and the computer supports it, clicking an OS or tool should launch it or enter a submenu. In a submenu, it is currently not possible to select a specific item; any click will launch the default submenu option. This option is incompatible with enable_touch. If both are specified, the one appearing later in refind.conf takes precedence. The default is off.
        /// </summary>
        [ConfigFileElement("enable_mouse")]
        public bool? EnableMouseSupport
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the size of the mouse pointer, in pixels squared. The default is 16.
        /// </summary>
        [ConfigFileElement("mouse_size")]
        public int? MouseSize
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the speed of the mouse pointer, with higher numbers meaning faster tracking. The default is 1.
        /// </summary>
        [ConfigFileElement("mouse_speed")]
        public int? MouseSpeed
        {
            get;
            set;
        }

        /// <summary>
        /// Ordinarily, rEFInd clears the screen and displays basic boot information when launching any OS but macOS. For macOS, the default behavior is to clear the screen to the default background color and display no information. You can specify the simpler Mac-style behavior by specifying the OSes or boot loaders you want to work this way with this option. (OSes that should use text-mode displays should be omitted from this list.) Note that this option doesn't affect what the follow-on boot loader does; it may display graphics, text, or nothing at all. Thus, the effect of this option is likely to last for just a fraction of a second. On at least one firmware (used on some Gigabyte boards), setting use_graphics_for linux is required to avoid a system hang when launching Linux via its EFI stub loader. To add to the default list, specify + as the first option, as in use_graphics_for + windows.
        /// </summary>
        [ConfigFileElement("use_graphics_for")]
        public UseGraphicsFor? Graphics
        {
            get;
            set;
        }

        /// <summary>
        /// Scans the specified directory or directories for EFI driver files. If rEFInd discovers .efi files in those directories, they're loaded and activated as drivers. This option sets directories to scan in addition to the drivers and drivers_arch subdirectories of the rEFInd installation directory, which are always scanned, if present.
        /// </summary>
        [ConfigFileElement("scan_driver_dirs")]
        public int? DriversDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// Tells rEFInd what methods to use to locate boot loaders:
        /// <list type="bullet">
        ///     <item> <term>internal, external, optical</term> <description>tell rEFInd to scan for EFI boot loaders on internal, external, and optical (CD, DVD, and Blu-ray) devices, respectively.</description> </item>
        ///     <item> <term>netboot</term> <description>relies on the presence of the ipxe.efi and ipxe_discover.efi program files in the EFI/tools directory to assist with network (Preboot Execution Environment, or PXE) booting. Note that netboot is experimental and deprecated. See the BUILDING.txt file for information on building the necessary binaries.</description> </item>
        ///     <item> <term>hdbios, biosexternal, cd </term> <description>are similar, but scan for BIOS boot loaders. (Note that the BIOS options scan more thoroughly and actively on Macs than on UEFI-based PCs; for the latter, only options in the firmware's boot list are scanned, as described on the Using rEFInd page.)</description> </item>
        ///     <item> <term>manual</term> <description>tells rEFInd to scan the configuration file for manual settings.</description> </item>
        ///     <item> <term>firmware</term> <description>tells rEFInd to scan for boot options stored in the EFI's own boot order list; when such a tag is selected by the user, rEFInd sets the EFI BootNext variable and reboots, causing the selected option to run. The upcoming section, Using Firmware Boot Options, covers this topic in more detail.</description> </item>
        /// </list>
        /// You can specify multiple parameters to have the program scan for multiple boot loader types. When you do so, the order determines the order in which the boot loaders appear in the menu, although the order of multiple options within each group is determined by the order in which rEFInd discovers the specific loaders. The default is internal, external, optical, manual on most systems, but internal, hdbios, external, biosexternal, optical, cd, manual on Macs.
        /// </summary>
        [ConfigFileElement("scanfor")]
        public ScanFor? ScanForSources
        {
            get;
            set;
        }

        /// <summary>
        /// If enabled, tells rEFInd to follow symbolic links on filesystems that support this feature. This is desirable if a symbolic link is in a scanned directory and references a loader or kernel you want to use, but the link points to a location that rEFInd does not scan. This is common on openSUSE systems that do not use a separate /boot partition. Following symbolic links is undesirable if both files are in scanned locations, as this can result in redundant boot entries. The default is false.
        /// </summary>
        [ConfigFileElement("follow_symlinks")]
        public bool? FollowSymlinks
        {
            get;
            set;
        }

        /// <summary>
        /// Tells rEFInd how aggressively to scan for BIOS/CSM/legacy boot loaders on UEFI-based PCs. Ordinarily or if this option is set to false, off, or 0, rEFInd presents only those options that were available in the NVRAM when it launched. When uncommented with no option or with true, on, or 1 set, rEFInd adds every possible BIOS-mode boot device (of types specified by scanfor) as a BIOS/CSM/legacy boot option. This latter behavior is sometimes required to detect USB flash drives or hard disks beyond the first one.
        /// </summary>
        [ConfigFileElement("uefi_deep_legacy_scan")]
        public bool? DeepLegacyScan
        {
            get;
            set;
        }

        /// <summary>
        /// Imposes a delay before rEFInd scans for disk devices. Ordinarily this is not necessary, but on some systems, some disks (particularly external drives and optical discs) can take a few seconds to become available. If some of your disks don't appear when rEFInd starts but they do appear when you press the Esc key to re-scan, try uncommenting this option and setting it to a modest value, such as 2, 5, or even 10. The default is 0.
        /// </summary>
        [ConfigFileElement("scan_delay")]
        public int? ScanDelay
        {
            get;
            set;
        }

        /// <summary>
        /// Adds the specified directory or directories to the directory list that rEFInd scans for EFI boot loaders when scanfor includes the internal, external, or optical options. Directories are specified relative to the filesystem's root directory. You may precede a directory path with a volume name and colon, as in somevol:/extra/path, to restrict the extra scan to a single volume. If you don't specify a volume name, this option is applied to all the filesystems that rEFInd scans. If a specified directory doesn't exist, rEFInd ignores it (no error results). The default value is boot, which is useful for locating Linux kernels when you have an EFI driver for your Linux root (/) filesystem. To add to, rather than replace, the default value, specify + as the first item in the list, as in also_scan_dirs +,loaders.
        /// </summary>
        [ConfigFileElement("also_scan_dirs")]
        public string? AlsoScanDirectories
        {
            get;
            set;
        }

        /// <summary>
        /// Adds the specified volume or volumes to a volume "exclusion list"—these filesystems are not scanned for EFI boot loaders. This may be useful to keep unwanted EFI boot entries, such as for an OS recovery partition, from appearing on the main list of boot loaders. You can identify a volume by its filesystem name, its GPT volume name, or by its GPT unique GUID value. The default value is LRS_ESP, to keep the Lenovo Windows recovery volume from appearing. (This volume should get its own tools icon instead—see the showtools token.) You can use dont_scan_volumes to hide disks or partitions from legacy-mode scans, too. In this case, you can enter any part of the description that appears beneath the icons to hide entries that include the string you specify.
        /// </summary>
        [ConfigFileElement("dont_scan_volumes")]
        public string? DontScanVolumes
        {
            get;
            set;
        }

        /// <summary>
        /// Adds the specified directory or directories to a directory "exclusion list"—these directories are not scanned for boot loaders. You may optionally precede a directory path with a volume name and a colon to limit the exclusion list to that volume; otherwise all volumes are affected. For instance, EFI/BOOT prevents scanning the EFI/BOOT directory on all volumes, whereas ESP:EFI/BOOT blocks scans of EFI/BOOT on the volume called ESP but not on other volumes. You can use a filesystem unique GUID, as in 2C17D5ED-850D-4F76-BA31-47A561740082, in place of a volume name. This token may be useful to keep duplicate boot loaders out of the menu; or to keep drivers or utilities out of the boot menu, if you've stored them in a subdirectory of EFI. This option takes precedence over also_scan_dirs; if a directory appears in both lists, it will not be scanned. To add directories to the default list rather than replace the list, specify + as the first option, as in dont_scan_dirs + EFI/dontscan. The default for this token is EFI/tools, EFI/tools/memtest86, EFI/tools/memtest, EFI/memtest86, EFI/memtest, com.apple.recovery.boot.
        /// </summary>
        [ConfigFileElement("dont_scan_dirs")]
        public string? DontScanDirectories
        {
            get;
            set;
        }

        /// <summary>
        /// Adds the specified filename or filenames to a filename "exclusion list" for OS loaders—these files are not included as boot loader options even if they're found on the disk. This is useful to exclude support programs (such as shim.efi and MokManager.efi) and drivers from your OS list. The default value is shim.efi, shim-fedora.efi, shimx64.efi, PreLoader.efi, TextMode.efi, ebounce.efi, GraphicsConsole.efi, MokManager.efi, HashTool.efi, HashTool-signed.efi, fb{arch}.efi (where {arch} is the architecture code, such as x64). You can add a pathname and even a volume specification (filesystem name, partition name, or partition unique GUID), as in ESP:/EFI/BOOT/backup.efi, /boot/vmlinuz-bad, to block the boot loaders only in those specified locations. To add files to the default list rather than replace the list, specify + as the first option, as in dont_scan_files + badloader.efi.
        /// </summary>
        [ConfigFileElement("dont_scan_files")]
        public string? DontScanFiles
        {
            get;
            set;
        }

        /// <summary>
        /// Adds the specified filename or filenames to a filename "exclusion list" for tools—these files are not included as tools (second-line options) even if they're found on the disk and are specified to be included via showtools. This is useful to trim an overabundance of tools. For instance, if you install multiple Linux distributions, you may end up with multiple MokManager entries, but you'll need just one. You can add a pathname and even a volume specification (filesystem name, partition name, or partition unique GUID), as in ESP:/EFI/tools/shellx64.efi, EFI/ubuntu/mmx64.efi, to block the tools only in those specified locations. The default value is an empty list (nothing is excluded).
        /// </summary>
        [ConfigFileElement("dont_scan_tools")]
        public string? DontScanTools
        {
            get;
            set;
        }

        /// <summary>
        /// Adds the specified filename or filenames that will be recognized as Windows recovery tools and presented as such on the second row, if windows_recovery is among the options to showtools. The filename must include a complete path and may optionally include a filesystem label, as in LRS_EFI:\EFI\Microsoft\Boot\LrsBootmgr.efi. Whatever you specify here is added to the dont_scan_files list. The default value is EFI\Microsoft\Boot\LrsBootmgr.efi. If you specify + as the first option, the following options will be added to the default rather than replace it.
        /// </summary>
        [ConfigFileElement("windows_recovery_files")]
        public string? WinRecoveryFiles
        {
            get;
            set;
        }

        /// <summary>
        /// When uncommented or set to true, on, or 1, causes rEFInd to add Linux kernels (files with names that begin with vmlinuz, bzImage, or kernel) to the list of EFI boot loaders, even if they lack .efi filename extensions. This simplifies use of rEFInd on most Linux distributions, which usually provide kernels with EFI stub loader support but don't give those kernels names that end in .efi. Of course, the kernels must still be stored on a filesystem that rEFInd can read, and in a directory that it scans. (Drivers and the also_scan_dirs options can help with those issues.) This option is enabled by default; to disable this feature, you must uncomment this token and set it to false or one of its synonyms (off or 0).
        /// </summary>
        [ConfigFileElement("scan_all_linux_kernels")]
        public bool? ScanAllLinuxKernels
        {
            get;
            set;
        }

        /// <summary>
        /// When uncommented or set to true, on, or 1, causes rEFInd to support launching loaders that have been compressed with gzip. This feature is useful mainly when booting Linux on ARM64 computers, since distributions for ARM64 often compress their whole kernel (including the EFI stub loader) in a single gzip archive. On x86 and x86-64 systems, by contrast, the main body of the kernel is compressed, but the EFI stub loader is not; the kernel can uncompress itself on x86 and x86-64, without rEFInd's help. This option is enabled by default on ARM64 and disabled by default on x86 and x86-64.
        /// </summary>
        [ConfigFileElement("support_gzipped_loaders")]
        public bool? SupportGZippedLoaders
        {
            get;
            set;
        }

        /// <summary>
        /// When uncommented or set to true, on, or 1 (the default), causes rEFInd to "fold" all Linux kernels in a given directory into a single main-menu icon. Selecting that icon launches the most recent kernel. To launch an older kernel, you must press F2, Insert, or Tab; older kernels appear on the resulting submenu display. (You can type, as root, touch /boot/vmlinuz-{whatever}, to make /boot/vmlinuz-{whatever} your default kernel in a directory.) If you prefer to see all your kernels in the main menu, set this option to false, off, or 0. Note that this option is new with version 0.9.0, which changes the default behavior; earlier versions of rEFInd behaved as if fold_linux_kernels false was set.
        /// </summary>
        [ConfigFileElement("fold_linux_kernels")]
        public bool? FoldLinuxKernels
        {
            get;
            set;
        }

        /// <summary>
        /// For the benefit of Linux distributions, such as Arch, that lack version numbers in their kernel filenames but that can provide multiple kernels, you can specify strings that can treated like version numbers. For instance, for Arch you might set this to linux-lts,linux; thereafter, the vmlinuz-linux-lts kernel will match to an initrd file containing the string linux-lts and vmlinuz-linux will match an initrd file with a filename that includes linux, but not linux-lts. Note that, if one specified string is a subset of the other (as in this example), the longer substring must appear first in the list. Also, if a filename includes both a specified string and one or more digits, the match covers both; for instance, vmlinuz-linux-4.8 would match an initrd file with a name that includes linux-4.8. The default is to do no extra matching.
        /// </summary>
        [ConfigFileElement("extra_kernel_version_strings")]
        public int? ExtraKernelVersionsIdentificators
        {
            get;
            set;
        }

        /// <summary>
        /// When activated, and when rEFInd believes it is launching an EFI-stub Linux kernel, ELILO, or GRUB, rEFInd will write the GUID of the partition from which it was launched to the LoaderDevicePartUUID EFI variable with a vendor GUID of 4a67b082-0a4c-41cf-b6c7-440b29bb8c4f, as described on the systemd Boot Loader Interface page. This has the effect of communicating to Linux's systemd what ESP is active; systemd will then mount the ESP at /boot or /efi if that directory is is empty and nothing else is mounted there. (Note that most Linux distributions use other means to mount the ESP at /boot/efi.) This option has no effect when launching non-Linux OSes (except via GRUB). It is disabled by default. Note that, because the variable is persistent, disabling this feature after booting with it enabled will not eliminate an existing entry. If this causes problems (say, if you want to keep your ESP unmounted by default but find that systemd keeps mounting it), you must alter the systemd configuration, as described in its documentation; or remove the relevant EFI variable. The latter can be done by typing "chattr -i /sys/firmware/efi/efivars/LoaderDevicePartUUID-4a67b082-0a4c-41cf-b6c7-440b29bb8c4f" as root, followed by "rm /sys/firmware/efi/efivars/LoaderDevicePartUUID-4a67b082-0a4c-41cf-b6c7-440b29bb8c4f", also as root.
        /// </summary>
        [ConfigFileElement("write_systemd_vars")]
        public bool? WriteSystemdVariables
        {
            get;
            set;
        }

        /// <summary>
        /// Limits the number of tags that rEFInd will display at one time. If rEFInd discovers more loaders than this value, they're shown in a scrolling list. The default value is 0, which imposes no limit.
        /// </summary>
        [ConfigFileElement("max_tags")]
        public int? MaximumNumberOfTags
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the default boot OS based on the loader's title, which appears in the main menu beneath the icons when you select the loader. This token takes one or three variables. The first variable is a set of one or more identifiers. If you specify more than one or if the identifier contains a space, it must be in quotation marks. If more than one identifier is present, they must be specified as a comma-separated list, all within a single set of quotation marks. For instance, default_selection "alpha,beta" will launch alpha if it's available, and beta if alpha is not available but beta is. Each identifier can be any one of three things:
        /// <list type="bullet">
        ///     <item> <term>symbol +</term> <description>which refers to the previously-launched boot entry. rEFInd stores (in NVRAM) the name of a boot entry before launching it, and effectively substitutes that stored string for the + in the default_selection line the next time rEFInd launches, then matches it as a string, as described next....</description> </item>
        ///     <item> <term>any string</term> <description>which is matched against boot descriptions. Note that rEFInd matches substrings, so you don't need to specify the complete description string, just a unique substring. Thus, default_selection vmlinuz matches vmlinuz, boot\vmlinuz-5.8.0-22-generic, or any other string that includes vmlinuz. rEFInd stops searching when it finds the first match. Because rEFInd sorts entries within a directory in descending order by file modification time, if you specify a directory (or volume name, for loaders in a partition's root directory) as the default_selection, the newest loader in that directory will be the default. As a special case, one-character strings are matched against the first character of the description, except for digits.</description> </item>
        ///     <item> <term>digit</term> <description>in which case the boot loader at that position in the boot list is launched. For instance, default_selection 2 makes the second boot entry the default.</description> </item>
        /// </list>
        /// You may optionally follow the match string by two times, in 24-hour format, in which case the entry applies only between those two times. For instance, default_selection Safety 1:30 2:30 boots the entry called Safety by default between the hours of 1:30 and 2:30. These times are specified in whatever format the motherboard clock uses (local time or UTC). If the first value is larger than the second, as in 23:00 1:00, it is interpreted as crossing midnight—11:00 PM to 1:00 AM in this example. The last default_selection setting takes precedence over preceding ones if the time value matches. Thus, you can set a main default_selection without a time specification and then set one or more others to override the main setting at specific times. If you do not specify a default_selection, rEFInd attempts to boot the previously-booted entry, or the first entry if there's no record of that or if the previously-booted entry can't be found.
        /// </summary>
        [ConfigFileElement("default_selection")]
        public string? DefaultSelection
        {
            get;
            set;
        }

        /// <summary>
        /// When set to true or a synonym, enable the CPU's VMX bit and lock the MSR. This configuration is necessary for some hypervisors (notably Microsoft's Hyper-V) to function properly. Activating it on other CPUs will, at best, have no effect, and could conceivably crash the computer, so enable it at your own risk! If your firmware supports activating these features, you should use it instead; this option is provided for users whose firmware does not provide this functionality. (Many Macs lack this configurability, for instance.) The default is false.
        /// </summary>
        [ConfigFileElement("enable_and_lock_vmx")]
        public bool? EnableLockVMX
        {
            get;
            set;
        }

        /// <summary>
        /// On some Macs, this option causes rEFInd to tell the firmware that the specified version of macOS is being launched, even when another OS is selected. The effect is that the firmware may initialize hardware differently, which may have beneficial (or detrimental) results. If your Mac's video output isn't working normally, this option may help. On the other hand, keyboards and mice are known to sometimes stop functioning if this option is used, so you shouldn't use it unnecessarily. This option has no effect on non-Apple hardware. The default is to not use this feature.
        /// </summary>
        [ConfigFileElement("spoof_osx_version")]
        public string? SpoofOSXVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies values that may be set via the csr_rotate tool for Apple's System Integrity Protection (SIP). SIP stores values in NVRAM to set restrictions on what users (even root) may do in macOS 10.11 and later. If you want to be able to control these restrictions in rEFInd, you must set the values you want to use here and set csr_rotate on the showtools line (which must also be uncommented). Note that values are specified in hexadecimal, with no leading 0x or other hexadecimal indicator. SIP is described in more detail on many Web sites
        /// </summary>
        [ConfigFileElement("csr_values")]
        public int[]? CsrValues
        {
            get;
            set;
        }

        /// <summary>
        /// Includes the specified file into the current configuration file. Essentially, the included file replaces the include line, so positioning of this token is important if the included file includes options that contradict those in the main file. The included file must reside in the same directory as the rEFInd binary and the main configuration file. This option is valid only in the main configuration file; included files may not include third-tier configuration files.
        /// </summary>
        [ConfigFileElement("include")]
        public string? Include
        {
            get;
            set;
        }
    }
}
