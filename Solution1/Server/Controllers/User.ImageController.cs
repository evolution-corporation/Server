using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("/user.image")]
public class UserImage: ControllerBase
{
    private IUserImageService _service;
    
    public UserImage(IUserImageService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetUserImage()
    {
        var token = HttpContext.Request.Headers.Authorization.ToString();
        return Ok(_service.GetUserImage(token));
    }
}


