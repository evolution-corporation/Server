using System.Security.Authentication;
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
    public IActionResult GetUserImage(string id)
    {
        var token = HttpContext.Request.Headers.Authorization.ToString();
        if (token != id)
            throw new AuthenticationException("Попытка получить фотографию пользователя без токена");
        return Ok(_service.GetUserImage(id));
    }

    [HttpDelete]
    public IActionResult DeleteUserImage()
    {
        var token = HttpContext.Request.Headers.Authorization.ToString();
        _service.DeleteUserImage(token);
        return Ok();
    }
}


