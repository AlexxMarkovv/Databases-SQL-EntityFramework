using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProductShop.Utilities
{
    public class XmlConverter
    {
        /// <summary>
        /// Convert from XML format into C# object types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputXml"></param>
        /// <param name="rootName"></param>
        /// <returns></returns>
        public T Deserialize<T>(string inputXml, string rootName)
        {
            XmlRootAttribute rootAttribute = new XmlRootAttribute(rootName);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), rootAttribute);

            using StringReader reader = new StringReader(inputXml);
            T deserializedObject = (T)xmlSerializer.Deserialize(reader);

            return deserializedObject;
        }

        /// <summary>
        /// Convert from XML format into C# collections
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputXml"></param>
        /// <param name="rootName"></param>
        /// <returns></returns>
        public T[] DeserializeCollection<T>(string inputXml, string rootName)
        {
            XmlRootAttribute rootAttribute = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T[]), rootAttribute);

            using StringReader reader = new StringReader(inputXml);
            T[] deserializedObject = (T[])xmlSerializer.Deserialize(reader);

            return deserializedObject;
        }

        /// <summary>
        /// Convert from C# object types into XML format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="rootName"></param>
        /// <returns></returns>
        public string Serialize<T>(T obj, string rootName)
        {
            XmlRootAttribute xmlRootAttribute = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), xmlRootAttribute);

            XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
            xmlSerializerNamespaces.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new StringWriter(sb);

            xmlSerializer.Serialize(writer, obj, xmlSerializerNamespaces);
            return sb.ToString().TrimEnd();
        }

        /// <summary>
        /// Convert from C# collections into XML format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="rootName"></param>
        /// <returns></returns>
        public string SerializeCollection<T>(T[] obj, string rootName)
        {
            XmlRootAttribute xmlRootAttribute = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T[]), xmlRootAttribute);

            XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
            xmlSerializerNamespaces.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new StringWriter(sb);

            xmlSerializer.Serialize(writer, obj, xmlSerializerNamespaces);
            return sb.ToString().TrimEnd();
        }
    }
}
