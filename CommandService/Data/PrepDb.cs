using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data;

public class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var grpcClient = scope.ServiceProvider.GetRequiredService<IPlatformDataClient>();
        var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
        var platforms = grpcClient.ReturnAllPlatforms();
        SeedData(repository, platforms);
    }

    private static void SeedData(ICommandRepository repository, IEnumerable<Platform> platforms)
    {
        Console.WriteLine("--> Seeding new platforms...");
        foreach (var platform in platforms)
        {
            if (!repository.ExternalPlatformExists(platform.ExternalId))
                repository.CreatePlatform(platform);
        }

        repository.SaveChanges();
    }
}