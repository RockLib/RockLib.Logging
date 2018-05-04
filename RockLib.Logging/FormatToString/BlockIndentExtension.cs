namespace RockLib.Logging
{
    internal static class BlockIndentExtension
    {
        public static string BlockIndent(this string s, string indention)
        {
            return indention + s.Replace("\n", "\n" + indention);
        }
    }
}
