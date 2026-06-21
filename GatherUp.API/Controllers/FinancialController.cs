using GatherUp.BL.Services;
using GatherUp.Core.DO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatherUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Manager")]
    public class FinancialController : ControllerBase
    {
        private readonly FinanceService _finance;

        public FinancialController(FinanceService finance) => _finance = finance;

        [HttpGet("event/{eventId}/vendors")]
        public IActionResult GetVendors(int eventId) =>
            Ok(_finance.GetEventVendors(eventId));

        [HttpGet("event/{eventId}/summary")]
        public IActionResult GetSummary(int eventId)
        {
            var (paid, totalIn, vendors, totalOut, balance) = _finance.GetFinancialSummary(eventId);
            return Ok(new { paid, totalIn, vendors, totalOut, balance });
        }

        [HttpGet("event/{eventId}/receipts")]
        public IActionResult GetReceipts(int eventId) =>
            Ok(_finance.GetFlatReceiptReport(eventId));

        [HttpPost("event/{eventId}/vendor")]
        public IActionResult AddVendor(int eventId, [FromBody] VendorAllocation vendor)
        {
            _finance.AddVendor(vendor, eventId);
            return Ok();
        }

        [HttpPut("vendor/{vendorId}/debt")]
        public IActionResult AddDebt(int vendorId, [FromQuery] decimal amount)
        {
            _finance.AddVendorDebt(vendorId, amount);
            return NoContent();
        }
    }
}
