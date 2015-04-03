using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Rock.Collections;
using Rock.Immutable;
using Rock.Logging.Defaults;

namespace Rock.Logging
{
    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    public class LogEntryExtendedProperties : SerializableDictionary<string, string>
    {
        private static readonly Semimutable<string> _xmlNamespace = new Semimutable<string>(GetDefaultXmlNamespace);

        private static readonly XmlSerializer _keySerializer = new XmlSerializer(typeof(string), new XmlRootAttribute("Key") { Namespace = XmlNamespace });
        private static readonly XmlSerializer _valueSerializer = new XmlSerializer(typeof(string), new XmlRootAttribute("Value") { Namespace = XmlNamespace });

        public LogEntryExtendedProperties()
            : base(_keySerializer, _valueSerializer)
        {
        }

        public LogEntryExtendedProperties(IEnumerable<KeyValuePair<string, string>> extendedProperties)
            : base(_keySerializer, _valueSerializer)
        {
            if (extendedProperties != null)
            {
                foreach (var item in extendedProperties)
                {
                    Add(item.Key, item.Value);
                }
            }
        }

        protected LogEntryExtendedProperties(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public static string XmlNamespace
        {
            get { return _xmlNamespace.Value; }
        }

        public static void SetXmlNamespace(string value)
        {
            _xmlNamespace.Value = value;
        }

        public static void SetXmlNamespace(IXmlNamespaceProvider provider)
        {
            _xmlNamespace.SetValue(provider.GetXmlNamespace);
        }

        private static string GetDefaultXmlNamespace()
        {
            return LogEntry.XmlNamespace;
        }
    }
}