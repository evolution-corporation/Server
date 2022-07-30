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
    public IActionResult SubscribeUser(Guid userId, DateTime subTime)
    {
        service.SubscribeUser(userId, subTime);
        return Ok();
    }
}