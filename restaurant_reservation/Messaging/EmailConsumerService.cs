using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace restaurant_reservation_api.Messaging
{
    /// <summary>
    /// RabbitMQ'dan mesajları dinleyen ve "email gönderen" background servis.
    /// Gerçek email göndermez, sadece log tutar ve console'a yazar.
    /// </summary>
    public class EmailConsumerService : BackgroundService
    {
        private readonly ILogger<EmailConsumerService> _logger;
        private IConnection? _connection;
        private IChannel? _channel;
        private const string QueueName = "email_notifications";

        public static ConcurrentBag<EmailLog> SentEmails { get; } = new();

        public EmailConsumerService(ILogger<EmailConsumerService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Email Consumer Service başlatılıyor...");

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = "localhost",
                    Port = 5672,
                    UserName = "guest",
                    Password = "guest"
                };

                _connection = await factory.CreateConnectionAsync(stoppingToken);
                _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

                await _channel.QueueDeclareAsync(
                    queue: QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: stoppingToken);

                // tek seferde 1 message gönderme
                await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: stoppingToken);

                _logger.LogInformation("RabbitMQ Consumer bağlantısı kuruldu. Queue dinleniyor: {QueueName}", QueueName);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);

                    try
                    {
                        var emailMessage = JsonSerializer.Deserialize<EmailMessage>(json);

                        if (emailMessage != null)
                        {
                            // "Email gönderme" simülasyonu - 500ms gecikme
                            await Task.Delay(500, stoppingToken);

                            // Email log'u kaydet
                            var emailLog = new EmailLog
                            {
                                MessageId = emailMessage.MessageId,
                                To = emailMessage.To,
                                Subject = emailMessage.Subject,
                                Body = emailMessage.Body,
                                SentAt = DateTime.UtcNow,
                                Status = "Sent"
                            };
                            SentEmails.Add(emailLog);

                            _logger.LogInformation(
                            " EMAIL GÖNDERİLDİ!\n" +
                            "   ├─ To: {To}\n" +
                            "   ├─ Subject: {Subject}\n" +
                            "   ├─ Body: {Body}\n" +
                            "   └─ MessageId: {MessageId}  ",
                            emailMessage.To, emailMessage.Subject, emailMessage.Body, emailMessage.MessageId);

                            await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, stoppingToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Email mesajı işlenirken hata: {Json}", json);

                        await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
                    }
                };

                await _channel.BasicConsumeAsync(
                    queue: QueueName,
                    autoAck: false,
                    consumer: consumer,
                    cancellationToken: stoppingToken);

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Email Consumer Service durduruluyor...");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RabbitMQ Consumer bağlantısı kurulamadı. Consumer devre dışı.");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Email Consumer Service kapatılıyor...");

            if (_channel != null)
                await _channel.CloseAsync(cancellationToken);
            if (_connection != null)
                await _connection.CloseAsync(cancellationToken);

            await base.StopAsync(cancellationToken);
        }
    }
}
