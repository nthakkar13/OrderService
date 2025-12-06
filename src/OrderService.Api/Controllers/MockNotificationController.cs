using Microsoft.AspNetCore.Mvc;
using OrderService.Domain;

namespace OrderService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MockNotificationController : ControllerBase
    {
        private static int _attemptCount = 0;
        public record NotificationPayload(Guid id, Guid customerId, OrderStatus status);

        /// <summary>
        /// Simulates the external NotificationService endpoint.
        /// </summary>
        [HttpPost("notify")]
        public IActionResult Notify([FromBody] NotificationPayload payload)
        {
            _attemptCount++;

            // Simulate a transient error for the first 2 attempts
            if (_attemptCount <= 2)
            {
                if (_attemptCount == 2) _attemptCount = 0;
                return StatusCode(500, new { Message = "Simulated transient error, please retry." });
            }

            _attemptCount = 0;
            return Ok(new { Message = $"Order {payload.id} successfully received for notification." });
        }
    }
}
