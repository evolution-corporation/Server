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

    [HttpGet]
    public IActionResult GetMeditation(string language,
        MeditationPreferences? preferences,
        bool? getIsNotListened = false,
        bool? popularToday = false,
        int? meditationId = 0)
    {
        var token = HttpContext.Request.Headers.Authorization.ToString();
        if (meditationId != null)
            return Ok(service.GetById((int)meditationId, token));
        if (preferences != null)
            return Ok(service.GetMeditationByPreferences(preferences));
        if (getIsNotListened != null && (bool)!getIsNotListened)
            return popularToday != null && (bool)popularToday
                ? Ok(service.GetPopular(language))
                : Ok(service.GetAllMeditation(language));
        if (token is null)
            throw new UnauthorizedAccessException();
        return Ok(service.GetNotListened(token, language));
    }

    [HttpGet("/meditation.count")]
    public IActionResult GetCountOfMeditation(MeditationPreferences preferences) =>
        Ok(service.GetCountOfMeditation(preferences));

    [HttpPost]
    public IActionResult AddMeditation(CreateMeditationRequest model)
    {
        service.Create(model);
        return Ok(new { message = "Meditation created" });
    }

    [HttpPatch]
    public IActionResult UpdateMeditation(int id, UpdateMeditationRequest model)
    {
        var token = HttpContext.Request.Headers.Authorization.ToString();
        service.Update(model, id, token);
        return Ok(new { message = "Meditation updated" });
    }
}