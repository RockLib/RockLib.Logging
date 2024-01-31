using System;
using System.Linq.Expressions;
using System.Reflection;

namespace RockLib.Logging.SafeLogging;

/// <summary>
/// An attribute that signifies a property as not safe for logging. This should be used when a class is
/// marked with the <see cref="SafeToLogAttribute"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class NotSafeToLogAttribute : Attribute
{
    /// <summary>
    /// Decorate the specified property with the <see cref="NotSafeToLogAttribute"/>.
    /// </summary>
    /// <param name="property">The property to decorate.</param>
    public static void Decorate(PropertyInfo property)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(property);
#else
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }
#endif

        SanitizeEngine.NotSafeProperties.Add(property);
    }

    /// <summary>
    /// Decorate the property specified by the expression with the <see cref="NotSafeToLogAttribute"/>.
    /// </summary>
    /// <typeparam name="T">The type of the property to decorate.</typeparam>
    /// <param name="expression">An expression that defines the property to decorate.</param>
    public static void Decorate<T>(Expression<Func<T, object>> expression)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(expression);
#else
        if (expression is null)
        {
            throw new ArgumentNullException(nameof(expression));
        }
#endif
        if (expression.Body is MemberExpression memberExpression
            && memberExpression.Expression == expression.Parameters[0]
            && memberExpression.Member is PropertyInfo property)
        {
            Decorate(property);
        }
        else
        {
            throw new ArgumentException($"Expression does not define a property of type {typeof(T).Name}.", nameof(expression));
        }
    }
}
