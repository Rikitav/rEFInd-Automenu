namespace rEFInd_Automenu.RuntimeConfiguration
{
    public enum LoaderScannerType
    {
        /// <summary>
        /// This key specifies that the program should search for bootloaders on the ESP, going through the directories located on it. This option can help if, for example, Windows ***accidentally*** killed the bootloader of a Linux Distribution.
        /// </summary>
        EspDirectoryEnumerator,

        /// <summary>
        /// This key specifies that the program should look for bootloaders in FwBootmgr, parsing values from the output of the BcdEdit program. This option should be used if LoaderScannerType.NvramLoadOptionReader does not work correctly.
        /// </summary>
        FwBootmgrRecordParser,

        /// <summary>
        /// This key specifies that the program should search for bootloaders in the Nvram of the UEFI, using an enumeration of all `Boot####` boot entries. Recommended scanner, as it works directly with the firmware. Used by default
        /// </summary>
        NvramLoadOptionReader
    }
}
