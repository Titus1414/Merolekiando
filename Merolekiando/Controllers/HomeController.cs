using Merolekando.Common;
using Merolekiando.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Merolekiando.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MerolikandoDBContext _Context;
        public HomeController(ILogger<HomeController> logger, MerolikandoDBContext Context)
        {
            _logger = logger;
            _Context = Context;
        }

        public IActionResult Index()
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                ViewBag.Categories = _Context.Categories.ToList().Count;
                ViewBag.Provinces = _Context.Provinces.ToList().Count;
                ViewBag.Users = _Context.Users.Where(a => a.IsDeleted != true).ToList().Count;
                return View();
            }
            return RedirectToAction("Login");
            
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("userId");
            return RedirectToAction("Login");
        }
        public IActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Login(User dto)
        {
            if (dto.LoginType == "Admin")
            {
                string encpass = Methods.Encrypt(dto.Password);
                var usr = _Context.Users.Where(a => a.Email == dto.Email && a.Password == encpass && a.IsDeleted == false).FirstOrDefault();
                if (usr != null)
                {
                    HttpContext.Session.SetInt32("userId", usr.Id);
                    return RedirectToAction("Index");
                }
                return View();
            }
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
