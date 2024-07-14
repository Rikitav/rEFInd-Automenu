using rEFInd_Automenu.Configuration.GlobalConfiguration;
using rEFInd_Automenu.Configuration.MenuEntry;
using System.Collections.Generic;

namespace rEFInd_Automenu.Configuration
{
    public class RefindConfiguration
    {
        public RefindGlobalConfigurationInfo Global
        {
            get;
            set;
        }

        public List<MenuEntryInfo> Entries
        {
            get;
            set;
        }

        public RefindConfiguration()
        {
            //Global = new RefindGlobalConfigurationInfo();
            //Entries = new List<MenuEntryInfo>();
        }
    }
}
