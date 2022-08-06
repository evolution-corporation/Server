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

    [HttpGet("{token}")]
    public IActionResult Get(string token)
    {
        return Ok(service.GetUserByToken(token));
    }
}