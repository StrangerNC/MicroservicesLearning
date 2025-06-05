using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/c/platforms/{platformId:int}/[controller]")]
[ApiController]
public class CommandsController(ICommandRepository repository, IMapper mapper) : ControllerBase
{
    private readonly IMapper _mapper = mapper;
    private readonly ICommandRepository _repository = repository;

    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
    {
        Console.WriteLine($"--> Getting commands for platform {platformId}");
        if (!_repository.PlatformExists(platformId)) return NotFound();

        var commands = _repository.GetCommandsForPlatform(platformId);
        return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
    }

    [HttpGet("{commandId:int}", Name = nameof(GetCommandForPlatform))]
    public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
    {
        Console.WriteLine($"--> Getting command {commandId} for platform {platformId}");
        if (!_repository.PlatformExists(platformId)) return NotFound();

        var command = _repository.GetCommand(platformId, commandId);
        if (command == null) return NotFound();

        return Ok(_mapper.Map<CommandReadDto>(command));
    }

    [HttpPost]
    public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
    {
        Console.WriteLine($"--> Create command for platform {platformId}");
        if (!_repository.PlatformExists(platformId)) return NotFound();

        var command = _mapper.Map<Command>(commandCreateDto);
        _repository.CreateCommand(platformId, command);
        _repository.SaveChanges();
        var commandReadDto = _mapper.Map<CommandReadDto>(command);
        return CreatedAtRoute(nameof(GetCommandForPlatform),
            new { platformId, commandId = commandReadDto.Id }, commandReadDto);
    }
}