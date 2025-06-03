using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.SyncDataService.Http;

namespace PlatformService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddHttpClient<ICommandDataClient, CommandDataClient>();
        Console.WriteLine($"--> CommandService Endpoint {builder.Configuration["CommandService"]}");
        builder.Services.AddControllers();
        // Configure the HTTP request pipeline.
        if (builder.Environment.IsProduction())
        {
            Console.WriteLine("--> Production environment");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("PlatformConn")));
        }
        else
        {
            Console.WriteLine("--> Development environment");
            // app.MapOpenApi();
            builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMemoryDatabase"));
        }

        var app = builder.Build();

        PrepDb.PrepPopulation(app, app.Environment.IsProduction());

        // app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}