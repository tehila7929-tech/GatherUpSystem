using GatherUp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.Core.DO
{
    public class VendorAllocation : IEntity
    {
        public int Id { get; set; }
        public required string Name { get; init; }
        public decimal AmountOwed { get; set; }
        public bool HasReceipt { get; set; }
        public List<ReceiptDetails> Receipts { get; set; } = new();

    }
}
