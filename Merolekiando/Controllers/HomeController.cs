using Merolekando.Common;
using Merolekando.Models.Dtos;
using Merolekando.Services.Auth;
using Merolekando.Services.Extra;
using Merolekando.Services.Product;
using Merolekiando.Models;
using Merolekiando.Models.Dtos;
using Merolekiando.Models.WebDtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
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
        private readonly IProductService _productService;
        private readonly IExtra _extra;
        private readonly IAuth _auth;
        public static string flag = "";
        public HomeController(ILogger<HomeController> logger, MerolikandoDBContext Context, IWebHostEnvironment? environment, IProductService productService, IAuth auth, IExtra extra)
        {
            _logger = logger;
            _Context = Context;
            _environment = environment;
            _productService = productService;
            _auth = auth;
            _extra = extra;
        }
        public async Task WebLogin(string flg)
        {
            if (flg == "goog")
            {
                await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
            }
            else if (flg == "face")
            {
                await HttpContext.ChallengeAsync(FacebookDefaults.AuthenticationScheme, new AuthenticationProperties()
                {
                    RedirectUri = Url.Action("FacebookResponse")
                });
            }
        }
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claim = result.Principal.Identities
                .FirstOrDefault().Claims
                .Select(claim => new
                {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value
                }
                );
            User usr = new();
            usr.LoginType = "Google";
            foreach (var item in claim)
            {
                if (item.Type.Contains("/nameidentifier"))
                {
                    usr.UniqueId = item.Value;
                }
                else if (item.Type.Contains("/name"))
                {
                    usr.Name = item.Value;
                }
                else if (item.Type.Contains("/emailaddress"))
                {
                    usr.Email = item.Value;
                }
            }
            return RedirectToAction("WebLoginT",usr);
        }
        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claim = result.Principal.Identities
                .FirstOrDefault().Claims.Select(claim => new
                {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value
                });
            User usr = new();
            usr.LoginType = "Facebook";
            foreach (var item in claim)
            {
                if (item.Type.Contains("/nameidentifier"))
                {
                    usr.UniqueId = item.Value;
                }
                else if (item.Type.Contains("/name"))
                {
                    usr.Name = item.Value;
                }
                
            }
            return RedirectToAction("WebLoginT", usr);
        }
        public async Task<IActionResult> WebLogout()
        {
            HttpContext.Session.Remove("WebUserId");
            await HttpContext.SignOutAsync();
            return RedirectToAction("WebIndex");
        }
        [HttpGet]
        public IActionResult WebLoginT(User user)
        {
            if (user.LoginType != null)
            {
                if (user.LoginType == "Custom")
                {
                    var us = _Context.Users.Where(a => a.Email == user.Email && a.IsDeleted != true).FirstOrDefault();
                    if (us != null)
                    {
                        var usera = _Context.Users.Where(a => a.Id == us.Id).FirstOrDefault();
                        if (usera.IsBlock == true)
                        {
                            flag = "Tu cuenta ha sido bloqueada";
                            return RedirectToAction("WebIndex");
                        }
                    }
                }
                else
                {
                    var us = _Context.Users.Where(a => a.Email == user.Email && a.IsDeleted != true && a.UniqueId == user.UniqueId).FirstOrDefault();
                    if (us != null)
                    {
                        var usera = _Context.Users.Where(a => a.Id == us.Id).FirstOrDefault();
                        if (usera.IsBlock == true)
                        {
                            flag = "Tu cuenta ha sido bloqueada";
                            return RedirectToAction("WebIndex");
                        }
                    }
                }

                var res = _auth.Login(user);
                if (res.Result == "Success")
                {
                    var usr = _Context.Users.Where(a => a.Email == user.Email && a.IsDeleted != true && a.LoginType == user.LoginType).FirstOrDefault();
                    if (user.LoginType != "Custom")
                    {
                        usr = _Context.Users.Where(a => a.UniqueId == user.UniqueId && a.IsDeleted != true && a.LoginType == user.LoginType).FirstOrDefault();
                    }
                    if (usr == null)
                    {
                        flag = "Error";
                        return RedirectToAction("WebIndex");
                    }
                    HttpContext.Session.SetInt32("WebUserId", usr.Id);
                    flag = "Success";
                    return RedirectToAction("WebIndex");
                }
                else if (res.Result == "Invalid credentials")
                {
                    flag = "credenciales no válidas";
                    return RedirectToAction("WebIndex");
                }
                else
                {
                    flag = "Error";
                    return RedirectToAction("WebIndex");
                }
            }
            return View();
        }
        public IActionResult GetProductDetails(int id)
        {
            var result = _productService.GetProductId(id);
            Productdto dto = new();
            if (result.Result != null)
            {
                dto = result.Result;
                //return Ok(new { result.Result });
            }
            var userid = HttpContext.Session.GetInt32("WebUserId");
            if (userid != null)
            {
                ViewBag.UserIdSession = userid;
            }else
            {
                ViewBag.UserIdSession = null;
            }
            bool favCheck = false;
            var s = _Context.Favorites.Where(a => a.Pid == id && a.UserId == userid).FirstOrDefault();
            if (s != null)
            {
                favCheck = true;
            }

            ViewBag.FavCheck = favCheck;
            if (userid != null)
            {
                var res = _extra.GetChatsByProduct((int)dto.Id, (int)userid);

                ViewBag.InboxYep = res.Result;
            }
            

            var results = _auth.GetUserById((int)dto.SellerId);

            ViewBag.UserbyId = results.Result;
            ViewBag.ToId = (int)dto.SellerId;
            ViewBag.ProdId = id;

            ViewBag.UserIdBtnId = dto.SellerId;


            return PartialView("~/Views/Home/_ProductDetails.cshtml", dto);
        }
        public IActionResult GetProductsByCat(string id)
        {
            var catid = _Context.Categories.Where(a => a.Name == id).FirstOrDefault();

            var prod = from t1 in _Context.Products
                       join t2 in _Context.ProdImages on t1.Id equals t2.PId into f
                       from pi in f.DefaultIfEmpty()
                       join t3 in _Context.Categories on t1.CategoryId equals t3.Id
                       where t1.IsActive == true && t1.CategoryId == catid.Id
                       select new { t1.Id, t1.Title, t1.Price, pi.Image, t3.Name, t1.IsPromoted, t1.SellerId };
            List<ProductCard> lst = new();
            foreach (var item in prod)
            {
                ProductCard product = new();
                product.Id = item.Id;
                product.Name = item.Title;
                product.Price = item.Price;
                product.Image = item.Image;
                product.Category = item.Name;
                product.IsPromot = item.IsPromoted;
                product.sellerid = item.SellerId;
                lst.Add(product);
            }
            ViewBag.Promoted = lst;
            ViewBag.CountProducts = lst.Count;
            var userid = HttpContext.Session.GetInt32("WebUserId");
            ViewBag.SessUserId = userid;

            return PartialView("~/Views/Home/_ProductsView.cshtml");
        }
        public IActionResult GetProductsBySubCat(string id)
        {
            var catid = _Context.SubCategories.Where(a => a.Name == id).FirstOrDefault();

            var prod = from t1 in _Context.Products
                       join t2 in _Context.ProdImages on t1.Id equals t2.PId into f
                       from pi in f.DefaultIfEmpty()
                       join t3 in _Context.Categories on t1.CategoryId equals t3.Id
                       where t1.IsActive == true && t1.SubCategoryId == catid.Id
                       select new { t1.Id, t1.Title, t1.Price, pi.Image, t3.Name, t1.IsPromoted };
            List<ProductCard> lst = new();
            foreach (var item in prod)
            {
                ProductCard product = new();
                product.Id = item.Id;
                product.Name = item.Title;
                product.Price = item.Price;
                product.Image = item.Image;
                product.Category = item.Name;
                product.IsPromot = item.IsPromoted;
                lst.Add(product);
            }
            ViewBag.Promoted = lst;
            ViewBag.CountProducts = lst.Count;

            return PartialView("~/Views/Home/_ProductsView.cshtml");
        }

        public async Task<IActionResult> DeleteProduct(List<string> id)
        {
            foreach (var item in id)
            {
                if (!string.IsNullOrEmpty(item) || item != "on")
                {
                    var res = await _productService.DeleteProduct(Convert.ToInt32(item));
                }
            }

            var userid = HttpContext.Session.GetInt32("WebUserId");

            var prod = from t1 in _Context.Products
                       join t2 in _Context.ProdImages on t1.Id equals t2.PId into f
                       from pi in f.DefaultIfEmpty()
                       join t3 in _Context.Categories on t1.CategoryId equals t3.Id
                       where t1.IsActive == true && t1.SellerId == userid
                       select new { t1.Id, t1.Title, t1.Price, pi.Image, t3.Name, t1.IsPromoted };
            List<ProductCard> lst = new();
            foreach (var item in prod)
            {
                ProductCard product = new();
                product.Id = item.Id;
                product.Name = item.Title;
                product.Price = item.Price;
                product.Image = item.Image;
                product.Category = item.Name;
                product.IsPromot = item.IsPromoted;
                lst.Add(product);
            }
            ViewBag.Promoted = lst;
            ViewBag.CountProducts = lst.Count;
            ViewBag.Flg = true;

            return PartialView("~/Views/Home/_ProductsView.cshtml");
        }

        public IActionResult GetMyProducts()
        {
            var userid = HttpContext.Session.GetInt32("WebUserId");

            var prod = from t1 in _Context.Products
                       join t2 in _Context.ProdImages on t1.Id equals t2.PId into f
                       from pi in f.DefaultIfEmpty()
                       join t3 in _Context.Categories on t1.CategoryId equals t3.Id
                       where t1.IsActive == true && t1.SellerId == userid
                       select new { t1.Id, t1.Title, t1.Price, pi.Image, t3.Name, t1.IsPromoted };
            List<ProductCard> lst = new();
            foreach (var item in prod)
            {
                ProductCard product = new();
                product.Id = item.Id;
                product.Name = item.Title;
                product.Price = item.Price;
                product.Image = item.Image;
                product.Category = item.Name;
                product.IsPromot = item.IsPromoted;
                lst.Add(product);
            }
            ViewBag.Promoted = lst;
            ViewBag.CountProducts = lst.Count;
            ViewBag.Flg = true;

            return PartialView("~/Views/Home/_ProductsView.cshtml");
        }
        public async Task<IActionResult> GetMyFavProducts()
        {
            var userid = HttpContext.Session.GetInt32("WebUserId");

            var prod = from t1 in _Context.Products
                       join t2 in _Context.ProdImages on t1.Id equals t2.PId into f
                       from pi in f.DefaultIfEmpty()
                       join t3 in _Context.Categories on t1.CategoryId equals t3.Id
                       join t4 in _Context.Favorites on t1.Id equals t4.Pid
                       where t1.IsActive == true && t4.UserId == userid
                       select new { t1.Id, t1.Title, t1.Price, pi.Image, t3.Name, t1.IsPromoted };
            List<ProductCard> lst = new();
            foreach (var item in prod)
            {
                ProductCard product = new();
                product.Id = item.Id;
                product.Name = item.Title;
                product.Price = item.Price;
                product.Image = item.Image;
                product.Category = item.Name;
                product.IsPromot = item.IsPromoted;
                lst.Add(product);
            }

            //var result = await _productService.GetFavProducts(Convert.ToInt32(userid));
            ViewBag.Promoted = lst;
            ViewBag.CountProducts = lst.Count;

            return PartialView("~/Views/Home/_ProductsView.cshtml");
        }
        public IActionResult WebIndex()
        {
            var banners = _Context.Banners.Where(a => a.IsActive == true).ToList();
            ViewBag.Banners = banners;

            var prod = from t1 in _Context.Products
                       join t2 in _Context.ProdImages on t1.Id equals t2.PId into f
                       from pi in f.DefaultIfEmpty()
                       join t3 in _Context.Categories on t1.CategoryId equals t3.Id
                       where t1.IsActive == true
                       select new { t1.Id, t1.Title, t1.Price, pi.Image, t3.Name, t1.IsPromoted };
            List<ProductCard> lst = new();
            foreach (var item in prod)
            {
                ProductCard product = new();
                product.Id = item.Id;
                product.Name = item.Title;
                product.Price = item.Price;
                product.Image = item.Image;
                product.Category = item.Name;
                product.IsPromot = item.IsPromoted;
                lst.Add(product);
            }
            ViewBag.Promoted = lst;
            ViewBag.CountProducts = lst.Count;
            ViewBag.alert = flag;

            var res = _extra.GetCategories();
            
            ViewBag.GetCategories = res.Result;
            ViewBag.SubCategories = _Context.SubCategories.ToList();
            var rs = _extra.GetProvinces();
            ViewBag.GetProvnces = rs.Result;

            ViewBag.SessionValue = HttpContext.Session.GetInt32("WebUserId");
            return View();
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
