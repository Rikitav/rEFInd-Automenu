namespace rEFInd_Automenu.Configuration.Serializing
{
    public static class ConfigToken
    {
        public const string OpenBracket = "{";
        public const string CloseBracket = "}";
        public const string MenuEntry = "menuentry";
        public const string SubMenuEntry = "submenuentry";

        public static string EntryName(string EntryName) => "\"" + EntryName + "\"";
        public static string Indent(this string Target, int Count) => new string('\t', Count) + Target;
    }
}
