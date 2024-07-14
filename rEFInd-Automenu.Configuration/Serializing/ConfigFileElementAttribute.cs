using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rEFInd_Automenu.Configuration.Serializing
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigFileElementAttribute : Attribute
    {
        public string ElementName
        {
            get;
            private set;
        }

        public ConfigFileElementAttribute(string elementName)
        {
            ElementName = elementName;
        }
    }
}
