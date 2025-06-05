using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlatformService.AsyncDataService;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _configuration;
    private IChannel _channel;
    private IConnection _connection;

    public MessageBusClient(IConfiguration configuration)
    {
        _configuration = configuration;
        _ = ConfigureRabbit();
    }

    public async Task PublishNewPlatform(PlatformPublishedDto platform)
    {
        var message = JsonSerializer.Serialize(platform);

        if (_connection.IsOpen)
        {
            Console.WriteLine("--> RabbitMQ connection is open, sending message");
            await SendMessage(message);
        }
        else
        {
            Console.WriteLine("--> RabbitMQ connection is not open, not sending message");
        }
    }

    private async Task ConfigureRabbit()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQHost"] ?? string.Empty,
            Port = Convert.ToInt32(_configuration["RabbitMQPort"])
        };
        try
        {
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.ExchangeDeclareAsync("trigger", ExchangeType.Fanout);
            _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;
            Console.WriteLine("--> Connected to RabbitMQ MessageBus");
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> RabbitMQ connection could not be created, ex: {e.Message}");
        }
    }

    private Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs @event)
    {
        Console.WriteLine("--> RabbitMQ connection shutdown");
        return Task.CompletedTask;
    }

    private async Task SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        await _channel.BasicPublishAsync("trigger", "", body);
        Console.WriteLine($"--> RabbitMQ message sent {message}");
    }

    public void Dispose()
    {
        Console.WriteLine("--> RabbitMQ dispose");
        if (_connection.IsOpen)
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}