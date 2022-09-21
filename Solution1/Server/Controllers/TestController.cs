using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;
[ApiController]
[Route("api/204")]
public class TestController: ControllerBase
{
    [HttpGet]
    public IActionResult Get204()
    {
        Console.Write(HttpContext.Connection.RemoteIpAddress);
        Console.Write(HttpContext.Connection.LocalIpAddress);
        return NoContent();
    }
}

