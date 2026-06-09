using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using GatherUp.Core.DO;

namespace GatherUp.Infrastructure.XML
{
    public class ReceiptRepository
    {
        private readonly string _filePath;
        private readonly string _receiptsFolder;

        public ReceiptRepository()
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataXML");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            _filePath = Path.Combine(folderPath, "ReceiptDetails.xml");

            _receiptsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UploadedReceipts");
            if (!Directory.Exists(_receiptsFolder))
            {
                Directory.CreateDirectory(_receiptsFolder);
            }

            if (!File.Exists(_filePath))
            {
                XDocument doc = new XDocument(new XElement("Receipts"));
                doc.Save(_filePath);
            }
        }

        public void Add(ReceiptDetails receipt, string sourceFilePath)
        {
            string fileName = Path.GetFileName(sourceFilePath);
            string targetPath = Path.Combine(_receiptsFolder, $"{receipt.ReceiptNumber}_{fileName}");

            if (File.Exists(sourceFilePath))
            {
                File.Copy(sourceFilePath, targetPath, true);
            }

            XDocument doc = XDocument.Load(_filePath);

            XElement newReceipt = new XElement("Receipt",
                new XAttribute("ReceiptNumber", receipt.ReceiptNumber),
                new XElement("Amount", receipt.Amount),
                new XElement("Date", receipt.Date.ToString("o")),
                new XElement("SavedFilePath", targetPath)
            );

            if (doc.Root != null)
            {
                doc.Root.Add(newReceipt);
                doc.Save(_filePath);
            }
        }

        public ReceiptDetails? GetByNumber(string receiptNumber)
        {
            XDocument doc = XDocument.Load(_filePath);

            if (doc.Root == null) return null;

            XElement? element = doc.Root.Elements("Receipt")
                .FirstOrDefault(x => x.Attribute("ReceiptNumber")?.Value == receiptNumber);

            if (element == null) return null;

            return new ReceiptDetails(
                element.Attribute("ReceiptNumber")!.Value,
                (decimal)element.Element("Amount")!,
                DateTime.Parse(element.Element("Date")!.Value)
            );
        }

        public string? GetUploadedFilePath(string receiptNumber)
        {
            XDocument doc = XDocument.Load(_filePath);
            if (doc.Root == null) return null;

            XElement? element = doc.Root.Elements("Receipt")
                .FirstOrDefault(x => x.Attribute("ReceiptNumber")?.Value == receiptNumber);

            return element?.Element("SavedFilePath")?.Value;
        }

        public void Update(ReceiptDetails entity)
        {
            throw new InvalidOperationException("חסימת אבטחה: לא ניתן לערוך קבלה לאחר יצירתה.");
        }

        public void Delete(string receiptNumber)
        {
            throw new InvalidOperationException("חסימת אבטחה: לא ניתן למחוק קבלה לאחר יצירתה.");
        }
    }
}