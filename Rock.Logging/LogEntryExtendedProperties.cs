using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Rock.Logging
{
    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    public class LogEntryExtendedProperties : Dictionary<string, string>, IXmlSerializable
    {
        private static readonly XmlSerializer _keySerializer = new XmlSerializer(typeof(string), new XmlRootAttribute("Key") { Namespace = LogEntry.XmlNamespace });
        private static readonly XmlSerializer _valueSerializer = new XmlSerializer(typeof(string), new XmlRootAttribute("Value") { Namespace = LogEntry.XmlNamespace });

        public LogEntryExtendedProperties()
        {
        }

        public LogEntryExtendedProperties(IEnumerable<KeyValuePair<string, string>> extendedProperties)
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

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="LogEntryExtendedProperties"/>.
        /// </summary>
        /// <param name="key">The string to use as the key of the element to add.</param>
        /// <param name="value">The object whose string representation is to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="LogEntryExtendedProperties"/>.</exception>
        public void Add(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            ((IDictionary<string, string>) this).Add(
                key,
                    value == null
                        ? null
                        : value.ToString());
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                reader.Read();
                return;
            }

            while (reader.Read() && reader.MoveToContent() != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("Item");

                var key = (string)_keySerializer.Deserialize(reader);
                var value = (string)_valueSerializer.Deserialize(reader);

                Add(key, value);
            }

            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach (var kvp in this)
            {
                writer.WriteStartElement("Item");

                // We'll never have a null key, so always write it.
                _keySerializer.Serialize(writer, kvp.Key);

                // But is is possible to have a null value, so only write it if it exists.
                if (kvp.Value != null)
                {
                    _valueSerializer.Serialize(writer, kvp.Value);
                }

                writer.WriteEndElement();
            }
        }
    }
}