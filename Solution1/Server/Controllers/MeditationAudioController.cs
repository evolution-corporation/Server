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

    [HttpGet("{meditationId:Guid}")]
    public IActionResult GetAudioUrl(Guid meditationId)
    {
        var token = HttpContext.Request.Headers.Authorization.ToString();
        return Ok(service.GetMeditationAudioUrl(meditationId,token));
    }
}