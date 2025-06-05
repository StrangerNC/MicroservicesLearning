using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing;

public class EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper) : IEventProcessor
{
    private readonly IMapper _mapper = mapper;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);
        switch (eventType)
        {
            case EventType.PlatformPublished:
                addPlatform(message);
                break;
        }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine("--> Determining event");
        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
        switch (eventType.Event)
        {
            case "PlatformPublished":
                Console.WriteLine("--> Platform published event detected");
                return EventType.PlatformPublished;
            default:
                Console.WriteLine("--> Unknown event");
                return EventType.Undetermined;
        }
    }

    private void addPlatform(string platformPublishedMessage)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
        var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
        try
        {
            var platform = _mapper.Map<Platform>(platformPublishedDto);
            if (!repo.ExternalPlatformExists(platform.ExternalId))
            {
                repo.CreatePlatform(platform);
                repo.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> Platform already exists");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not add Platform to DB {e}");
        }
    }
}

internal enum EventType
{
    PlatformPublished,
    Undetermined
}