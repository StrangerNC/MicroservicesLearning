using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("/api/c/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    [HttpPost]
    public ActionResult Test()
    {
        Console.WriteLine("--> Inbound POST # CommandsService");
        return Ok("Inbound POST # CommandsService");
    }
}