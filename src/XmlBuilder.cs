using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace MooGet {

	public class XmlBuilder {
		public MemoryStream Stream { get; set; }
		public XmlWriter    Writer { get; set; }

		public XmlBuilder() {
			Stream = new MemoryStream();
			Writer = XmlWriter.Create(Stream, new XmlWriterSettings { Indent = true });
			Writer.WriteStartDocument();
		}

		public XmlBuilder WriteElement(string name) {
			return WriteElement(name, null, null);
		}
		public XmlBuilder WriteElement(string name, string innerText) {
			return WriteElement(name, innerText, null);
		}
		public XmlBuilder WriteElement(string name, object attributes) {
			return WriteElement(name, null, attributes);
		}
		public XmlBuilder WriteElement(string name, string innerText, object attributes) {
			StartElement(name, innerText, attributes);
			EndElement();
			return this;
		}

		public XmlBuilder StartElement(string name) {
			return StartElement(name, null, null);
		}
		public XmlBuilder StartElement(string name, string innerText) {
			return StartElement(name, innerText, null);
		}
		public XmlBuilder StartElement(string name, object attributes) {
			return StartElement(name, null, attributes);
		}
		public XmlBuilder StartElement(string name, string innerText, object attributes) {
			var attrs = ObjectToAttributes(attributes);

			if (attrs != null && attrs.ContainsKey("xmlns"))
				Writer.WriteStartElement(name, attrs["xmlns"]);
			else
				Writer.WriteStartElement(name);

			if (attrs != null) {
				foreach (var attr in attrs) {
					Writer.WriteStartAttribute(attr.Key.Replace("_", ":")); // foo_bar becomes foo:bar
					Writer.WriteString(attr.Value);
					Writer.WriteEndAttribute();
				}
			}

			if (innerText != null)
				Writer.WriteString(innerText);

			return this;
		}

		public XmlBuilder EndElement() {
			Writer.WriteEndElement();
			return this;
		}

		string _xml;
		public string ToString() {
			if (_xml == null) {
				Writer.WriteEndDocument();
				Writer.Flush();
				var buffer = Stream.ToArray();
				_xml = System.Text.Encoding.UTF8.GetString(buffer).Trim();
			}
			return _xml;
		}

		static IDictionary<string, string> ObjectToAttributes(object anonymousType) {
			if (anonymousType == null)
				return null;

			var attr = BindingFlags.Public | BindingFlags.Instance;
			var dict = new Dictionary<string, string>();
			foreach (var property in anonymousType.GetType().GetProperties(attr))
				if (property.CanRead)
					dict.Add(property.Name, property.GetValue(anonymousType, null).ToString());
			return dict;
		} 
	}
}
