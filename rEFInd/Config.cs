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
            ConfigFile = new (rEFIndInstall + @"\refind.conf");

            //Конфигурация rEFInd
            ConfigFile.WriteLine("Use_nvram 1");
            ConfigFile.WriteLine("Timeout 20");
            ConfigFile.WriteLine("use_graphics_for linux, windows");
            ConfigFile.WriteLine($"showtools shutdown, reboot, memtest, shell, firmware");
            ConfigFile.WriteLine($"scanfor manual, biosexternal, external, optical, CD\n");

            if (Options.IgnoreCGL)
            {
                //Бета функция! не рекомендуется к использованию
                //Генерирует загрузочное меню по загрузчикам на ESP игнорируюя заданный список (CGL)
                Menuentry(Name: "Windows", Loader: @"EFI\Microsoft\Boot\bootmgfw.efi", OSType: "Windows");
                foreach (string Dir in Directory.GetDirectories(ESP + "EFI"))
                    if (Directory.EnumerateFiles(Dir, "*.efi").Any())
                        foreach (string file in Directory.GetFiles(Dir, "*" + Arch[0] + ".efi"))
                        {
                            string LoaderDir = Dir.Substring(7);
                            if (!LoaderDir.Contains("Boot"))
                                Menuentry(Name: LoaderDir, Loader: file.Substring(3));
                        }
            }
            else //Загрузочные записи rEFInd поддерживаемых ОС (Config Generator List)
            {
                //Windows, DOS
                Menuentry(Name: "Windows", Loader: @"EFI\Microsoft\Boot\bootmgfw.efi", OSType: "Windows", Disabled: File.Exists(ESP + @"EFI\HackBGRT\boot" + Arch[0] + ".efi"));
                Menuentry(Name: "HackBGRT", Loader: @"EFI\HackBGRT\boot" + Arch[0] + ".efi", OSType: "Windows");

                //Android Based
                Menuentry(Name: "PhoenixOS", Loader: @"EFI\PhoenixOS\kernel", InitRD: @"EFI\PhoenixOS\initrd.img", Options: "quiet root=/dev/ram0 androidboot.hardware=android_x86 SRC=/PhoenixOS vga=788", OSType: "Linux");

                //UNIX-Like
                Menuentry(Name: "Ubuntu", Loader: @"EFI\ubuntu\shim" + Arch[0] + ".efi", OSType: "Linux");
                Menuentry(Name: "CentOS", Loader: @"EFI\centos\shim" + Arch[0] + ".efi", OSType: "Linux");
                Menuentry(Name: "Debian", Loader: @"EFI\debian\shim" + Arch[0] + ".efi", OSType: "Linux");
                Menuentry(Name: "Fedora", Loader: @"EFI\fedora\shim" + Arch[0] + ".efi", OSType: "Linux");
                Menuentry(Name: "Kali", Loader: @"EFI\kali\grub" + Arch[0] + ".efi", OSType: "Linux");

                //Tools
                Menuentry(Name: "Grub FileManager", Loader: @"EFI\GrubFM\grubfm" + Arch[0] + ".efi", OSType: "Linux");
            }

            //Установка темы
            if (!string.IsNullOrWhiteSpace(Options.Theme))
                ConfigFile.Write("Include Theme\\theme.conf");

            //Запись результата
            ConfigFile.Close();
            SpinTasker.Stop(File.Exists(ESP + @"EFI\rEFInd\refind.conf"), ErrorMessage: "Impossible to save Config File");
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

            if (Options.IgnoreCGL)
            {
                //Бета функция! не рекомендуется к использованию
                //Генерирует загрузочное меню по загрузчикам на ESP игнорируюя заданный список (CGL)
                foreach (string Dir in Directory.GetDirectories(ESP + "EFI"))
                    if (Directory.EnumerateFiles(Dir, "*.efi").Any())
                        foreach (string file in Directory.GetFiles(Dir, "*" + Arch[0] + ".efi"))
                        {
                            string LoaderDir = Dir.Substring(7);
                            if (!LoaderDir.Contains("Boot"))
                                Menuentry(Name: LoaderDir, Loader: file.Substring(3));
                        }
            }

            if (!string.IsNullOrWhiteSpace(Options.Theme))
                ConfigFile.Write("Include Theme\\theme.conf");
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
            if (!string.IsNullOrWhiteSpace(Icon))    MenuEntry.Append("\n\ticon " + Icon);
            if (!string.IsNullOrWhiteSpace(Volume))  MenuEntry.Append("\n\tvolume " + Volume);
            if (!string.IsNullOrWhiteSpace(InitRD))  MenuEntry.Append("\n\tinitrd " + InitRD);
            if (!string.IsNullOrWhiteSpace(Options)) MenuEntry.Append($"\n\toptions \"{Options}\"");
            if (!string.IsNullOrWhiteSpace(OSType))  MenuEntry.Append("\n\tostype " + OSType);
            if (Graphics)                            MenuEntry.Append("\n\tGraphics on");
            if (Disabled)                            MenuEntry.Append("\n\tDisabled");
            MenuEntry.Append("\n}\n");

            //Отправляем в refind.conf
            if (File.Exists(ESP + Loader))
                ConfigFile.WriteLine(MenuEntry);
        }

    }
}
