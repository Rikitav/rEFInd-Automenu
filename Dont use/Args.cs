using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rEFInd
{
    internal class Args
    {
        internal void args()
        {
            switch (args[0])
            {
                case "help":
                    {
                        Help();
                        break;
                    }

                case "esp":
                    {
                        Process.Start("cmd.exe", "/c" + $@"dir {ESP}EFI\rEFInd /d > %UserProfile%\Desktop\rEFInd Directory.txt");
                        Console.WriteLine("List added to your Desktop");
                        break;
                    }

                default:
                    {

                        break;
                    }
            }
        }
    }
}
