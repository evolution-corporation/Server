using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("/authentication")]
public class AuthenticationController: ControllerBase
{
    private IAuthenticationService service;
    public AuthenticationController(IAuthenticationService service)
    {
        this.service = service;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var token = HttpContext.Request.Headers.Authorization.ToString();   
        return Ok(service.GetUserByToken(token));
    }
}