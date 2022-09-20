using Microsoft.AspNetCore.Mvc;
using Server.Entities;
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
    public IActionResult GetMeditation(TypeMeditation? type, CountDayMeditation? day, TimeMeditation? time,
        int countOfMeditations,
        string? language = "ru",
        bool? getIsNotListened = false,
        int? meditationId = null,
        bool? popularToday = false)
    {
        MeditationPreferences? preferences = null;
        if (type != null || day != null || time != null)
            preferences = new MeditationPreferences { TypeMeditation = type, CountDay = day, Time = time };
        var token = HttpContext.Request.Headers.Authorization.ToString();
        if (meditationId != null)
            return Ok(service.GetById((int)meditationId, token));
        if (preferences != null)
            return Ok(service.GetMeditationByPreferences(preferences));
        if (getIsNotListened != null && (bool)!getIsNotListened)
            return popularToday != null && (bool)popularToday
                ? Ok(service.GetPopular(language))
                : Ok(service.GetAllMeditation(language, countOfMeditations));
        if (token is null)
            throw new UnauthorizedAccessException();
        return Ok(service.GetNotListened(token, language));
    }

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