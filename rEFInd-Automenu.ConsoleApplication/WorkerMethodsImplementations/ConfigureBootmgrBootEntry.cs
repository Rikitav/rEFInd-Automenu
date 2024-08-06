using rEFInd_Automenu.Booting;

namespace rEFInd_Automenu.ConsoleApplication.WorkerMethodsImplementations
{
    public partial class WorkerMethods
    {
        public void ConfigureBootmgrBootEntry(EnvironmentArchitecture Arch) => ConsoleProgram.Interface.Execute("Configuring boot entry", commands, (ctrl) =>
        {
            log.Info("Configuring bootmgr entry for rEFInd loading");

            // Backuping bootmgr menuentry
            BcdEditBridge.BackupBootmgr();

            // Setting new bootmgr infos for refind booting
            string RelativeLoaderPath = string.Format("EFI\\refind\\refind_{0}.efi", Arch.GetArchPostfixString());
            BcdEditBridge.SetBootOptionPath("bootmgr", "rEFInd Boot manager", RelativeLoaderPath);
            BcdEditBridge.SetBootOrderFirst("bootmgr");
        });

        public void ConfigureBootmgrBootEntry(string RelativeLoaderPath) => ConsoleProgram.Interface.Execute("Configuring boot entry", commands, (ctrl) =>
        {
            log.Info("Configuring bootmgr entry for rEFInd loading");

            // Backuping bootmgr menuentry
            BcdEditBridge.BackupBootmgr();

            // Setting new bootmgr infos for refind booting
            if (!RelativeLoaderPath.StartsWith("EFI"))
                RelativeLoaderPath = RelativeLoaderPath.Substring(RelativeLoaderPath.IndexOf("EFI"));

            BcdEditBridge.SetBootOptionPath("bootmgr", "rEFInd Boot manager", RelativeLoaderPath);
            BcdEditBridge.SetBootOrderFirst("bootmgr");
        });
    }
}
