using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("/api/c/[controller]")]
[ApiController]
public class PlatformsController(ICommandRepository repository, IMapper mapper) : ControllerBase
{
    private readonly IMapper _mapper = mapper;
    private readonly ICommandRepository _repository = repository;

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        Console.WriteLine("--> Getting all platforms");
        var platformItem = _repository.GetAllPlatforms();
        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItem));
    }

    [HttpPost]
    public ActionResult Test()
    {
        Console.WriteLine("--> Inbound POST # CommandService");
        return Ok("Inbound POST # CommandService");
    }
}