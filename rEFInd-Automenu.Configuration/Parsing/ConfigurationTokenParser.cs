using System;
using System.Collections.Generic;
using System.Reflection;

namespace rEFInd_Automenu.Configuration.Parsing
{
    public static class ConfigurationTokenParser
    {
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

            // Getting property type
            Type PropType = AssociatedProp.PropertyType.Name.StartsWith("Nullable")
                ? AssociatedProp.PropertyType.GenericTypeArguments[0]
                : AssociatedProp.PropertyType;

            // Parsing
            object? LineValueObj = ConvertTokenValue(PropType, LineValueString);
            if (LineValueObj == null)
                return;

            // Setting value
            AssociatedProp.SetValue(InfoObj, LineValueObj);
        }

        private static object? ConvertTokenValue(Type ValueType, string StringValue)
        {
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
                        "Boolean" => bool.Parse(StringValue),
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
    }
}
