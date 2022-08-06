using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("/meditation.image")]
public class MeditationImageController: ControllerBase
{
    private MeditationImageService service;

    public MeditationImageController(MeditationImageService service)
    {
        this.service = service;
    }

    [HttpGet]
    public IActionResult GetMeditationImage(int id)
    {
        return Ok(service.GetMeditationImage(id));
    }
}