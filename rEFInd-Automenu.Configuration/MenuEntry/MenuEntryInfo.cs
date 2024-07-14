using rEFInd_Automenu.Configuration.Serializing;
using System;
using System.Collections.Generic;

#pragma warning disable CS8618
namespace rEFInd_Automenu.Configuration.MenuEntry
{
    public class MenuEntryInfo
    {
        /// <summary>
        /// Sets the name that's displayed along with the icon for this entry. If the name should contain a space, it must be enclosed in quotes. Following the name, an open curly brace ({) ends the menuentry line.
        /// </summary>
        public string EntryName
        {
            get;
            set;
        } = null;

        /// <summary>
        /// Sets the volume that's used for subsequent file accesses (by icon and loader, and by implication by initrd if loader follows volume). You pass this token a filesystem's label, a partition's label, or a partition's GUID. A filesystem or partition label is typically displayed under the volume's icon in file managers and rEFInd displays it on its menu at the start of the identifying string for an auto-detected boot loader. If this label isn't unique, the first volume with the specified label is used. The matching is nominally case-insensitive, but on some EFIs it's case-sensitive. If a volume has no label, you can use a partition GUID number. If this option is not set, the volume defaults to the one from which rEFInd launched.
        /// </summary>
        [ConfigFileElement("volume")]
        public Guid? Volume
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the filename for the boot loader. You may use either Unix-style slashes (/) or Windows/EFI-style backslashes (\) to separate directory elements. In either case, the references are to files on the ESP from which rEFInd launched or to the one identified by a preceding volume token. The filename is specified as a path relative to the root of the filesystem, so if the file is in a directory, you must include its complete path, as in \EFI\myloader\loader.efi. This option should normally be the first in the body of an OS stanza; if it's not, some other options may be ignored. An exception is if you want to boot a loader from a volume other than the one on which rEFInd resides, in which case volume should precede loader.
        /// </summary>
        [ConfigFileElement("loader")]
        public string Loader
        {
            get;
            set;
        } = null;

        /// <summary>
        /// Sets the filename for a Linux kernel's initial RAM disk (initrd). This option is useful only when booting a Linux kernel that includes an EFI stub loader, which enables you to boot a kernel without the benefit of a separate boot loader. When booted in this way, though, you must normally pass an initrd filename to the boot loader. You must specify the complete EFI path to the initrd file with this option, as in initrd EFI/linux/initrd-3.8.0.img. You'll also have to use the options line to pass the Linux root filesystem, and perhaps other options (as in options "root=/dev/sda4 ro"). The initial RAM disk file must reside on the same volume as the kernel.
        /// </summary>
        [ConfigFileElement("initrd")]
        public string? InitRD
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the EFI boot loader's boot number to be executed. These numbers appear in the output of Linux's efibootmgr and in rEFInd's own EFI boot order list (when showtools bootorder is activated) as Boot#### values, for instance. When this option is used, most other tokens have no effect. In particular, this option is incompatible with volume, loader, initrd, options, and most other tokens. Exceptions are menuentry, icon, and disabled; these three tokens work with firmware_bootnum (and of course menuentry is required). The upcoming section, Using Firmware Boot Options, describes this boot method in more detail.
        /// </summary>
        [ConfigFileElement("firmware_bootnum")]
        public short? FirmwareBootnum
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the filename for an icon for the menu. If you omit this item, a default icon will be used, based on rEFInd's auto-detection algorithms. The filename should be a complete path from the root of the current directory, not relative to the default icons subdirectory or the one set via icons_dir.
        /// </summary>
        [ConfigFileElement("icon")]
        public string? Icon
        {
            get;
            set;
        }

        /// <summary>
        /// Determines the options that are available on a sub-menu obtained by pressing the Insert key with an OS selected in the main menu. If you omit this option, rEFInd selects options using an auto-detection algorithm. Note that this option is case-sensitive.
        /// </summary>
        [ConfigFileElement("ostype")]
        public OSType? OSType
        {
            get;
            set;
        }

        /* <-- DISABLED BECAUSE WORKS ONLY ON MACINTOSH
        /// <summary>
        /// Enables or disables a graphical boot mode. This option has an effect only on Macintoshes; UEFI PCs seem to be unaffected by it.
        /// </summary>
        [ConfigFileElement("graphics")]
        public bool? Graphics
        {
            get;
            set;
        }
        //*/

        /// <summary>
        /// Pass arbitrary options to your boot loader with this line. Note that if the option string should contain spaces (as it often should) or characters that should not be modified by rEFInd's option parser (such as slashes or commas), it must be enclosed in quotes. If you must include quotes in an option, you can double them up, as in my_opt=""with quotes"", which passes my_opt="with quotes" as an option.
        /// </summary>
        [ConfigFileElement("options")]
        public string? Options
        {
            get;
            set;
        }

        /// <summary>
        /// Disable an entry. This is often easier than commenting out an entire entry if you want to temporarily disable it.
        /// </summary>
        [ConfigFileElement("disabled")]
        public bool? Disabled
        {
            get;
            set;
        }

        /// <summary>
        /// This keyword identifies a submenu entry, as described in more detail shortly.
        /// </summary>
        public List<SubMenuEntryInfo>? SubMenuEntries
        {
            get;
            set;
        } = null;

        /// <summary>
        /// Disable this entry
        /// </summary>
        public void Disable()
        {
            this.Disabled = true;
        }
    }
}
