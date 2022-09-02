using Merolekiando.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Merolekiando.Controllers
{
    public class ProvinceController : Controller
    {
        private readonly MerolikandoDBContext _Context;
        public ProvinceController(MerolikandoDBContext Context)
        {
            _Context = Context;
        }
        public IActionResult Index()
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                ViewBag.Provices = _Context.Provinces.ToList();
                return View();
            }
            return RedirectToAction("Login", "Home");
        }
        public IActionResult Municiplity(int Id)
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                ViewBag.Municipilities = _Context.Municipalities.Where(a => a.PrvId == Id).ToList();
                ViewBag.ProvinceName = _Context.Provinces.Where(a => a.Id == Id).Select(a => a.Name).FirstOrDefault();
                ViewBag.PrvId = Id;
                return View();
            }
            return RedirectToAction("Login", "Home");
        }
        public IActionResult Manage(Province dt)
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                try
                {
                    if (dt.Id > 0)
                    {
                        var data = _Context.Provinces.Where(a => a.Id == dt.Id).FirstOrDefault();
                        data.Name = dt.Name;
                        _Context.Provinces.Update(data);
                        _Context.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Province province = new();
                        province.Name = dt.Name;
                        province.Time = (int)DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        _Context.Provinces.Add(province);
                        _Context.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                catch (System.Exception)
                {
                    throw;
                }
            }
            return RedirectToAction("Login", "Home");
        }
        public IActionResult ManageMunicipality(Municipality dt)
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                try
            {
                if (dt.Id > 0)
                {
                    var data = _Context.Municipalities.Where(a => a.Id == dt.Id).FirstOrDefault();
                    data.Name = dt.Name;
                    _Context.Municipalities.Update(data);
                    _Context.SaveChanges();
                    return RedirectToAction("Municiplity", new { id = dt.PrvId });
                }
                else
                {
                    Municipality province = new();
                    province.Name = dt.Name;
                    province.Time = (int)DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    province.PrvId = dt.PrvId;
                    _Context.Municipalities.Add(province);
                    _Context.SaveChanges();
                    return RedirectToAction("Municiplity", new { id = dt.PrvId });
                }
            }
            catch (Exception)
            {

                throw;
            }
            }
            return RedirectToAction("Login", "Home");
        }
        public IActionResult ProvinceEditModal(int id)
        {
            var dto = _Context.Provinces.Where(a => a.Id == id).FirstOrDefault();
            return PartialView("~/Views/Province/_ProvinceEditModal.cshtml", dto);
        }
        public IActionResult MunicipalityEditModal(int id)
        {
            var dto = _Context.Municipalities.Where(a => a.Id == id).FirstOrDefault();
            return PartialView("~/Views/Province/_MunicipalityEditModal.cshtml", dto);
        }
    }
}
