using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("/subscribe")]
public class SubscribeController: ControllerBase
{
    private readonly ISubscribeService _service;

    public SubscribeController(ISubscribeService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetSubscribe()
    {
        var token = HttpContext.Request.Headers.Authorization.ToString();
        return Ok(_service.GetUserSubscribe(token));
    }
}