using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("nickname")]
public class NicknameController: ControllerBase
{
    private INicknameService _service;

    public NicknameController(INicknameService service) => _service = service;

    [HttpGet]
    public IActionResult GetUserByNickname(string nickname, bool isMinimumData)
    {
        var user = _service.GetUserByNickname(nickname);
        if (user == null)
            return NotFound();
        return isMinimumData ? Ok($"{user.NickName + user.Birthday + user.Category}") : Ok(user);
    }
}