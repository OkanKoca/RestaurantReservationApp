using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace restaurant_reservation_api.Messaging
{
    /// <summary>
    /// RabbitMQ'ya mesaj gönderen publisher servisi
    /// </summary>
    public interface IRabbitMQPublisher
    {
        Task PublishEmailAsync(EmailMessage message);
    }

    public class RabbitMQPublisher : IRabbitMQPublisher, IAsyncDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly ILogger<RabbitMQPublisher> _logger;
        private const string QueueName = "email_notifications";
        private bool _initialized = false;

        public RabbitMQPublisher(ILogger<RabbitMQPublisher> logger)
        {
            _logger = logger;
        
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            try
            {
                _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
                _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

                _channel.QueueDeclareAsync(
                    queue: QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null)
                .GetAwaiter().GetResult();

                _initialized = true;
                _logger.LogInformation("RabbitMQ Publisher baþarýyla baþlatýldý. Queue: {QueueName}", QueueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RabbitMQ baðlantýsý kurulamadý. Publisher devre dýþý.");
                _initialized = false;
            }
        }

        public async Task PublishEmailAsync(EmailMessage message)
        {
            if (!_initialized)
            {
                _logger.LogWarning("RabbitMQ baðlantýsý yok. Mesaj gönderilemedi: {Subject}", message.Subject);
                return;
            }

            try
            {
                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                var properties = new BasicProperties
                {
                    Persistent = true,
                    ContentType = "application/json"
                };

                await _channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: QueueName,
                    mandatory: false,
                    basicProperties: properties,
                    body: body
                );

                _logger.LogInformation("Email mesajý queue'ya gönderildi. To: {To}, Subject: {Subject}, MessageId: {MessageId}",
                message.To, message.Subject, message.MessageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mesaj gönderilirken hata oluþtu: {Subject}", message.Subject);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
                await _channel.CloseAsync();
            if (_connection != null)
                await _connection.CloseAsync();
        }
    }
}
