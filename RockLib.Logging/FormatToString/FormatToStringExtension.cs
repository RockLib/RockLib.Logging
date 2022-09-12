using RockLib.Reflection.Optimized;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;

namespace RockLib.Logging;

internal static class FormatToStringExtension
{
    private const string _indent = "   ";

    private static readonly string[] _skipProperties = { "InnerException", "InnerExceptions", "Message", "Data", "StackTrace", "TargetSite", "Source", "EntityValidationErrors" };
    private static readonly ConcurrentDictionary<Type, Func<Exception, string, string>> _formatExceptionFuncs = new();
    private static readonly Type? _dbEntityValidationExceptionType;
    private static readonly Action<Exception, StringBuilder, string>? _addValidationErrorMessages;

    static FormatToStringExtension() => InitDbEntityValidationExceptionHandler(
        out _dbEntityValidationExceptionType, out _addValidationErrorMessages);

    public static string? FormatToString(this Exception exception)
    {
        if (exception is null)
        {
            return null;
        }

        var formatException = GetFormatExceptionFunc(exception.GetType());
        return formatException(exception, "");
    }

    private static Func<Exception, string, string> GetFormatExceptionFunc(Type exceptionType) => _formatExceptionFuncs.GetOrAdd(
            exceptionType,
            type =>
            {
                var appendPropertyValueFuncs =
                    type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => !_skipProperties.Contains(p.Name))
                        .Select(GetAppendPropertyValueFunc)
                        .ToList();

                return (ex, indention) =>
                {
                    var additionalIndention = indention + _indent;

                    var sb = new StringBuilder();

                    sb.AppendLine(("Type: " + ex.GetType()).BlockIndent(indention));

                    var message = ex.Message.Trim();

#if NET48
                    if (message.Contains('\n'))
#else
                    if (message.Contains('\n', StringComparison.InvariantCulture))
#endif
                    {
                        sb.AppendLine("Message:".BlockIndent(indention));
                        sb.AppendLine(message.BlockIndent(additionalIndention));
                    }
                    else
                    {
                        sb.AppendLine(("Message: " + message).BlockIndent(indention));
                    }

                    sb.AppendLine("Properties:".BlockIndent(indention));

                    appendPropertyValueFuncs
                        .Aggregate(
                            sb,
                            (stringBuilder, appendPropertyValue) =>
                                appendPropertyValue(stringBuilder, ex, additionalIndention));

                    if (_dbEntityValidationExceptionType is not null
                        && _dbEntityValidationExceptionType.IsInstanceOfType(ex))
                    {
                        sb.AppendLine("EntityValidationErrors:".BlockIndent(additionalIndention));
                        _addValidationErrorMessages?.Invoke(ex, sb, additionalIndention + _indent);
                    }

                    if (ex.Source is not null)
                    {
                        sb.AppendLine(("Source: " + ex.Source).BlockIndent(indention));
                    }

                    if (ex.Data.Count > 0)
                    {
                        sb.AppendLine("Exception Data:".BlockIndent(indention));

                        foreach (DictionaryEntry data in ex.Data)
                        {
                            sb.AppendLine(string.Concat(data.Key, " - ", data.Value).BlockIndent(additionalIndention));
                        }
                    }

                    if (ex.StackTrace is not null)
                    {
                        sb.AppendLine("Stack Trace:".BlockIndent(indention));
                        sb.AppendLine(ex.StackTrace.BlockIndent(indention));
                    }

                    var aggregateException = ex as AggregateException;

                    if (aggregateException is not null)
                    {
                        for (var i = 0; i < aggregateException.InnerExceptions.Count; i++)
                        {
                            var innerException = aggregateException.InnerExceptions[i];

                            if (innerException is not null)
                            {
                                var formatInnerException = GetFormatExceptionFunc(innerException.GetType());

                                sb.AppendLine(("InnerExceptions[" + i + "]:").BlockIndent(indention));
                                sb.AppendLine(formatInnerException(innerException, additionalIndention));
                            }
                        }
                    }
                    else if (ex.InnerException is not null)
                    {
                        var formatInnerException = GetFormatExceptionFunc(ex.InnerException.GetType());

                        sb.AppendLine("InnerException:".BlockIndent(indention));
                        sb.AppendLine(formatInnerException(ex.InnerException, additionalIndention));
                    }

                    return sb.ToString().TrimEnd();
                };
            });

    private static Func<StringBuilder, Exception, string, StringBuilder> GetAppendPropertyValueFunc(PropertyInfo property)
    {
        var getPropertyValue = property.CreateGetter();

        if (property.Name == "HResult")
        {
            var localGetPropertyValue = getPropertyValue;
            getPropertyValue = exception => string.Format(CultureInfo.InvariantCulture, "0x{0:X8}", localGetPropertyValue(exception));
        }

        return
            (sb, exception, indention) =>
            {
                string value;

                try
                {
                    var propertyValue = getPropertyValue(exception);

                    if (property.Name == "HelpLink" && propertyValue is null)
                    {
                        return sb;
                    }

                    value =
                        propertyValue is not null
                            ? propertyValue.ToString()!
                            : "[null]";
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    value = ex.Message.Trim();
                }

#if NET48
                if (value.Contains('\n'))
#else
                if (value.Contains('\n', StringComparison.InvariantCulture))
#endif
                {
                    sb.AppendLine((property.Name + ":").BlockIndent(indention));
                    sb.AppendLine(value.BlockIndent(indention + _indent));
                }
                else
                {
                    sb.AppendLine((property.Name + ": " + value).BlockIndent(indention));
                }

                return sb;
            };
    }

    private static void InitDbEntityValidationExceptionHandler(
        out Type? dbEntityValidationExceptionType,
        out Action<Exception, StringBuilder, string>? addValidationErrorMessages)
    {
        dbEntityValidationExceptionType = null;
        addValidationErrorMessages = null;

        try
        {
            var localDbEntityValidationExceptionType = Type.GetType("System.Data.Entity.Validation.DbEntityValidationException, EntityFramework");
            var dbEntityValidationResultType = Type.GetType("System.Data.Entity.Validation.DbEntityValidationResult, EntityFramework");
            var dbValidationErrorType = Type.GetType("System.Data.Entity.Validation.DbValidationError, EntityFramework");
            var dbEntityEntryType = Type.GetType("System.Data.Entity.Infrastructure.DbEntityEntry, EntityFramework");

            var objectContextType = Type.GetType("System.Data.Entity.Core.Objects.ObjectContext, EntityFramework")
                ?? Type.GetType("System.Data.Objects.ObjectContext, System.Data.Entity");

            if (localDbEntityValidationExceptionType is null
                || dbEntityValidationResultType is null
                || dbValidationErrorType is null
                || dbEntityEntryType is null
                || objectContextType is null)
            {
                return;
            }

            var enumerableOfDbEntityValidationResultType = typeof(IEnumerable<>).MakeGenericType(dbEntityValidationResultType);
            var collectionOfDbValidationErrorType = typeof(ICollection<>).MakeGenericType(dbValidationErrorType);

            var entityValidationErrorsProperty = localDbEntityValidationExceptionType.GetProperty("EntityValidationErrors");
            if (entityValidationErrorsProperty is null
                || entityValidationErrorsProperty.PropertyType != enumerableOfDbEntityValidationResultType)
            {
                return;
            }

            var isValidProperty = dbEntityValidationResultType.GetProperty("IsValid");
            if (isValidProperty is null
                || isValidProperty.PropertyType != typeof(bool))
            {
                return;
            }

            var entryProperty = dbEntityValidationResultType.GetProperty("Entry");
            if (entryProperty is null
                || entryProperty.PropertyType != dbEntityEntryType)
            {
                return;
            }

            var entityProperty = dbEntityEntryType.GetProperty("Entity");
            if (entityProperty is null
                || entityProperty.PropertyType != typeof(object))
            {
                return;
            }

            var getObjectTypeMethod = objectContextType.GetMethod("GetObjectType");
            if (getObjectTypeMethod is null
                || getObjectTypeMethod.ReturnType != typeof(Type)
                || getObjectTypeMethod.GetParameters().Length != 1
                || getObjectTypeMethod.GetParameters()[0].ParameterType != typeof(Type))
            {
                return;
            }

            var validataionErrorsProperty = dbEntityValidationResultType.GetProperty("ValidationErrors");
            if (validataionErrorsProperty is null
                || validataionErrorsProperty.PropertyType != collectionOfDbValidationErrorType)
            {
                return;
            }

            var propertyNameProperty = dbValidationErrorType.GetProperty("PropertyName");
            if (propertyNameProperty is null
                || propertyNameProperty.PropertyType != typeof(string))
            {
                return;
            }

            var errorMessageProperty = dbValidationErrorType.GetProperty("ErrorMessage");
            if (errorMessageProperty is null
                || errorMessageProperty.PropertyType != typeof(string))
            {
                return;
            }

            var getEntityValidationErrors = entityValidationErrorsProperty.CreateGetter<IEnumerable>();
            var isValid = isValidProperty.CreateGetter<bool>();
            var getEntry = entryProperty.CreateGetter();
            var getEntity = entityProperty.CreateGetter();
            var getObjectType = GetGetObjectTypeFunc(getObjectTypeMethod);
            var getValidationErrors = validataionErrorsProperty.CreateGetter<IEnumerable>();
            var getPropertyName = propertyNameProperty.CreateGetter<string>();
            var getErrorMessage = errorMessageProperty.CreateGetter<string>();

            dbEntityValidationExceptionType = localDbEntityValidationExceptionType;
            addValidationErrorMessages = (exception, sb, indention) =>
            {
                // Ultimately, this is what we want, but because we can't have a
                // reference to EntityFramework (because Rock.Core doesn't have a
                // reference to EntityFramework), we have to go the long way around:

                //foreach (var entityValidationError in exception.EntityValidationErrors)
                //{
                //    if (!entityValidationError.IsValid)
                //    {
                //        var entityType = ObjectContext.GetObjectType(
                //            entityValidationError.Entry.Entity.GetType());

                //        sb.AppendLine((entityType + ":").BlockIndent(indention));

                //        foreach (var validationError in entityValidationError.ValidationErrors)
                //        {
                //            sb.AppendLine(validationError.PropertyName + ": " + validationError.ErrorMessage)
                //                .BlockIndent(additionalIndention));
                //        }
                //    }
                //}

                try
                {
                    var additionalIndention = indention + _indent;

                    var entityValidationErrorsEnumerator =
                        getEntityValidationErrors(exception).GetEnumerator();

                    while (entityValidationErrorsEnumerator.MoveNext())
                    {
                        var entityValidationError = entityValidationErrorsEnumerator.Current;

                        if (!isValid(entityValidationError))
                        {
                            var entry = getEntry(entityValidationError);
                            var entity = getEntity(entry);
                            var entityType = getObjectType(entity.GetType()).FullName;

                            sb.AppendLine((entityType + ":").BlockIndent(indention));

                            var validationErrorsEnumerator =
                                getValidationErrors(entityValidationError).GetEnumerator();

                            while (validationErrorsEnumerator.MoveNext())
                            {
                                var validationError = validationErrorsEnumerator.Current;

                                var propertyName = getPropertyName(validationError);
                                var errorMessage = getErrorMessage(validationError);

                                sb.AppendLine((propertyName + ": " + errorMessage).BlockIndent(additionalIndention));
                            }
                        }
                    }
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch { }
#pragma warning restore CA1031 // Do not catch general exception types
            };
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch 
#pragma warning restore CA1031 // Do not catch general exception types
        {
            // If anything goes wrong, no harm no foul - we just won't add validation
            // error messages to the formatted exception.
        }
    }

    // TODO: remove this method and the following class when method invocation is available from RockLib.Reflection.Optimized.
    private static Func<Type, Type> GetGetObjectTypeFunc(MethodInfo getObjectTypeMethod)
    {
        var invoker = new GetObjectTypeInvoker(getObjectTypeMethod);
        ThreadPool.QueueUserWorkItem(state => ((GetObjectTypeInvoker)state!).SetFunc(), invoker);
        return invoker.GetObjectType;
    }

    private class GetObjectTypeInvoker
    {
        public readonly MethodInfo Method;
        public Func<Type, Type> Func;
        public GetObjectTypeInvoker(MethodInfo method)
        {
            Method = method;
            Func = type => (Type)method.Invoke(null, new object[] { type })!;
        }

        public Type GetObjectType(Type type) => Func.Invoke(type);

        public void SetFunc()
        {
            var typeParameter = Expression.Parameter(typeof(Type), "type");

            var body = Expression.Call(Method, typeParameter);

            var lambda =
                    Expression.Lambda<Func<Type, Type>>(
                        body,
                        "GetObjectType",
                        new[] { typeParameter });

            Func = lambda.Compile();
        }
    }
}
