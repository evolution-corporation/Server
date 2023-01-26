using Microsoft.AspNetCore.Mvc;
using Server.Entities;
using Server.Entities.Payment;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("/payment")]
public class PaymentController : ControllerBase
{
    private IPaymentService service;

    public PaymentController(IPaymentService service)
    {
        this.service = service;
    }

    [HttpGet]
    public IActionResult SubscribeUser(SubscribeType type, bool? needRecurrent)
    {
        var token = HttpContext.Request.Headers.Authorization.ToString();
        var recurrent = needRecurrent ?? false;
        return Redirect(service.SubscribeUser(token, recurrent, type));
    }
}