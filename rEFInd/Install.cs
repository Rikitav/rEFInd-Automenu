using System.Diagnostics;

namespace rEFInd
{
    internal class Install : WorkClass
    {
        internal static void Computer(bool Desktop = false)
        {
            //Подготовка к установке
            string rEFIndInstall = Desktop ? Environment.GetEnvironmentVariable("USERPROFILE") + @"\Desktop\rEFInd\" : ESP + @"EFI\rEFInd\";
            SpinTasker.Start("Installing rEFInd");

            //Проверка на уже установленный rEFInd
            if (!Desktop)
                if (Directory.Exists(rEFIndInstall))
                    SpinTasker.Stop(ErrorMessage: "You already have installed rEFInd, Remove him first with \'-r\' parametr");
            
            if (Desktop)
                if (Directory.Exists(rEFIndInstall))
                    Directory.Delete(rEFIndInstall);

            //Создание необходимых директорий
            Directory.CreateDirectory(rEFIndInstall);
            Directory.CreateDirectory(rEFIndInstall + "drivers_" + Arch[0]);
            Directory.CreateDirectory(rEFIndInstall + "Icons");

            //Копируем директории и загрузчик
            CopyDirectory(rEFIndData + "Icons", rEFIndInstall + "Icons", true);
            CopyDirectory(rEFIndData + "drivers_" + Arch[0], rEFIndInstall + "drivers_" + Arch[0], true);
            File.Copy(rEFIndData + "refind_" + Arch[0] + ".efi", rEFIndInstall + "refind_" + Arch[0] + ".efi");

            SpinTasker.Stop(File.Exists(rEFIndInstall + @$"\refind_" + Arch[0] + ".efi"), ErrorMessage: "Impossible to unpack the archive");

            //Запись загрузку rEFInd в BCD контейнер
            if (!Desktop)
            {
                SpinTasker.Start("Booting rEFInd");
                var BcdEdit = Process.Start("cmd.exe", "/c" + "bcdedit /set \"{bootmgr}\"" + $" path \\EFI\\refind\\refind_{Arch[0]}.efi > nul");
                BcdEdit.WaitForExit();
                SpinTasker.Stop(!(BcdEdit.ExitCode > 0), ErrorMessage: "CMD \'BcdEdit\' Сommand Exception : " + BcdEdit.ExitCode);
                Process.Start("cmd.exe", "/c " + "bcdedit /set \"{bootmgr}\" description \"rEFInd\" > nul");
            }

            if (!string.IsNullOrWhiteSpace(Options.Theme))
                ThemeInstall(rEFIndInstall);
        }

        internal static void USB()
        {
            string rEFIndInstall = Drive.Name + @"EFI\Boot\";

            if (Directory.Exists(rEFIndInstall))
                if (Directory.EnumerateFiles(rEFIndInstall, "*boot*.efi").Any())
                    SpinTasker.Stop(ErrorMessage: $"Drive {Drive.Name} already has a Boot-Loader");

            if (Drive.DriveFormat != "FAT32")
                if (Options.Format == true)
                {
                    SpinTasker.Start("Formating Drive");
                    var FormatDrive = Process.Start("cmd.exe", "/c" + $"Format " + Drive.Name[..2] + " /FS:FAT32 /V:REFIND /Q /Y > nul");
                    FormatDrive.WaitForExit();
                    SpinTasker.Stop(!(FormatDrive.ExitCode > 0), ErrorMessage: "CMD \'Format\' Сommand Exception : " + FormatDrive.ExitCode);
                }
                else
                    SpinTasker.Stop(ErrorMessage: $"Drive {Drive.Name} has FileSystem {Drive.DriveFormat}\nrEFInd needs FAT32 file system for work\nTry to add '-f' parametr for allow formating this drive or rormat the drive yourself");

            SpinTasker.Start("Installing rEFInd");

            //Создание необходимых директорий
            Directory.CreateDirectory(rEFIndInstall); 
            Directory.CreateDirectory(rEFIndInstall + "drivers_" + Arch[0]);
            Directory.CreateDirectory(rEFIndInstall + "Icons");

            //Копируем директории
            CopyDirectory(rEFIndData + "Icons", rEFIndInstall + "Icons", true);
            CopyDirectory(rEFIndData + "drivers_" + Arch[0], rEFIndInstall + "drivers_" + Arch[0], true);

            //копируем загрузчики
            File.Move(rEFIndInstall + "refind_x64.efi", rEFIndInstall + "bootx64.efi");
            File.Move(rEFIndInstall + "refind_aa64.efi", rEFIndInstall + "bootaa64.efi");
            File.Move(rEFIndInstall + "refind_ia32.efi", rEFIndInstall + "bootia32.efi");

            SpinTasker.Stop(File.Exists($@"{rEFIndInstall}\Bootx64.efi"), ErrorMessage: "Impossible to unpack the archive");

            if (!string.IsNullOrWhiteSpace(Options.Theme))
                ThemeInstall(rEFIndInstall);
        }

        internal static void ThemeInstall(string Dest)
        {
            SpinTasker.Start("Theme install");
            if (File.Exists($"{Options.Theme}\\theme.conf"))
            {
                Directory.CreateDirectory(Dest + @"\Theme");
                var ThemeCopy = Process.Start("cmd.exe", "/c" + $@"xcopy {Options.Theme} {Dest}\Theme /I /E /C /Q /Y > nul");
                ThemeCopy.WaitForExit();
                SpinTasker.Stop(!(ThemeCopy.ExitCode > 0), ErrorMessage: "CMD \'XCopy\' Сommand Exception : " + ThemeCopy.ExitCode);
            }
            else
                SpinTasker.Stop(ErrorMessage: "Theme is not exists in folder \"" + Dest + "\"");
        }
    }
}