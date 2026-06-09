using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GatherUp.Infrastructure.XML
{
    public static class XMLDocManager
    {
        public static XDocument LoadOrCreate(string filePath, string rootName)
        {
            if (!File.Exists(filePath))
                return new XDocument(new XElement(rootName));
            return XDocument.Load(filePath);
        }

        public static IEnumerable<XElement> GetAll(XDocument doc, string elementName)
        {
            return doc.Root!.Elements(elementName);
        }

        public static XElement? FindById(XDocument doc, string elementName, int id)
        {
            return doc.Root!.Elements(elementName)
                .FirstOrDefault(e => (int?)e.Attribute("Id") == id);
        }

        public static void Save(XDocument doc, string filePath)
        {
            doc.Save(filePath);
        }
    }
}
