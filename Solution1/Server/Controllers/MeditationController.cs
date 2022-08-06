using Microsoft.AspNetCore.Mvc;
using Server.Models.Meditation;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("/meditation")]
public class MeditationController : ControllerBase
{
    private readonly IMeditationService service;

    public MeditationController(IMeditationService service)
    {
        this.service = service;
    }

    [HttpGet("{token} {language}")]
    public IActionResult GetMeditation(string language,
        MeditationPreferences? preferences,
        bool getIsNotListened = false,
        bool popularToday = false,
        int meditationId = 0,
        string? token = null)
    {
        if (meditationId != 0)
            return Ok(service.GetById(meditationId, token));
        if (preferences != null)
            return Ok(service.GetMeditationByPreferences(preferences));
        if (!getIsNotListened)
            return popularToday ? Ok(service.GetPopular(language)) : Ok(service.GetAllMeditation(language));
        if (token is null) throw new UnauthorizedAccessException();
        return Ok(service.GetNotListened(token, language));
    }

    [HttpPost("{token}")]
    public IActionResult AddMeditation(CreateMeditationRequest model, string token)
    {
        service.Create(model, token);
        return Ok(new { message = "Meditation created" });
    }

    [HttpPatch("{token}")]
    public IActionResult UpdateMeditation(int id, UpdateMeditationRequest model, string token)
    {
        service.Update(model, id, token);
        return Ok(new { message = "Meditation updated" });
    }
}