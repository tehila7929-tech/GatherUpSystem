using System;
using System.IO;
using System.Xml.Serialization;

namespace GatherUp.Infrastructure.XML
{
    public static class XMLSerializer
    {
        public static void Save<T>(string filePath, T obj) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                stream.Flush();
                serializer.Serialize(stream, obj);
            }
        }

        public static T? Load<T>(string filePath) where T : class
        {
            if (!File.Exists(filePath) || new FileInfo(filePath).Length == 0) return null;

            var serializer = new XmlSerializer(typeof(T));
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return (T?)serializer.Deserialize(stream);
            }
        }
    }
}