## rEFInd Automenu

![Markdown](https://github.com/Rikitav/rEFInd-Automenu/blob/main/Formalization/banner.png)
<h3 align="center">A convenient command line tool for installing, updating and managing the rEFInd boot manager on UEFI systems.</h3>

### Features
* **Automatic installation:** rEFInd Automenu will easily install rEFInd on your EFI partition and configure booting.
* **OS Detection:** All alternative operating systems will be identified and added for boot from rEFInd.
* **Update rEFInd:** Upgrade to the latest version of rEFInd with one command.
* **Theme Customization:** Set a theme for your rEFInd to personalize your boot menu.
* **Configuration controlling:** Edit config file just from console using simple commands.

### Usage

usage pattern: `refind <command> [<args>]`
```
Commands:

  Install          Installing a boot manager on a computer or flash drive
  Instance         Working with an instance of rEFInd already installed on your computer
  Obtain           Obtaining rEFInd and related to it resources
  Configuration    Managing global fields of current instance's configuration file
  MenuEntry        Managing boot entries of current instance's configuration file

use 'refind <command> --help' for more details about the command.
```

### Examples

* Installing rEFInd : `refind install --computer` or on removable drive `refind install --flashdrive <drive_letter>`
* Update rEFInd : `refind instance --update`
* Installing the theme : `refind instance --installtheme <theme_directory_path>` or during installation `refind install --computer --theme <theme_directory_path>`
* Removing rEFInd : `refind instance --remove`
* Tweaking config : `refind config --set timeout --value 30`
* Creating menuentry : `refind menuentry --create "Arch linux" --Volume "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX" --Loader "/vmlinuz-linux" --InitRD "/initramfs-linux.img" --Options "root=/dev/sda3 ro"`

### Requirements and building
* `Windows 10 or later` is recommended for use (but versions up to 7 are supported)
* `.NET 6.0 Desktop SDK` For project building