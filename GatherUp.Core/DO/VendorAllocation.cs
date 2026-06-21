using GatherUp.Core.Interfaces;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GatherUp.Core.DO
{
    public class VendorAllocation : IEntity
    {
        [XmlAttribute]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal AmountOwed { get; set; }
        public bool HasReceipt { get; set; }
        [XmlIgnore]
        public List<ReceiptDetails> Receipts { get; set; } = new();
        public VendorAllocation() { }
    }
}
