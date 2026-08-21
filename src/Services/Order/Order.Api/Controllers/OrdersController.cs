using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Commands;
using Order.Application.Queries;

namespace Order.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderCommand command)
    {
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetByCustomer), new { customerId = command.CustomerId }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetByCustomer([FromQuery] string customerId)
    {
        var result = await mediator.Send(new GetOrdersByCustomerQuery(customerId));
        return Ok(result);
    }
}
