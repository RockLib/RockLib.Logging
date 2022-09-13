using System;
using RockLib.Dynamic;

namespace RockLib.Logging.Tests;

internal static class DelegateErrorHandler
{
    private const string _delegateErrorHandlerTypeName = "RockLib.Logging.ErrorHandlerExtensions+DelegateErrorHandler, RockLib.Logging";
    private static readonly Type _delegateErrorHandlerType = Type.GetType(_delegateErrorHandlerTypeName, true)!;

    public static IErrorHandler New(Action<Error> handleError) => _delegateErrorHandlerType.New(handleError);
}
