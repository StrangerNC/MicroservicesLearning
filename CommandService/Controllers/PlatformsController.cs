using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("/api/c/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    [HttpPost]
    public ActionResult Test()
    {
        Console.WriteLine("--> Inbound POST # CommandService");
        return Ok("Inbound POST # CommandService");
    }
}