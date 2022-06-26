namespace rEFInd
{
    internal class Config
    {
        internal void ConfigUSB(String Drive)
        {
            //Can be added some instruments to menu generator like :
            //Grub2 FileManager, Xorboot
        }

        internal void ConfigComputer(String ESP, String Theme)
        {
            int OsNumber = 6;
            String Arch = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            StreamWriter sw = new StreamWriter(@$"{ESP}EFI\rEFInd\rEFInd.conf");
            string[] OsString = new string[] { string.Empty };

            sw.WriteLine("Use_nvram 1");
            sw.WriteLine("Timeout 20");
            sw.WriteLine("use_graphics_for linux, windows");
            sw.WriteLine($"showtools shutdown, reboot, memtest, shell, firmware");
            sw.WriteLine($"scanfor manual, biosexternal, external, optical, CD\n");

            string[] OsPath =
            {
                @$"{ESP}EFI\PhoenixOS",
                @$"{ESP}EFI\ubuntu",
                @$"{ESP}EFI\debian",
                @$"{ESP}EFI\centos",
                @$"{ESP}EFI\fedora",
                @$"{ESP}EFI\kali"
            };

            if (Arch == "AMD64" || Arch == "IA64")
            {
                OsString = new string[]
                {
                    "menuentry PhoenixOS {\n\tloader /EFI/PhoenixOS/Boot/bootx64.efi\n\tOstype Linux\n\tGraphics on\n}\n",                                    //PhoenixOS
                    "menuentry Ubuntu {\n\tloader EFI/ubuntu/shimx64.efi\n\tOstype Linux\n\tGraphics on\n}\n",                                                //Ubuntu
                    "menuentry Debian {\n\tloader /EFI/debian/shimx64.efi\n\tOstype Linux\n\tGraphics on\n}\n",                                               //Debian
                    "menuentry CentOS {\n\tloader /EFI/centos/shimx64.efi\n\tOstype Linux\n\tGraphics on\n}\n",                                               //CentOS
                    "menuentry Fedora {\n\tloader /EFI/fedora/shimx64.efi\n\tOstype Linux\n\tGraphics on\n}\n",                                               //Fedora
                    "menuentry Kali {\n\tloader /EFI/kali/grubx64.efi\n\tOstype Linux\n\tGraphics on\n}\n"                                                    //Kali
                };
            }

            if (Arch == "ARM64")
            {
                OsString = new string[]
                {
                    "menuentry PhoenixOS {\n\tloader /EFI/PhoenixOS/Boot/bootaa64.efi\n\tOstype Linux\n\tGraphics on\n}\n",                                   //PhoenixOS
                    "menuentry Ubuntu {\n\tloader EFI/ubuntu/shimaa64.efi\n\tOstype Linux\n\tGraphics on\n}\n",                                               //Ubuntu
                    "menuentry Debian {\n\tloader /EFI/debian/shimaa64.efi\n\tOstype Linux\n\tGraphics on\n}\n",                                              //Debian
                    "menuentry CentOS {\n\tloader /EFI/centos/shimaa64.efi\n\tOstype Linux\n\tGraphics on\n}\n",                                              //CentOS
                    "menuentry Fedora {\n\tloader /EFI/fedora/shimaa64.efi\n\tOstype Linux\n\tGraphics on\n}\n",                                              //Fedora
                    "menuentry Kali {\n\tloader /EFI/kali/grubaa64.efi\n\tOstype Linux\n\tGraphics on\n}\n"                                                   //Kali
                };
            }

            if (Arch == "x86")
            {
                OsString = new string[]
                {
                    "menuentry PhoenixOS {\n\tloader /EFI/PhoenixOS/Boot/bootia32.efi\n\tOstype Linux\n\tGraphics on\n}\n",                                   //PhoenixOS
                    "menuentry Ubuntu {\n\tloader EFI/ubuntu/shimia32.efi\n\tOstype Linux\n\tGraphics on\n}\n",                                               //Ubuntu
                    "menuentry Debian {\n\tloader /EFI/debian/shimia32.efi\n\tOstype Linux\n\tGraphics on\n}\n",                                              //Debian
                    "menuentry CentOS {\n\tloader /EFI/centos/shimia32.efi\n\tOstype Linux\n\tGraphics on\n}\n",                                              //CentOS
                    "menuentry Fedora {\n\tloader /EFI/fedora/shimia32.efi\n\tOstype Linux\n\tGraphics on\n}\n",                                              //Fedora
                    "menuentry Kali {\n\tloader /EFI/kali/grubia32.efi\n\tOstype Linux\n\tGraphics on\n}\n"                                                   //Kali
                };
            }

            if (Directory.Exists(@$"{ESP}EFI\HackBGRT"))
            {
                sw.WriteLine("menuentry Windows {\n\tloader /EFI/HackBGRT/bootia32.efi\n\tIcon EFI/rEFInd/icons/os_win8.png\n\tOstype Windows\n\tGraphics on\n}\n");
            }
            else
            {
                if (Directory.Exists(@$"{ESP}EFI\Microsoft"))
                {
                    sw.WriteLine("menuentry Windows {\n\tloader /EFI/Microsoft/Boot/bootmgfw.efi\n\tOstype Windows\n\tGraphics on\n}\n");
                }
            }

            for (int i = 0; i < OsNumber; i++)
            {
                if (Directory.Exists(OsPath[i]))
                {
                    sw.WriteLine(OsString[i]);
                }
            }

            if (!string.IsNullOrWhiteSpace(Theme))
                sw.WriteLine($"include theme/theme.conf");

            sw.Close();

            /*
            String RedHat = "menuentry RedHat {\n\tloader EFI//.efi\n\tOstype Linux\n\tGraphics on\n}";
            String Mandriva = "menuentry Mandriva {\n\tloader EFI//.efi\n\tOstype Linux\n\tGraphics on\n}";
            String OpenSUSE = "menuentry OpenSUSE {\n\tloader EFI//.efi\n\tOstype Linux\n\tGraphics on\n}";
            String Manjaro = "menuentry Manjaro {\n\tloader EFI//.efi\n\tOstype Linux\n\tGraphics on\n}";
            String Mint = "menuentry Mint {\n\tloader EFI/mint/shimia32.efi\n\tOstype Linux\n\tGraphics on\n}";
            */

            if (File.Exists(@$"{ESP}EFI\rEFInd\refind.conf"))
            {
                Console.WriteLine("Config generation... OK");
            }
            else
            {
                Console.WriteLine("Config generation... !ERROR!");
                ClearComp(ESP);
                Environment.Exit(1);
            }
        }

        public static void ClearComp(String ESP)
        {
            String Temp = Path.GetTempPath();
            String rEFIndZip = @$"{Temp}\rEFInd.zip";
            String rEFIndData = @$"{Temp}\rEFInd";
            String rEFIndESP = @$"{ESP}EFI\rEFInd";

            if (File.Exists(rEFIndZip))
            {
                File.Delete(rEFIndZip);
            }

            if (Directory.Exists(rEFIndData))
            {
                Directory.Delete(rEFIndData, true);
            }

            if (Directory.Exists(rEFIndESP))
            {
                Directory.Delete(rEFIndESP, true);
            }
        }

        public static void ClearUSB(String Drive)
        {
            String Temp = Path.GetTempPath();
            String rEFIndZip = @$"{Temp}\rEFInd.zip";
            String rEFIndData = @$"{Temp}\rEFInd";
            String rEFIndUSB = @$"{Drive}EFI\Boot";

            if (File.Exists(rEFIndZip))
            {
                File.Delete(rEFIndZip);
            }

            if (Directory.Exists(rEFIndData))
            {
                Directory.Delete(rEFIndData, true);
            }

            if (Directory.Exists(rEFIndUSB))
            {
                Directory.Delete(rEFIndUSB, true);
            }
        }
    }
}