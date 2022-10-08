using System.Security.Authentication;
using FirebaseAdmin.Auth;

namespace Server.Controllers;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Models.Users;
using Services;

[ApiController]
[Route("/users")]
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
    

    //TODO: Написать получение типа подписки пользователя
    [HttpGet]
    public IActionResult GetByIdWithShort(string? id,bool? min)
    {
        if (id is null)
            return Ok(_userService.GetAll());
        var user = _userService.GetById(id);
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
    public IActionResult Update( UpdateUserRequest model, string userId)
    {
        var token = HttpContext.Request.Headers.Authorization.ToString();
        var task = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
        task.Wait();
        _userService.Update(task.Result.Uid, model, userId);
        return Ok(new { message = "User updated" });
    }

    [HttpPatch]
    public IActionResult UpdateByUser(UpdateUserRequest model)
    {
        var token = HttpContext.Request.Headers.Authorization.ToString(); 
        var user = _userService.UpdateByUser(token, model);
        return Ok(user);
    }
}