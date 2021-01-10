using System;
using System.IO;
using System.Text;
using MovieStorm.Models;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Net.Mail;
using MovieStorm.Data;
using System.Net;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MovieStorm.Services
{
    public class AccountService
    {
        private StormContext db;
        private byte[] crypto_key, salt;
        private readonly IConfiguration Configuration;
        private readonly IAccountSettings setup;

        public AccountService(IConfiguration Configuration, IAccountSettings setup)
        {
            this.Configuration = Configuration;
            crypto_key = Convert.FromBase64String(this.Configuration["AppSettings:secret"]);
            salt = Convert.FromBase64String(this.Configuration["AppSettings:salt"]);
            db = new StormContext(Configuration);
            this.setup = setup;
        }

        public async Task<string> Login(string address, string password)
        {
            string key = EncryptString(password);

            var user = db.User.Where(u => u.address == address && u.password == key)
                    .FirstOrDefault();

            if (user == null) return string.Empty;
            var token = GenToken(user);

            user.token = token;
            await db.SaveChangesAsync();
            return token;
        }

        public async Task<string> Signup(string username, string password, string address, IFormFile logo, bool admin)
        {
            var exists = db.User.Where(u => u.username == username || u.address == address)
        .FirstOrDefault();

            if (exists != null) return string.Empty;
            string url = "./Storage/profiles/user.png";

            if (logo != null)
            {
                url = $"./Storage/profiles/{logo.FileName}";
                var ms = new MemoryStream();
                await logo.CopyToAsync(ms);
                System.IO.File.WriteAllBytes(url, ms.ToArray());
            }

            var user = new User
            {
                username = username,
                password = EncryptString(password),
                address = address,
                logo = url,
                admin = admin
            };

            db.User.Add(user);
            await db.SaveChangesAsync();

            var token = GenToken(user);
            user.token = token;

            await db.SaveChangesAsync();
            return token;
        }

        public async Task<string> RefreshToken(string token)
        {
            var user = db.User.FirstOrDefault(u => u.token == token);
            var refresh_token = GenToken(user);
            user.token = refresh_token;

            await db.SaveChangesAsync();
            return refresh_token;
        }

        private string GenToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(Configuration["JwtSettings:Issuer"],
              Configuration["JwtSettings:Audience"],
              new Claim[] {
                  new Claim("email", user.address),
                  new Claim("admin", user.admin ? "true" : "false")
              },
              expires: DateTime.Now.AddMinutes(2),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool Recover(string address)
        {
            var user = db.User.Where(u => u.address == address)
                    .FirstOrDefault();

            if (user == null) return false;
            string password = DecryptString(user.password);
            SendEmail(user.address, "Recover Password", $"Hi {user.username}!<br> Your password is <b style='color: #5f9ea0;'>{password}.</b><br>Have a nice day!");
            return true;
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
