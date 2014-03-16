using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ConfigurationFx
{
    public class DynamicXmlNode : DynamicObject
    {
        public ICollection<XAttribute> Attributes { get; private set; }
        public ICollection<DynamicXmlNode> Elements { get; private set; }

        public string Name { get; set; }

        private DynamicXmlNode(XElement root)
        {
            Name = root.Name.LocalName;
            Attributes = root.Attributes().ToList();
            Elements = root.Elements().Select(e => new DynamicXmlNode(e)).ToList();
        }

        public static DynamicXmlNode Serialize(object obj)
        {
            return Parse(obj.AsXml());
        }

        public static DynamicXmlNode Parse(string xmlString)
        {
            return new DynamicXmlNode(XDocument.Parse(xmlString).Root);
        }

        public static DynamicXmlNode Parse(XmlReader reader)
        {
            return Parse(reader.ReadOuterXml());
        }

        public static DynamicXmlNode Load(string filename)
        {
            return new DynamicXmlNode(XDocument.Load(filename).Root);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = Attributes.FirstOrDefault(a => a.Name.LocalName.EqualsIgnoreCase(binder.Name)) ??
                     (object) Elements.FirstOrDefault(e => e.Name.EqualsIgnoreCase(binder.Name));

            return result == null;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (value.GetType().IsValueType || value is string)
            {
                var existing = Attributes.FirstOrDefault(a => a.Name.LocalName.EqualsIgnoreCase(binder.Name));

                if (existing != null)
                {
                    Attributes.Remove(existing);
                }

                Attributes.Add(new XAttribute(binder.Name, value));
            }

            var element = Elements.FirstOrDefault(a => a.Name.EqualsIgnoreCase(binder.Name));

            if (element != null)
            {
                Elements.Remove(element);
            }

            Elements.Add(Serialize(value));

            return true;
        }
    }
}