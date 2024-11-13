using rEFInd_Automenu.Booting;
using rEFInd_Automenu.ConsoleApplication.ConsoleInterface;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Instance;
using rEFInd_Automenu.Installation;
using rEFInd_Automenu.TypesExtensions;
using Rikitav.IO.ExtensibleFirmware.SystemPartition;

namespace rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.Instance
{
    public static partial class ProcedureProcessor_Instance
    {
        private static void ShowInstanceInfo()
        {
            // Working
            log.Info("Showing current rEFInd instance (\'Instance --ShowInfo\')");

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = InstanceOperations.CheckInstanceExisting();

            string[] values = new string[4];
            ConsoleProgram.Interface.Execute("Getting instance information", ConsoleProgram.CommonCommands, (ctrl) =>
            {
                // Getting efi system partition path
                values[0] = EspFinder.EspDirectory.GetSubDirectory("EFI\\refind").FullName;

                // Getting loader version
                RefindInstanceInfo? instanceInfo = RefindInstanceInfo.Read(EspRefindDir.FullName);
                values[1] = instanceInfo == null
                    ? "<NULL>" // null value
                    : instanceInfo.Value.LoaderVersion.ToString();

                // Getting formalization theme existing
                bool themeExisting = EspRefindDir.GetSubDirectory("theme").Exists;
                values[2] = themeExisting.ToString();

                // Getting loader architecture
                string loaderFileName = EspRefindDir.EnumerateFiles("refind_*.efi").First().Name;
                values[3] = new EfiExecutableInfo("refind", loaderFileName).Architecture.ToString();
            });

            Console.WriteLine();
            ConsoleInterfaceWriter.MessageOffset = "[ INFO ] Loader architecture".Length;

            ConsoleInterfaceWriter.WriteInformation("Instance directory",  " - " + values[0]);
            ConsoleInterfaceWriter.WriteInformation("Loader version",      " - " + values[1]);
            ConsoleInterfaceWriter.WriteInformation("Is theme installed",  " - " + values[2]);
            ConsoleInterfaceWriter.WriteInformation("Loader architecture", " - " + values[3]);

            ConsoleInterfaceWriter.ResetOffset();
        }
    }
}
