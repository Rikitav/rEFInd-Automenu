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

            //Microsoft Windows-EFI
            if (File.Exists(ESP + @"EFI\Microsoft\Boot\bootmgfw.efi"))
                ConfigFile.WriteLine(Menuentry(Name: "Windows", Loader: @"EFI\Microsoft\Boot\bootmgfw.efi", OSType: "Windows", Disabled: File.Exists(ESP + @"EFI\HackBGRT\boot" + Arch[0] + ".efi")));

            //PhoenixOS
            if (File.Exists(ESP + @"EFI\PhoenixOS\kernel") & File.Exists(ESP + @"EFI\PhoenixOS\initrd.img"))
                ConfigFile.WriteLine(Menuentry(Name: "PhoenixOS", Loader: @"EFI\PhoenixOS\kernel", InitRD: @"EFI\PhoenixOS\initrd.img", Options: "quiet root=/dev/ram0 androidboot.hardware=android_x86 SRC=/PhoenixOS vga=788", OSType: "Linux"));

            //Сканирование других загрузчиков
            foreach (string EfiDir in Directory.GetDirectories(ESP + @"EFI\"))
                if (!EfiDir.Contains("Microsoft") && !EfiDir.Contains("Boot") && !EfiDir.Contains("rEFInd"))
                    if (Directory.EnumerateFiles(EfiDir, @$"*{Arch[0]}*.efi").Any())
                    {
                        string Name = Path.GetFileName(EfiDir);
                        if (!Name.Any(char.IsUpper))
                            Name = string.Concat(Name[0].ToString().ToUpper(), Name[1..]);

                        string Loader = Directory.EnumerateFiles(EfiDir, @$"*{Arch[0]}*.efi").First()[2..];
                        ConfigFile.WriteLine(Menuentry(Name: Name, Loader: Loader, OSType: "Linux"));
                    }

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

        internal static string Menuentry(string Name, string Loader, string? Icon = null, string? Volume = null, string? InitRD = null, string? Options = null, string? OSType = null, bool Graphics = true, bool Disabled = false)
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

            return MenuEntry.ToString();
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
