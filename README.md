![Markdown](https://github.com/Rikitav/rEFInd-Automenu/blob/main/Formalization/banner.png)
<h3 align="center">rEFInd Automenu is utility designed for installing and configuring the rEFInd boot manager</h1>

# Features
* **Installing**<br> rEFInd can be installed on a PC or unpacked onto a flash drive as a fallback loader
* **Configuration**<br> Automatic creation of a configuration file containing all settings and boot entries of their current UEFI platform
* **Instance updating**<br> The installed instance can be updated to the latest version without the need to replace anything manually
* **Formalization**<br> Together with rEFInd you can install a beautiful design theme that can be found on the Internet
<br>

# Using
Pass the `--help` param to see available verbs
```
  Install     Contains options for installing rEFInd on a computer or extracting
              it to a USB flash drive

  Instance    Working with an instance of rEFInd already installed on your
              computer

  Get         Options related to obtaining rEFInd resources

  help        Display more information on a specific command.

  version     Display version information.
```
<br>

# Installation (Install)
Install verb has the following parameters :
```
  --Computer      (Group: Install Destination) Specifies that rEFInd should be
                  installed on the current computer. High parsing priority

  --FlashDrive    (Group: Install Destination) Specifies that rEFInd should be
                  installed on removable storage as fallback loader. The
                  parameter must contain a drive letter (for example "C") or a
                  path to the root directory (for example "C:\"). Medium parsing
                  priority

  -L, --Latest    (Default: false) Download latest version of rEFInd from
                  SourceForge.com before installation

  -T, --Theme     Set path to your Theme folder. The parameter must contain the
                  path to the directory (For example, "C:\rEFInd\Bin"), and
                  target directory must contain theme configuration file named
                  "Theme.conf"

  -A, --Arch      (Default: None) Force set installation arcitecture
                  Permissible values : "AMD64, ARM64, x86"

  -F, --Format    (Default: false)

  -X, --Force     (Default: false) Trying to fix some errors during
                  installation, such as trying to remove an existing rEFInd
                  instance on the computer

  --help          Display this help screen.

  --version       Display version information.
```
<br>

## Installation on current computer
To install rEFInd on the current computer, you must specify the `--computer` parameter.
During installation, there will be
* unpacked the loader and its accompanying files and directories on the EFI System Partition from the resource archive downloaded from [SourceForge](https://sourceforge.net/projects/refind/) or extracted from the built-in resources
* Generated rEFInd configuration file containing boot entries for all detected operating systems
* Changed boot entry {bootmgr} to boot rEFInd

* `-l` `--latest`<br> Indicates that the latest available resource archive from the repository should be downloaded, however, if there is already an archive with the latest version in the local storage, then it will be used
* `-t` `--theme`<br> Specifies the path to the directory where the compatible rEFInd theme is located. The directory must contain the configuration file "theme.conf"!
* `-a` `-arch`<br> Specifies the specific processor architecture for which rEFInd should be installed. if this parameter is not specified, the architecture is determined automatically. Valid values `AMD64`, `ARM64`, `x86`
* `-x` `-force`<br> Indicates that if it detects an already installed rEFInd instance, the program will try to remove it first
* `-f` `-format`<br> Doesn't matter in this context
<br>

## Installation on Flash drive
To install rEFInd on the current computer, you must specify the `--flashdrive` parameter.
During installation there will be
* unpacked the loader and its accompanying files and directories onto media from the resource archive downloaded from the [SourceForge](https://sourceforge.net/projects/refind/) or extracted from the built-in resources
* Generated blank configuration file

* `-l` `--latest`<br> Indicates that the latest available resource archive from the repository should be downloaded, however, if there is already an archive with the latest version in the local storage, then it will be used
* `-t` `--theme`<br> Specifies the path to the directory where the compatible rEFInd theme is located. The directory must contain the configuration file "theme.conf"!
* `-a` `-arch`<br> Specifies the specific processor architecture for which rEFInd should be installed. if this parameter is not specified, all presented bootloaders will be installed
* `-x` `-force`<br> Indicates that if another fallback bootloader is detected on the drive, the program should remove it along with its directory
* `-f` `-format`<br> If the flash drive has a file system other than FAT32, the program will format it
<br>

# Instance management (Instance)
Instance verb has the following parameters :
```
  -R, --Remove    (Group: Instance command) Remove rEFInd from current Computer

  -U, --Update    (Group: Instance command) If rEFInd already installed on
                  computer, reinstall rEFInd to newest version and regenerate
                  config file
                  Else just install rEFInd with clear configuration

  -C, --Config    (Group: Instance command) If rEFInd already installed on
                  computer, this parametr programm will open "refind.conf"

  --RegenConf     (Group: Instance command) If your config file was corrupted,
                  you can regenerate it

  --RegenBoot     (Group: Instance command) If rEFInd won't load and you think
                  it's because BootMgr has redrawn, you can rewrite it to load
                  rEFInd again

  --help          Display this help screen.

  --version       Display version information.
```
<br>

* `-r` `--remove`<br> Indicates that the program should remove the currently installed instance of rEFInd
* `-u` `--update`<br> Indicates that the program should rewrite the bootloader and its accompanying directories to the newest ones
* `-c` `--config`<br> Indicates that the program should open the configuration file of the currently installed rEFInd instance
* `--tegenconf`<br> Indicates that the program should generate and rewrite a configuration file of the currently installed rEFInd instance
* `--regenboot`<br> Indicates that the program must rewrite the bootmgr entry to boot rEFInd
<br>
