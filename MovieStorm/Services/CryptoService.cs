using System;
using System.Text;
using MovieStorm.Models;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;

namespace MovieStorm.Services
{
    public class CryptoService
    {
        private byte[] crypto_key, salt;
        private readonly IConfiguration Configuration;
        private readonly IAccountSettings setup;

        public CryptoService(IConfiguration Configuration, IAccountSettings setup)
        {
            this.Configuration = Configuration;
            crypto_key = Convert.FromBase64String(this.Configuration["AppSettings:secret"]);
            salt = Convert.FromBase64String(this.Configuration["AppSettings:salt"]);
            this.setup = setup;
        }

        public string EncryptString(string data)
        {
            using (var aes = Aes.Create())
            using (var encryptor = aes.CreateEncryptor(crypto_key, salt))
            {
                var plainText = Encoding.UTF8.GetBytes(data);
                byte[] model = encryptor.TransformFinalBlock(plainText, 0, plainText.Length);
                return Convert.ToBase64String(model);
            }
        }

        public void SendEmail(string to, string subject, string body)
        {
            string server = setup.host;
            int port = int.Parse(setup.port);

            string client = setup.client;
            string secret = setup.secret;

            SmtpClient host = new SmtpClient(server, port);
            host.EnableSsl = true;
            host.UseDefaultCredentials = false;
            host.Credentials = new NetworkCredential(client, secret);

            MailMessage mail = new MailMessage();
            string data = $"<html><body><p>{body}</p></body></html>";

            mail.To.Add(to);
            mail.Subject = subject;
            mail.From = new MailAddress(client);
            mail.Body = data;
            mail.IsBodyHtml = true;
            host.Send(mail);
        }

        public string DecryptString(string data)
        {
            var buffer = Convert.FromBase64String(data);

            using (var aes = Aes.Create())
            using (var encryptor = aes.CreateDecryptor(crypto_key, salt))
            {
                var decryptedBytes = encryptor
                    .TransformFinalBlock(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }
}
