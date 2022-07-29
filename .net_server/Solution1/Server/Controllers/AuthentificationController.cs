using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers;

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