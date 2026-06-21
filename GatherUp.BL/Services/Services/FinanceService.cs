using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;

namespace GatherUp.BL.Services
{
    public class FinanceService
    {
        private readonly IRepository<VendorAllocation> _vendors;
        private readonly IRepository<Participant> _participants;
        private readonly IRepository<Event> _events;

        public FinanceService(IRepository<VendorAllocation> vendors, IRepository<Participant> participants, IRepository<Event> events)
        {
            _vendors = vendors;
            _participants = participants;
            _events = events;
        }

        public void AddVendor(VendorAllocation vendor, int eventId)
        {
            var ev = _events.GetById(eventId) ?? throw new InvalidOperationException("האירוע לא נמצא.");
            _vendors.Add(vendor);
            ev.VendorIds.Add(vendor.Id);
            _events.Update(ev);
        }

        public void AddVendorDebt(int vendorId, decimal amount)
        {
            var vendor = _vendors.GetById(vendorId) ?? throw new InvalidOperationException("הספק לא נמצא.");
            vendor.AmountOwed += amount;
            _vendors.Update(vendor);
        }

        public IEnumerable<VendorAllocation> GetEventVendors(int eventId)
        {
            var ev = _events.GetById(eventId) ?? throw new InvalidOperationException("האירוע לא נמצא.");
            return _vendors.GetAll().Where(v => ev.VendorIds.Contains(v.Id));
        }

        public (IEnumerable<Participant> paidParticipants, decimal totalIn, IEnumerable<VendorAllocation> vendors, decimal totalOut, decimal balance) GetFinancialSummary(int eventId)
        {
            var ev = _events.GetById(eventId) ?? throw new InvalidOperationException("האירוע לא נמצא.");

            var paidParticipants = GetPaidParticipants(eventId);
            decimal totalIn = CalcTotalIn(paidParticipants);

            var vendors = GetEventVendors(eventId);
            decimal totalOut = CalcTotalOut(vendors);

            return (paidParticipants, totalIn, vendors, totalOut, totalIn - totalOut);
        }

        public IEnumerable<(string ReceiptNumber, decimal Amount, DateTime Date)> GetFlatReceiptReport(int eventId)
        {
            return GetEventVendors(eventId)
                .SelectMany(v => v.Receipts)
                .OrderByDescending(r => r.Date)
                .Select(r => (r.ReceiptNumber, r.Amount, r.Date));
        }

        private IEnumerable<Participant> GetPaidParticipants(int eventId)
        {
            var ev = _events.GetById(eventId)!;
            return _participants.GetAll().Where(p => ev.ParticipantIds.Contains(p.Id) && p.HasPaid);
        }

        private decimal CalcTotalIn(IEnumerable<Participant> paidParticipants) =>
            paidParticipants.Sum(p => p.AmountContributed);

        private decimal CalcTotalOut(IEnumerable<VendorAllocation> vendors) =>
            vendors.Sum(v => v.AmountOwed);
    }
}
