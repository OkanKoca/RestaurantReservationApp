namespace restaurant_reservation_api.Messaging
{
    /// <summary>
    /// Gönderilen email'lerin logunu tutan model
    /// </summary>
    public class EmailLog
    {
        public string MessageId { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public string Status { get; set; } = "Sent"; // Sent, Failed
    }
}
