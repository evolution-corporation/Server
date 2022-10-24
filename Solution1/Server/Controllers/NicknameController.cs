using System.Net;
using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("/nickname")]
public class NicknameController : ControllerBase
{
    private INicknameService _service;

    public NicknameController(INicknameService service) => _service = service;

    [HttpGet]
    public IActionResult GetUserByNickname(string nickname)
    {
        var user = _service.GetUserByNickname(nickname);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    [HttpPost]
    public IActionResult Book()
    {
        var xyu = HttpContext.Request.Body;
        var str = new StreamReader(xyu);
        var task = str.ReadLineAsync();
        task.Wait();
        var x = task.Result;
        var token = HttpContext.Request.Headers.Authorization.ToString();
        if (x == null)
            throw new NullReferenceException();
        _service.NicknameBooking(x,token);
        return Ok();
    }
    
    //[HttpPost]
    public IActionResult NicknameBooking(string nickname)
    {
        var token = HttpContext.Request.Headers.Authorization.ToString();
        _service.NicknameBooking(nickname,token);
        return Ok();
    }
}