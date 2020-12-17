using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MovieStorm.Services;
using MovieStorm.Data;
using MovieStorm.Models;
using Microsoft.AspNetCore.Http;

namespace MovieStorm.Controllers
{
    public class AccountController : Controller
    {
        private StormContext db;
        private CryptoService crypt;
        private readonly IConfiguration Configuration;

        public AccountController(IConfiguration Configuration, IAccountSettings account)
        {
            this.Configuration = Configuration;
            crypt = new CryptoService(Configuration, account);
            db = new StormContext(Configuration);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string address, string password)
        {
            string key = crypt.EncryptString(password);

            var user = db.User.Where(u => u.address == address && u.password == password)
                    .FirstOrDefault();

            if (user == null) return Unauthorized(new { error = "Unauthorized access!" }); 
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(string username, string password, string address, IFormFile logo, bool admin)
        {
            var exists = db.User.Where(u => u.username == username || u.address == address)
                    .FirstOrDefault();

            if (exists != null) return Unauthorized(new { error = "User already exists!" });
            string url = "./Storage/profiles/user.png";

            if(logo != null)
            {
                url = $"./Storage/profiles/{logo.FileName}";
                var ms = new MemoryStream();
                await logo.CopyToAsync(ms);
                System.IO.File.WriteAllBytes(url, ms.ToArray());
            }

            var user = new User
            {
                username = username,
                password = crypt.EncryptString(password),
                address = address,
                logo = url,
                admin = admin
            };

            db.User.Add(user);
            await db.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public IActionResult Recover(string address)
        {
            var user = db.User.Where(u => u.address == address)
                    .FirstOrDefault();

            if (user == null) return NotFound(new { error = "There is no user with the specified email address." });
            string password = crypt.DecryptString(user.password);
            crypt.SendEmail(user.address, "Recover Password", $"Hi {user.username}!<br> Your password is <b style='color: #5f9ea0;'>{password}.</b><br>Have a nice day!");
            return Ok(new { message = "Please check your email address!" });
        }
    }
}
