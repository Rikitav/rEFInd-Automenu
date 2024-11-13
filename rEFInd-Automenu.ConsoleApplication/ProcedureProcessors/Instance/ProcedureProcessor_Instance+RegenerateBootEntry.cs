using rEFInd_Automenu.Booting;
using rEFInd_Automenu.ConsoleApplication.ConsoleInterface;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Booting;
using rEFInd_Automenu.ConsoleApplication.OperationsManager.Instance;
using rEFInd_Automenu.RuntimeConfiguration;

namespace rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.Instance
{
    public static partial class ProcedureProcessor_Instance
    {
        private static void RegenerateBootEntry()
        {
            // Working
            log.Info("Regenerating current rEFInd instance booting entry (\'Instance --RegenBoot\' flag)");

            // Trying getting ddestination directory access
            DirectoryInfo EspRefindDir = InstanceOperations.CheckInstanceExisting();

            // Setting new loader information
            string LoaderPath = EspRefindDir.EnumerateFiles("refind_*.efi").First().FullName;
            if (!File.Exists(LoaderPath))
            {
                log.Error("rEFInd bootloader not found");
                ConsoleInterfaceWriter.WriteError(Console.CursorTop, nameof(RegenerateBootEntry), "rEFInd bootloader not found, please reinstall boot manager");
                return;
            }

            // Getting Arch
            FirmwareExecutableArchitecture Arch = ArchitectureInfo.FromPostfix(Path.GetFileNameWithoutExtension(LoaderPath).Substring("refind_".Length));
            if (Arch == FirmwareExecutableArchitecture.None)
            {
                log.Warn("Could not find out the bootloader architecture");
                ConsoleInterfaceWriter.WriteError(Console.CursorTop, nameof(RegenerateBootEntry), "Could not find out the bootloader architecture");
                return;
            }

            // Configuring boot loader for rEFInd boot manager
            if (!ProgramConfiguration.Instance.PreferBootmgrBooting)
            {
                // Creating rEFInd boot option
                BootingOperations.CreateRefindFirmwareLoadOption(
                    true,  // overrideExisting
                    true,  // addFirst
                    Arch); // Arch
            }
            else
            {
                // Configuring bootmagr to loading rEFInd
                BootingOperations.ConfigureBootmgrBootEntry(
                    Arch); // Arch
            }
        }
    }
}
