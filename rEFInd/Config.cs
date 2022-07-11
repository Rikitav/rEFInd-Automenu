namespace rEFInd
{
    internal class Config
    {
        /*
        internal void USB(string Drive, String Theme)
        {
            Can be added some instruments to menu generator like :
            Grub2 FileManager, Xorboot
        }
        */

        internal static void Computer(string ESP, string Theme)
        {
            Console.Write("Config generation... ");
            string Arch = Install.ArchParse()[0];
            StreamWriter sw = new(@$"{ESP}EFI\rEFInd\rEFInd.conf");

            sw.WriteLine("Use_nvram 1");
            sw.WriteLine("Timeout 20");
            sw.WriteLine("use_graphics_for linux, windows");
            sw.WriteLine($"showtools shutdown, reboot, memtest, shell, firmware");
            sw.WriteLine($"scanfor manual, biosexternal, external, optical, CD\n");

            //CGL - Config Generator List
            Dictionary<string, string> CGL = new();
            CGL.Add(@$"{ESP}EFI\Microsoft", "menuentry \"Windows\" {\n\tloader /EFI/Microsoft/Boot/bootmgfw.efi\n\tOstype Windows\n\tGraphics on\n}\n");
            CGL.Add(@$"{ESP}EFI\HackBGRT", "menuentry \"HackBGRT\" {\n\tloader /EFI/HackBGRT/boot" + Arch + ".efi\n\tOstype Windows\n\tGraphics on\n}\n");
            CGL.Add(@$"{ESP}EFI\PhoenixOS", "menuentry \"Phoenix OS\" {\n\tloader /EFI/PhoenixOS/kernel\n\tinitrd /EFI/PhoenixOS/initrd.img\n\tOptions \"quiet root=/dev/ram0 androidboot.hardware=android_x86 SRC=/PhoenixOS vga=788\"\n\tOstype Linux\n\tGraphics on\n}\n");
            CGL.Add(@$"{ESP}EFI\ubuntu", "menuentry \"Ubuntu\" {\n\tloader /EFI/ubuntu/shim" + Arch + ".efi\n\tOstype Linux\n\tGraphics on\n}\n");
            CGL.Add(@$"{ESP}EFI\debian", "menuentry \"Debian\" {\n\tloader /EFI/debian/shim" + Arch + ".efi\n\tOstype Linux\n\tGraphics on\n}\n");
            CGL.Add(@$"{ESP}EFI\centos", "menuentry \"CentOS\" {\n\tloader /EFI/centos/shim" + Arch + ".efi\n\tOstype Linux\n\tGraphics on\n}\n");
            CGL.Add(@$"{ESP}EFI\fedora", "menuentry \"Fedora\" {\n\tloader /EFI/fedora/shim" + Arch + ".efi\n\tOstype Linux\n\tGraphics on\n}\n");
            CGL.Add(@$"{ESP}EFI\kali", "menuentry \"Kali\" {\n\tloader /EFI/kali/grub" + Arch + ".efi\n\tOstype Linux\n\tGraphics on\n}\n");

            foreach (KeyValuePair<string, string> CGLWork in CGL)
                if (Directory.Exists(CGLWork.Key))
                    sw.WriteLine(CGLWork.Value);

            if (!string.IsNullOrWhiteSpace(Theme))
                sw.WriteLine($"include theme/theme.conf");

            sw.Close();

            /*
            Возможно сделаю :
            String RedHat = "menuentry RedHat {\n\tloader EFI//.efi\n\tOstype Linux\n\tGraphics on\n}";
            String Mandriva = "menuentry Mandriva {\n\tloader EFI//.efi\n\tOstype Linux\n\tGraphics on\n}";
            String OpenSUSE = "menuentry OpenSUSE {\n\tloader EFI//.efi\n\tOstype Linux\n\tGraphics on\n}";
            String Manjaro = "menuentry Manjaro {\n\tloader EFI//.efi\n\tOstype Linux\n\tGraphics on\n}";
            String Mint = "menuentry Mint {\n\tloader EFI//.efi\n\tOstype Linux\n\tGraphics on\n}";
            */

            if (File.Exists(@$"{ESP}EFI\rEFInd\refind.conf"))
            {
                Console.WriteLine("OK");
            }
            else
            {
                Console.WriteLine("!ERROR!");
                Automenu.Clear();
                Environment.Exit(1);
            }
        }
    }
}