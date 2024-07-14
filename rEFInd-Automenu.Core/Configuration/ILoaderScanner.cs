using rEFInd_Automenu.Booting;
using rEFInd_Automenu.Configuration.MenuEntry;
using System.Collections.Generic;

namespace rEFInd_Automenu.Configuration
{
    public interface ILoadersScanner
    {
        public IEnumerable<MenuEntryInfo> Parse(EnvironmentArchitecture Arch);
    }
}
