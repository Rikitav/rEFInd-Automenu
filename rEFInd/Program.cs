using CommandLine;
using ErrorIndicateList;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;

namespace rEFInd
{
    //CommandLineParser - Класс парсера
    class Options : WorkClass
    {
        //Task Options
        [Option('i', "Install", HelpText = "Set install Mode\nfor example : \n\"refind -i\" - rEFInd should be installed on current Computer, \n\"refind --install E:\" - rEFInd should be installed on Flash Drive E:\\, \n\"refind -i Desktop\" - rEFInd should be unpacked on current User\'s Desktop")]
        public string? Install { get; set; }

        [Option('r', "Remove", HelpText = "Remove rEFInd from current Computer\nfor example : \n\"refind -r\"\nCan be Combined with other parametrs")]
        public bool Delete { get; set; }

        [Option('s', "Scan", HelpText = "If rEFInd already installed on computer, Scan rEFInd folder on EFI Volume and write ressult to \"rEFInd Dir.txt\" on your desktop\nfor example : \n\"refind -s\"\nCan be Combined with other parametrs")]
        public bool Dir { get; set; }

        [Option('c', "Config", HelpText = "If rEFInd already installed on computer, this parametr programm will open \"refind.conf\"\nfor example : \n\"refind -c\"\nCan be Combined with other parametrs")]
        public bool Config { get; set; }

        //"Install" Options
        [Option('d', "Download", HelpText = "Download latest version of rEFInd from SourceForge.com before installation\nfor example : \n\"refind -d -i\" - rEFInd should be installed on current Computer with latest version of rEFInd\nCan be Combined only with \'--Install\' parametr")]
        public bool Download { get; set; }

        [Option('t', "Theme", HelpText = "Set path to your theme folder (only with -i parametr)\nfor example : \n\"refind -t C:\\Theme --install\" - rEFInd should be installed on current Computer with theme in folder C:\\\"Theme\"\nCan be Combined only with \'--Install\' parametr")]
        public string? Theme { get; set; }

        [Option('a', "Arch", HelpText = "Force set installation arcitecture \nPermissible values : \n\"AMD64, IA64, ARM64, x86\"\nfor example :\n\"refind -a AMD64 -i\nCan be Combined omly with \'--Install\' parametr")]
        public string? Architecture { get; set; }

        [Option('f', "Format", HelpText = "If drive has File System not FAT32, parametr allow to format him\nfor example : \n\"refind -f --install E:\\\" - rEFInd should be installed on flash drive E:\\ with formating into FAT32 File system if nedded\nCan be Combined only with \'--Install\' parametr")]
        public bool Format { get; set; }

        [Option("IgnoreCGL", HelpText = "Beta function !!! \nScan EFI System Partition for boot loaders and make Loader Config ignoring Config Generator List\nfor example : \n\"refind --IgnoreCGL --install\" - rEFInd should be installed on current Computer ignoring Config Generator List\nCan be Combined only with \'--Install\' parametr")]
        public bool IgnoreCGL { get; set; }

        [Option("Wait")]
        public bool Wait { get; set; }
    }

    class WorkClass
    {
        //Переменные содержащие методы
        public static string ProgName = Path.GetFileNameWithoutExtension(Environment.ProcessPath);   //Имя программы
        public static string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(); //Версия сборки
        public static string Temp = Path.GetTempPath();                                              //Путь к папке Temp
        public static DriveInfo[] AllDrives = DriveInfo.GetDrives();                                 //Список всех накопителей
        public static bool InstallParsed;                                                            //Указан ли параметр "-i"
        public static SpinTasker SpinTasker = new();                                                 //SpinTasker

        //Информация необходимая для работы
        public static Options? Options;   //Аргументы CMD парсера
        public static DriveInfo? Drive;   //USB для установки
        public static string[]? Arch;     //Aрхитектура процессора
        public static string? ESP;        //Диск подключенный как ESP
        public static string? rEFIndData; //Путь к rEFInd Data

        //Методы
        public static void Error(string ErrorMessage = "Unexpected Error")                        //Метод воспроизводящий ошибки 
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("!ERROR!\n" + ErrorMessage);
            Console.ForegroundColor = ConsoleColor.White;
            Clear();
            Console.CursorVisible = true;

            if (Options.Wait)
                Console.ReadLine();

