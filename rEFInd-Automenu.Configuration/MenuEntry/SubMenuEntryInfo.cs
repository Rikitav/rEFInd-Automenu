using rEFInd_Automenu.Configuration.Serializing;

#pragma warning disable CS8618
namespace rEFInd_Automenu.Configuration.MenuEntry
{
    public class SubMenuEntryInfo
    {
        /// <summary>
        /// Sets the name that's displayed for this entry on the submenu page. If the name should contain a space, it must be enclosed in quotes. Following the name, an open curly brace ({) ends the submenuentry line.
        /// </summary>
        public string EntryName
        {
            get;
            set;
        } = null;

        /// <summary>
        /// Sets the filename for the boot loader, as described in Table 2. Note that the loader is read from whatever filesystem is specified by the main stanza's volume option, provided that option precedes the submenu definition.
        /// </summary>
        [ConfigFileElement("loader")]
        public string? Loader
        {
            get;
            set;
        } = null;

        /// <summary>
        /// Sets the filename for a Linux kernel's initial RAM disk (initrd), as described in Table 2. If you want to eliminate the initrd specification, you should use this keyword alone, with no options. You might do this because your main entry is for a Linux kernel with EFI stub support and this submenu entry launches ELILO, which sets the initrd in its own configuration file.
        /// </summary>
        [ConfigFileElement("initrd")]
        public string? InitRD
        {
            get;
            set;
        }

        /* // DISABLED BECAUSE WORKS ONLY ON MACINTOSH
        /// <summary>
        /// Enables or disables a graphical boot mode, as described in Table 2.
        /// </summary>
        [ConfigFileElement("graphics")]
        public bool? Graphics
        {
            get;
            set;
        }
        //*/

        /// <summary>
        /// Pass arbitrary options to your boot loader with this line, as described in Table 2. As with initrd, you can eliminate all options by passing this keyword alone on a line.
        /// </summary>
        [ConfigFileElement("options")]
        public string? OverrideOptions
        {
            get;
            set;
        }

        /// <summary>
        /// This token works just like options, except that instead of replacing the default options, it causes the specified options to be added to those specified in the main stanza listing's options line.
        /// </summary>
        [ConfigFileElement("add_options")]
        public string? AddonOptions
        {
            get;
            set;
        }

        /// <summary>
        /// Disable a submenu entry. This is often easier than commenting out an entire entry if you want to temporarily disable it.
        /// </summary>
        [ConfigFileElement("disabled")]
        public bool? Disabled
        {
            get;
            set;
        }
    }
}
