using CommandLine;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;

namespace rEFInd
{
    class Options
    {
        [Option('i', "Install", HelpText = "Set install Mode\nfor example : \n\"refind -i computer\" - rEFInd should be installed on current Computer, \n\"refind --install E:\"- rEFInd should be installed on Flash Drive E:\\")]
        public string Install { get; set; }

        [Option('t', "Theme", HelpText = "Set path to your theme folder (only with -i parametr)\nfor example : \n\"refind -install Comp -t C:\\Theme\" - rEFInd should be installed on current Computer with theme in folder C:\\\"Theme\", \n\"refind -i E:\\ -t Theme\" - rEFInd should be installed on Flash Drive E:\\ with theme with theme in folder \"Theme\" near program")]
        public string Theme { get; set; }

        [Option('f', "Format", HelpText = "If drive has File System not FAT32, parametr allow to format him\nfor example : \n\"refind --install E:\\ -f\"")]
        public bool format { get; set; }

        [Option('r', "Remove", HelpText = "Remove rEFInd from current Computer\nfor example : \n\"refind -r\"")]
        public bool Delete { get; set; }

        [Option('d', "Dir", HelpText = "If rEFInd already installed on computer, Scan rEFInd folder on EFI Volume and write ressult to \"rEFInd Dir.txt\" on your desktop\nCan be Combined")]
        public bool Dir { get; set; }

        [Option('c', "Config", HelpText = "If rEFInd already installed on computer, this parametr programm will open \"refind.conf\"\nCan be Combined")]
        public bool Config { get; set; }
    }

    class Automenu
    {
        static void Main(string[] args)
        {
            bool BootMode = IsWindowsUEFI();
            if (BootMode = false)
            {
                Console.WriteLine("Your mainboard doesn't support UEFI and/or Windows is installed in legacy BIOS mode\nCannot install rEFInd !");
                Environment.Exit(1);
            }

            //Script String's
            String Temp = Path.GetTempPath();
            String rEFIndZip = @$"{Temp}\rEFInd.zip";
            String rEFIndData = @$"{Temp}\rEFInd";

            if (args.Length == 0)
            {
                Console.WriteLine("No valid arguments, try \"refind --help\"");
                Environment.Exit(1);
            }

            //Class's
            Install I = new Install();
            Config G = new Config();

            //Подготовка
            if (File.Exists(rEFIndZip))
            {
                File.Delete(rEFIndZip);
            }

            if (Directory.Exists(rEFIndData))
            {
                Directory.Delete(rEFIndData, true);
            }

            Directory.CreateDirectory(rEFIndData);

            var myAssembly = Assembly.GetExecutingAssembly();
            string resourceName = "rEFInd.zip";
            string fullResourceName = $"{myAssembly.GetName().Name}.{resourceName.Replace('\\', '.')}";

            bool isExistsResourceName = myAssembly.GetManifestResourceNames()
                .Contains(fullResourceName);

            if (isExistsResourceName)
            {
                using (Stream wstream = File.Create($@"{Temp}\rEFInd.zip"))
                using (Stream rstream = myAssembly.GetManifestResourceStream(fullResourceName))
                    rstream.CopyTo(wstream);
            }

            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       if (!string.IsNullOrWhiteSpace(o.Install))
                       {
                           if (o.Install.Equals("comp", StringComparison.InvariantCultureIgnoreCase) || o.Install.Equals("computer", StringComparison.InvariantCultureIgnoreCase))
                           {
                               Console.WriteLine("\nrEFInd AUTOMENU console tool v1.5.3 - Install on Computer");
                               if (File.Exists($@"{Temp}\rEFInd.zip"))
                               {
                                   ZipFile.ExtractToDirectory(rEFIndZip, rEFIndData);
                                   Console.WriteLine("\nPreparation for installation... OK");
                               }
                               else
                               {
                                   Console.WriteLine("\nPreparation for installation... !ERROR!");
                                   ClearComp(null);
                                   Environment.Exit(1);
                               }

                               String ESP = MountESP();
                               I.InstallComputer(ESP);
                               G.ConfigComputer(ESP, o.Theme);

                               if (!string.IsNullOrWhiteSpace(o.Theme))
                               {
                                   if (File.Exists($"{o.Theme}\\theme.conf"))
                                   {
                                       var b = Process.Start("cmd.exe", "/c" + $@"xcopy {o.Theme} {ESP}EFI\rEFInd\Theme /I /E /C /Q /Y > nul");
                                       b.WaitForExit();
                                       int bExitCode = b.ExitCode;

                                       if (b.ExitCode > 0)
                                       {
                                           Console.WriteLine("Theme install... !ERROR!");
                                           ClearComp(ESP);
                                           Environment.Exit(1);
                                       }
                                       else
                                       {
                                           Console.WriteLine("Theme install... OK");
                                       }
                                   }
                               }
                           }
                           else
                           {
                               if (Directory.Exists(o.Install))
                               {
                                   Console.WriteLine("\nrEFInd AUTOMENU console tool v1.5.3 - Install on Flash Drive {0}", o.Install);

                                   if (File.Exists($@"{Temp}\rEFInd.zip"))
                                   {
                                       ZipFile.ExtractToDirectory(rEFIndZip, rEFIndData);
                                       Console.WriteLine("\nPreparation for installation... OK");
                                   }
                                   else
                                   {
                                       Console.WriteLine("\nPreparation for installation... !ERROR!");
                                       ClearUSB(null);
                                       Environment.Exit(1);
                                   }

                                   string DriveFS = null;
                                   string Drive = null;
                                   DriveInfo[] allDrives = DriveInfo.GetDrives();
                                   foreach (DriveInfo d in allDrives)
                                   {
                                       if (d.Name == o.Install)
                                           I.InstallUSB(d.Name, d.DriveFormat, o.format);

                                       if (!string.IsNullOrWhiteSpace(o.Theme))
                                       {
                                           if (File.Exists($"{o.Theme}\\theme.conf"))
                                           {
                                               var b = Process.Start("cmd.exe", "/c" + $@"xcopy {o.Theme} {d.Name}EFI\Boot\Theme /I /E /C /Q /Y > nul");
                                               b.WaitForExit();
                                               int bExitCode = b.ExitCode;

                                               if (b.ExitCode > 0)
                                               {
                                                   Console.WriteLine("Theme install... !ERROR!");
                                                   ClearUSB(Drive);
                                                   Environment.Exit(1);
                                               }
                                               else
                                               {
                                                   Console.WriteLine("Theme install... OK");
                                               }
                                           }
                                       }
                                   }
                               }
                               else
                               {
                                   Console.WriteLine("\nDrive {0} doesnt exists or not found", o.Install);
                                   Console.WriteLine("Please indicate the actual letter of the flash drive !");
                                   Environment.Exit(1);
                               }
                           }
                       }

                       if (o.Delete)
                           rEFIndRemove();

                       if (o.Config)
                           OpenConfig();

                       if (o.Dir)
                           ScanDir();

                   });
            Environment.Exit(0);
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

