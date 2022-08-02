using Microsoft.AspNetCore.Mvc;
using Server.Helpers;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("/subscribe")]
public class SubscribeController: ControllerBase
{
    private DataContext context;
    private ISubscribeService service;

    public SubscribeController(DataContext context, ISubscribeService service)
    {
        this.context = context;
        this.service = service;
    }

    [HttpPost]
    public IActionResult SubscribeUser(string token, int subTime)
    {
        service.SubscribeUser(token, subTime);
        return Ok();
    }

    [HttpDelete]
    public IActionResult UnsubscribeUser(string token)
    {
        service.UnsubscribeUser(token);
        return Ok();
    }
}