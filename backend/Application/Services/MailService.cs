using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Domain.ViewModels;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string receptor, string subject, string body)
        {
            var email = _configuration["EMAIL_CONFIGURATION:EMAIL"];
            var password = _configuration["EMAIL_CONFIGURATION:PASSWORD"];
            var host = _configuration["EMAIL_CONFIGURATION:HOST"];
            var port = int.Parse(_configuration["EMAIL_CONFIGURATION:PORT"]);

            var client = new SmtpClient(host, port);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(email, password);

            var message = new MailMessage(email, receptor, subject, body)
            {
                IsBodyHtml = true  // Make sure to send as HTML email
            };

            await client.SendMailAsync(message);
        }
        public async Task SendEmailAsync(EmailData emailConfig)
        {
            // Validate configuration early
            var email = _configuration["EMAIL_CONFIGURATION:EMAIL"];
            var password = _configuration["EMAIL_CONFIGURATION:PASSWORD"];
            var host = _configuration["EMAIL_CONFIGURATION:HOST"];
            var portStr = _configuration["EMAIL_CONFIGURATION:PORT"];
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(portStr))
                throw new InvalidOperationException("EMAIL_CONFIGURATION values are missing.");

            if (!int.TryParse(portStr, out var port))
                throw new InvalidOperationException("EMAIL_CONFIGURATION:PORT is not a valid integer.");

            try
            {
                using var client = new SmtpClient(host, port)
                {
                    EnableSsl = true, // see note below about ports
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(email, password)
                };

                using var message = new MailMessage(email, emailConfig.Receptor)
                {
                    Subject = emailConfig.Subject,
                    Body = emailConfig.Body,
                    IsBodyHtml = true
                };

                // Optional encodings (fixes some providers complaining about charset)
                message.SubjectEncoding = System.Text.Encoding.UTF8;
                message.BodyEncoding = System.Text.Encoding.UTF8;

                await client.SendMailAsync(message);
            }
            catch (Exception)
            {
                // Re-throw so controller can log & return the message in DEBUG
                throw;
            }
        }

    }
}
