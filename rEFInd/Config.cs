using System.Diagnostics;
using System.Text;

namespace rEFInd
{
    internal class Config : WorkClass
    {
        public static StreamWriter? ConfigFile; //Конфиг для записи
        internal static void Computer(bool Desktop = false)
        {
            //Создание загрузочного меню
            string rEFIndInstall = Desktop ? Environment.GetEnvironmentVariable("USERPROFILE") + @"\Desktop\rEFInd" : ESP + @"EFI\rEFInd\";
            SpinTasker.Start("Config generation");
            ConfigFile = new(rEFIndInstall + @"\refind.conf");

            //Конфигурация rEFInd
            ConfigFile.WriteLine("Use_nvram 1");
            ConfigFile.WriteLine("Timeout 20");
            ConfigFile.WriteLine("use_graphics_for linux, windows");
            ConfigFile.WriteLine($"showtools shutdown, reboot, memtest, shell, firmware");
            ConfigFile.WriteLine($"scanfor manual, biosexternal, external, optical, CD\n");

            #region Загрузочные записи rEFInd для поддерживаемых ОС (Config Generator List)
            //Загатовленные записи
            Menuentry(Name: "Windows", Loader: @"EFI\Microsoft\Boot\bootmgfw.efi", OSType: "Windows", Disabled: File.Exists(ESP + @"EFI\HackBGRT\boot" + Arch[0] + ".efi"));
            Menuentry(Name: "HackBGRT", Loader: @"EFI\HackBGRT\boot" + Arch[0] + ".efi", OSType: "Windows");
            Menuentry(Name: "PhoenixOS", Loader: @"EFI\PhoenixOS\kernel", InitRD: @"EFI\PhoenixOS\initrd.img", Options: "quiet root=/dev/ram0 androidboot.hardware=android_x86 SRC=/PhoenixOS vga=788", OSType: "Linux");

            //UNIX-Like System's Menuentry Generator - ULSMG
            string[] List = ExtMounter("/list");
            for (int i = 1; i < List.Length; i++)
            {
                //необходимые переменные
                string Volume = "null";
                string Name = "null";
                string Loader = "null";
                string InitRD = "null";
                string Options = "null";

                //Узнаем имя тома на котором находится система
                for (int j = 0; j < List.Length; j++)
                    if (List[i][j] != ' ')
                        for (int g = 0; g < List[i].Length; g++)
                            if (List[i].Substring(j, g).Contains(' '))
                            {
                                Volume = List[i].Substring(j, g--);
                                break;
                            }

                //Подключаем и читаем grub.cfg
                ExtMounter("/mount disk0,lvm" + i + 3 + " H:\\");
                using (StreamReader Grub = new("H:\\boot\\grub\\grub.cfg"))
                {
                    while (!Grub.EndOfStream)
                    {
                        string Line = Grub.ReadLine().ToLower();
                        if (Line.Contains("submenu"))
                            break;

                        if (Line.Contains("menuentry \'"))
                        {
                            //Узнаем имя системы
                            for (int j = 0; j < Line.Length; j++)
                                if (Line.Substring(11, j).Contains('\''))
                                {
                                    Name = string.Concat(Line.Substring(11, j - 1)[0].ToString().ToUpper(), Line.AsSpan(12, j - 2));
                                    break;
                                }

                            string MenuentryLine = "null";
                            while (!MenuentryLine.Contains('}'))
                            {
                                MenuentryLine = Grub.ReadLine().ToLower();

                                if (MenuentryLine.Contains("\tlinux"))
                                {
                                    //Узнаем расположение загрузчика системы
                                    for (int j = 0; j < MenuentryLine.Length; j++)
                                        if (MenuentryLine.Substring(7, j).Contains(' '))
                                        {
                                            Loader = MenuentryLine.Substring(7, j--);
                                            break;
                                        }

                                    //Узнаем опции загрузчика системы
                                    for (int j = 0; j < MenuentryLine.Length; j++)
                                        if (MenuentryLine.Substring(0, j).Contains(' '))
                                        {
                                            Options = MenuentryLine.Substring(j++, MenuentryLine.Length - j);
                                            break;
                                        }
                                }

                                if (MenuentryLine.Contains("\tinitrd"))
                                {
                                    //узнаем расположение виртуальной файловой системы
                                    for (int j = 0; j < MenuentryLine.Length; j++)
                                    {
                                        try { MenuentryLine.Substring(8, j); }
                                        catch (Exception)
                                        {
                                            InitRD = MenuentryLine.Substring(8, MenuentryLine.Length - 8);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    Menuentry(Name: Name, Volume: Volume, Loader: Loader, InitRD: InitRD, Options: Options, OSType: "Linux");
                }

                ExtMounter("/umount disk0,lvm" + i + 3 + " H:\\");
            }

            //Other
            Menuentry(Name: "Grub FileManager", Loader: @"EFI\GrubFM\grubfm" + Arch[0] + ".efi", OSType: "Linux");
            #endregion

            //Установка темы
            if (!string.IsNullOrWhiteSpace(Options.Theme))
                ConfigFile.Write("Include Theme\\theme.conf");

            //Запись результата
            ConfigFile.Close();
            SpinTasker.Stop(File.Exists(rEFIndInstall + @"\refind.conf"), ErrorMessage: "Impossible to save Config File");
        }

        internal static void USB()
        {
            //Создание загрузочного меню
            string rEFIndInstall = Drive.Name + @"EFI\Boot\";
            SpinTasker.Start("Config generation");
            ConfigFile = new(rEFIndInstall + @"\refind.conf");

            //Конфигурация rEFInd
            ConfigFile.WriteLine("Use_nvram 1");
            ConfigFile.WriteLine("Timeout 20");
            ConfigFile.WriteLine("use_graphics_for linux, windows");
            ConfigFile.WriteLine($"showtools shutdown, reboot, memtest, shell, firmware");
            ConfigFile.WriteLine($"scanfor manual, biosexternal, external, optical, CD\n");

            //Установка темы
            if (!string.IsNullOrWhiteSpace(Options.Theme))
                ConfigFile.Write("Include Theme\\theme.conf");

            //Запись результата
            ConfigFile.Close();
            SpinTasker.Stop(File.Exists(rEFIndInstall + @"\refind.conf"), ErrorMessage: "Impossible to save Config File");
        }

        internal static void Menuentry(string Name, string Loader, string? Icon = null, string? Volume = null, string? InitRD = null, string? Options = null, string? OSType = null, bool Graphics = true, bool Disabled = false)
        {
            StringBuilder MenuEntry = new($"menuentry \"" + Name + "\" {", 50);

            //Сканируем загрузчик
            if (OSType.ToLower() == "linux" && Directory.Exists(ESP + Loader))
                MenuEntry.Append("\n\tloader " + Directory.GetFiles(Loader, Arch[0])[0]);
            else
                MenuEntry.Append("\n\tloader " + Loader);

            //Добавляем парметры
            if (!string.IsNullOrWhiteSpace(Icon)) MenuEntry.Append("\n\ticon " + Icon);
            if (!string.IsNullOrWhiteSpace(Volume)) MenuEntry.Append("\n\tvolume " + Volume);
            if (!string.IsNullOrWhiteSpace(InitRD)) MenuEntry.Append("\n\tinitrd " + InitRD);
            if (!string.IsNullOrWhiteSpace(Options)) MenuEntry.Append($"\n\toptions \"{Options}\"");
            if (!string.IsNullOrWhiteSpace(OSType)) MenuEntry.Append("\n\tostype " + OSType);
            if (Graphics) MenuEntry.Append("\n\tGraphics on");
            if (Disabled) MenuEntry.Append("\n\tDisabled");
            MenuEntry.Append("\n}\n");

            ConfigFile.WriteLine(MenuEntry);
        }

        public static string[] ExtMounter(string Args)
        {
            var ExtMounterProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "extmounter.exe",
                    Arguments = Args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            ExtMounterProcess.Start();
            ExtMounterProcess.WaitForExit();

            if (ExtMounterProcess.ExitCode > 0)
                SpinTasker.Stop(ErrorMessage: "Ext File System Driver Error!");

            List<string> Output = new();
            while (!ExtMounterProcess.StandardOutput.EndOfStream)
            {
                Output.Add(ExtMounterProcess.StandardOutput.ReadLine());
            }

            return Output.ToArray();
        }
    }
}
