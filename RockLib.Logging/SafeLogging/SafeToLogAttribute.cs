using System;
using System.Linq.Expressions;
using System.Reflection;

namespace RockLib.Logging.SafeLogging;

/// <summary>
/// An attribute that signifies a property or all the properties of a class are safe to add as extended properties
/// in a log. When used with a class, use the <see cref="NotSafeToLogAttribute"/> to mark specific properites
/// that should not be logged.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public sealed class SafeToLogAttribute : Attribute
{
    /// <summary>
    /// Decorate the specified type with the <see cref="SafeToLogAttribute"/>.
    /// </summary>
    /// <param name="type">The type to decorate.</param>
    public static void Decorate(Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        SanitizeEngine.SafeTypes.Add(type);
    }

    /// <summary>
    /// Decorate the type of <typeparamref name="T"/> with the <see cref="SafeToLogAttribute"/>.
    /// </summary>
    /// <typeparam name="T">The type to decorate.</typeparam>
    public static void Decorate<T>() => Decorate(typeof(T));

    /// <summary>
    /// Decorate the specified property with the <see cref="SafeToLogAttribute"/>.
    /// </summary>
    /// <param name="property">The property to decorate.</param>
    public static void Decorate(PropertyInfo property)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        SanitizeEngine.SafeProperties.Add(property);
    }

    /// <summary>
    /// Decorate the property specified by the expression with the <see cref="SafeToLogAttribute"/>.
    /// </summary>
    /// <typeparam name="T">The type of the property to decorate.</typeparam>
    /// <param name="expression">An expression that defines the property to decorate.</param>
    public static void Decorate<T>(Expression<Func<T, object>> expression)
    {
        if (expression is null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

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
