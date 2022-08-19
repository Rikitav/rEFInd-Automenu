using System.Diagnostics;

namespace rEFInd_Automenu
{
    internal class Install : Form1
    {
        public static Form1 rEFInd = new();
        
        internal static void Desktop()
        {
            //Подготовка к установке
            string rEFIndInstall = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Desktop\rEFInd\";
            rEFInd.ToolStripLabel.Text = "Installing rEFInd";

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
                rEFInd.ToolStripLabel.Text = "Installing rEFInd - Error";
                MessageBox.Show("Error is ocurred when installing rEFInd\nError Message : " + Error.Message);
                return;
            }

            //Создание загрузочного меню
            rEFInd.ToolStripLabel.Text = "Config generation";
            StreamWriter ConfigFile = new(rEFIndInstall + @"\rEFInd\refind.conf");

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
                rEFInd.ToolStripLabel.Text = "Installing rEFInd - Succes";
                MessageBox.Show("rEFInd succesfuly unpacked on your desktop");
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
            string rEFIndInstall = InstallDrive.Name + @"\EFI\";
            if (InstallDrive.DriveFormat != "FAT32")
                if (rEFInd.CheckFormat.Checked & rEFInd.CheckFormat.Enabled)
                {
                    rEFInd.ToolStripLabel.Text = "Formating Drive";
                    var FormatDrive = Process.Start("cmd.exe", "/c" + $"Format " + InstallDrive.Name[..2] + " /FS:FAT32 /V:REFIND /Q /Y > nul");
                    FormatDrive.WaitForExit();
                    if (FormatDrive.ExitCode != 0)
                    {
                        rEFInd.ToolStripLabel.Text = "Formating Drive - Error";
                        MessageBox.Show("Error is ocurred when formating drive" + InstallDrive.Name + "\nCMD \'Format\' Сommand Exception : " + FormatDrive.ExitCode);
                        return;
                    }
                }
                else
                {
                    rEFInd.ToolStripLabel.Text = "Formating Drive - Error";
                    MessageBox.Show($"Drive {InstallDrive.Name} has FileSystem {InstallDrive.DriveFormat}\nrEFInd needs FAT32 file system for work");
                    return;
                }

            rEFInd.ToolStripLabel.Text = "Installing rEFInd";
            if (Directory.Exists(rEFIndInstall + "\\boot"))
            {
                if (Directory.EnumerateFiles(rEFIndInstall + @"\boot", "boot*.efi").Any())
                {
                    rEFInd.ToolStripLabel.Text = "Installing rEFInd - Error";
                    MessageBox.Show($"Drive {InstallDrive.Name} Already has a Boot-Loader\nDelete it first or move");
                    return;
                }
            }

            try
            {
                //Создание, копирование необходимых директорий
                CopyDirectory(rEFIndData + @"\keys", rEFIndInstall + @"\keys", true);
                CopyDirectory(rEFIndData + @"\refind", rEFIndInstall + @"rEFInd", true);
                File.Move(rEFIndInstall + @"\rEFInd\refind_x64.efi", rEFIndInstall + @"boot\bootx64.efi");
                File.Move(rEFIndInstall + @"\rEFInd\refind_aa64.efi", rEFIndInstall + @"boot\bootaa64.efi");
                File.Move(rEFIndInstall + @"\rEFInd\refind_ia32.efi", rEFIndInstall + @"boot\bootia32.efi");
            }
            catch (FileNotFoundException Error)
            {
                rEFInd.ToolStripLabel.Text = "Installing rEFInd - Error";
                MessageBox.Show("Error is ocurred when coping binary\nError Message : " + Error.Message);
                return;
            }

            //Создание загрузочного меню
            rEFInd.ToolStripLabel.Text = "Config generation";
            StreamWriter ConfigFile = new(rEFIndInstall + @"\boot\refind.conf");

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
                rEFInd.ToolStripLabel.Text = "Installing rEFInd - Succes";
                MessageBox.Show("rEFInd succesfuly installed on " + InstallDrive.Name);
                return;
            }
            catch (Exception Error)
            {
                rEFInd.ToolStripLabel.Text = "Config generation - Error";
                MessageBox.Show("Error is ocurred when generating rEFInd.conf\nError Message : " + Error.Message);
                return;
            }
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
