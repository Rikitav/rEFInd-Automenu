using CommandLine;
using CommandLine.Text;
using log4net;
using rEFInd_Automenu.ConsoleApplication.ConsoleInterface;
using System.Reflection;
using System.Text;

namespace rEFInd_Automenu.ConsoleApplication
{
    public static class ArgumentsHelper
    {
        public static bool ValidMainArgumentsCount(object argumentsInfo, string[] args)
        {
            int MainArgsCount = argumentsInfo.GetType().GetProperties().Count(prop =>
            {
                OptionAttribute? optionAttribute = prop.GetCustomAttribute<OptionAttribute>();
                if (optionAttribute == null)
                    return false;

                if (string.IsNullOrEmpty(optionAttribute.Group))
                    return false;

                return
                    args.Contains("-" + optionAttribute.ShortName, StringComparer.CurrentCultureIgnoreCase) ||
                    args.Contains("--" + optionAttribute.LongName, StringComparer.CurrentCultureIgnoreCase);
            });

            return MainArgsCount == 1;
        }

        public static void WriteHelp<TArguments>(ParserResult<TArguments> parserResult)
        {
            HelpText Help = HelpText.AutoBuild(parserResult);
            Help.Copyright = string.Empty;
            Help.Heading = string.Empty;
            Help.AddEnumValuesToHelpText = true;

            string HelpString = Help.ToString();
            ConsoleInterfaceWriter.WriteWarning("\n" + HelpString.TrimStart('\n', '\r'));
        }

        public static void LogArguments(object ArgumentsInfo, ILog Logger)
        {
            // Getting properties of ArgumentInfo class
            Type ObjType = ArgumentsInfo.GetType();
            PropertyInfo[] Props = ObjType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            // Parsing values
            string[] FieldNames = Props.Select(x => x.Name).ToArray();
            string[] TypeNames = Props.Select(x => x.PropertyType.FullName ?? "NullPropertyName").ToArray();
            string[] Values = Props.Select(x => x.GetValue(ArgumentsInfo)?.ToString() ?? "<NULL>").ToArray();

            // Getting maximum columns padding
            int FieldNamesPadding = FieldNames.Select(x => x.Length).Max();
            int TypeNamesPadding = TypeNames.Select(x => x.Length).Max();
            int ValuesPadding = Values.Select(x => x.Length).Max();

            // Building log text
            StringBuilder builder = new StringBuilder("From \"" + ObjType.Name + "\" Parsed :\n");
            builder.AppendLine(string.Format("{0} | {1} | {2}", "Field".PadRight(FieldNamesPadding), "Type".PadRight(TypeNamesPadding), "Value".PadRight(ValuesPadding)));
            builder.AppendLine(string.Format("{0} | {1} | {2}", new string(' ', FieldNamesPadding), new string(' ', TypeNamesPadding), new string(' ', ValuesPadding)));

            for (int i = 0; i < Values.Length; i++)
            {
                builder.AppendLine(string.Format("{0} | {1} | {2}",
                    FieldNames[i].PadRight(FieldNamesPadding),
                    TypeNames[i].PadRight(TypeNamesPadding),
                    Values[i].PadRight(ValuesPadding)));
            }

            // Logging arguments
            Logger.Info(builder.ToString());
        }
    }
}
