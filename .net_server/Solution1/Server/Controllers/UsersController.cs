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

    [HttpGet]
    public IActionResult GetByIdWithShort(bool min = false, string id = "")
    {
        if (id.Equals(string.Empty))
            return Ok(_userService.GetAll());
        var user = _userService.GetById(new Guid(id));
        return min ? Ok($"{user.Id + user.NickName}") : Ok(user);
    }

    [HttpPost("{token}")]
    public IActionResult Create(CreateUserRequest model, string token)
    {
        _userService.Create(model, token);
        Console.Write("Have request");
        return Ok(new { message = "User created" });
    }

    [HttpPut]
    public IActionResult Update(string id, UpdateUserRequest model)
    {
        _userService.Update(new Guid(id), model);
        return Ok(new { message = "User updated" });
    }

    [HttpPatch("{token}")]
    public IActionResult UpdateByUser(string token, UpdateUserRequest model)
    {
        _userService.UpdateByUser(token, model);
        return Ok(new { message = "User updated" });
    }
    
}