using Microsoft.AspNetCore.Mvc;
using BookingTicket.Application.Services;
using BookingTicket.Application.Interfaces.IServices;
using System;
using System.Threading.Tasks;
using BookingTicket.Application.DTOs.Booking;

namespace BookingTicket.API.Controllers
{
    public class PayOSRequest
    {
        public int BookingId { get; set; }
    }

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
        public async Task<IActionResult> CreatePayOSPayment([FromBody] PayOSRequest payOsRequest)
        {
            try
            {
                var bookingDto = await _bookingService.GetBookingByIdAsync(payOsRequest.BookingId);
                if (bookingDto == null) return NotFound("Booking not found");

                try
                {
                    var result = await _payOSService.CreatePaymentLinkAsync(bookingDto);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    string errorMsg = $"[PAYOS_ERROR] {DateTime.Now}: {ex.Message}\n{ex.StackTrace}\n";
                    if (ex.InnerException != null) errorMsg += $"Inner: {ex.InnerException.Message}\n";
                    
                    try {
                        System.IO.File.AppendAllText("payos_error.txt", errorMsg);
                    } catch {}

                    Console.WriteLine(errorMsg);
                    return StatusCode(500, new { message = "Lỗi khi kết nối với PayOS", details = ex.Message });
                }
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
                if (body.code == "00")
                {
                    int dslIndex = body.data.description.IndexOf("DSL");
                    if (dslIndex >= 0 && int.TryParse(body.data.description.Substring(dslIndex + 3), out int bookingId))
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

        [HttpPost("payos-check")]
        public async Task<IActionResult> CheckPayOSStatus([FromBody] PayOSCheckRequest request)
        {
            try
            {
                dynamic orderInfo = await _payOSService.GetOrderDetailsAsync(request.OrderCode);
                if (orderInfo != null && orderInfo.status == "PAID")
                {
                    await _bookingService.UpdateBookingStatusAsync(request.BookingId, 1);
                    return Ok(new { status = "PAID", message = "Thanh toán thành công" });
                }
                return Ok(new { status = orderInfo.status });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class PayOSCheckRequest
    {
        public int BookingId { get; set; }
        public long OrderCode { get; set; }
    }
}
