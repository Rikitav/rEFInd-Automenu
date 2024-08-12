using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace rEFInd_Automenu.Configuration.Parsing
{
    public static class ConfigurationTokenParser
    {
        /*
        public static void SetConfigurationInformationValue(string Token, object? Value, object InfoObj, Dictionary<string, PropertyInfo> PropertiesHashMap)
        {
            // Getting property associated with token
            if (!PropertiesHashMap.TryGetValue(Token, out PropertyInfo? AssociatedProp))
                return;

            // Null prop
            if (AssociatedProp == null)
                return;

            // Getting property type
            Type PropType = AssociatedProp.PropertyType.Name.StartsWith("Nullable")
                ? AssociatedProp.PropertyType.GenericTypeArguments[0]
                : AssociatedProp.PropertyType;

            // Parsing
            if (Value != null)
            {
                // Setting value
                object? LineValueObj = ConvertTokenValue(PropType, Value.ToString());
                AssociatedProp.SetValue(InfoObj, LineValueObj);
            }
            else
            {
                // Setting value
                AssociatedProp.SetValue(InfoObj, null);
            }
        }
        */

        public static void ProcessLineOfConfig(string Line, object InfoObj, Dictionary<string, PropertyInfo> PropertiesHashMap)
        {
            // 'No gap' checking
            if (!Line.Contains(' '))
                return;

            // 'No value' checking
            string Token = Line.Substring(0, Line.IndexOf(' '));
            if (Token.Length + 1 == Line.Length)
                return;

            // 'Empty value' checking
            string LineValueString = Line.Substring(Token.Length + 1);
            if (string.IsNullOrEmpty(LineValueString))
                return;

            // Getting property associated with token
            if (!PropertiesHashMap.TryGetValue(Token, out PropertyInfo? AssociatedProp))
                return;

            // Null prop
            if (AssociatedProp == null)
                return;

            // Checking value
            object? PropVal = AssociatedProp.GetValue(InfoObj);
            if (PropVal != null)
                return; // Already has value?

            // Parsing
            object? LineValueObj = ConvertTokenValue(AssociatedProp.PropertyType, LineValueString);
            if (LineValueObj == null)
                return;

            // Setting value
            AssociatedProp.SetValue(InfoObj, LineValueObj);
        }

        public static object? ConvertTokenValue(Type ValueType, string StringValue)
        {
            if (ValueType.Name.StartsWith("Nullable"))
                ValueType = ValueType.GenericTypeArguments[0];

            try
            {
                if (ValueType.IsEnum)
                {
                    // Getting value
                    return Enum.Parse(ValueType, StringValue);
                }
                else
                {
                    // Getting value
                    return ValueType.Name switch
                    {
                        "String" => StringValue,
                        "Int32" => int.Parse(StringValue),
                        "Int16" => short.Parse(StringValue),
                        "Boolean" => ParseBoolean(StringValue),
                        "Guid" => Guid.Parse(StringValue),
                        "Int32[]" => null, // Because such type have only ONE option and its not for Windows
                        _ => null
                    };
                }
            }
            catch
            {
                // Value parsing error
                return null;
            }
        }

        private static bool? ParseBoolean(string StringValue)
        {
            string[] TrueValues = { "true", "on", "1" };
            string[] FalseValues = { "false", "off", "0" };

            if (TrueValues.Contains(StringValue, StringComparer.CurrentCultureIgnoreCase))
                return true;

            if (FalseValues.Contains(StringValue, StringComparer.CurrentCultureIgnoreCase))
                return false;

            return null;
        }
    }
}
