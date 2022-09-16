using System;

namespace RockLib.Logging;

internal static class BlockIndentExtension
{
    public static string BlockIndent(this string self, string indention) =>
#if NETCOREAPP3_1_OR_GREATER
        $"{indention}{self.Replace("\n", $"\n{indention}", StringComparison.CurrentCulture)}";
#else
        $"{indention}{self.Replace("\n", $"\n{indention}")}";
#endif
}
