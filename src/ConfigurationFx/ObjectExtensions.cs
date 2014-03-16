using System.IO;
using System.Xml.Serialization;

namespace ConfigurationFx
{
    public static class ObjectExtensions
    {
        public static string AsXml(this object obj)
        {
            var serializer = new XmlSerializer(obj.GetType());

            using (var stream = new StringWriter())
            {
                serializer.Serialize(stream, obj);
                return stream.ToString();
            }
        }
    }
}