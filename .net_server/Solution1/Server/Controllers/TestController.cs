using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Server.Controllers;
[ApiController]
[Route("api/204")]
public class TestController: ControllerBase
{
    [HttpGet]
    public IActionResult Get204()
    {
        return NoContent();
    }
}

