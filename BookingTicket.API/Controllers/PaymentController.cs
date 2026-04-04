using Microsoft.AspNetCore.Mvc;
using BookingTicket.Application.Services;
using System;

namespace BookingTicket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVNPayService _vnpayService;

        public PaymentController(IVNPayService vnpayService)
        {
            _vnpayService = vnpayService;
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
    }
}
