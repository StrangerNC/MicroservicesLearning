using PlatformService.Models;

namespace PlatformService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        SeedData(scope.ServiceProvider.GetRequiredService<AppDbContext>());
    }

    private static void SeedData(AppDbContext context)
    {
        if (!context.Platforms.Any())
        {
            Console.WriteLine("-->Seeding data...");

            context.Platforms.AddRange(
                new Platform()
                {
                    Name = "Dotnet Core",
                    Publisher = "Microsoft",
                    Cost = "Free"
                },
                new Platform()
                {
                    Name = "Sql Server Express",
                    Publisher = "Microsoft",
                    Cost = "Free"
                },
                new Platform()
                {
                    Name = "Kubernetes",
                    Publisher = "Cloud Native Computing Foundation",
                    Cost = "Free"
                }
            );

            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("-->Prep population already seeded");
        }
    }
}