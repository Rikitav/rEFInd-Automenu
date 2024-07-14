﻿using System.Collections.Generic;
using System.IO;

namespace rEFInd_Automenu.Configuration
{
    public class MenuEntryIconsAliases
    {
        public static readonly Dictionary<string, string> AliasDict = new Dictionary<string, string>()
        {
            { "microsoft", "os_win8.png" },
            { "ubuntu", "os_ubuntu.png" },
            { "hackbgrt", "os_windows.png" },
            { "OpenMandriva_Lx_[GRUB]", "os_openmandriva" }
        };

        public static string GetIconName(string LoaderRoot)
        {
            if (AliasDict.ContainsKey(LoaderRoot))
                return AliasDict[LoaderRoot];

            return string.Format("os_{0}.png", LoaderRoot);
        }
    }
}
