using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;

namespace GatherUp.Infrastructure.XML
{
    public class ReceiptRepository : XmlRepository<VendorAllocation>
    {
        private readonly string _receiptsXmlPath;
        private readonly string _uploadsFolder;

        public ReceiptRepository(string folderPath) : base(folderPath)
        {
            _receiptsXmlPath = Path.Combine(folderPath, "ReceiptDetails.xml");
            _uploadsFolder = Path.Combine(folderPath, "UploadedReceipts");

            if (!Directory.Exists(_uploadsFolder))
                Directory.CreateDirectory(_uploadsFolder);

            if (!File.Exists(_receiptsXmlPath))
                new XDocument(new XElement("Receipts")).Save(_receiptsXmlPath);
        }

        public void AddReceipt(ReceiptDetails receipt, string sourceFilePath)
        {
            string fileName = Path.GetFileName(sourceFilePath);
            string targetPath = Path.Combine(_uploadsFolder, $"{receipt.ReceiptNumber}_{fileName}");

            if (File.Exists(sourceFilePath))
                File.Copy(sourceFilePath, targetPath, true);

            XDocument doc = XDocument.Load(_receiptsXmlPath);
            doc.Root!.Add(new XElement("Receipt",
                new XAttribute("ReceiptNumber", receipt.ReceiptNumber),
                new XElement("Amount", receipt.Amount),
                new XElement("Date", receipt.Date.ToString("o")),
                new XElement("SavedFilePath", targetPath)
            ));
            doc.Save(_receiptsXmlPath);
        }

        public ReceiptDetails? GetReceiptByNumber(string receiptNumber)
        {
            XDocument doc = XDocument.Load(_receiptsXmlPath);
            XElement? element = doc.Root!.Elements("Receipt")
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
            XDocument doc = XDocument.Load(_receiptsXmlPath);
            return doc.Root!.Elements("Receipt")
                .FirstOrDefault(x => x.Attribute("ReceiptNumber")?.Value == receiptNumber)
                ?.Element("SavedFilePath")?.Value;
        }

        public void UpdateReceipt(ReceiptDetails receipt) =>
            throw new InvalidOperationException("לא ניתן לערוך קבלה לאחר היצירה.");

        public void DeleteReceipt(string receiptNumber) =>
            throw new InvalidOperationException("לא ניתן למחוק קבלה לאחר היצירה.");
    }
}
