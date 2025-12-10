using Microsoft.AspNetCore.Mvc;
using restaurant_reservation_api.Messaging;

namespace restaurant_reservation_api.Controllers
{
    /// <summary>
    /// RabbitMQ üzerinden gönderilen email'lerin loglarýný gösteren controller.
    /// Bu sayede RabbitMQ'nun çalýþtýðýný doðrulayabiliriz.
    /// </summary>
    [Route("api/[controller]")]
 [ApiController]
    public class EmailLogsController : ControllerBase
    {
 private readonly IRabbitMQPublisher _publisher;
        private readonly ILogger<EmailLogsController> _logger;

        public EmailLogsController(IRabbitMQPublisher publisher, ILogger<EmailLogsController> logger)
     {
      _publisher = publisher;
            _logger = logger;
        }

  /// <summary>
        /// RabbitMQ Consumer tarafýndan iþlenen tüm email loglarýný döner.
        /// Bu endpoint sayesinde RabbitMQ'nun çalýþýp çalýþmadýðýný görebilirsiniz.
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<EmailLog>> GetEmailLogs()
  {
     var logs = EmailConsumerService.SentEmails
         .OrderByDescending(e => e.SentAt)
   .ToList();

         return Ok(new
            {
    TotalEmailsSent = logs.Count,
     Message = logs.Count > 0 
          ? "? RabbitMQ çalýþýyor! Aþaðýda gönderilen email loglarý var." 
            : "?? Henüz email gönderilmedi. Bir rezervasyon oluþturun.",
                Emails = logs
            });
        }

    /// <summary>
        /// Test amaçlý manuel email gönderir.
        /// RabbitMQ'nun çalýþýp çalýþmadýðýný test etmek için kullanabilirsiniz.
        /// </summary>
        [HttpPost("test")]
      public async Task<ActionResult> SendTestEmail([FromBody] TestEmailRequest? request)
    {
            var emailMessage = new EmailMessage
    {
              To = request?.Email ?? "test@example.com",
         Subject = "?? Test Email - RabbitMQ Çalýþýyor!",
        Body = $"Bu bir test emailidir. Gönderilme zamaný: {DateTime.Now:dd.MM.yyyy HH:mm:ss}"
            };

            await _publisher.PublishEmailAsync(emailMessage);

 return Ok(new
    {
                Message = "? Test email mesajý RabbitMQ queue'suna gönderildi!",
         Note = "Birkaç saniye içinde /api/emaillogs endpoint'inden email logunu görebilirsiniz.",
 EmailMessage = emailMessage
            });
        }

        /// <summary>
        /// Email loglarýný temizler
      /// </summary>
    [HttpDelete]
        public ActionResult ClearLogs()
        {
          EmailConsumerService.SentEmails.Clear();
            return Ok(new { Message = "? Email loglarý temizlendi." });
      }
    }

    public class TestEmailRequest
    {
        public string? Email { get; set; }
    }
}
