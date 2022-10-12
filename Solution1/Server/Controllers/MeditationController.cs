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
        Guid? meditationId = null,
        bool? popularToday = false)
    {
        MeditationPreferences? preferences;
        if (type != null || day != null || time != null)
        {
            preferences = new MeditationPreferences
            {
                TypeMeditation = TypeMeditationConverter.Convert(type),
                Time = TimeMeditationConverter.Convert(time)
            };
            return Ok(service.GetMeditationByPreferences(preferences));
        }
        string? token;
        if (meditationId != null)
        {
            token = HttpContext.Request.Headers.Authorization.ToString();
            return Ok(service.GetById((Guid)meditationId, token));
        }

        if (getIsNotListened != null && (bool)!getIsNotListened)
            return popularToday != null && (bool)popularToday
                ? Ok(service.GetPopular(language))
                : Ok(service.GetAllMeditation(language, countOfMeditations));
        token = HttpContext.Request.Headers.Authorization.ToString();
        return Ok(service.GetNotListened(token, language));
    }
    
    [HttpPut]
    public IActionResult AddMeditation(Guid meditationId)
    {
        var token = HttpContext.Request.Headers.Authorization.ToString();
        service.UserListened(token, meditationId);
        return Ok();
    }

    // [HttpPost]
    // public IActionResult AddMeditation(CreateMeditationRequest model)
    // {
    //     var token = HttpContext.Request.Headers.Authorization.ToString();
    //     service.Create(model, token);
    //     return Ok(new { message = "Meditation created" });
    //}

    // [HttpPatch]
    // public IActionResult UpdateMeditation(Guid id, UpdateMeditationRequest model)
    // {
    //     var token = HttpContext.Request.Headers.Authorization.ToString();
    //     service.Update(model, id, token);
    //     return Ok(new { message = "Meditation updated" });
    // }
}