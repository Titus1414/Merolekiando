using Merolekando.Common;
using Merolekiando.Models;
using Merolekiando.Models.Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Merolekiando.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MerolikandoDBContext _Context;
        public static IWebHostEnvironment? _environment;
        public HomeController(ILogger<HomeController> logger, MerolikandoDBContext Context, IWebHostEnvironment? environment)
        {
            _logger = logger;
            _Context = Context;
            _environment = environment;
        }
        public IActionResult User()
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                ViewBag.Users = _Context.Users.Where(a => a.IsDeleted != true && a.LoginType != "Admin").ToList();
                return View();
            }
            return RedirectToAction("Login");

        }
        public IActionResult UserModal(int id)
        {
            var dt = _Context.Users.Where(a => a.Id == id).FirstOrDefault();
            return PartialView("~/Views/Home/_UserBlock.cshtml", dt);
        }
        public IActionResult BlockUser(int Id)
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                var usr = _Context.Users.Where(a => a.Id == Id).FirstOrDefault();
                if (usr.IsBlock == true)
                {
                    usr.IsBlock = false;
                }
                else
                {
                    usr.IsBlock = true;
                }
                _Context.Users.Update(usr);
                _Context.SaveChanges();
                return RedirectToAction("User");
            }
            return RedirectToAction("Login");
        }
        public IActionResult DeleteUser(int Id)
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                try
                {
                    var user = _Context.Users.Where(a => a.Id == Id && a.IsDeleted != true).FirstOrDefault();
                    if (user != null)
                    {
                        user.IsDeleted = true;
                        _Context.Users.Update(user);
                        _Context.SaveChanges();

                        var chat = _Context.Chats.Where(a => a.SenderId == Id || a.RecieverId == Id).ToList();
                        _Context.Chats.RemoveRange(chat);

                        var prd = _Context.Products.Where(a => a.SellerId == Id).ToList();
                        foreach (var item in prd)
                        {
                            item.IsActive = false;
                            _Context.Products.Update(item);
                        }
                        _Context.SaveChanges();

                        return RedirectToAction("User");
                    }

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return RedirectToAction("Login");
        }
        public IActionResult Index()
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                DateTimeOffset dateTime = new();
                dateTime = DateTime.Now;

                ViewBag.Categories = _Context.Categories.ToList().Count;
                ViewBag.Provinces = _Context.Provinces.ToList().Count;
                ViewBag.Users = _Context.Users.Where(a => a.IsDeleted != true && a.LoginType != "Admin").ToList().Count;
                ViewBag.SubsCnt = _Context.Users.Where(a => a.IsDeleted != true && a.IsBlock != true && a.Subscriptions >= dateTime.ToUnixTimeMilliseconds()).ToList().Count;
                return View();
            }
            return RedirectToAction("Login");

        }
        public IActionResult Contact()
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                var dt = _Context.Users.Where(a => a.LoginType == "Admin" && a.IsDeleted != true).FirstOrDefault();
                dt.Image = Methods.baseurl + dt.Image;
                return View(dt);
            }
            return RedirectToAction("Login");

        }
        [HttpPost]
        public IActionResult Contact(ContactDto dto)
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                var dt = _Context.Users.Where(a => a.Id == dto.Id).FirstOrDefault();

                var filename1 = "";
                Random rnd = new();
                var rn = rnd.Next(111, 999);
                var img = "";
                if (dto.Image != null)
                {
                    var ImagePath1 = rn + Methods.RemoveWhitespace(dto.Image.FileName);
                    var pathh = "";
                    using (FileStream fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Resources\\Images\\Users\\" + ImagePath1))
                    {
                        dto.Image.CopyTo(fileStream);
                        pathh = Path.Combine(_environment.WebRootPath, "/Resources/Images/Users/" + ImagePath1);
                        filename1 = ImagePath1;
                        fileStream.Flush();
                    }
                    img = "/Resources/Images/Users/" + filename1;
                }
                else
                {
                    img = "";
                }
                dt.Image = img;
                dt.Email = dto.Email;
                dt.Number = dto.Number;
                dt.Description = dto.Description;
                _Context.Users.Update(dt);
                _Context.SaveChanges();
                return RedirectToAction("Contact");
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
            ViewBag.Flag = 1;
            return View();
        }
        [HttpPost]
        public IActionResult Login(User dto)
        {
            int flg = 0;
            if (dto.LoginType == "Admin")
            {
                if (string.IsNullOrEmpty(dto.Password) || dto.Password == "0")
                {
                    ViewBag.Flag = flg;
                    return View();
                }
                string encpass = Methods.Encrypt(dto.Password);
                var usr = _Context.Users.Where(a => a.Email == dto.Email && a.Password == encpass && a.IsDeleted != true).FirstOrDefault();
                if (usr != null)
                {
                    HttpContext.Session.SetInt32("userId", usr.Id);
                    flg = 1;
                    ViewBag.Flag = flg;
                    return RedirectToAction("Index");
                }
                ViewBag.Flag = flg;
                return View();
            }
            ViewBag.Flag = flg;
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
        public IActionResult PasswordRest(int id, string email)
        {
            var dt = _Context.Users.Where(a => a.Id == id && a.Email == email).FirstOrDefault();
            if (dt != null)
            {
                ViewBag.Email = dt.Email;
                ViewBag.Id = id;
            }
            return View();
        }
        [HttpPost]
        public IActionResult PasswordRest(string Email, string Password, string LoginType, int Id)
        {
            var dt = _Context.Users.Where(a => a.Id == Id && a.Email == Email && a.IsDeleted != true && a.LoginType == "Custom").FirstOrDefault();
            if (dt != null)
            {
                dt.Password = Methods.Encrypt(Password);

                _Context.Users.Update(dt);
                _Context.SaveChanges();

                return View("Thanks");
            }

            return View("Error");
        }
    }
}
