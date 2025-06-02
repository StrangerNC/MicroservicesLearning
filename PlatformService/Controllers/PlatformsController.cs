using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataService.Http;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController(IPlatformRepository repository, IMapper mapper, ICommandDataClient commandDataClient)
    : ControllerBase
{
    private readonly IPlatformRepository _repository = repository;
    private readonly IMapper _mapper = mapper;
    private readonly ICommandDataClient _commandDataClient = commandDataClient;

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms()
    {
        Console.WriteLine("--> Get all platforms...");
        var platformItem = _repository.GetAllPlatforms();
        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItem));
    }

    [HttpGet("{id:int}", Name = nameof(GetPlatformById))]
    public ActionResult<PlatformReadDto> GetPlatformById(int id)
    {
        Console.WriteLine($"--> Get platform by id - {id}...");
        var platformItem = _repository.GetPlatformById(id);
        if (platformItem != null)
            return Ok(_mapper.Map<PlatformReadDto>(platformItem));
        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto dto)
    {
        Console.WriteLine($"--> Create platform Name - {dto.Name} Publisher - {dto.Publisher} Cost - {dto.Cost}...");
        var platformModel = _mapper.Map<Platform>(dto);
        _repository.CreatePlatform(platformModel);
        _repository.SaveChanges();
        var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);
        try
        {
            await _commandDataClient.SendPlatformToCommand(platformReadDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine("--> Exception thrown - {ex.Message}");
        }

        return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
    }
}