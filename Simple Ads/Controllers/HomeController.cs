using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleAds.Models;
using SimpleAds.Data;
using SimpleAds.Models;

namespace Simple_Ads.Controllers
{
    public class HomeController : Controller
    {
        private string _connection = @"Data Source=.\sqlexpress;Initial Catalog=Ads;Integrated Security=True";
        public IActionResult Index()
        {   Adsdb db = new Adsdb(_connection);
            AdView view = new AdView();
            bool IsLoggedIn=User.Identity.IsAuthenticated;
            if(IsLoggedIn)
            {
                string email = User.Identity.Name;
                view.Id = db.GetIdByEmail(email);
            }
            view.Ads = db.GetAds();
            return View(view);
        }
        [Authorize]
        public IActionResult CreateAd()
        {
            AdViewModel vm = new AdViewModel();
            vm.email = (User.Identity.Name);
            return View(vm);
        }
        [HttpPost]
        public IActionResult CreateAd(Ad ad)
        {
            Adsdb db = new Adsdb(_connection);
            db.NewAd(ad);
            return Redirect("/");
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Adsdb db = new Adsdb(_connection);
            int userId = db.GetIdByEmail(User.Identity.Name);
            db.Delete(id, userId);
            return Redirect("/");
        }
        [Authorize]
        public IActionResult MyAccount(int id)
        {
            Adsdb db = new Adsdb(_connection);
            int userId = db.GetIdByEmail(User.Identity.Name);
            return View(db.GetAds(userId));
        }
    }
}
