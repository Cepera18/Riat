using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RIAT_1 {
	public class XmlSerialize : ISerialize {
		private XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
		private XmlWriterSettings settings = new XmlWriterSettings();
		private readonly ConcurrentDictionary<Type, XmlSerializer> serializers;

		public XmlSerialize() {
			settings = new XmlWriterSettings();
			settings.Indent = false;
			settings.IndentChars = "\t";
			settings.OmitXmlDeclaration = true;

			namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
			serializers = new ConcurrentDictionary<Type, XmlSerializer>();
		}

		public byte[] Serializing<T>(T obj) {
			XmlSerializer formatter = new XmlSerializer(typeof(T));

			StringBuilder sb = new StringBuilder();
			using (XmlWriter writer = XmlWriter.Create(sb, settings)){
				formatter.Serialize(writer, obj, namespaces);
				return Encoding.UTF8.GetBytes(sb.ToString());
			}

		}

		public T Deserializing<T>(byte[] bytes) {
			XmlSerializer formatter = new XmlSerializer(typeof(T));
			using (MemoryStream ms = new MemoryStream(bytes)) {
				return (T)formatter.Deserialize(ms);
			}
		}

		private XmlSerializer GetXmlSerializer(Type currentType) {
			return serializers.GetOrAdd(currentType, type => new XmlSerializer(type));
		}
	}
}