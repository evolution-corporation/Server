using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers;

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
    public IActionResult GetUserImage(string id)
    {
        return Ok(_service.GetUserImage(new Guid(id)));
    }
}


