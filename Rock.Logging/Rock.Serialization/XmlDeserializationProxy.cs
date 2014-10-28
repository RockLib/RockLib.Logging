using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Rock.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Rock.Serialization
{
    /// <summary>
    /// A class that creates instances of type <typeparamref name="TTarget"/>. Instances of
    /// <see cref="XmlDeserializationProxy{TTarget}"/> are intended to be created via
    /// standard deserialization.
    /// </summary>
    /// <typeparam name="TTarget">The type of object that an instance of
    /// <see cref="XmlDeserializationProxy{TTarget}"/></typeparam> creates.
    /// <remarks>
    /// The <see cref="XmlDeserializationProxy{TTarget}"/> class is flexible in the xml that
    /// it accepts.
    /// 
    /// For example, we want to obtain an instance of the FooContainer class:
    /// 
    /// <code>
    /// <![CDATA[
    /// public class FooContainer
    /// {
    ///     public XmlDeserializationProxy<IFoo> Foo { get; set; }
    /// }
    /// 
    /// public interface IFoo
    /// {
    ///     void FooBar();
    /// }
    /// 
    /// public class Foo : IFoo
    /// {
    ///     private readonly string _bar;
    /// 
    ///     public Foo(string bar)
    ///     {
    ///         _bar = bar;
    ///     }
    /// 
    ///     public void FooBar()
    ///     {
    ///         Console.WriteLine("Foo: {0}", _bar);
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// 
    /// In order to obtain an instance of the 'FooContainer' class, use standard xml
    /// serialization.
    /// 
    /// <code>
    /// string xml = GetXml();
    /// 
    /// FooContainer fooContainer;
    /// 
    /// using (var stringReader = new StringReader(xml))
    /// {
    ///     var serializer = new XmlSerializer(typeof(FooContainer));
    ///     fooContainer = (FooContainer)serializer.Deserialize(stringReader);
    /// }
    /// </code>
    /// 
    /// Now we can create an instance of the 'IFoo' interface, using the 'Foo' property.
    /// 
    /// <code>
    /// IFoo foo = fooContainer.Foo.CreateInstance();
    /// </code>
    /// 
    /// We can use this xml to deserialize an instance of FooContainer. Note that the
    /// 'Foo' element has a 'type' attribute that describes the type that should be
    /// created by the XmlDeserializationProxy&lt;IFoo&gt;.
    /// 
    /// <code>
    /// <![CDATA[
    /// <FooContainer>
    ///   <Foo type="MyNamespace.Foo, MyAssembly">
    ///     <Bar>abc</Bar>
    ///   </Foo>
    /// </FooContainer>
    /// ]]>
    /// </code>
    /// 
    /// Note that the 'Bar' property of the 'Foo' class is specified by an element. We
    /// can also specify the 'Bar' property with an xml attribute.
    /// 
    /// <code>
    /// <![CDATA[
    /// <FooContainer>
    ///   <Foo type="MyNamespace.Foo, MyAssembly" Bar="abc" />
    /// </FooContainer>
    /// ]]>
    /// </code>
    /// 
    /// These dynamic properties are also case-insensitive.
    /// 
    /// <code>
    /// <![CDATA[
    /// <FooContainer>
    ///   <Foo type="MyNamespace.Foo, MyAssembly">
    ///     <bar>abc</bar>
    ///   </Foo>
    /// </FooContainer>
    /// 
    /// <FooContainer>
    ///   <Foo type="MyNamespace.Foo, MyAssembly" bar="abc" />
    /// </FooContainer>
    /// ]]>
    /// </code>
    /// 
    /// If we want to supply a default type (and omit the 'type' xml attribute), we need
    /// to create a subclass of <see cref="XmlDeserializationProxy{TTarget}"/>.
    /// 
    /// <code>
    /// <![CDATA[
    /// public class FooProxy : XmlDeserializationProxy<IFoo>
    /// {
    ///     public FooProxy()
    ///         : base(typeof(Foo))
    ///     {
    ///     }
    /// }
    /// 
    /// public class FooContainer
    /// {
    ///     public FooProxy Foo { get; set; }
    /// }
    /// ]]>
    /// </code>
    /// 
    /// Now our xml doesn't need to specify the 'type' xml attribute (but it still can if 
    /// it needs a type other than the default type).
    /// 
    /// <code>
    /// <![CDATA[
    /// <FooContainer>
    ///   <Foo Bar="abc" />
    /// </FooContainer>
    /// ]]>
    /// </code>
    /// </remarks>
    public class XmlDeserializationProxy<TTarget>
    {
        private readonly Type _defaultType;
        private Lazy<Type> _type;

        /// <summary>
        /// Initializes a new instance of <see cref="XmlDeserializationProxy{TTarget}"/>
        /// without specifying a default type. If no type is provided via the 
        /// <see cref="TypeAssemblyQualifiedName"/> property after this instance of
        /// <see cref="XmlDeserializationProxy{TTarget}"/> has been create, then subsequent
        /// calls to the <see cref="CreateInstance()"/> or <see cref="CreateInstance(IResolver)"/>
        /// methods will fail.
        /// </summary>
        public XmlDeserializationProxy()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="XmlDeserializationProxy{TTarget}"/>,
        /// specifying a default type. If <paramref name="defaultType"/> is null, and if no 
        /// type is provided via the <see cref="TypeAssemblyQualifiedName"/> property after
        /// this instance of <see cref="XmlDeserializationProxy{TTarget}"/> has been create, 
        /// then subsequent calls to the <see cref="CreateInstance()"/> or
        /// <see cref="CreateInstance(IResolver)"/> /// methods will fail.
        /// </summary>
        /// <param name="defaultType">
        /// The type of object to create if <see cref="TypeAssemblyQualifiedName"/> is not specified.
        /// </param>
        public XmlDeserializationProxy(Type defaultType)
        {
            _defaultType = ThrowIfNotAssignableToT(defaultType);
            _type = new Lazy<Type>(() => _defaultType);
        }

        /// <summary>
        /// Gets or sets the assembly qualified name of the type that this proxy serializes.
        /// NOTE: Do not use this property directly - it exists as an implementation detail
        /// for the internal use of the <see cref="XmlDeserializationProxy{TTarget}"/> class.
        /// </summary>
        [XmlAttribute("type")]
        public string TypeAssemblyQualifiedName
        {
            get { return _type.Value.AssemblyQualifiedName; }
            set { _type = new Lazy<Type>(() => value != null ? ThrowIfNotAssignableToT(Type.GetType(value)) : _defaultType); }
        }

        /// <summary>
        /// Gets or sets any xml attributes that exist in the xml document, but are not
        /// associated with a property of this class (whether this class is
        /// <see cref="XmlDeserializationProxy{TTarget}"/> or its inheritor).
        /// NOTE: Do not use this property directly - it exists as an implementation detail
        /// for the internal use of the <see cref="XmlDeserializationProxy{TTarget}"/> class.
        /// </summary>
        [XmlAnyAttribute]
        public XmlAttribute[] AdditionalAttributes { get; set; }

        /// <summary>
        /// Gets or sets any xml elements that exist in the xml document, but are not
        /// associated with a property of this class (whether this class is
        /// <see cref="XmlDeserializationProxy{TTarget}"/> or its inheritor).
        /// NOTE: Do not use this property directly - it exists as an implementation detail
        /// for the internal use of the <see cref="XmlDeserializationProxy{TTarget}"/> class.
        /// </summary>
        [XmlAnyElement]
        public XmlElement[] AdditionalElements { get; set; }

        /// <summary>
        /// Create a new instance of the type specified by the <see cref="TypeAssemblyQualifiedName"/>
        /// property, using values from the <see cref="AdditionalAttributes"/> and
        /// <see cref="AdditionalElements"/> properties, along with any properties specified by
        /// an inheritor of the <see cref="XmlDeserializationProxy{TTarget}"/> class.
        /// </summary>
        /// <returns>
        /// A new instance of the type specified by the <see cref="TypeAssemblyQualifiedName"/> property.
        /// </returns>
        public TTarget CreateInstance()
        {
            return CreateInstance(null);
        }

        /// <summary>
        /// Create a new instance of the type specified by the <see cref="TypeAssemblyQualifiedName"/>
        /// property, using values from the <see cref="AdditionalAttributes"/> and
        /// <see cref="AdditionalElements"/> properties, along with any properties specified by
        /// an inheritor of the <see cref="XmlDeserializationProxy{TTarget}"/> class.
        /// </summary>
        /// <param name="resolver">
        /// An optional <see cref="IResolver"/> that can supply any missing values required by a
        /// constructor of the type specified by the <see cref="TypeAssemblyQualifiedName"/>
        /// property.
        /// </param>
        /// <returns>
        /// A new instance of the type specified by the <see cref="TypeAssemblyQualifiedName"/> property.
        /// </returns>
        public virtual TTarget CreateInstance(IResolver resolver)
        {
            if (_type.Value == null)
            {
                throw new InvalidOperationException("A value for 'type' must provided - no default value exists.");
            }

            var creationScenario = GetCreationScenario();
            var args = CreateArgs(creationScenario.Parameters, resolver);
            var instance = creationScenario.Constructor.Invoke(args);

            foreach (var property in creationScenario.Properties)
            {
                object value;

                if (TryGetValueForInstance(property.Name, property.PropertyType, out value))
                {
                    property.SetValue(instance, value);
                }
            }

            return (TTarget)instance;
        }

        private CreationScenario GetCreationScenario()
        {
            return new CreationScenario(GetConstructor(), _type.Value);
        }

        private ConstructorInfo GetConstructor()
        {
            return _type.Value.GetConstructors().OrderByDescending(c => c.GetParameters().Length).First();
        }

        private object[] CreateArgs(IEnumerable<ParameterInfo> parameters, IResolver resolver)
        {
            var argsList = new List<object>();

            foreach (var parameter in parameters)
            {
                object argValue;

                if (TryGetValueForInstance(parameter.Name, parameter.ParameterType, out argValue))
                {
                    argsList.Add(argValue);
                }
                else if (resolver != null && resolver.CanGet(parameter.ParameterType))
                {
                    argsList.Add(resolver.Get(parameter.ParameterType));
                }
                else
                {
                    argsList.Add(parameter.HasDefaultValue ? parameter.DefaultValue : null);
                }
            }

            return argsList.ToArray();
        }

        private bool TryGetValueForInstance(string name, Type type, out object value)
        {
            if (TryGetPropertyValue(name, type, out value))
            {
                if (value == null)
                {
                    object additionalValue;
                    if (TryGetAdditionalValue(name, type, out additionalValue))
                    {
                        value = additionalValue;
                    }
                }

                return true;
            }

            if (TryGetAdditionalValue(name, type, out value))
            {
                return true;
            }

            return false;
        }

        private bool TryGetPropertyValue(string name, Type type, out object value)
        {
            var properties =
                GetType().GetProperties()
                    .Where(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && p.PropertyType == type)
                    .OrderBy(p => p.Name, new CaseSensitiveEqualityFirstAsComparedTo(name));

            foreach (var property in properties)
            {
                // ReSharper disable once EmptyGeneralCatchClause
                try
                {
                    value = property.GetValue(this);
                    return true;
                }
                catch
                {
                }
            }

            value = null;
            return false;
        }

        private bool TryGetAdditionalValue(string name, Type type, out object value)
        {
            foreach (var additionalNode in GetAdditionalNodes(name))
            {
                var additionalElement = additionalNode as XmlElement;

                if (additionalElement != null)
                {
                    if (TryGetElementValue(additionalElement, type, out value))
                    {
                        return true;
                    }
                }
                else
                {
                    var additionalAttribute = (XmlAttribute)additionalNode;

                    if (TryConvert(additionalAttribute.Value, type, out value))
                    {
                        return true;
                    }
                }
            }

            value = null;
            return false;
        }

        private IEnumerable<XmlNode> GetAdditionalNodes(string name)
        {
            var allAdditionalNodes =
                (AdditionalElements ?? Enumerable.Empty<XmlNode>())
                    .Concat(AdditionalAttributes ?? Enumerable.Empty<XmlNode>());

            var additionalNodes = allAdditionalNodes
                .Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.Name, new CaseSensitiveEqualityFirstAsComparedTo(name))
                .ThenBy(x => x, new ElementsBeforeAttributes());

            return additionalNodes;
        }

        private static bool TryGetElementValue(XmlElement additionalElement, Type type, out object value)
        {
            if (!additionalElement.HasChildNodes && !additionalElement.HasAttributes)
            {
                if (TryConvert(additionalElement.Value, type, out value))
                {
                    return true;
                }
            }

            using (var reader = new StringReader(additionalElement.OuterXml))
            {
                try
                {
                    XmlSerializer serializer;

                    if (type.IsInterface || type.IsAbstract)
                    {
                        var typeName = additionalElement.GetAttribute("type");
                        var typeFromAttribute = Type.GetType(typeName);

                        if (typeFromAttribute == null)
                        {
                            value = null;
                            return false;
                        }

                        serializer = new XmlSerializer(typeFromAttribute, new XmlRootAttribute(additionalElement.Name));
                    }
                    else
                    {
                        serializer = new XmlSerializer(type, new XmlRootAttribute(additionalElement.Name));
                    }

                    value = serializer.Deserialize(reader);
                    return true;
                }
                catch
                {
                    value = null;
                    return false;
                }
            }
        }

        private static bool TryConvert(string stringValue, Type type, out object value)
        {
            var converter = TypeDescriptor.GetConverter(type);

            if (converter.CanConvertFrom(typeof(string)))
            {
                value = converter.ConvertFrom(stringValue);
                return true;
            }

            converter = TypeDescriptor.GetConverter(typeof(string));

            if (converter.CanConvertTo(type))
            {
                value = converter.ConvertTo(stringValue, type);
                return true;
            }

            value = null;
            return false;
        }

        private static Type ThrowIfNotAssignableToT(Type type)
        {
            if (type == null)
            {
                return null;
            }

            if (!typeof(TTarget).IsAssignableFrom(type))
            {
                throw new ArgumentException(string.Format("The provided Type, {0}, must be assignable to Type {1}.", type, typeof(TTarget)));
            }

            return type;
        }

        private class CreationScenario
        {
            private readonly ConstructorInfo _ctor;
            private readonly ParameterInfo[] _parameters;
            private readonly IEnumerable<PropertyInfo> _properties;

            public CreationScenario(ConstructorInfo ctor, Type type)
            {
                _ctor = ctor;
                _parameters = ctor.GetParameters();

                var parameterNames = _parameters.Select(p => p.Name).ToList();

                _properties =
                    type.GetProperties()
                        .Where(p =>
                            p.CanRead
                            && p.CanWrite
                            && p.GetGetMethod().IsPublic
                            && p.GetSetMethod().IsPublic
                            && parameterNames.All(parameterName => !parameterName.Equals(p.Name, StringComparison.OrdinalIgnoreCase)));
            }

            public ConstructorInfo Constructor { get { return _ctor; } }
            public IEnumerable<ParameterInfo> Parameters { get { return _parameters; } }
            public IEnumerable<PropertyInfo> Properties { get { return _properties; } }
        }

        private class CaseSensitiveEqualityFirstAsComparedTo : IComparer<string>
        {
            private readonly string _nameToMatch;

            public CaseSensitiveEqualityFirstAsComparedTo(string nameToMatch)
            {
                _nameToMatch = nameToMatch;
            }

            public int Compare(string lhs, string rhs)
            {
                if (string.Equals(lhs, rhs, StringComparison.Ordinal))
                {
                    return 0;
                }

                if (string.Equals(lhs, _nameToMatch, StringComparison.Ordinal))
                {
                    return -1;
                }

                if (string.Equals(rhs, _nameToMatch, StringComparison.Ordinal))
                {
                    return 1;
                }

                return 0;
            }
        }

        private class ElementsBeforeAttributes : IComparer<XmlNode>
        {
            public int Compare(XmlNode lhs, XmlNode rhs)
            {
                if (lhs is XmlElement)
                {
                    return (rhs is XmlAttribute) ? -1 : 0;
                }

                return (rhs is XmlElement) ? 1 : 0;
            }
        }
    }
}