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

        public AccountController(AccountService settings)
        {
            this.settings = settings;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string address, string password)
        {
            string token = await settings.Login(address, password);
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { error = "Unauthorized access!" });
            return Ok(new { token });
        }

        [HttpPost]
        public async Task<IActionResult> Signup(string username, string password, string address, IFormFile logo, bool admin)
        {
            string token = await settings.Signup(username, password, address, logo, admin);
            if (string.IsNullOrEmpty(token)) return Unauthorized(new { error = "User already exists!" });
            return Ok(new { token });
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
