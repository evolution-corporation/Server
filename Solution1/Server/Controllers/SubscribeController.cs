using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("/subscribe")]
public class SubscribeController : ControllerBase
{
    private readonly ISubscribeService service;

    public SubscribeController(ISubscribeService service)
    {
        this.service = service;
    }

    [HttpGet]
    public IActionResult GetUserSubscribe()
    {
        var token = HttpContext.Request.Headers.Authorization.ToString();
        return Ok(service.GetUserSubscribe(token));
    }

    [HttpGet("userId")]
    public IActionResult GetUserSubscribeByAdmin(string userId)
    {
        var token = HttpContext.Request.Headers.Authorization.ToString();
        return Ok(service.GetUserSubscribeByAdmin(userId,token));
    }
}