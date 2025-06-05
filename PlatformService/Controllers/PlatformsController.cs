using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataService;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataService.Http;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController(
    IPlatformRepository repository,
    IMapper mapper,
    ICommandDataClient commandDataClient,
    IMessageBusClient messageBusClient)
    : ControllerBase
{
    private readonly ICommandDataClient _commandDataClient = commandDataClient;
    private readonly IMapper _mapper = mapper;
    private readonly IMessageBusClient _messageBusClient = messageBusClient;
    private readonly IPlatformRepository _repository = repository;

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms()
    {
        Console.WriteLine("--> Get all platforms");
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
        //Send Sync Message
        try
        {
            await _commandDataClient.SendPlatformToCommand(platformReadDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Exception thrown - {ex.Message}");
        }

        //Send Async Message
        try
        {
            var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
            platformPublishedDto.Event = "PlatformPublished";
            await _messageBusClient.PublishNewPlatform(platformPublishedDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not send asynchronously: - {e.Message}");
        }

        return CreatedAtRoute(nameof(GetPlatformById), new { platformReadDto.Id }, platformReadDto);
    }
}