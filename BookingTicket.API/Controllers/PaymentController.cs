using Microsoft.AspNetCore.Mvc;
using BookingTicket.Application.Services;
using BookingTicket.Application.Interfaces.IServices;
using System;
using System.Threading.Tasks;

namespace BookingTicket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVNPayService _vnpayService;
        private readonly IPayOSService _payOSService;
        private readonly IBookingService _bookingService;

        public PaymentController(IVNPayService vnpayService, IPayOSService payOSService, IBookingService bookingService)
        {
            _vnpayService = vnpayService;
            _payOSService = payOSService;
            _bookingService = bookingService;
        }

        [HttpPost("vnpay")]
        public IActionResult CreatePaymentUrl([FromBody] VnPayRequestModel model)
        {
            try
            {
                if (model.CreatedDate == default)
                {
                    model.CreatedDate = DateTime.Now;
                }
                var url = _vnpayService.CreatePaymentUrl(HttpContext, model);
                return Ok(new { paymentUrl = url });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("vnpay-callback")]
        public IActionResult PaymentCallback()
        {
            var response = _vnpayService.PaymentExecute(Request.Query);
            return Ok(response);
        }

        [HttpPost("payos")]
        public async Task<IActionResult> CreatePayOSPayment([FromBody] int bookingId)
        {
            try
            {
                var bookingDto = await _bookingService.GetBookingByIdAsync(bookingId);
                if (bookingDto == null) return NotFound("Booking not found");

                var url = await _payOSService.CreatePaymentLinkAsync(bookingDto);
                return Ok(new { paymentUrl = url });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("payos-webhook")]
        public async Task<IActionResult> PayOSWebhook([FromBody] Net.payOS.Types.WebhookType body)
        {
            try
            {
                var isValid = await _payOSService.VerifyWebhookAsync(body);
                if (!isValid) return BadRequest("Invalid Webhook signature");

                // Process payment result
                // body.data.orderCode matches our generated orderCode
                // PayOS uses orderCode to reference the transaction
                
                // Note: body.data.code == "00" usually means success
                if (body.code == "00")
                {
                    // Update booking status
                    // Since orderCode is long and contains bookingId at the end
                    // In PayOSService: long.Parse(DateTimeOffset.Now.ToString("ffffff") + booking.BookingId.ToString())
                    // This is a bit tricky to recover the ID. 
                    // Better approach: Use orderCode as bookingId if possible or find mapping.
                    // For now, let's assume body.data.description contains "Thanh toán vé #ID"
                    
                    var parts = body.data.description.Split('#');
                    if (parts.Length > 1 && int.TryParse(parts[1], out int bookingId))
                    {
                         await _bookingService.UpdateBookingStatusAsync(bookingId, 1); // 1 = Confirmed/Paid
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
