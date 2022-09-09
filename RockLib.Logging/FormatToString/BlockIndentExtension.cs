using System;

namespace RockLib.Logging;

internal static class BlockIndentExtension
{
    public static string BlockIndent(this string self, string indention) =>
#if NET6_0_OR_GREATER
        $"{indention}{self.Replace("\n", $"\n{indention}", StringComparison.CurrentCulture)}";
#else
        $"{indention}{self.Replace("\n", $"\n{indention}")}";
#endif
}
