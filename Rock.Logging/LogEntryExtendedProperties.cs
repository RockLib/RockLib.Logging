using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Rock.Collections;

namespace Rock.Logging
{
    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    public class LogEntryExtendedProperties : SerializableDictionary<string, string>
    {
        private static readonly XmlSerializer _keySerializer = new XmlSerializer(typeof(string), new XmlRootAttribute("Key") { Namespace = LogEntry.XmlNamespace });
        private static readonly XmlSerializer _valueSerializer = new XmlSerializer(typeof(string), new XmlRootAttribute("Value") { Namespace = LogEntry.XmlNamespace });

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
    }
}