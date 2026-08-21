using MediatR;
using Microsoft.AspNetCore.Mvc;
using Customer.Application.Commands;
using Customer.Application.Queries;

namespace Customer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command)
    {
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetAll), new { }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetCustomersQuery());
        return Ok(result);
    }
}
