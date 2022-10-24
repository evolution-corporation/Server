using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders.Physical;

namespace Server.Controllers;
[ApiController]
[Route("api/204")]
public class TestController: ControllerBase
{
    [HttpGet]
    public IActionResult Get204()
    {
        var file = new FileInfo("/test/xyu");
        HttpContext.Response.SendFileAsync(new PhysicalFileInfo(file));
        return Ok();
    }
}

