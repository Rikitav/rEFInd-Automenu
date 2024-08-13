The utility has several registry keys that affect its behavior during certain operations.
All keys are located along the path `HKEY_LOCAL_MACHINE\SOFTWARE\rEFInd Automenu`

### BackupedBootmgrIdentificator
GUID (string). This key is the identifier of the backuped original BOOTMGR boot record that was changed during the first installation of rEFInd trough this program or another operation that works with BcdEdit. DO NOT CHANGE (only if you know what you doing)

### LogInterfaceExecution
Boolean. Specifies that the program should log the start and end of tasks

### LogInterfaceResults
Boolean. Specifies that the program should log the return value of tasks upon completion. Will log the completion of tasks even if the `LogInterfaceExecution` key is not specified

### PreferMountvolEspSearch
If the WinNT kernel version is greater than 6, to work with an instance of the boot manager and install it on the computer, the program will look for the Volume identifier of the EFI system partition, otherwise the program will use MountVol. This key specifies that the program should use Mountvol ESP search even if the kernel version is greater than 6

### PreferLoadersEspScan
According to the standard, the program searches for alternative operating systems through NVRAM, that is, boot records existing in UEFI. This key specifies that the program should search for bootloaders on the ESP, going through the directories located on it. This option can help if, for example, Windows *accidentally* killed the bootloader of a Linux system
