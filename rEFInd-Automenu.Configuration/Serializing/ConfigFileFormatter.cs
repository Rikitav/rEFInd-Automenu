using rEFInd_Automenu.Configuration.GlobalConfiguration;
using rEFInd_Automenu.Configuration.MenuEntry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace rEFInd_Automenu.Configuration.Serializing
{
    public static class ConfigFileFormatter
    {
        /*
        public static RefindConfiguration Parse(string Data)
        {
            return null;
        }
        */

        public static void FormatToString(RefindConfiguration Data, TextWriter writer)
        {
            // ...
            if (Data.Global != null)
                BuildConfigGlobalString(writer, Data.Global);

            if (Data.Entries != null)
                BuildConfigEntriesString(writer, Data.Entries);
        }

        private static string FormatPropertyInfo(PropertyInfo prop, object obj)
        {
            ConfigFileElementAttribute? ElementNameAttr = prop.GetCustomAttribute<ConfigFileElementAttribute>();
            if (ElementNameAttr == null)
                return string.Empty; // MUST contain attribute.

            object? PropValue = prop.GetValue(obj);
            if (PropValue == null)
                return string.Empty; // MUST contain value

            // Some types may need 'special' formating
            switch (prop.PropertyType)
            {
                case Type _ when prop.PropertyType.IsArray: // If is array, convert to sequence of values
                    {
                        IEnumerable<string?> Values = ((IEnumerable)PropValue).Cast<object>().Select(x => x.ToString());
                        return string.Format("{0} {1}",
                            ElementNameAttr.ElementName,
                            string.Join(" ", Values));
                    }

                default: // If is enum, or simple type, or string, convert to string
                    {
                        return string.Format("{0} {1}",
                            ElementNameAttr.ElementName,
                            PropValue);
                    }
            }
        }

        private static void BuildConfigGlobalString(TextWriter ConfigBuilder, RefindGlobalConfigurationInfo Global)
        {
            // Getting Info's properties
            Type GlobalType = Global.GetType();
            PropertyInfo[] GlobalProps = GlobalType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            // Enumerating all the properties
            foreach (PropertyInfo prop in GlobalProps)
            {
                // Formatting properties
                string Token = FormatPropertyInfo(prop, Global);
                if (!string.IsNullOrEmpty(Token))
                    ConfigBuilder.WriteLine(Token);
            }
        }

        private static void BuildConfigEntriesString(TextWriter ConfigBuilder, IEnumerable<MenuEntryInfo> Entries)
        {
            // Getting Info's properties
            Type EntryType = typeof(MenuEntryInfo);
            PropertyInfo[] EntryProps = EntryType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            // Enumerating all the entries
            foreach (MenuEntryInfo Entry in Entries)
            {
                // Adding entry header
                ConfigBuilder.WriteLine("\nmenuentry " + ConfigToken.EntryName(Entry.EntryName) + " {");

                // Adding entry tokens
                foreach (PropertyInfo prop in EntryProps)
                {
                    // Formatting properties
                    string Token = FormatPropertyInfo(prop, Entry);
                    if (!string.IsNullOrEmpty(Token))
                        ConfigBuilder.WriteLine(Token.Indent(1));
                }

                // If contain Sub-MenuEntries
                if (Entry.SubMenuEntries?.Any() ?? false)
                {
                    Type SubEntryType = typeof(SubMenuEntryInfo);
                    PropertyInfo[] SubEntryProps = SubEntryType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                    foreach (SubMenuEntryInfo SubEntry in Entry.SubMenuEntries)
                    {
                        // Adding subMenuEntry header
                        ConfigBuilder.WriteLine("\n\t" + "submenuentry " + ConfigToken.EntryName(SubEntry.EntryName) + " {");

                        // Adding subMenuEntry tokens
                        foreach (PropertyInfo prop in SubEntryProps)
                        {
                            // Formatting properties
                            string Token = FormatPropertyInfo(prop, SubEntry);
                            if (!string.IsNullOrEmpty(Token))
                                ConfigBuilder.WriteLine(Token.Indent(2));
                        }

                        ConfigBuilder.WriteLine(ConfigToken.CloseBracket.Indent(1));
                    }
                }

                ConfigBuilder.WriteLine(ConfigToken.CloseBracket);
            }
        }
    }
}
