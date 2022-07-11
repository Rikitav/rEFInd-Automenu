using System.Diagnostics;
using System.IO.Compression;

namespace rEFInd
{
    internal class Install
    {
        internal static void USB(string Drive, string FS, bool Format, string Theme)
        {
            Console.WriteLine("Architecture : USB Drive - rEFInd_(ia32, x64, aa64)");

            String Temp = Path.GetTempPath();
            String rEFIndZip = ($"{Temp}\\rEFInd\\rEFInd_Flash.zip");
            String rEFIndInstall = ($@"{Drive}EFI\Boot");

            if (File.Exists(@$"{Drive}EFI\Boot\Bootia32.efi") || File.Exists(@$"{Drive}EFI\Boot\Bootx64.efi") || File.Exists(@$"{Drive}EFI\Boot\Bootaa64.efi"))
            {
                Console.WriteLine("!ERROR!\nDrive {0} already has a Boot-Loader, rEFInd cannot be installed !", Drive);
                Environment.Exit(1);
            }

            if (FS != "FAT32")
            {
                if (Format == false)
                {
                    Console.WriteLine(@"!ERROR!\nDrive {0} has FileSystem {1}. Cannot install rEFInd on this drive. Try to add '-f' parametr for allow formating this drive", Drive, FS);
                    Automenu.Clear();
                    Environment.Exit(1);
                }
                else
                {
                    Console.Write("Formating Drive... ");
                    var FormatDrive = Process.Start("cmd.exe", "/c" + $"FORMAT {Drive[0]}: /Y /FS:FAT32 /V:REFIND /Q > nul");
                    FormatDrive.WaitForExit();

                    if (FormatDrive.ExitCode > 0)
                    {
                        Console.WriteLine("!ERROR!");
                        Automenu.Clear();
                        Environment.Exit(1);
                    }
                    else
                    {
                        Console.WriteLine("OK");
                    }
                }
            }

            Console.Write("Installing rEFInd... ");
            Directory.CreateDirectory(rEFIndInstall);
            ZipFile.ExtractToDirectory(rEFIndZip, rEFIndInstall);

            if (File.Exists($@"{rEFIndInstall}\Bootx64.efi"))
            {
                Console.WriteLine("OK");
                if (!string.IsNullOrWhiteSpace(Theme))
                    ThemeInstall(Theme, Drive + @"EFI\Boot");
            }
            else
            {
                Console.WriteLine("!ERROR!");
                Automenu.Clear();
                Environment.Exit(1);
            }
        }

        internal static void Computer(string ESP, string Theme)
        {
            if (Directory.Exists(ESP + @"EFI\rEFInd"))
                Directory.Delete(ESP + @"EFI\rEFInd", true);

            String[] Arch = ArchParse();
            String Temp = Path.GetTempPath();
            Console.WriteLine(Arch[1]);
            Console.Write("Installing rEFInd... ");
            ZipFile.ExtractToDirectory(Temp + $"\\rEFInd\\refind_{Arch[0]}.zip", $@"{ESP}EFI\rEFInd");
            if (File.Exists($@"{ESP}EFI\refind\refind_{Arch[0]}.efi"))
            {
                var BcdEdit = Process.Start("cmd.exe", "/c" + "bcdedit /set \"{bootmgr}\"" + $" path \\EFI\\refind\\refind_{Arch[0]}.efi > nul");
                BcdEdit.WaitForExit();

                if (BcdEdit.ExitCode > 0)
                {
                    Console.WriteLine("!ERROR!");
                    Automenu.Clear();
                    Environment.Exit(1);
                }
                else
                {
                    Process.Start("cmd.exe", "/c" + "bcdedit /set \"{bootmgr}\" description \"rEFInd\" > nul");
                    Console.WriteLine("OK");

                    if (!string.IsNullOrWhiteSpace(Theme))
                        ThemeInstall(Theme, ESP + @"EFI\rEFInd");
                }
            }
            else
            {
                Console.WriteLine("!ERROR!");
                Automenu.Clear();
                Environment.Exit(1);
            }
        }

        internal static void ThemeInstall(string Theme, string Dest)
        {
            Console.Write("Theme install... ");
            if (File.Exists($"{Theme}\\theme.conf"))
            {
                var ThemeCopy = Process.Start("cmd.exe", "/c" + $@"xcopy {Theme} {Dest}\Theme /I /E /C /Q /Y > nul");
                ThemeCopy.WaitForExit();

                if (ThemeCopy.ExitCode > 0)
                {
                    Console.WriteLine("!ERROR!");
                    Automenu.Clear();
                    Environment.Exit(1);
                }
                else
                {
                    Console.WriteLine("OK");
                }
            }
        }

        public static string[] ArchParse()
        {
            string Arch = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            switch (Arch)
            {
                case "AMD64": return new string[] { "x64", "Architecture : AMD Architecture64 - rEFInd_x64" };
                case "IA64": return new string[] { "x64", "Architecture : Intel Architecture64 - rEFInd_x64" };
                case "ARM64": return new string[] { "aa64", "Architecture : ARM64 - rEFInd_aa64" };
                case "x86": return new string[] { "ia32", "Architecture : x32 or x86 - rEFInd_ia32" };
                default:
                    {
                        Console.WriteLine("rEFInd do not support {0} processors", Arch);
                        Environment.Exit(1);
                        break;
                    }
            }
            return null;
        }
    }
}