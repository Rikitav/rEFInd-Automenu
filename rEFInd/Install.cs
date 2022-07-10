using System.Diagnostics;
using System.IO.Compression;

namespace rEFInd
{
    internal class Install
    {
        internal void InstallUSB(String Drive, string FS, bool Format)
        {
            String ProgramData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            String rEFIndZip = ($"{ProgramData}\\rEFInd\\rEFInd_Flash.zip");
            String rEFIndInstall = ($@"{Drive}EFI\Boot");
            String rEFIndData = ($"{ProgramData}\\rEFInd");

            if (File.Exists(@$"{Drive}EFI\Boot\Bootia32.efi") || File.Exists(@$"{Drive}EFI\Boot\Bootx64.efi") || File.Exists(@$"{Drive}EFI\Boot\Bootaa64.efi"))
            {
                Console.WriteLine("Drive {0} already has a Boot-Loader, rEFInd cannot be installed !", Drive);
                Environment.Exit(1);
            }

            if (FS == "FAT32")
            {
                Console.WriteLine("Architecture : USB Drive - rEFInd_(ia32, x64, aa64)");
                Directory.CreateDirectory(rEFIndInstall);
                ZipFile.ExtractToDirectory(rEFIndZip, rEFIndInstall);
            }
            else
            {
                if (Format == true)
                {
                    Console.WriteLine("");
                    var m = Process.Start("cmd.exe", "/c" + $"FORMAT {Drive[0]}: /Y /FS:FAT32 /V:REFIND /Q");
                    m.WaitForExit();
                    Console.WriteLine("");
                    Console.WriteLine("Architecture : USB Drive - rEFInd_(ia32, x64, aa64)");
                    Directory.CreateDirectory(rEFIndInstall);
                    ZipFile.ExtractToDirectory(rEFIndZip, rEFIndInstall);
                }
                else
                {
                    Console.WriteLine(@"Drive {0} has FileSystem {1}. Cannot install rEFInd on this drive. Try to add '-f' parametr for allow formating this drive", Drive, FS);
                    ClearUSB(Drive);
                    Environment.Exit(1);
                }
            }

            if (File.Exists($@"{rEFIndInstall}\Bootx64.efi"))
            {
                Console.WriteLine("installing rEFInd... OK");
            }
            else
            {
                Console.WriteLine("installing rEFInd... !ERROR!");
                ClearUSB(Drive);
                Environment.Exit(1);
            }
        }

        internal void InstallComputer(String ESP)
        {
            String Temp = Path.GetTempPath();
            String[] Arch = ArchParse();

            //Extract to ESP
            Console.WriteLine(Arch[1]);
            ZipFile.ExtractToDirectory($"{Temp}\\rEFInd\\refind_{Arch[0]}.zip", $@"{ESP}EFI\rEFInd");
            if (File.Exists($@"{ESP}EFI\refind\refind_{Arch[0]}.efi"))
            {
                //BcdEdit
                var b = Process.Start("cmd.exe", "/c" + "bcdedit /set \"{bootmgr}\"" + $" path \\EFI\\refind\\refind_{Arch[0]}.efi > nul");
                b.WaitForExit();
                int bExitCode = b.ExitCode;

                if (b.ExitCode > 0)
                {
                    Console.WriteLine("Installing rEFInd... !ERROR!");
                    ClearComp(ESP);
                    Environment.Exit(1);
                }
                else
                {
                    Process.Start("cmd.exe", "/c" + "bcdedit /set \"{bootmgr}\" description \"rEFInd\" > nul");
                    Console.WriteLine("Installing rEFInd... OK");
                }
            }
            else
            {
                Console.WriteLine("Installing rEFInd... !ERROR!");
                ClearComp(ESP);
                Environment.Exit(1);
            }
        }

        public static string[] ArchParse()
        {
            String Arch = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            switch (Arch)
            {
                case "AMD64": return new string[] { "x64", "Architecture : AMD Architecture64 - rEFInd_x64" };
                case "IA64": return new string[] { "x64", "Architecture : Intel Architecture64 - rEFInd_x64" };
                case "ARM64": return new string[] { "aa64", "Architecture : ARM64 - rEFInd_aa64" };
                case "x86": return new string[] { "ia32", "Architecture : x32 or x86 - rEFInd_ia32" };
            }
            return null;
        }

        public static void ClearComp(String ESP)
        {
            String Temp = Path.GetTempPath();
            String rEFIndZip = @$"{Temp}\rEFInd.zip";
            String rEFIndData = @$"{Temp}\rEFInd";
            String rEFIndESP = @$"{ESP}EFI\rEFInd";

            if (File.Exists(rEFIndZip))
            {
                File.Delete(rEFIndZip);
            }

            if (Directory.Exists(rEFIndData))
            {
                Directory.Delete(rEFIndData, true);
            }

            if (Directory.Exists(rEFIndESP))
            {
                Directory.Delete(rEFIndESP, true);
            }
        }

        public static void ClearUSB(String Drive)
        {
            String Temp = Path.GetTempPath();
            String rEFIndZip = @$"{Temp}\rEFInd.zip";
            String rEFIndData = @$"{Temp}\rEFInd";
            String rEFIndUSB = @$"{Drive}EFI\Boot";

            if (File.Exists(rEFIndZip))
            {
                File.Delete(rEFIndZip);
            }

            if (Directory.Exists(rEFIndData))
            {
                Directory.Delete(rEFIndData, true);
            }
        }
    }
}