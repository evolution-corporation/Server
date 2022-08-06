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

    [HttpGet("{token}")]
    public IActionResult GetUserImage(string token)
    {
        return Ok(_service.GetUserImage(token));
    }
}


