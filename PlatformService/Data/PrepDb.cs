using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
    {
        using var scope = app.ApplicationServices.CreateScope();
        SeedData(scope.ServiceProvider.GetRequiredService<AppDbContext>(), isProduction);
    }

    private static void SeedData(AppDbContext context, bool isProduction)
    {
        if (isProduction)
        {
            Console.WriteLine("--> Attempting to apply migrations...");
            try
            {
                context.Database.Migrate();
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Migration failed: {e.Message}");
            }
        }

        if (!context.Platforms.Any())
        {
            Console.WriteLine("--> Seeding data...");

            context.Platforms.AddRange(
                new Platform
                {
                    Name = "Dotnet Core",
                    Publisher = "Microsoft",
                    Cost = "Free"
                },
                new Platform
                {
                    Name = "Sql Server Express",
                    Publisher = "Microsoft",
                    Cost = "Free"
                },
                new Platform
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