using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RIAT_1
{
	public class XmlSerialize : ISerialize
	{
		private XmlSerializerNamespaces namespaces;
		private XmlWriterSettings settings;
		private ConcurrentDictionary<Type, XmlSerializer> serializers;

		public XmlSerialize()
		{
			settings = new XmlWriterSettings();
			settings.OmitXmlDeclaration = true;

			namespaces = new XmlSerializerNamespaces();
			namespaces.Add("", "");

			serializers = new ConcurrentDictionary<Type, XmlSerializer>();
		}

		public byte[] Serializing<T>(T obj)
		{
			XmlSerializer formatter = serializers.GetOrAdd(typeof(T), type => new XmlSerializer(type));

			StringBuilder sb = new StringBuilder();
			using (XmlWriter writer = XmlWriter.Create(sb, settings))
			{
				formatter.Serialize(writer, obj, namespaces);
				return Encoding.UTF8.GetBytes(sb.ToString());
			}

		}

		public T Deserializing<T>(byte[] bytes)
		{
			XmlSerializer formatter = serializers.GetOrAdd(typeof(T), type => new XmlSerializer(type));

			using (MemoryStream ms = new MemoryStream(bytes))
				return (T)formatter.Deserialize(ms);
		}
	}
}