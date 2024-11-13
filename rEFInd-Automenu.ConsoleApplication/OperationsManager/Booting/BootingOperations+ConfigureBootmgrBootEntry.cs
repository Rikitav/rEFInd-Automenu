using rEFInd_Automenu.Booting;

namespace rEFInd_Automenu.ConsoleApplication.OperationsManager.Booting
{
    public static partial class BootingOperations
    {
        public static void ConfigureBootmgrBootEntry(FirmwareExecutableArchitecture Arch) => ConsoleProgram.Interface.Execute("Configuring boot entry", ConsoleProgram.CommonCommands, (ctrl) =>
        {
            log.Info("Configuring bootmgr entry for rEFInd loading");
            BcdEditBridge bcdEdit = new BcdEditBridge();

            // Backuping bootmgr menuentry
            bcdEdit.BackupBootmgr();

            // Setting new bootmgr infos for refind booting
            string RelativeLoaderPath = string.Format("\\EFI\\refind\\refind_{0}.efi", Arch.GetArchPostfixString());
            bcdEdit.SetBootOptionPath("bootmgr", "rEFInd Boot manager", RelativeLoaderPath);
            bcdEdit.SetBootOrderFirst("bootmgr");
        });
    }
}
