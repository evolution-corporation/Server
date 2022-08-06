using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("/dmd")]
public class DmdController: ControllerBase
{
    private readonly IDmdService service;

    public DmdController(IDmdService service)
    {
        this.service = service;
    }

    [HttpGet]
    public IActionResult GetAllDmd()
    {
        return Ok(service.GetAllDmd());
    }

    [HttpGet]
    public IActionResult GetDMD(int id)
    {
        return Ok(service.GetDmd(id));
    }

    [HttpPost]
    public IActionResult PostDmd(int[] ids, string name)
    {
        service.PostDmd(ids.ToList(),name);
        return Ok();
    }
}