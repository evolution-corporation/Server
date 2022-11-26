using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Server.Entities.Payment;
using Server.Helpers;
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
    public IActionResult NotificationAccept()
    {
        var xyu = HttpContext.Request.Body;
        var str = new StreamReader(xyu);
        var task = str.ReadLineAsync();
        task.Wait();
        var x = task.Result!;
        Console.WriteLine(x);
        var notification = (TinkoffNotification)JsonConvert.DeserializeObject(x,typeof(TinkoffNotification))!;;
        service.CheckPayment(notification);
        return Ok();
        return Ok("OK");
    }
}