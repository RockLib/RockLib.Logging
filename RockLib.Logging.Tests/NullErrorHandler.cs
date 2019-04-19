using RockLib.Dynamic;
using System;

namespace RockLib.Logging.Tests
{
    internal static class NullErrorHandler
    {
        private const string _nullErrorHandlerTypeName = "RockLib.Logging.LogProcessing.LogProcessor+NullErrorHandler, RockLib.Logging";

        public static readonly IErrorHandler Instance =
            UniversalMemberAccessor.GetStatic(Type.GetType(_nullErrorHandlerTypeName, true)).Instance;
    }
}
