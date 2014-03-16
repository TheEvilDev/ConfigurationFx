using System.Configuration;
using System.Xml;

namespace ConfigurationFx
{
    public class FreeFormConfigurationSection : ConfigurationSection
    {
        public DynamicXmlNode Node { get; set; }

        public FreeFormConfigurationSection()
        {
        }

        public FreeFormConfigurationSection(object section)
        {
            Node = DynamicXmlNode.Serialize(section);
        }

        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            Node = DynamicXmlNode.Parse(reader);
        }

        protected override bool SerializeElement(XmlWriter writer, bool serializeCollectionKey)
        {
            SerializeSectionRecursive(ref writer, Node);
            return true;
        }

        private void SerializeSectionRecursive(ref XmlWriter writer, DynamicXmlNode node)
        {
            writer.WriteStartElement(node.Name);

            foreach (var attribute in node.Attributes)
            {
                writer.WriteAttributeString(attribute.Name.LocalName, attribute.Value);
            }

            foreach (var element in node.Elements)
            {
                SerializeSectionRecursive(ref writer, element);
            }

            writer.WriteEndElement();
        }
    }
}