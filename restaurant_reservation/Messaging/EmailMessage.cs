namespace restaurant_reservation_api.Messaging
{
    /// <summary>
    /// RabbitMQ ile gönderilecek email mesaj modeli
    /// </summary>
    public class EmailMessage
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string MessageId { get; set; } = Guid.NewGuid().ToString();
    }
}
