using System.Security.Authentication;
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
    

    [HttpGet("{id}")]
    public IActionResult GetByIdWithShort(string id,bool? min)
    {
        var token = HttpContext.Request.Headers.Authorization.ToString();
        if (token.Equals(""))
            return Ok(_userService.GetAll());
        var task = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        task.Wait();
        if (task.Result.Uid != id)
            throw new AuthenticationException("You try to get user with another id");
        var user = _userService.GetById(task.Result.Uid);
        return min != null && (bool)min ? Ok($"{user.Id + user.NickName}") : Ok(user);
    }
    
    [HttpPost]
    public IActionResult Create(CreateUserRequest model)
    {
        var token = HttpContext.Request.Headers.Authorization.ToString();
        var user = _userService.Create(model, token);
        return Ok(user);
    }

    [HttpPut]
    public IActionResult Update(string token, UpdateUserRequest model)
    {
        var task = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
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
        var user = _userService.UpdateByUser(token, model);
        return Ok(user);
    }
}