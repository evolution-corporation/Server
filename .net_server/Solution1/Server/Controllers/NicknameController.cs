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
    public IActionResult GetUserByNickname(string nickname, bool is_minimum_data, bool is_strong)
    {
        var user = _service.GetUserByNickname(nickname);
        return is_minimum_data ? Ok($"{user.NickName + user.Birthday + user.Category}") : Ok(user);
    }
}