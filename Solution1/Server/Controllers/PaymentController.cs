using Microsoft.AspNetCore.Mvc;
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
    public IActionResult GenerateUniqueId(string token)
    {
        return Ok(service.GenerateUniqueId(token));
    }

    [HttpPatch]
    public IActionResult Confirm(string token,int id)
    {
        service.ConfirmPayment(token,id);
        return Ok();
    }
}