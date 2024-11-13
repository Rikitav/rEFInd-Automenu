using rEFInd_Automenu.ConsoleApplication.OperationsManager.Instance;
using System.Diagnostics;

namespace rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.Instance
{
    public static partial class ProcedureProcessor_Instance
    {
        private static void OpenInstanceConfig()
        {
            // Working
            log.Info("Opening current instance config file (\'Instance -Config\' flag)");

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = InstanceOperations.CheckInstanceExisting();

            // Opening config file "refind.conf" on ESP directory "refind"
            ConsoleProgram.Interface.Execute("Opening config file", ConsoleProgram.CommonCommands, (ctrl) =>
            {
                string RefindConf = Path.Combine(EspRefindDir.FullName, "refind.conf");
                if (!File.Exists(RefindConf))
                {
                    log.Fatal("rEFInd.conf is not found");
                    ctrl.Error("rEFInd.conf is not found", true);
                    return;
                }

                Process.Start(new ProcessStartInfo()
                {
                    FileName = RefindConf,
                    UseShellExecute = true
                });
            });
        }
    }
}
