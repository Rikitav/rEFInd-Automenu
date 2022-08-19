using rEFIndAutomenu;
using System.Text;

namespace rEFInd_Automenu
{
    internal class Config : Form1
    {
        public static Form1 rEFInd = new();
        public static void Computer()
        {
            //Создание загрузочного меню
            string rEFIndInstall = ESP + @"EFI\rEFInd\";
            rEFInd.ToolStripLabel.Text = "Config generation";
            StreamWriter ConfigFile = new(rEFIndInstall + @"\refind.conf");

            //Конфигурация rEFInd
            ConfigFile.WriteLine("Use_nvram 1");
            ConfigFile.WriteLine("Timeout 20");
            ConfigFile.WriteLine("use_graphics_for linux, windows");
            ConfigFile.WriteLine($"showtools shutdown, reboot, memtest, shell, firmware");
            ConfigFile.WriteLine($"scanfor manual, biosexternal, external, optical, CD\n");

            //Microsoft Windows-EFI
            if (File.Exists(ESP + @"EFI\Microsoft\Boot\bootmgfw.efi"))
                ConfigFile.WriteLine(Menuentry(Name: "Windows", Loader: @"EFI\Microsoft\Boot\bootmgfw.efi", OSType: "Windows", Disabled: File.Exists(ESP + @"EFI\HackBGRT\boot" + Arch + ".efi")));

            //PhoenixOS
            if (File.Exists(ESP + @"EFI\PhoenixOS\kernel") & File.Exists(ESP + @"EFI\PhoenixOS\initrd.img"))
                ConfigFile.WriteLine(Menuentry(Name: "PhoenixOS", Loader: @"EFI\PhoenixOS\kernel", InitRD: @"EFI\PhoenixOS\initrd.img", Options: "quiet root=/dev/ram0 androidboot.hardware=android_x86 SRC=/PhoenixOS vga=788", OSType: "Linux"));

            //Сканирование других загрузчиков
            foreach (string EfiDir in Directory.GetDirectories(ESP + @"EFI\"))
                if (!EfiDir.ToLower().Contains("microsoft") && !EfiDir.ToLower().Contains("boot") && !EfiDir.ToLower().Contains("refind"))
                    if (Directory.EnumerateFiles(EfiDir, @$"*{Arch}*.efi").Any())
                    {
                        string Name = Path.GetFileName(EfiDir);
                        if (!Name.Any(char.IsUpper))
                            Name = string.Concat(Name[0].ToString().ToUpper(), Name[1..]);

                        string Loader = Directory.EnumerateFiles(EfiDir, @$"*{Arch}*.efi").First()[2..];
                        ConfigFile.WriteLine(Menuentry(Name: Name, Loader: Loader, OSType: "Linux"));
                    }

            if (rEFInd.CheckThemePath.Checked & File.Exists(rEFInd.TextBoxThemeConf.Text))
            {
                try
                {
                    rEFInd.ToolStripLabel.Text = "Theme install";
                    CopyDirectory(Path.GetDirectoryName(rEFInd.TextBoxThemeConf.Text), rEFIndInstall + @"\Theme", true);
                    ConfigFile.WriteLine(@"include \Theme\theme.conf");
                }
                catch (Exception Error)
                {
                    rEFInd.ToolStripLabel.Text = "Theme Install - Error";
                    MessageBox.Show("Error is occured when theme for rEFInd\nTheme will not be installed\nException Message : " + Error.Message);
                    ConfigFile.Close();
                    return;
                }
            }

            //Запись результата
            try
            {
                ConfigFile.Close();
                rEFInd.ToolStripLabel.Text = "Installing rEFInd - Succes";
                MessageBox.Show("rEFInd succesfuly installed on current computer");
                return;
            }
            catch (Exception Error)
            {
                rEFInd.ToolStripLabel.Text = "Config generation - Error";
                MessageBox.Show("Error is ocurred when generating rEFInd.conf\nError Message : " + Error.Message);
                return;
            }
        }

        public static void USB()
        {
            //Создание загрузочного меню
            rEFInd.ToolStripLabel.Text = "Config generation";
            string rEFIndInstall = InstallDrive.Name + @"\EFI\boot";
            StreamWriter ConfigFile = new(rEFIndInstall + @"\refind.conf");

            //Установка Темы
            if (rEFInd.CheckThemePath.Checked & File.Exists(rEFInd.TextBoxThemeConf.Text))
            {
                try
                {
                    rEFInd.ToolStripLabel.Text = "Theme install";
                    CopyDirectory(rEFInd.TextBoxThemeConf.Text, rEFIndInstall + @"\Theme", true);
                    ConfigFile.WriteLine(@"include \Theme\theme.conf");
                }
                catch (Exception Error)
                {
                    rEFInd.ToolStripLabel.Text = "Theme install - Error";
                    MessageBox.Show("Error is occured when theme for rEFInd\nTheme will not be installed\nException Message : " + Error.Message);
                    ConfigFile.Close();
                    return;
                }
            }

            //Конфигурация rEFInd
            ConfigFile.WriteLine("Use_nvram 1");
            ConfigFile.WriteLine("Timeout 20");
            ConfigFile.WriteLine("use_graphics_for linux, windows");
            ConfigFile.WriteLine($"showtools shutdown, reboot, memtest, shell, firmware");
            ConfigFile.WriteLine($"scanfor manual, biosexternal, external, optical, CD\n");

            //Запись результата
            try
            {
                ConfigFile.Close();
            }
            catch (Exception Error)
            {
                rEFInd.ToolStripLabel.Text = "Config generation - Error";
                MessageBox.Show("Error is ocurred when generating rEFInd.conf\nError Message : " + Error.Message);
                return;
            }
        }

        public static void Desktop()
        {
            //Создание загрузочного меню
            rEFInd.ToolStripLabel.Text = "Config generation";
            string rEFIndInstall = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Desktop\rEFInd\rEFInd\";
            StreamWriter ConfigFile = new(rEFIndInstall + @"\refind.conf");

            //Установка Темы
            if (rEFInd.CheckThemePath.Checked & File.Exists(rEFInd.TextBoxThemeConf.Text))
            {
                try
                {
                    rEFInd.ToolStripLabel.Text = "Theme install";
                    CopyDirectory(rEFInd.TextBoxThemeConf.Text, rEFIndInstall + @"\Theme", true);
                    ConfigFile.WriteLine(@"include \Theme\theme.conf");
                }
                catch (Exception Error)
                {
                    rEFInd.ToolStripLabel.Text = "Theme install - Error";
                    MessageBox.Show("Error is occured when theme for rEFInd\nTheme will not be installed\nException Message : " + Error.Message);
                    ConfigFile.Close();
                    return;
                }
            }

            //Конфигурация rEFInd
            ConfigFile.WriteLine("Use_nvram 1");
            ConfigFile.WriteLine("Timeout 20");
            ConfigFile.WriteLine("use_graphics_for linux, windows");
            ConfigFile.WriteLine($"showtools shutdown, reboot, memtest, shell, firmware");
            ConfigFile.WriteLine($"scanfor manual, biosexternal, external, optical, CD\n");

            //Запись результата
            try
            {
                ConfigFile.Close();
            }
            catch (Exception Error)
            {
                rEFInd.ToolStripLabel.Text = "Config generation - Error";
                MessageBox.Show("Error is ocurred when generating rEFInd.conf\nError Message : " + Error.Message);
                return;
            }
        }

        private static string Menuentry(string Name, string Loader, string? Icon = null, string? Volume = null, string? InitRD = null, string? Options = null, string? OSType = null, bool Graphics = true, bool Disabled = false)
        {
            StringBuilder MenuEntry = new($"menuentry \"" + Name + "\" {", 50);
            MenuEntry.Append("\n\tloader " + Loader);

            //Добавляем парметр иконки
            if (!string.IsNullOrWhiteSpace(Icon)) 
                MenuEntry.Append("\n\ticon " + Icon);

            //Добавляем парметр раздела
            if (!string.IsNullOrWhiteSpace(Volume)) 
                MenuEntry.Append("\n\tvolume " + Volume);

            //Добавляем парметр виртуальной файловой системы
            if (!string.IsNullOrWhiteSpace(InitRD)) 
                MenuEntry.Append("\n\tinitrd " + InitRD);

            //Добавляем парметр опций загрузчика
            if (!string.IsNullOrWhiteSpace(Options)) 
                MenuEntry.Append($"\n\toptions \"{Options}\"");

            //Добавляем парметр типа системы
            if (!string.IsNullOrWhiteSpace(OSType)) 
                MenuEntry.Append("\n\tostype " + OSType);

            //Добавляем парметр использования графики
            if (Graphics) 
                MenuEntry.Append("\n\tGraphics on");

            //Добавляем парметр отключенного загрузчика
            if (Disabled) 
                MenuEntry.Append("\n\tDisabled");

            //Отправка реультата
            return MenuEntry.Append("\n}\n").ToString();
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
    }
}

// Эти строки остались после технологии CGL
/*
            //Microsoft Windows-EFI
            if (File.Exists(ESP + @"EFI\Microsoft\Boot\bootmgfw.efi")) 
                ConfigFile.WriteLine(Menuentry(Name: "Windows", Loader: @"EFI\Microsoft\Boot\bootmgfw.efi", OSType: "Windows", Disabled: File.Exists(ESP + @"EFI\HackBGRT\boot" + Arch + ".efi")));

            //HackBGRT
            if (File.Exists(ESP + @"EFI\HackBGRT\boot" + Arch + ".efi"))
                ConfigFile.WriteLine(Menuentry(Name: "HackBGRT", Loader: @"EFI\HackBGRT\boot" + Arch + ".efi", OSType: "Windows"));

            //PhoenixOS
            if (File.Exists(ESP + @"EFI\PhoenixOS\kernel") & File.Exists(ESP + @"EFI\PhoenixOS\initrd.img"))
                ConfigFile.WriteLine(Menuentry(Name: "PhoenixOS", Loader: @"EFI\PhoenixOS\kernel", InitRD: @"EFI\PhoenixOS\initrd.img", Options: "quiet root=/dev/ram0 androidboot.hardware=android_x86 SRC=/PhoenixOS vga=788", OSType: "Linux"));
            
            //Android x86 Project
            if (File.Exists(ESP + @"EFI\android\boot" + Arch + ".efi"))
                ConfigFile.WriteLine(Menuentry(Name: "Android", Loader: @"EFI\android\boot" + Arch + ".efi", OSType: "Linux"));

            //Linux Ubuntu
            if (File.Exists(ESP + @"EFI\ubuntu\shim" + Arch + ".efi"))
                ConfigFile.WriteLine(Menuentry(Name: "Ubuntu", Loader: @"\EFI\debian\shim" + Arch + ".efi", OSType: "Linux"));

            //Linux Debian
            if (File.Exists(ESP + @"EFI\debian\shim" + Arch + ".efi"))
                ConfigFile.WriteLine(Menuentry(Name: "Debian", Loader: @"EFI\debian\shim" + Arch + ".efi", OSType: "Linux"));

            //CentOS
            if (File.Exists(ESP + @"EFI\centos\shim" + Arch + ".efi"))
                ConfigFile.WriteLine(Menuentry(Name: "CentOS", Loader: @"EFI\centos\shim" + Arch + ".efi", OSType: "Linux"));

            //Fedora Linux
            if (File.Exists(ESP + @"EFI\fedora\shim" + Arch + ".efi"))
                ConfigFile.WriteLine(Menuentry(Name: "Fedora", Loader: @"EFI\fedora\shim" + Arch + ".efi", OSType: "Linux"));
            
            //Kali Linux
            if (File.Exists(ESP + @"EFI\kali\grub" + Arch + ".efi"))
                ConfigFile.WriteLine(Menuentry(Name: "Kali", Loader: @"EFI\kali\grub" + Arch + ".efi", OSType: "Linux"));

            //OpenMandrivaLX
            if (File.Exists(ESP + @"EFI\OpenMandriva_Lx_[GRUB]\grub" + Arch + ".efi"))
                ConfigFile.WriteLine(Menuentry(Name: "OpenMandriva", Loader: @"EFI\OpenMandriva_Lx_[GRUB]\grub" + Arch + ".efi", OSType: "Linux"));

            //Grub FileManager
            if (File.Exists(ESP + @"EFI\GrubFM\grubfm" + Arch + ".efi"))
                ConfigFile.WriteLine(Menuentry(Name: "Grub FileManager", Loader: @"EFI\GrubFM\grubfm" + Arch + ".efi", OSType: "Linux"));
            */