using System.Text;
using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataService;

public class MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor) : BackgroundService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IEventProcessor _eventProcessor = eventProcessor;
    private readonly string _queueName = "CommandsServiceQueue";
    private IChannel _channel;
    private IConnection _connection;

    private async Task InitializeRabbitMq()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQHost"],
            Port = Convert.ToInt32(_configuration["RabbitMQPort"])
        };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        await _channel.ExchangeDeclareAsync("trigger", ExchangeType.Fanout);
        await _channel.QueueDeclareAsync(
            queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        await _channel.QueueBindAsync(_queueName, "trigger", "");
        Console.WriteLine("--> Listening for messages");
        _connection.ConnectionShutdownAsync += async (sender, @event) => Console.WriteLine("--> Connection Shutdown");
    }

    public override void Dispose()
    {
        if (_channel?.IsOpen ?? false)
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }

        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitializeRabbitMq();
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, @event) =>
        {
            Console.WriteLine("--> Received event");
            var body = @event.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
            _eventProcessor.ProcessEvent(notificationMessage);
        };

        await _channel.BasicConsumeAsync(_queueName, true, consumer);
    }
}