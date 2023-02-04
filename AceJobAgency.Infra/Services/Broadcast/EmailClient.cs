using AceJobAgency.Core.Interfaces.Utility;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace AceJobAgency.Infra.Services.Broadcast
{
    public class EmailClient : IEmailClient 
    {
        private readonly IConfiguration _config;

        public EmailClient(IConfiguration config)
        {
            _config = config;
        }
        public bool SendMail(string toEmail, 
            string firstName,
            string body,
            string subject)
        {
            var message = new MimeMessage();
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;
            message.Body = bodyBuilder.ToMessageBody();
            var to = new MailboxAddress(firstName, toEmail);
            message.To.Add(to);
            message.From.Add(new MailboxAddress(_config.GetSection("MailKit:From:Name").Value, _config.GetSection("MailKit:Username").Value));
            if (!string.IsNullOrEmpty(subject))
            {
                message.Subject = subject;
            }
            SendEmail(message);
            return true;
        }

        private void SendEmail(MimeMessage mimeMessage)
        {
            try
            {
                using (var client = new SmtpClient())
                {

                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect("smtp.gmail.com", 587, false);
                    client.Authenticate(_config.GetSection("MailKit:Username").Value, _config.GetSection("MailKit:Password").Value);
                    client.Send(mimeMessage);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


    }
}
