using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace TMS.HelperFunctions
{
    public class Helpers
    {
        private readonly IConfiguration _configuration;

        public Helpers(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string template, string username, string userEmail, string? message, string subject)
        {
            string FilePath = Directory.GetCurrentDirectory() + $"\\Templates\\{template}.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();
            MailText = MailText.Replace("[Message]", message).Replace("[User]", username);


            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("Mail").Value));
            email.To.Add(MailboxAddress.Parse(userEmail));
            email.Subject = subject;
            var builder = new BodyBuilder();

            builder.HtmlBody = MailText;
            email.Body = builder.ToMessageBody();
            //email.Body = new TextPart(TextFormat.Html) { Text = mailRequest.Body };
            using var smtp = new SmtpClient();

            string secureSocketOptionsString = _configuration.GetSection("SecureSocketOptions").Value;
            SecureSocketOptions secureSocketOptions = Enum.Parse<SecureSocketOptions>(secureSocketOptionsString);

            //smtp.Connect(_configuration.GetSection("Host").Value, int.Parse(_configuration.GetSection("Port").Value), SecureSocketOptions.Auto);
            //smtp.Connect(_configuration.GetSection("Host").Value, int.Parse(_configuration.GetSection("Port").Value), SecureSocketOptions.SslOnConnect);

            smtp.Connect(_configuration.GetSection("Host").Value, int.Parse(_configuration.GetSection("Port").Value), secureSocketOptions);
            smtp.Authenticate(_configuration.GetSection("Mail").Value, _configuration.GetSection("Password").Value);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
            smtp.Dispose();
        }

        public string GetRandomID()
        {
            Random random = new Random();
            string randomNumber = random.Next(100000, 999999).ToString();
            return randomNumber;
        }
    }
}
