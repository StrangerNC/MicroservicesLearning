using PlatformService.Dtos;

namespace PlatformService.AsyncDataService;

public interface IMessageBusClient
{
    Task PublishNewPlatform(PlatformPublishedDto platform);
}