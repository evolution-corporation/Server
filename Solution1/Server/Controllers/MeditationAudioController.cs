using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("/meditation.audio")]
public class MeditationAudioController: ControllerBase
{
    private readonly IMeditationAudioService service;

    public MeditationAudioController(IMeditationAudioService service)
    {
        this.service = service;
    }

    [HttpGet("{meditationId:int}")]
    public IActionResult GetAudioUrl(int meditationId)
    {
        var token = HttpContext.Request.Headers.Authorization.ToString();
        return Ok(service.GetMeditationAudioUrl(meditationId,token));
    }
}