using MediatR;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Commands;

namespace Payment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController(IMediator mediator) : ControllerBase
{
    // این اندپوینت برای پرداخت مستقیم است (مطابق نمونه API سند)
    // پردازش خودکار پس از ثبت سفارش از طریق Consumer در بالا انجام می‌شود.
    [HttpPost]
    public async Task<IActionResult> Pay([FromBody] PayRequest request)
    {
        var status = await mediator.Send(new ProcessPaymentCommand(request.OrderId, request.PaymentMethod, request.Amount ?? 0));
        return Ok(new { status });
    }
}

public record PayRequest(Guid OrderId, string PaymentMethod, decimal? Amount);
