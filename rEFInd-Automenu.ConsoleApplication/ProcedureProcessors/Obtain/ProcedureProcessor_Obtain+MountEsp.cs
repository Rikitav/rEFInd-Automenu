using rEFInd_Automenu.Booting;
using rEFInd_Automenu.ConsoleApplication.ConsoleInterface;

namespace rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.Obtain
{
    public static partial class ProcedureProcessor_Obtain
    {
        private static void MountEsp()
        {
            // Working
            log.Info("Checking Efi system partition (Obtain -SysPar flag)");
            MountVolBribge mountVol = new MountVolBribge();

            ConsoleProgram.Interface.Execute("Checking ESP", ConsoleProgram.CommonCommands, (ctrl) =>
            {
                ConsoleControllerCommands commands = (ConsoleControllerCommands)ctrl.ExecutingCommand;
                if (!MountVolBribge.TryFindEspMountPoint(out _))
                {
                    commands.ChangeLabel("Mounting ESP");
                    mountVol.MountEsp();
                    MountVolBribge.DeleteMountPointAfterExit = false;
                }
                else
                {
                    commands.ChangeLabel("Unmounting ESP");
                    MountVolBribge.DeleteMountPointAfterExit = true;
                    mountVol.UnmountEsp();
                }
            });
        }
    }
}
