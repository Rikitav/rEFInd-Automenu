using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rEFInd_Automenu.Configuration.GlobalConfiguration
{
    [Flags]
    public enum UserInterface
    {
        banner = 2,
        label = 4,
        singleuser = 8,
        safemode = 16,
        hwtest = 32,
        arrows = 64,
        hints = 128,
        editor = 256,
        badges = 512,
        all = banner | label | singleuser | safemode | hwtest | arrows | hints | editor | badges
    }

    public enum BannerScale
    {
        noscale,
        fillscreen
    }

    [Flags]
    public enum ScanFor
    {
        @internal = 2,
        external = 4,
        optical = 8,
        netboot = 16,
        hdbios = 32,
        biosexternal = 64,
        cd = 128,
        manual = 256,
        firmware = 512
    }

    [Flags]
    public enum ShowTools
    {
        shell = 2,
        memtest = 4,
        gdisk = 8,
        gptsync = 16,
        install = 32,
        bootorder = 64,
        apple_recovery = 128,
        csr_rotate = 256,
        mok_tool = 512,
        fwupdate = 1024,
        netboot = 2048,
        about = 4096,
        hidden_tags = 8192,
        exit = 16384,
        shutdown = 32768,
        reboot = 65536,
        firmware = 131072
    }

    [Flags]
    public enum UseGraphicsFor
    {
        osx = 2,
        linux = 4,
        elilo = 8,
        grub = 16,
        windows = 32
    }
}
