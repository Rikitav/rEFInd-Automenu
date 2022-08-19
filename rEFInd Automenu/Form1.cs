using System.Diagnostics;
using System.Reflection;

namespace rEFInd_Automenu
{
    public partial class Form1 : Form
    {
        //WorkInfo
        public static string ESP;
        public static string Arch;
        public static DriveInfo InstallDrive;
        public static string Temp = Path.GetTempPath();
        public static string rEFIndBin = Temp + "refind-bin.zip";
        public static string rEFIndData;

        //Метод проыеряющий подключение к интернету
        public static int InternetDesc;
        [System.Runtime.InteropServices.DllImport("wininet.dll")]
        public static extern bool InternetGetConnectedState(out int Description, int ReservedValue);

        public Form1()
        {
            //Инициализируем компоненты
            InitializeComponent();
            ToolStripESP.Text = "ESP : " + ESP;
            CheckDownload.Enabled = InternetGetConnectedState(out InternetDesc, 0);

            //Проверяем установленный rEFInd
            bool rEFIndInstalled = Directory.Exists(ESP + @"EFI\rEFInd\") && Directory.EnumerateFiles(ESP + @"EFI\rEFInd\", "refind_*.efi").Any();
            ConfigButton.Enabled = rEFIndInstalled;
            RemoveButton.Enabled = rEFIndInstalled;

            //Добавляем накопители
            IEnumerable<string> Drives = from Drive in DriveInfo.GetDrives() where Drive.DriveType == DriveType.Removable select Drive.Name;
            ComboBoxSelectDrive.Items.AddRange(Drives.ToArray());
        }

        //RadioBoutton's
        private void RadioInstallComputer_CheckedChanged(object sender, EventArgs e)
        {
            //Button's
            InstallButton.Enabled = true;

            //GroupBoxe's
            GroupBoxInstallOptions.Enabled = true;
            GroupBoxFlashOptions.Enabled = false;

            //CheckBoxe's
            CheckForceArch.Enabled = true;
            CheckThemePath.Enabled = true;
        }
        private void RadioInstallFlash_CheckedChanged(object sender, EventArgs e)
        {
            //Button's
            InstallButton.Enabled = true;

            //GroupBoxe's
            GroupBoxInstallOptions.Enabled = true;
            GroupBoxFlashOptions.Enabled = true;

            //CheckBoxe's
            CheckForceArch.Enabled = false;
            CheckThemePath.Enabled = true;
        }
        private void RadioInstallDesktop_CheckedChanged(object sender, EventArgs e)
        {
            //Button's
            InstallButton.Enabled = true;

            //GroupBoxe's
            GroupBoxInstallOptions.Enabled = false;
            GroupBoxFlashOptions.Enabled = false;

            //CheckBoxe's
            CheckForceArch.Enabled = false;
            CheckThemePath.Enabled = false;
        }

        //Button's
        private void InstallButton_Click(object sender, EventArgs e)
        {
            if (PrepareForInstall())
            {
                if (RadioInstallComputer.Checked)
                {
                    Computer();
                }
                else if (RadioInstallDesktop.Checked)
                {
                    Desktop();
                }
                else if (RadioInstallFlash.Checked)
                {
                    InstallDrive = (from Drive in DriveInfo.GetDrives() where Drive.Name == ComboBoxSelectDrive.Text select Drive).First();
                    USB();
                }
            }

            void Computer()
            {
                //Подготовка к установке
                ToolStripLabel.Text = "Preparing for installing";
                string rEFIndInstall = ESP + @"EFI\";

                //Проверка на уже установленный rEFInd
                if (Directory.Exists(rEFIndInstall + @"\rEFInd"))
                {
                    MessageBox.Show("You already have installed rEFInd, Remove him first");
                    ToolStripLabel.Text = "Installing rEFInd - Error";
                    return;
                }

                ToolStripLabel.Text = "Installing rEFInd";
                try
                {
                    //Создание, копирование необходимых файлов
                    Directory.CreateDirectory(rEFIndInstall + @"rEFInd\");
                    CopyDirectory(rEFIndData + @"\keys", rEFIndInstall + @"\keys", true);
                    CopyDirectory(rEFIndData + @"\refind\Icons", rEFIndInstall + @"\rEFInd\Icons", true);
                    CopyDirectory(rEFIndData + @"\refind\drivers_" + Arch, rEFIndInstall + @"\rEFInd\drivers_" + Arch, true);
                    CopyDirectory(rEFIndData + @"\refind\tools_" + Arch, rEFIndInstall + @"\rEFInd\tools_" + Arch, true);
                    File.Copy(rEFIndData + @"refind\refind_" + Arch + ".efi", rEFIndInstall + @"\rEFInd\refind_" + Arch + ".efi");
                }
                catch (Exception Error)
                {
                    ToolStripLabel.Text = "Installing rEFInd - Error";
                    MessageBox.Show("Error is ocurred when installing rEFInd\nError Message : " + Error.Message);
                    return;
                }

                if (!File.Exists(rEFIndInstall + @"\rEFInd\refind_" + Arch + ".efi") || !Directory.Exists(rEFIndInstall + @"\rEFInd\Icons") || !Directory.Exists(rEFIndInstall + @"\rEFInd\drivers_" + Arch))
                {
                    ToolStripLabel.Text = "Installing rEFInd - Error";
                    MessageBox.Show("Impossible to unpack the binary");
                    return;
                }

                //Запись загрузку rEFInd в BCD контейнер
                ToolStripLabel.Text = "Booting rEFInd";
                var BcdEdit = new Process();
                BcdEdit.StartInfo.FileName = "bcdedit.exe";
                BcdEdit.StartInfo.CreateNoWindow = true;

                BcdEdit.StartInfo.Arguments = "/set \"{bootmgr}\" " + @"path \EFI\refind\refind_" + Arch + ".efi";
                BcdEdit.Start();
                BcdEdit.WaitForExit();

                if (BcdEdit.ExitCode != 0)
                {
                    ToolStripLabel.Text = "Booting rEFInd - Error";
                    MessageBox.Show("Error is ocurred when registring rEFInd in EFI-BCD\nCMD \'BcdEdit\' Сommand Exception : " + BcdEdit.ExitCode);
                    return;
                }
                else
                {
                    BcdEdit.StartInfo.Arguments = "/set \"{bootmgr}\" description \"rEFInd\"";
                    BcdEdit.Start();
                    BcdEdit.WaitForExit();
                }

                //Создание загрузочного меню
                ToolStripLabel.Text = "Config generation";
                StreamWriter ConfigFile = new(rEFIndInstall + @"\rEFInd\refind.conf");

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

                if (CheckThemePath.Checked & File.Exists(TextBoxThemeConf.Text))
                {
                    try
                    {
                        ToolStripLabel.Text = "Theme install";
                        CopyDirectory(Path.GetDirectoryName(TextBoxThemeConf.Text), rEFIndInstall + @"\rEFInd\Theme", true);
                        ConfigFile.WriteLine(@"include \Theme\theme.conf");
                    }
                    catch (Exception Error)
                    {
                        ToolStripLabel.Text = "Theme Install - Error";
                        MessageBox.Show("Error is occured when theme for rEFInd\nException Message : " + Error.Message + "\nTheme will not be installed");
                    }
                }

                //Запись результата
                try
                {
                    ConfigFile.Close();
                    ToolStripLabel.Text = "Installing rEFInd - Succes";
                    MessageBox.Show("rEFInd succesfuly installed on current computer");
                    return;
                }
                catch (Exception Error)
                {
                    ToolStripLabel.Text = "Config generation - Error";
                    MessageBox.Show("Error is ocurred when generating rEFInd.conf\nError Message : " + Error.Message);
                    return;
                }
            }
            void USB()
            {
                string rEFIndInstall = InstallDrive.Name + @"\EFI\";
                if (InstallDrive.DriveFormat != "FAT32")
                    if (CheckFormat.Checked & CheckFormat.Enabled)
                    {
                        ToolStripLabel.Text = "Formating Drive";
                        var FormatDrive = Process.Start("cmd.exe", "/c" + $"Format " + InstallDrive.Name[..2] + " /FS:FAT32 /V:REFIND /Q /Y > nul");
                        FormatDrive.WaitForExit();
                        if (FormatDrive.ExitCode != 0)
                        {
                            ToolStripLabel.Text = "Formating Drive - Error";
                            MessageBox.Show("Error is ocurred when formating drive" + InstallDrive.Name + "\nCMD \'Format\' Сommand Exception : " + FormatDrive.ExitCode);
                            return;
                        }
                    }
                    else
                    {
                        ToolStripLabel.Text = "Formating Drive - Error";
                        MessageBox.Show($"Drive {InstallDrive.Name} has FileSystem {InstallDrive.DriveFormat}\nrEFInd needs FAT32 file system for work");
                        return;
                    }

                ToolStripLabel.Text = "Installing rEFInd";
                if (Directory.Exists(rEFIndInstall + "\\boot"))
                {
                    if (Directory.EnumerateFiles(rEFIndInstall + @"\boot", "boot*.efi").Any())
                    {
                        ToolStripLabel.Text = "Installing rEFInd - Error";
                        MessageBox.Show($"Drive {InstallDrive.Name} Already has a Boot-Loader\nDelete it first or move");
                        return;
                    }
                }

                try
                {
                    //Создание, копирование необходимых директорий
                    Directory.CreateDirectory(rEFIndInstall + @"\boot");
                    CopyDirectory(rEFIndData + @"\keys", rEFIndInstall + @"\keys", true);
                    CopyDirectory(rEFIndData + @"\refind", rEFIndInstall + @"rEFInd", true);
                    File.Move(rEFIndInstall + @"\rEFInd\refind_x64.efi", rEFIndInstall + @"boot\bootx64.efi");
                    File.Move(rEFIndInstall + @"\rEFInd\refind_aa64.efi", rEFIndInstall + @"boot\bootaa64.efi");
                    File.Move(rEFIndInstall + @"\rEFInd\refind_ia32.efi", rEFIndInstall + @"boot\bootia32.efi");
                }
                catch (FileNotFoundException Error)
                {
                    ToolStripLabel.Text = "Installing rEFInd - Error";
                    MessageBox.Show("Error is ocurred when coping binary\nError Message : " + Error.Message);
                    return;
                }

                //Создание загрузочного меню
                ToolStripLabel.Text = "Config generation";
                StreamWriter ConfigFile = new(rEFIndInstall + @"\boot\refind.conf");

                //Установка Темы
                if (CheckThemePath.Checked & File.Exists(TextBoxThemeConf.Text))
                {
                    try
                    {
                        ToolStripLabel.Text = "Theme install";
                        CopyDirectory(Path.GetDirectoryName(TextBoxThemeConf.Text), rEFIndInstall + @"rEFInd\Theme", true);
                        ConfigFile.WriteLine(@"include \Theme\theme.conf");
                    }
                    catch (Exception Error)
                    {
                        ToolStripLabel.Text = "Theme install - Error";
                        MessageBox.Show("Error is occured when coping theme for rEFInd\nException Message : " + Error.Message + "\nTheme will not be installed");
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
                    ToolStripLabel.Text = "Installing rEFInd - Succes";
                    MessageBox.Show("rEFInd succesfuly installed on " + InstallDrive.Name);
                    return;
                }
                catch (Exception Error)
                {
                    ToolStripLabel.Text = "Config generation - Error";
                    MessageBox.Show("Error is ocurred when generating rEFInd.conf\nError Message : " + Error.Message);
                    return;
                }
            }
            void Desktop()
            { //Подготовка к установке
                string rEFIndInstall = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Desktop\rEFInd\";
                ToolStripLabel.Text = "Installing rEFInd";

                //Проверка на уже распакованный rEFInd
                if (Directory.Exists(rEFIndInstall))
                    Directory.Delete(rEFIndInstall);

                try
                {
                    //Создание, копирование необходимых директорий
                    Directory.CreateDirectory(rEFIndInstall + @"rEFInd\");
                    CopyDirectory(rEFIndData + @"\keys", rEFIndInstall + @"\keys", true);
                    CopyDirectory(rEFIndData + @"\refind", rEFIndInstall + @"rEFInd", true);
                }
                catch (Exception Error)
                {
                    ToolStripLabel.Text = "Installing rEFInd - Error";
                    MessageBox.Show("Error is ocurred when installing rEFInd\nError Message : " + Error.Message);
                    return;
                }

                //Создание загрузочного меню
                ToolStripLabel.Text = "Config generation";
                StreamWriter ConfigFile = new(rEFIndInstall + @"\rEFInd\refind.conf");

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
                    ToolStripLabel.Text = "Installing rEFInd - Succes";
                    MessageBox.Show("rEFInd succesfuly unpacked on your desktop");
                    return;
                }
                catch (Exception Error)
                {
                    ToolStripLabel.Text = "Config generation - Error";
                    MessageBox.Show("Error is ocurred when generating rEFInd.conf\nError Message : " + Error.Message);
                    return;
                }
            }
            bool PrepareForInstall()
            {
                //Получаем архитектуру процессора
                ToolStripLabel.Text = "Getting processor architecture";
                string ArchInit = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE").ToUpper();
                if (CheckForceArch.Checked)
                {
                    if (RadioArch64.Checked) ArchInit = "AMD64";
                    else if (RadioArchARM64.Checked) ArchInit = "ARM64";
                    else if (RadioArchX86.Checked) ArchInit = "X86";
                }

                switch (ArchInit)
                {
                    case "AMD64": Arch = "x64"; break;
                    case "IA64": Arch = "x64"; break;
                    case "X86": Arch = "ia32"; break;
                    case "ARM64": Arch = "aa64"; break;
                    default:
                        {
                            ToolStripLabel.Text = "Getting processor architecture - Error";
                            MessageBox.Show($"rEFInd do not support {ArchInit} processors");
                            return false;
                        }
                }

                if (File.Exists(Temp + "rEFInd-Bin.zip"))
                    File.Delete(Temp + "rEFInd-Bin.zip");

                if (Directory.Exists(Temp + "rEFInd"))
                    Directory.Delete(Temp + "rEFInd", true);

                //Получаем архив
                if (CheckDownload.Enabled & CheckDownload.Checked & InternetGetConnectedState(out InternetDesc, 0))
                {
                    try
                    {
                        using (System.Net.WebClient rEFIndHttp = new())
                        {
                            ToolStripLabel.Text = "Downloading rEFInd";
                            rEFIndHttp.DownloadFile(@"https://sourceforge.net/projects/refind/files/latest/download", Temp + "refind-bin.zip");
                        }
                    }
                    catch (Exception Error)
                    {
                        MessageBox.Show("Error is occured when downloading rEFInd\nException Message : " + Error.Message + "\nWe unpack Pre-Installed version");
                        if (!UnpackArchive())
                        {
                            ToolStripLabel.Text = "Unpacking rEFInd - Error";
                            MessageBox.Show("Error is ocurred when unpacking Bin-Archive");
                            return false;
                        }
                    }
                }
                else
                {
                    if (!UnpackArchive())
                    {
                        ToolStripLabel.Text = "Unpacking rEFInd - Error";
                        MessageBox.Show("Error is ocurred when unpacking Bin-Archive");
                        return false;
                    }
                }

                try
                {
                    Directory.CreateDirectory(Temp + "rEFInd");
                    System.IO.Compression.ZipFile.ExtractToDirectory(rEFIndBin, Temp + "rEFInd", true);
                    rEFIndData = Directory.GetDirectories(Temp + "rEFInd", "refind-bin-0.13*")[0] + "\\";
                }
                catch (Exception Error)
                {
                    ToolStripLabel.Text = "Unpacking rEFInd - Error";
                    MessageBox.Show("Error is ocurred when unpacking Bin-Archive\nError Message : " + Error.Message);
                    return false;
                }

                return true;
            }
            bool UnpackArchive()
            {
                /*
                var myAssembly = Assembly.GetExecutingAssembly();
                string resourceName = "refind-bin.zip";
                string fullResourceName = $"{myAssembly.GetName().Name}.{resourceName.Replace('\\', '.')}";

                ToolStripLabel.Text = "Unpacking rEFInd";
                if (myAssembly.GetManifestResourceNames().Contains(fullResourceName))
                {
                    using Stream wstream = File.Create(Temp + "\\refind-bin.zip");
                    using Stream rstream = myAssembly.GetManifestResourceStream(fullResourceName);
                    rstream.CopyTo(wstream);
                }
                */

                File.Copy("refind-bin.zip", rEFIndBin);
                return File.Exists(rEFIndBin);
            }
            
            //Проверяем установленный rEFInd
            bool rEFIndInstalled = Directory.Exists(ESP + @"EFI\rEFInd\") && Directory.EnumerateFiles(ESP + @"EFI\rEFInd\", "refind_*.efi").Any();
            ConfigButton.Enabled = rEFIndInstalled;
            RemoveButton.Enabled = rEFIndInstalled;

            //Конец
            GroupBoxInstallMode.Enabled = true;
            InstallButton.Enabled = true;
            ConfigButton.Enabled = true;
            RemoveButton.Enabled = true;
        }
        private void RemoveButton_Click(object sender, EventArgs e)
        {
            ToolStripLabel.Text = "Removing rEFInd";
            if (Directory.Exists(ESP + @"EFI\rEFInd\") && Directory.EnumerateFiles(ESP + @"EFI\rEFInd\", "refind_*.efi").Any())
            {
                try
                {
                    Directory.Delete(ESP + @"EFI\rEFInd\", true);
                    Directory.Delete(ESP + @"EFI\keys\", true);
                    ToolStripLabel.Text = "Removing rEFInd - Succes";
                    MessageBox.Show("rEFInd ssucesfuly removed from current computer");
                }
                catch (Exception Error)
                {
                    ToolStripLabel.Text = "Removing rEFInd - Error";
                    MessageBox.Show("Error is occured when removing rEFInd\nException Message : " + Error.Message);
                    return;
                }

                //Проверяем установленный rEFInd
                bool rEFIndInstalled = Directory.Exists(ESP + @"EFI\rEFInd\") && Directory.EnumerateFiles(ESP + @"EFI\rEFInd\", "refind_*.efi").Any();
                ConfigButton.Enabled = rEFIndInstalled;
                RemoveButton.Enabled = rEFIndInstalled;

                return;
            }
            else
            {
                ToolStripLabel.Text = "Removing rEFInd - Error";
                MessageBox.Show("rEFInd is not installed on current computer");

                //Проверяем установленный rEFInd
                bool rEFIndInstalled = Directory.Exists(ESP + @"EFI\rEFInd\") && Directory.EnumerateFiles(ESP + @"EFI\rEFInd\", "refind_*.efi").Any();
                ConfigButton.Enabled = rEFIndInstalled;
                RemoveButton.Enabled = rEFIndInstalled;

                return;
            }
        }
        private void ConfigButton_Click(object sender, EventArgs e)
        {
            ToolStripLabel.Text = "Open rEFInd Config";
            if (Directory.Exists(ESP + @"EFI\rEFInd\") && Directory.EnumerateFiles(ESP + @"EFI\rEFInd\", "refind_*.efi").Any())
            {
                if (File.Exists(ESP + @"EFI\rEFInd\refind.conf"))
                {
                    var Config = new System.Diagnostics.Process();
                    Config.StartInfo.FileName = ESP + @"EFI\rEFInd\refind.conf";
                    Config.StartInfo.UseShellExecute = true;
                    Config.Start();
                    ToolStripLabel.Text = "Open rEFInd Config - Succes";

                    //Проверяем установленный rEFInd
                    bool rEFIndInstalled = Directory.Exists(ESP + @"EFI\rEFInd\") && Directory.EnumerateFiles(ESP + @"EFI\rEFInd\", "refind_*.efi").Any();
                    ConfigButton.Enabled = rEFIndInstalled;
                    RemoveButton.Enabled = rEFIndInstalled;

                    return;
                }
                else
                {
                    ToolStripLabel.Text = "Open rEFInd Config - Error";
                    MessageBox.Show("rEFInd.conf is not found");

                    //Проверяем установленный rEFInd
                    bool rEFIndInstalled = Directory.Exists(ESP + @"EFI\rEFInd\") && Directory.EnumerateFiles(ESP + @"EFI\rEFInd\", "refind_*.efi").Any();
                    ConfigButton.Enabled = rEFIndInstalled;
                    RemoveButton.Enabled = rEFIndInstalled;

                    return;
                }
            }
            else
            {
                ToolStripLabel.Text = "Open rEFInd Config - Error";
                MessageBox.Show("rEFInd is not installed on current computer");
                return;
            }
        }
        private void ButtonSelectThemeConf_Click(object sender, EventArgs e)
        {
            if (OpenFileThemeConf.ShowDialog() == DialogResult.OK)
                TextBoxThemeConf.Text = OpenFileThemeConf.FileName;
        }

        //ComboBox's
        private void ComboBoxSelectDrive_SelectedIndexChanged(object sender, EventArgs e)
        {
            DriveInfo SelectedDrive = (from Drive in DriveInfo.GetDrives() where Drive.Name == ComboBoxSelectDrive.Text select Drive).First();
            CheckFormat.Enabled = SelectedDrive.DriveFormat != "FAT32";
        }

        //CheckBox's
        private void CheckForceArch_CheckedChanged(object sender, EventArgs e)
        {
            RadioArch64.Enabled = CheckForceArch.Checked;
            RadioArchARM64.Enabled = CheckForceArch.Checked;
            RadioArchX86.Enabled = CheckForceArch.Checked;
        }
        private void CheckThemePath_CheckedChanged(object sender, EventArgs e)
        {
            TextBoxThemeConf.Enabled = CheckThemePath.Checked;
            ButtonSelectThemeConfig.Enabled = CheckThemePath.Checked;
        }

        //Method's
        private static string Menuentry(string Name, string Loader, string? Icon = null, string? Volume = null, string? InitRD = null, string? Options = null, string? OSType = null, bool Graphics = true, bool Disabled = false)
        {
            System.Text.StringBuilder MenuEntry = new($"menuentry \"" + Name + "\" {", 50);
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