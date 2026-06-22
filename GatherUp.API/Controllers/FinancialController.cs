using GatherUp.BL.Services;
using GatherUp.Core.DO;
using GatherUp.Core.Exceptions;
using GatherUp.Infrastructure.XML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GatherUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FinancialController : ControllerBase
    {
        private readonly FinanceService _finance;
        private readonly EventService _events;
        private readonly ReceiptRepository _receipts;

        public FinancialController(FinanceService finance, EventService events, ReceiptRepository receipts)
        {
            _finance = finance;
            _events = events;
            _receipts = receipts;
        }

        int CallerId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        bool IsManagerOf(int eventId) => _events.GetById(eventId).ManagerId == CallerId;

        [HttpGet("event/{eventId}/vendors")]
        public IActionResult GetVendors(int eventId)
        {
            if (!IsManagerOf(eventId)) return Forbid();
            return Ok(_finance.GetEventVendors(eventId));
        }

        [HttpGet("event/{eventId}/summary")]
        public IActionResult GetSummary(int eventId)
        {
            if (!IsManagerOf(eventId)) return Forbid();
            var (paid, totalIn, vendors, totalOut, balance) = _finance.GetFinancialSummary(eventId);
            return Ok(new { paid, totalIn, vendors, totalOut, balance });
        }

        [HttpGet("event/{eventId}/receipts")]
        public IActionResult GetReceipts(int eventId)
        {
            if (!IsManagerOf(eventId)) return Forbid();
            return Ok(_finance.GetFlatReceiptReport(eventId));
        }

        [HttpPost("event/{eventId}/vendor")]
        public IActionResult AddVendor(int eventId, [FromBody] VendorAllocation vendor)
        {
            if (!IsManagerOf(eventId)) return Forbid();
            _finance.AddVendor(vendor, eventId);
            return Ok(new { vendor.Id, vendor.Name });
        }

        [HttpPut("vendor/{vendorId}/debt")]
        public IActionResult AddDebt(int vendorId, [FromQuery] decimal amount, [FromQuery] int eventId)
        {
            if (!IsManagerOf(eventId)) return Forbid();
            _finance.AddVendorDebt(vendorId, amount);
            return NoContent();
        }

        [HttpPost("vendor/{vendorId}/receipt")]
        public async Task<IActionResult> UploadReceipt(
            int vendorId,
            [FromQuery] int eventId,
            [FromForm] string receiptNumber,
            [FromForm] decimal amount,
            IFormFile file)
        {
            if (!IsManagerOf(eventId)) return Forbid();
            if (file == null || file.Length == 0)
                throw new InvalidInputException("A receipt file is required.");
            if (string.IsNullOrWhiteSpace(receiptNumber))
                throw new InvalidInputException("Receipt number is required.");
            if (amount <= 0)
                throw new InvalidInputException("Amount must be positive.");

            var tempPath = Path.Combine(Path.GetTempPath(), file.FileName);
            await using (var stream = new FileStream(tempPath, FileMode.Create))
                await file.CopyToAsync(stream);

            var receipt = new ReceiptDetails(receiptNumber, amount, DateTime.UtcNow);
            _receipts.AddReceipt(receipt, tempPath);
            _finance.AttachReceiptToVendor(vendorId, receipt);

            if (System.IO.File.Exists(tempPath))
                System.IO.File.Delete(tempPath);

            return Ok(new { receipt.ReceiptNumber, receipt.Amount, receipt.Date });
        }
    }
}
