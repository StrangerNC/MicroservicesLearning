using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataService;
using PlatformService.Data;
using PlatformService.SyncDataService.Grpc;
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
        builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
        builder.Services.AddHttpClient<ICommandDataClient, CommandDataClient>();
        builder.Services.AddGrpc();
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

        Console.WriteLine("HELLO");

        PrepDb.PrepPopulation(app, app.Environment.IsProduction());

        // app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        app.MapGrpcService<GrpcPlatformService>();
        app.MapGet("/protos/platform.proto",
            async context => { await context.Response.WriteAsync(File.ReadAllText("Protos/platform.proto")); });

        app.Run();
    }
}