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
using Microsoft.AspNetCore.Authorization;

namespace MovieStorm.Controllers
{
    public class AccountController : Controller
    {
        private AccountService settings;

        public AccountController(IConfiguration config, StormContext _context, IAccountSettings account)
        {
            settings = new AccountService(config, _context, account);
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Signup()
        {
            return View();
        }

        public IActionResult Recover()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string address, string password)
        {
            string token = await settings.Login(address, password);
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { error = "Unauthorized access!" });
            Response.Cookies.Append("token", token);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(string username, string password, string address, IFormFile logo, bool admin)
        {
            string token = await settings.Signup(username, password, address, logo, admin);
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { error = "User already exists!" });
            Response.Cookies.Append("token", token);
            return Ok();
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> RefreshToken(string token)
        {
            string newToken = await settings.RefreshToken(token);
            return Ok(new { token = newToken });
        }

        [Authorize]
        public IActionResult About(string token)
        {
            var data = settings.About(token);
            return Json(data);
        }

        public IActionResult Show()
        {
            var token = Request.Cookies["token"];
            var data = settings.About(token);
            int index = data.path.LastIndexOf('.');

            string ext = data.path.Substring(index + 1);
            byte[] buffer = System.IO.File.ReadAllBytes(data.path);
            return File(buffer, $"image/{ext}");
        }

        public IActionResult Signout()
        {
            Response.Cookies.Delete("token");
            return Redirect("/Account/Login");
        }

        [HttpPost]
        public IActionResult Recover(string address)
        {
            bool ok = settings.Recover(address);
            if (!ok) return NotFound(new { error = "There is no user with the specified email address." });
            return Ok(new { message = "Please check your email address!" });
        }
    }
}
