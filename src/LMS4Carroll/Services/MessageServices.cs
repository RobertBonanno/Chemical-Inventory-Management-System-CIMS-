using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using System.Threading.Tasks;
using LMS4Carroll.Configuration;
using Microsoft.Extensions.Options;
using Google.Apis.Auth.OAuth2;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace LMS4Carroll.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly EmailSettings _emailSettings;

        public AuthMessageSender(IOptions<EmailSettings> emailOptions)
        {
            _emailSettings = emailOptions.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Carroll LMS", "testlms4carroll@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = message };

            /*
            var certificate = new X509Certificate2(@"~/Certificates/LMS Carroll-cbbd77b9b3f8.p12", "notasecret", X509KeyStorageFlags.Exportable);
            var credential = new ServiceAccountCredential(new ServiceAccountCredential
                .Initializer("devsgill@lms-carroll.iam.gserviceaccount.com")
            {
                // Note: other scopes can be found here: https://developers.google.com/gmail/api/auth/scopes
                Scopes = new[] { "https://mail.google.com/" },
                User = "testlms4carroll@gmail.com"
            }.FromCertificate(certificate));

            bool result = await credential.RequestAccessTokenAsync(CancellationToken.None);
            */
            using (var client = new SmtpClient())
            {
                //client.Connect("smtp.gmail.com", 587, SecureSocketOptions.SslOnConnect);
                //client.Connect("smtp.gmail.com", 587, false);
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.Auto).ConfigureAwait(false);
                // disable OAuth2 authentication unless you are actually using an access_token
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate("testlms4carroll", "Carroll2016");
                //client.Authenticate("testlms4carroll", credential.Token.AccessToken);
                await client.SendAsync(emailMessage).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }

      
    }
}
