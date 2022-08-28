using Microsoft.AspNetCore.Mvc;
using Server.Entities;
using Server.Entities.Payment;
using Server.Services;

namespace Server.Controllers;

//TODO: Написать проверку оплаты в течении часа
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
    public IActionResult SubscribeUser(string token, SubscribeType type, bool? needRecurrent)
    {
        var recurrent = needRecurrent ?? false;
        return Ok(service.SubscribeUser(token, recurrent, type));
    }
    
}