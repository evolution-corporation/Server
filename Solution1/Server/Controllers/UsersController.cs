using FirebaseAdmin.Auth;

namespace Server.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Models.Users;
using Services;

[ApiController]
[Route("/Users")]
public class UsersController : ControllerBase
{
    private IUserService _userService;
    private IMapper _mapper;

    public UsersController(
        IUserService userService,
        IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }
    

    [HttpGet]
    public IActionResult GetByIdWithShort(bool? min)
    {
        var token = HttpContext.Request.Headers.Authorization.ToString();
        if (token.Equals(""))
            return Ok(_userService.GetAll());
        var id = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        id.Wait();
        var user = _userService.GetById(id.Result.Uid);
        return min != null && (bool)min ? Ok($"{user.Id + user.NickName}") : Ok(user);
    }

    [HttpPost]
    public IActionResult Create(CreateUserRequest model)
    {
        var token = HttpContext.Request.Headers.Authorization.ToString();
        _userService.Create(model, token);
        return Ok(new { message = "User created" });
    }

    [HttpPut]
    public IActionResult Update(string id, UpdateUserRequest model)
    {
        var task = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(id);
        task.Wait();
        _userService.Update(task.Result.Uid, model);
        return Ok(new { message = "User updated" });
    }

    [HttpPut]
    public IActionResult AddMeditation(int meditationId)
    {
        var token = HttpContext.Request.Headers.Authorization.ToString();
        _userService.UserListened(token, meditationId);
        return Ok();
    }

    [HttpPatch]
    public IActionResult UpdateByUser(UpdateUserRequest model)
    {
        var token = HttpContext.Request.Headers.Authorization.ToString();
        _userService.UpdateByUser(token, model);
        return Ok(new { message = "User updated" });
    }
}