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
        public string? Install { get; set; }

        [Option('t', "Theme", HelpText = "Set path to your theme folder (only with -i parametr)\nfor example : \n\"refind -install Comp -t C:\\Theme\" - rEFInd should be installed on current Computer with theme in folder C:\\\"Theme\", \n\"refind -i E:\\ -t Theme\" - rEFInd should be installed on Flash Drive E:\\ with theme with theme in folder \"Theme\" near program")]
        public string? Theme { get; set; }

        [Option('f', "Format", HelpText = "If drive has File System not FAT32, parametr allow to format him\nfor example : \n\"refind --install E:\\ -f\"")]
        public bool Format { get; set; }

        [Option('r', "Remove", HelpText = "Remove rEFInd from current Computer\nfor example : \n\"refind -r\"")]
        public bool Delete { get; set; }

        [Option('d', "Dir", HelpText = "If rEFInd already installed on computer, Scan rEFInd folder on EFI Volume and write ressult to \"rEFInd Dir.txt\" on your desktop\nCan be Combined")]
        public bool Dir { get; set; }

        [Option('c', "Config", HelpText = "If rEFInd already installed on computer, this parametr programm will open \"refind.conf\"\nCan be Combined")]
        public bool Config { get; set; }
    }

    class Automenu
    {
        public const int ERROR_INVALID_FUNCTION = 1;

        [DllImport("kernel32.dll",
           EntryPoint = "GetFirmwareEnvironmentVariableW",
           SetLastError = true,
           CharSet = CharSet.Unicode,
           ExactSpelling = true,
          CallingConvention = CallingConvention.StdCall)]
        public static extern int GetFirmwareType(string lpName, string lpGUID, IntPtr pBuffer, uint size);

        static void Main(string[] args)
        {
            GetFirmwareType("", "{00000000-0000-0000-0000-000000000000}", IntPtr.Zero, 0);
            if (Marshal.GetLastWin32Error() == ERROR_INVALID_FUNCTION)
            {
                Console.WriteLine("Your mainboard doesn't support UEFI and/or Windows is installed in legacy BIOS mode\nCannot install rEFInd !");
                Environment.Exit(1);
            }

            if (args.Length == 0)
            {
                Console.WriteLine("No arguments, try \"refind --help\"");
                Environment.Exit(1);
            }

            //Script String's
            String Temp = Path.GetTempPath();
            String rEFIndZip = @$"{Temp}\rEFInd.zip";
            String rEFIndData = @$"{Temp}\rEFInd";
            Clear(true);

            //Class's
            Install Install = new();
            Config Config = new();

            //Создание Ресурса
            var myAssembly = Assembly.GetExecutingAssembly();
            string resourceName = "rEFInd.zip";
            string fullResourceName = $"{myAssembly.GetName().Name}.{resourceName.Replace('\\', '.')}";
            bool isExistsResourceName = myAssembly.GetManifestResourceNames()
                .Contains(fullResourceName);

            if (isExistsResourceName)
            {
                using (Stream wstream = File.Create(rEFIndZip))
                using (Stream rstream = myAssembly.GetManifestResourceStream(fullResourceName))
                    rstream.CopyTo(wstream);

                if (File.Exists(rEFIndZip))
                    ZipFile.ExtractToDirectory(rEFIndZip, rEFIndData, true);
            }

            //Начало выполнения
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       if (o.Delete)
                           Remove();

                       if (!string.IsNullOrWhiteSpace(o.Install))
                       {
                           if (o.Install.Equals("comp", StringComparison.InvariantCultureIgnoreCase) || o.Install.Equals("computer", StringComparison.InvariantCultureIgnoreCase))
                           {
                               Console.WriteLine("\nrEFInd AUTOMENU console tool v1.5.3 - Install on Computer\n");
                               Console.Write("Preparation for installation... ");
                               if (File.Exists($@"{Temp}\rEFInd.zip"))
                               {
                                   Console.WriteLine("OK");
                                   String ESP = MountESP();
                                   rEFInd.Install.Computer(ESP, o.Theme);
                                   rEFInd.Config.Computer(ESP, o.Theme);
                               }
                               else
                               {
                                   Console.WriteLine("!ERROR!");
                                   Automenu.Clear();
                                   Environment.Exit(1);
                               }
                           }
                           else
                           {
                               if (Directory.Exists(o.Install) || Directory.Exists($@"{o.Install}\") || Directory.Exists($@"{o.Install}:\"))
                               {
                                   Console.WriteLine("\nrEFInd AUTOMENU console tool v1.5.3 - Install on Flash Drive {0}\n", o.Install);
                                   Console.Write("Preparation for installation... ");

                                   if (File.Exists($@"{Temp}\rEFInd.zip"))
                                   {
                                       Console.WriteLine("OK");
                                       DriveInfo[] allDrives = DriveInfo.GetDrives();
                                       foreach (DriveInfo d in allDrives)
                                           if (d.Name == o.Install || d.Name == $@"{o.Install}\" || d.Name == $@"{o.Install}:\")
                                               rEFInd.Install.USB(d.Name, d.DriveFormat, o.Format, o.Theme);
                                   }
                                   else
                                   {
                                       Console.WriteLine("!ERROR!");
                                       Clear();
                                       Environment.Exit(1);
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

                       if (o.Config)
                           OpenConfig();

                       if (o.Dir)
                           ScanDir();

                   });

            Environment.Exit(0);
        }

        public static void Clear(bool CreateDirectory = false)
        {
            String Temp = Path.GetTempPath();

            if (File.Exists(@$"{Temp}\rEFInd.zip"))
                File.Delete(@$"{Temp}\rEFInd.zip");

            if (Directory.Exists(@$"{Temp}\rEFInd"))
                Directory.Delete(@$"{Temp}\rEFInd", true);

            if (CreateDirectory == true)
                Directory.CreateDirectory(@$"{Temp}\rEFInd");
        }

        public static string MountESP()
        {
            Console.Write($"Mounting ESP... ");
            string ESP = null;
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
                if (File.Exists($@"{d.Name}EFI\Microsoft\Boot\bootmgfw.efi"))
                    ESP = d.Name;

            if (Directory.Exists(ESP))
            {
                Console.WriteLine($"Already in {ESP}");
                return ESP;
            }
            else
            {
                var MountVol = Process.Start("cmd.exe", "/c " + "MountVol S: /s");
                MountVol.WaitForExit();

                if (MountVol.ExitCode > 0)
                {
                    Console.WriteLine("!ERROR!");
                    Automenu.Clear();
                    Environment.Exit(1);
                }
                else
                {
                    Console.WriteLine("OK");
                    return @"S:\";
                }
            }

            return ESP;
        }

        public static void OpenConfig()
        {
            Console.WriteLine("\nrEFInd AUTOMENU console tool v1.5.3 - Open rEFInd Config\n");
            var Config = Process.Start("notepad.exe", MountESP() + $@"EFI\rEFInd\refind.conf");
            Console.Write("Opening config... ");
            Config.WaitForExit();

            if (Config.ExitCode > 0)
            {
                Console.WriteLine("!ERROR!");
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("OK");
            }
        }

        public static void ScanDir()
        {
            Console.WriteLine("\nrEFInd AUTOMENU console tool v1.5.3 - Scan rEFInd Directory\n");
            var Scan = Process.Start("cmd.exe", "/c" + $"Dir /s /b " + MountESP() + "EFI\\rEFInd > \"%UserProfile%\\Desktop\\rEFInd Dir.txt\"");
            Console.Write("Scanning Directory... ");
            Scan.WaitForExit();

            if (Scan.ExitCode > 0)
            {
                Console.WriteLine("!ERROR!");
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("OK");
            }
        }

        public static void Remove()
        {
            Console.WriteLine("\nrEFInd AUTOMENU console tool v1.5.3 - Removing rEFInd\n");
            String ESP = MountESP();
            Console.Write("Removing rEFInd... ");
            if (Directory.Exists(@$"{ESP}EFI\rEFInd"))
            {
                Directory.Delete(@$"{ESP}EFI\rEFInd", true);
                if (Directory.Exists(@$"{ESP}EFI\rEFInd"))
                {
                    Console.WriteLine("!ERROR!");
                    Environment.Exit(1);
                }
                else
                    Console.WriteLine("OK");
            }
            else
            {
                Console.WriteLine("rEFInd is not installed on current computer !");
                Environment.Exit(1);
            }
        }
    }
}
