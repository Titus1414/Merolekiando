using Merolekando.Common;
using Merolekando.Models.Dtos;
using Merolekiando.Models;
using Merolekiando.Models.Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Merolekiando.Controllers
{
    public class PromotionController : Controller
    {
        private readonly MerolikandoDBContext _Context;
        public static IWebHostEnvironment? _environment;
        public PromotionController(MerolikandoDBContext Context, IWebHostEnvironment? environment)
        {
            _Context = Context;
            _environment = environment;
        }
        public IActionResult Index()
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                var dt = from t1 in _Context.Products
                         join t2 in _Context.Users on t1.SellerId equals t2.Id
                         join t3 in _Context.ProdImages on t1.Id equals t3.PId
                         where t1.IsActive == true
                         select new { t1.Id, t1.Title, t1.Description, t2.Name, Uid = t2.Id, t3.Image, t1.IsPromoted  };

                List<PromotionDto> lst = new();
                foreach (var item in dt.ToList())
                {
                    PromotionDto dto = new();
                    dto.Id = item.Id;
                    dto.UserName = item.Name;
                    dto.Decription = item.Description;
                    dto.Title = item.Title;
                    dto.Uid = item.Uid;
                    dto.Image = Methods.baseurl + item.Image;
                    dto.IsPromote = item.IsPromoted;
                    lst.Add(dto);
                }
                ViewBag.Promotions = lst;


                var dt1 = from t1 in _Context.Products
                         join t2 in _Context.Users on t1.SellerId equals t2.Id
                         where t1.IsActive == true && t1.IsReported == true
                          join t3 in _Context.ProdImages on t1.Id equals t3.PId
                          select new { t1.Id, t1.Title, t1.Description, t2.Name, Uid = t2.Id, t3.Image };

                List<PromotionDto> lst1 = new();
                foreach (var item in dt1.ToList())
                {
                    PromotionDto dto = new();
                    dto.Id = item.Id;
                    dto.UserName = item.Name;
                    dto.Decription = item.Description;
                    dto.Title = item.Title;
                    dto.Uid = item.Uid;
                    dto.Image = Methods.baseurl + item.Image;
                    lst1.Add(dto);
                }
                ViewBag.ReportedProducts = lst1;


                ViewBag.Bannerdata = _Context.Banners.ToList();



                return View();
            }
            return RedirectToAction("Login", "Home");
        }
        public IActionResult PromoteClickModal(int id)
        {
            var dt = _Context.Products.Where(a => a.Id == id).FirstOrDefault();
            return PartialView("~/Views/Promotion/_PromotModal.cshtml", dt);
        }
        public IActionResult DeleteClickModal(int id)
        {
            var dt = _Context.Products.Where(a => a.Id == id).FirstOrDefault();
            return PartialView("~/Views/Promotion/_DeleteModal.cshtml", dt);
        }
        public IActionResult IgnoreModal(int id)
        {
            var dt = _Context.Products.Where(a => a.Id == id).FirstOrDefault();
            return PartialView("~/Views/Promotion/_IgnoreModal.cshtml", dt);
        }
        public IActionResult PromoteProduct(int Id)
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                var dt = _Context.Products.Where(a => a.Id == Id).FirstOrDefault();
                if (dt != null)
                {
                    if (dt.IsPromoted == true)
                    {
                        dt.IsPromoted = false;
                    }
                    else
                    {
                        dt.IsPromoted = true;
                    }
                    _Context.Products.Update(dt);
                    _Context.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Login", "Home");
        }
        public IActionResult IgnoreProduct(int Id)
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                var dt = _Context.Products.Where(a => a.Id == Id).FirstOrDefault();
                if (dt != null)
                {
                    dt.IsReported = false;
                    _Context.Products.Update(dt);
                    _Context.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Login", "Home");
        }
        public IActionResult DeleteProduct(int Id)
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                var dt = _Context.Products.Where(a => a.Id == Id).FirstOrDefault();
                if (dt != null)
                {
                    dt.IsActive = false;
                    _Context.Products.Update(dt);
                    _Context.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Login", "Home");
        }
        [HttpPost]
        public IActionResult AddBanner(BannerDto dto)
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                if (dto.Id > 0)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    Banner banner = new();

                    var filename1 = "";
                    Random rnd = new();
                    var rn = rnd.Next(111, 999);
                    var img = "";
                    if (dto.Image != null)
                    {
                        var ImagePath1 = rn + Methods.RemoveWhitespace(dto.Image.FileName);
                        var pathh = "";
                        using (FileStream fileStream = System.IO.File.Create(_environment.WebRootPath + "\\Resources\\Images\\Banner\\" + ImagePath1))
                        {
                            dto.Image.CopyTo(fileStream);
                            pathh = Path.Combine(_environment.WebRootPath, "/Resources/Images/Banner/" + ImagePath1);
                            filename1 = ImagePath1;
                            fileStream.Flush();
                        }
                        img = "/Resources/Images/Banner/" + filename1;
                    }
                    else
                    {
                        img = "";
                    }
                    banner.Image = img;
                    banner.Name = dto.Name;
                    banner.Description = dto.Description;
                    banner.IsActive = true;
                    _Context.Banners.Add(banner);
                    _Context.SaveChanges();
                    return RedirectToAction("Index");
                }

                return RedirectToAction("Index");
            }
            return RedirectToAction("Login", "Home");
        }
        public IActionResult BannerModal(int id)
        {
            var dt = _Context.Banners.Where(a => a.Id == id).FirstOrDefault();
            return PartialView("~/Views/Promotion/_DeleteBanner.cshtml", dt);
        }
        public IActionResult DeleteBanner(int Id)
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                var dt = _Context.Banners.Where(a => a.Id == Id).FirstOrDefault();
                _Context.Banners.Remove(dt);
                _Context.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Login", "Home");
        }

    }
}