        public const int ERROR_INVALID_FUNCTION = 1;

        [DllImport("kernel32.dll",
           EntryPoint = "GetFirmwareEnvironmentVariableW",
           SetLastError = true,
           CharSet = CharSet.Unicode,
           ExactSpelling = true,
          CallingConvention = CallingConvention.StdCall)]
        public static extern int GetFirmwareType(string lpName, string lpGUID, IntPtr pBuffer, uint size);

        public static bool IsWindowsUEFI()
        {
            // Call the function with a dummy variable name and a dummy variable namespace (function will fail because these don't exist.)
            GetFirmwareType("", "{00000000-0000-0000-0000-000000000000}", IntPtr.Zero, 0);

            if (Marshal.GetLastWin32Error() == ERROR_INVALID_FUNCTION)
            {
                // Calling the function threw an ERROR_INVALID_FUNCTION win32 error, which gets thrown if either
                // - The mainboard doesn't support UEFI and/or
                // - Windows is installed in legacy BIOS mode
                return false;
            }
            else
            {
                // If the system supports UEFI and Windows is installed in UEFI mode it doesn't throw the above error, but a more specific UEFI error
                return true;
            }
        }

        public static String MountESP()
        {
            //Подключение ESP раздела
            string ESP = null;
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
                if (File.Exists($@"{d.Name}EFI\Microsoft\Boot\bootmgfw.efi"))
                    ESP = d.Name;

            if (Directory.Exists(ESP))
            {
                Console.WriteLine($"Mounting ESP... Already in {ESP}");
            }
            else
            {
                var m = Process.Start("cmd.exe", "/c " + "MountVol S: /s");
                m.WaitForExit();
                int mExitCode = m.ExitCode;

                if (m.ExitCode > 0)
                {
                    Console.WriteLine("Mounting ESP Volume... !ERROR!");
                    ClearComp(null);
                    Environment.Exit(1);
                }
                else
                {
                    Console.WriteLine("Mounting ESP Volume... OK");
                    return @"S:\";
                }
            }

            return ESP;
        }

        public static void OpenConfig()
        {
            Console.WriteLine("\nrEFInd AUTOMENU console tool v1.5.3 - Open rEFInd Config");
            Console.WriteLine();
            String ESP = MountESP();
            var b = Process.Start("notepad.exe", $@"{ESP}EFI\rEFInd\refind.conf");
            b.WaitForExit();
            int bExitCode = b.ExitCode;

            if (b.ExitCode > 0)
            {
                Console.WriteLine("Opening config... !ERROR!");
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("Opening config... OK");
            }
        }

        public static void ScanDir()
        {
            Console.WriteLine("\nrEFInd AUTOMENU console tool v1.5.3 - Scan rEFInd Directory");
            Console.WriteLine();
            String ESP = MountESP();
            var b = Process.Start("cmd.exe", "/c" + $"Dir /s /b {ESP}EFI\\rEFInd > \"%UserProfile%\\Desktop\\rEFInd Dir.txt\"");
            b.WaitForExit();
            int bExitCode = b.ExitCode;

            if (b.ExitCode > 0)
            {
                Console.WriteLine("Scanning... !ERROR!");
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("Scanning... OK");
            }
        }

        public static void rEFIndRemove()
        {
            Console.WriteLine("\nrEFInd AUTOMENU console tool v1.5.3 - Removing rEFInd");
            Console.WriteLine();
            String ESP = MountESP();
            if (Directory.Exists(@$"{ESP}EFI\rEFInd"))
            {
                Directory.Delete(@$"{ESP}EFI\rEFInd", true);
                if (!Directory.Exists(@$"{ESP}EFI\rEFInd"))
                {
                    Console.WriteLine("Removing rEFInd... OK");
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Removing rEFInd... ERROR");
                    Environment.Exit(1);
                }
            }
            else
            {
                Console.WriteLine("rEFInd is not installed on current computer !");
                Environment.Exit(1);
            }
        }
            

        public static void ThemeInstall(String ESP)
        {
            if (File.Exists(@"theme\theme.conf"))
            {
                Process.Start("cmd.exe", "/c" + $@"xcopy Theme {ESP}EFI\rEFInd\Theme");
                if (File.Exists(@"S:\EFI\rEFInd\Theme\theme.conf"))
                {
                    Console.WriteLine("Theme install... OK");
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Theme install... !ERROR!");
                    Environment.Exit(0);
                }
            }
        }
    }
}
