using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Website_forum.Models
{
    public class EmailMessage
    {
        public string Destination { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class EmailService
    {
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;

        public EmailService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task SendAsync(EmailMessage message)
        {
            // Sender must be the email address you verified in Brevo (Senders & IP).
            var senderEmail = configuration["Smtp:User"];
            var apiKey = configuration["Brevo:ApiKey"];

            var payload = new
            {
                sender = new { email = senderEmail },
                to = new[] { new { email = message.Destination } },
                subject = message.Subject,
                htmlContent = message.Body
            };

            var json = JsonSerializer.Serialize(payload);

            using var client = httpClientFactory.CreateClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");
            request.Headers.Add("api-key", apiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Brevo send failed ({response.StatusCode}): {errorBody}");
            }
        }
    }
}
