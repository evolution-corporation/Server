using Microsoft.AspNetCore.Mvc;
using Server.Entities.Payment;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("/tinkoffNotification")]
public class TinkoffNotificationsController: ControllerBase
{
    private readonly ITinkoffNotificationService service;

    public TinkoffNotificationsController(ITinkoffNotificationService service)
    {
        this.service = service;
    }

    [HttpPost]
    public IActionResult NotificationAccept(TinkoffNotification notification)
    {
        service.CheckPayment(notification);
        return Ok("OK");
    }
}