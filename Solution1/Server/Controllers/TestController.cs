using Microsoft.AspNetCore.Mvc;

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

