using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleAds.Data;

namespace Simple_Ads.Controllers
{
    public class AccountController : Controller
    {
        private string _connection = @"Data Source=.\sqlexpress;Initial Catalog=Ads;Integrated Security=True";
        public IActionResult Login()
        {
            if (TempData["Error"] != null)
            {
                ViewBag.Message = TempData["Error"];
                ViewBag.Color = "red";
            }
            return View();
        }
        [HttpPost]
        public IActionResult Login(string password, string email)
        {
            Adsdb db = new Adsdb(_connection);
            User u = db.Login(password, email);
            if (u == null)
            {
                TempData["Error"] = "Invalid Login Please Try Again";
                return Redirect("/account/login");
            }
            var claims = new List<Claim>
            {
                new Claim("user", email)
            };
            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();

            return Redirect("/home/createad");
        }
        public IActionResult CreateAccount()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddAccount(string name, string email, string password)
        {
            Adsdb db = new Adsdb(_connection);
            db.AddUser(name, email, password);
            return Redirect("/account/login");
        }
        [Authorize]
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/account/login");
        }
    }
}