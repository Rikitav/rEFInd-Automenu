using CommandLine;
using log4net;
using rEFInd_Automenu.Configuration.MenuEntry;
using rEFInd_Automenu.Configuration.Parsing;
using System.Reflection;

namespace rEFInd_Automenu.ConsoleApplication.ProcedureProcessors.MenuEntries
{
    public static partial class ProcedureProcessor_MenuEntries
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProcedureProcessor_MenuEntries));

        public static void Execute(EntriesConfigurationArgumentsInfo argumentsInfo)
        {
            // --Create
            if (!string.IsNullOrEmpty(argumentsInfo.CreateEntry))
            {
                CreateMenuEntry(argumentsInfo);
                return;
            }

            // --Remove
            if (!string.IsNullOrEmpty(argumentsInfo.RemoveEntry))
            {
                RemoveMenuEntry(argumentsInfo);
                return;
            }

            // --Edit
            if (!string.IsNullOrEmpty(argumentsInfo.EditEntry))
            {
                EditMenuEntry(argumentsInfo);
                return;
            }

            // --Get
            if (!string.IsNullOrEmpty(argumentsInfo.ShowEntry))
            {
                ShowMenuEntry(argumentsInfo);
                return;
            }
        }

        private static void EnumerateAndSetMenuEntryValues(MenuEntryInfo entryInfo, EntriesConfigurationArgumentsInfo argumentsInfo)
        {
            foreach (PropertyInfo ValProp in argumentsInfo.GetType().GetProperties())
            {
                // Getting option attributes
                OptionAttribute? optionAttribute = ValProp.GetCustomAttribute<OptionAttribute>();
                if (optionAttribute == null)
                    continue;

                // Getting if option is associated with main arguments
                if (!string.IsNullOrEmpty(optionAttribute.Group))
                    continue;

                // Getting property of info associated with this option
                PropertyInfo? entryInfoProperty = typeof(MenuEntryInfo).GetProperty(optionAttribute.LongName);
                if (entryInfoProperty == null)
                    continue;

                // Getting option's value
                string? OptionValue = ValProp.GetValue(argumentsInfo)?.ToString();
                if (string.IsNullOrEmpty(OptionValue))
                    continue;

                // Setting info's value
                object? NewInfoValue = OptionValue == "NULL" ? null : ConfigurationTokenParser.ConvertTokenValue(entryInfoProperty.PropertyType, OptionValue);
                log.InfoFormat("Setting {0} property to {1}", ValProp.Name, NewInfoValue == null ? "<NULL>" : NewInfoValue.ToString());
                entryInfoProperty.SetValue(entryInfo, NewInfoValue);
            }
        }
    }
}