            Environment.Exit(1);
        }
        public static void Clear()                                                                //Метод очистки рабочего мусора 
        {
            if (File.Exists(@$"{Temp}\rEFInd.zip"))
                File.Delete(@$"{Temp}\rEFInd.zip");

            if (Directory.Exists(@$"{Temp}\rEFInd"))
                Directory.Delete(@$"{Temp}\rEFInd", true);
        }
        public static bool DriveExists()                                                          //Метод устанавливающий значение Drive 
        {
            foreach (var SearchDrive in AllDrives)
                if (SearchDrive.Name.ToString().ToLower().Contains(Options.Install.ToLower()))
                {
                    Drive = SearchDrive;
                    return true;
                }

            return false;
        }
        public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive) //Метод копирующий директории 
        {
            var dir = new DirectoryInfo(sourceDir);
            if (!dir.Exists) throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destinationDir);
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            if (recursive)
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
        }

        //Метод проверяющий тип загрузки и поддержку UEFI
        public const int ERROR_INVALID_FUNCTION = 1;
        [DllImport("kernel32.dll",
           EntryPoint = "GetFirmwareEnvironmentVariableW",
           SetLastError = true,
           CharSet = CharSet.Unicode,
           ExactSpelling = true,
           CallingConvention = CallingConvention.StdCall)]
        public static extern int GetFirmwareType(string lpName, string lpGUID, IntPtr pBuffer, uint size);

        //Метод проыеряющий подключение к интернету
        public static int InternetDesc;
        [DllImport("wininet.dll")]
        public static extern bool InternetGetConnectedState(out int Description, int ReservedValue);
    }

    class Automenu : WorkClass
    {
        //В методе Main производится проверка на железо и аргументы, создание ресурса, обновление переменных и запуск метода Work (Работа)
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = Parser.Default.ParseArguments<Options>(args);
            parserResult.WithParsed(Opt =>
            {
                //Установка перменных
                Options = Opt;

                //Проверка на установленный параметр 'Install'
                if ((from Arg in args where Arg.ToLower().Contains("-i") select Arg).Any())
                {
                    if (args[^1].ToLower().Contains("-i"))
                        InstallParsed = true;
                    else
                        Error("\'Install\' Parametr can only be indicated at the end");
                }

                //Приветствие
                Console.WriteLine("rEFInd Automenu v" + Version + "\n");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Preparation for Work :");
                Console.ForegroundColor = ConsoleColor.White;
                Clear();

                //Проверка на аргументы командной строки
                if (args.Length == 0)
                    Error("No CMD arguments, try \"" + ProgName + " --help\"");

                //Проверка на поддержку и активированный UEFI (Позаимствовал)
                GetFirmwareType("", "{00000000-0000-0000-0000-000000000000}", IntPtr.Zero, 0);
                if (Marshal.GetLastWin32Error() == ERROR_INVALID_FUNCTION)
                    Error("Your mainboard doesn't support UEFI and/or Windows is installed in legacy BIOS mode\nrEFInd can be installed only on UEFI Machines");

                //Подключение раздела Efi System Partition
                SpinTasker.Start("Mounting ESP");
                foreach (DriveInfo Drive in AllDrives)
                    if (File.Exists(Drive.Name + @"EFI\Microsoft\Boot\bootmgfw.efi") & Drive.DriveType == DriveType.Fixed)
                    {
                        //Если нужный нам раздел уже подключен, перменная обновляется и работа продолжается
                        SpinTasker.Stop(true, OkMessage: "Already in " + Drive.Name);
                        ESP = Drive.Name;
                    }

                //А если нет, то сами его подключаем CMD командой
                if (string.IsNullOrWhiteSpace(ESP))
                {
                    var MountVol = Process.Start("cmd.exe", "/c " + "MountVol S: /s");
                    MountVol.WaitForExit();
                    SpinTasker.Stop(!(MountVol.ExitCode > 0), ErrorMessage: "CMD \'Mountvol\' Сommand Exception : " + MountVol.ExitCode, OkMessage: @"S:\");
                    ESP = @"S:\";
                }

                //Узнаем архитектуру процессора и делаем массив с нужной информацией
                //либо если указан параметр -a ставим массив по нему
                if (InstallParsed)
                {
                    SpinTasker.Start("Getting Architecture");
                    string ArchInit = string.IsNullOrWhiteSpace(Options.Architecture) ? Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") : Options.Architecture.ToUpper();
                    switch (ArchInit)
                    {
                        case "AMD64": Arch = new string[] { "x64",  "AMD x64 Processor - rEFInd_x64",                    "AMD64" }; break;
                        case "IA64":  Arch = new string[] { "x64",  "Intel x64 Processor - rEFInd_x64",                  "IA64"  }; break;
                        case "x86":   Arch = new string[] { "ia32", "Intel x32 or i386 Processor - rEFInd_ia32",         "x86"   }; break;
                        case "ARM64": Arch = new string[] { "aa64", "Advanced RISC Machine x64 Processor - rEFInd_aa64", "ARM64" }; break;
                        default: SpinTasker.Stop(ErrorMessage: $"rEFInd do not support {ArchInit} processors"); break;
                    }
                    SpinTasker.Stop(!string.IsNullOrWhiteSpace(Arch[0]), ErrorMessage: "Unexpected Error", OkMessage: Arch[1]);
                }

                //Распаковка или Скачивание архива rEFInd
                if (InstallParsed)
                {
                    string rEFIndBin = Temp + "refind-bin.zip";
                    if (Options.Download)
                    {
                        using WebClient rEFIndHttp = new();
                        {
                            SpinTasker.Start("Downloading rEFInd");
                            if (InternetGetConnectedState(out InternetDesc, 0))
                            {
                                try { rEFIndHttp.DownloadFile(@"https://sourceforge.net/projects/refind/files/latest/download", rEFIndBin); }
                                catch (Exception ex) { SpinTasker.Stop(ErrorMessage: ex.Message); }
                            }
                            else
                                SpinTasker.Stop(ErrorMessage: "No internet connection");
                        }
                    }
                    else
                    {
                        SpinTasker.Start("Unpacking rEFInd");
                        var myAssembly = Assembly.GetExecutingAssembly();
                        string resourceName = "refind-bin.zip";
                        string fullResourceName = $"{myAssembly.GetName().Name}.{resourceName.Replace('\\', '.')}";

                        if (myAssembly.GetManifestResourceNames().Contains(fullResourceName))
                        {
                            using Stream wstream = File.Create(rEFIndBin);
                            using Stream rstream = myAssembly.GetManifestResourceStream(fullResourceName);
                            rstream.CopyTo(wstream);
                        }
                    }

                    Directory.CreateDirectory(Temp + "rEFInd");
                    ZipFile.ExtractToDirectory(rEFIndBin, Temp + "rEFInd", true);
                    rEFIndData = Directory.GetDirectories(Temp + "rEFInd", "refind-bin-0.13.*.*")[0] + @"\refind\";
                    SpinTasker.Stop(Directory.Exists(rEFIndData), ErrorMessage: "Imposible to Create or Download rEFInd-Bin.zip", OkMessage: "rEFInd-Bin.zip");
                }

                //Начало выполнения
                Work();

                //Конец выполнения
                Console.CursorVisible = true;
                Clear();

                if (Options.Wait)
                    Console.ReadLine();

                return;
            });
        }

        public static void Work()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\nStarting Work :");
            Console.ForegroundColor = ConsoleColor.White;

            if (Options.Delete) //Опция для удаления rEFInd с компьютера
            {
                SpinTasker.Start("Removing rEFInd");
                if (Directory.Exists(ESP + $@"EFI\rEFInd"))
                {
                    var Remove = Process.Start("cmd.exe", "/c" + $"RmDir /s /q " + ESP + "EFI\\rEFInd");
                    Remove.WaitForExit();
                    SpinTasker.Stop(!(Remove.ExitCode > 0), ErrorMessage: "CMD \'RmDir\' Сommand Exception : " + Remove.ExitCode);
                }
                else
                    SpinTasker.Stop(ErrorMessage: "rEFInd is not installed on this computer");
            } 

            if (InstallParsed) //Здесь производится установка rEFInd
            {
                if (string.IsNullOrWhiteSpace(Options.Install))
                {
                    Install.Computer();
                    Config.Computer();
                }
                else if (Options.Install.Equals("desktop", StringComparison.InvariantCultureIgnoreCase))
                {
                    Install.Computer(Desktop: true);
                    Config.Computer(Desktop: true);
                }
                else if (Install.DriveExists())
                {
                    Install.USB();
                    Config.USB();
                }
                else
                    SpinTasker.Stop(false, ErrorMessage: "Unknown --Install commmand\nTry " + ProgName + " --Help");
            }  

            if (Options.Config) //В этом методе мы открываем конфиг rEFInd установленного на компьютер (refind.conf)
            {
                SpinTasker.Start("Opening config");
                if (File.Exists(ESP + $@"EFI\rEFInd\refind.conf"))
                {
                    var Config = Process.Start("notepad", ESP + $@"EFI\rEFInd\refind.conf");
                    Config.WaitForExit();
                    SpinTasker.Stop(!(Config.ExitCode > 0), ErrorMessage: "Notepad Exception : " + Config.ExitCode);
                }
                else
                    SpinTasker.Stop(ErrorMessage: "Notepad Exception : Cannot Find Config File");
            } 

            if (Options.Dir) //Метод который сканирует директорию с rEFInd и записывает рузультат в текст. документ на рабочем столе
            {
                SpinTasker.Start("Scanning Directory");
                if (Directory.Exists(ESP + $@"EFI\rEFInd"))
                {
                    var Scan = Process.Start("cmd.exe", "/c" + $"Dir /s /b " + ESP + "EFI\\rEFInd > \"%UserProfile%\\Desktop\\rEFInd Dir.txt\"");
                    Scan.WaitForExit();
                    SpinTasker.Stop(!(Scan.ExitCode > 0), ErrorMessage: "CMD \'Dir\' Сommand Exception : " + Scan.ExitCode);
                }
                else
                    SpinTasker.Stop(ErrorMessage: "CMD \'Dir\' Сommand Exception : Cannot Find \"rEFInd\" Directory");
            }    
        }
    }
}
