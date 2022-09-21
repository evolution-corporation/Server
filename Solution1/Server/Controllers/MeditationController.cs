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
    public IActionResult GetMeditation(string? type, string? day, string? time,
        int countOfMeditations,
        string? language = "ru",
        bool? getIsNotListened = false,
        int? meditationId = null,
        bool? popularToday = false)
    {
        MeditationPreferences? preferences = null;
        if (type != null || day != null || time != null)
            preferences = new MeditationPreferences
            {
                TypeMeditation = TypeMeditationConverter.Convert(type),
                CountDay = CountDayMeditationConverter.Convert(day),
                Time = TimeMeditationConverter.Convert(time)
            };
        string? token;
        if (meditationId != null)
        {
            token = HttpContext.Request.Headers.Authorization.ToString();
            return Ok(service.GetById((int)meditationId, token));
        }

        if (preferences != null)
            return Ok(service.GetMeditationByPreferences(preferences));
        if (getIsNotListened != null && (bool)!getIsNotListened)
            return popularToday != null && (bool)popularToday
                ? Ok(service.GetPopular(language))
                : Ok(service.GetAllMeditation(language, countOfMeditations));
        token = HttpContext.Request.Headers.Authorization.ToString();
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