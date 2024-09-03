The utility has several registry keys that affect its behavior during certain operations.
All keys are located along the path `HKEY_LOCAL_MACHINE\SOFTWARE\rEFInd Automenu`

### BackupedBootmgrIdentificator
**GUID (string) as REG_SZ parametr.** This key is the identifier of the backuped original BOOTMGR boot record that was changed during the first installation of rEFInd trough this program or another operation that works with BcdEdit. DO NOT CHANGE (only if you know what you doing).

### LogInterfaceExecution
**Boolean as REG_DWORD parametr.** Specifies that the program should log the start and end of tasks

### LogInterfaceResults
**Boolean as REG_DWORD parametr.** Specifies that the program should log the return value of tasks upon completion. Will log the completion of tasks even if the `LogInterfaceExecution` key is not specified

### PreferMountvolEspSearch
**Boolean as REG_DWORD parametr.** If the __WinNT kernel version is greater than 6__, to work with an instance of the boot manager and install it on the computer, the program will look for the Volume identifier of the EFI system partition, otherwise the program will use MountVol. This key specifies that the program should use Mountvol ESP search even if the kernel version is greater than 6

### PreferBootmgrBooting
**Boolean as REG_DWORD parametr.** This option specifies that the program should overwrite the current system's Bootmgr instead of creating its own boot entry. During the Bootmgr census process, its backup copy will be created, the identifier of which will be recorded in the `BackupedBootmgrIdentificator` registry key

### PreferLoadersEspScan  (OBSOLETE)
**Boolean as REG_DWORD parametr.** According to the standard, the program searches for alternative operating systems through NVRAM, that is, boot records existing in UEFI. This key specifies that the program should search for bootloaders on the ESP, going through the directories located on it. This option can help if, for example, Windows ***accidentally*** killed the bootloader of a Linux Distribution. Removed due to the fact that the scanner selection mechanism was changed

### LoaderScannerType
**LoaderScannerType (string) as REG_SZ parametr.** This parameter specifies which bootloader scanning method will be used during configuration file generation. If this parameter is not specified, the program will use the most convenient scanner.
**Possible values:** `EspDirectoryEnumerator`, `FwBootmgrRecordParser`, `NvramLoadOptionReader`

### Description - LoaderScannerType.EspDirectoryEnumerator
This key specifies that the program should search for bootloaders on the ESP, going through the directories located on it. This option can help if, for example, Windows ***accidentally*** killed the bootloader of a Linux Distribution.

### Description - LoaderScannerType.FwBootmgrRecordParser
This key specifies that the program should look for bootloaders in FwBootmgr, parsing values from the output of the BcdEdit program. This option should be used if LoaderScannerType.NvramLoadOptionReader does not work correctly.

### Description - LoaderScannerType.NvramLoadOptionReader
This key specifies that the program should search for bootloaders in the Nvram of the UEFI, using an enumeration of all `Boot####` boot entries. Recommended scanner, as it works directly with the firmware. Used by default