using rEFInd_Automenu;
using System.Runtime.InteropServices;

namespace rEFIndAutomenu
{
    internal static class Program
    {
        //Метод проверяющий тип загрузки и поддержку UEFI
        public const int ERROR_INVALID_FUNCTION = 1;
        [DllImport("kernel32.dll",
           EntryPoint = "GetFirmwareEnvironmentVariableW",
           SetLastError = true,
           CharSet = CharSet.Unicode,
           ExactSpelling = true,
           CallingConvention = CallingConvention.StdCall)]
        public static extern int GetFirmwareType(string lpName, string lpGUID, IntPtr pBuffer, uint size);

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Проверка на поддержку и активированный UEFI (Позаимствовал)
            GetFirmwareType("", "{00000000-0000-0000-0000-000000000000}", IntPtr.Zero, 0);
            if (Marshal.GetLastWin32Error() == ERROR_INVALID_FUNCTION)
            {
                MessageBox.Show("Your mainboard doesn't support UEFI and/or Windows is installed in legacy BIOS mode\nrEFInd can be installed only on UEFI Machines");
                Environment.Exit(1);
            }

            if ((from Drive in DriveInfo.GetDrives() where File.Exists(Drive.Name + @"EFI\Microsoft\Boot\bootmgfw.efi") & Drive.DriveType == DriveType.Fixed select Drive.Name).Any())
            {
                Form1.ESP = (from Drive in DriveInfo.GetDrives() where File.Exists(Drive.Name + @"EFI\Microsoft\Boot\bootmgfw.efi") & Drive.DriveType == DriveType.Fixed select Drive.Name).First();
            }
            else
            {
                var MountVol = System.Diagnostics.Process.Start("cmd.exe", "/c " + "MountVol S: /s");
                MountVol.WaitForExit();
                if (MountVol.ExitCode != 0)
                {
                    MessageBox.Show("Cannot mount EFI System Partition\nMountvol command exitcode : " + MountVol.ExitCode);
                    Environment.Exit(MountVol.ExitCode);
                }
                else
                    Form1.ESP = @"S:\";
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}