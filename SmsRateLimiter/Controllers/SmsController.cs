using Microsoft.AspNetCore.Mvc;
using SmsRateLimiter.Models;
using SmsRateLimiter.Services;

namespace SmsRateLimiter.Controllers
{
    [ApiController]
    [Route("api/sms")]
    public class SmsController : ControllerBase
    {
        private readonly ISmsRateLimiterService _smsRateLimiterService;

        public SmsController(ISmsRateLimiterService smsRateLimiterService)
        {
            _smsRateLimiterService = smsRateLimiterService;
        }

        [HttpPost("check")]
        public async Task<IActionResult> CheckLimit([FromBody] SmsRequest request)
        {
            // We need to add PhoneNumber and AccountId validation like
            if (string.IsNullOrWhiteSpace(request.PhoneNumber) || string.IsNullOrWhiteSpace(request.AccountId))
            {
                throw new ArgumentException("PhoneNumber and AccountId are required.");
            }
            bool canSend = await _smsRateLimiterService.CanSendMessageAsync(request.PhoneNumber, request.AccountId);
            return Ok(new { canSend });
        }
    }

}
