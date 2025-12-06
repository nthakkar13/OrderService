using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Orders.Commands;
using OrderService.Application.Orders.Queries;
using OrderService.Domain;
using MediatR;

namespace OrderService.Api.Controllers
{
    [ApiController] 
    [Route("[controller]")]
    public class OrdersController : Controller
    {
        private readonly ISender _mediator;

        public OrdersController(ISender mediator)
        {
            _mediator = mediator;
        }

        public record OrderRequest(Guid CustomerId, List<OrderItem> Items, OrderStatus Status);

        /// <summary>
        /// Creates a new order and processes status-specific logic (Notifications, Kafka, Retries).
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            var command = new CreateOrderCommand(
                request.CustomerId,
                request.Items,
                request.Status
            );

            var orderId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetOrder), new { id = orderId }, orderId);
        }

        /// <summary>
        /// Retrieves order details, using Redis cache if available.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Order>> GetOrder(Guid id)
        {
            var query = new GetOrderQuery(id);
            var order = await _mediator.Send(query);

            return order == null ? NotFound() : Ok(order);
        }
    }
}
