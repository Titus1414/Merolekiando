using Merolekiando.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

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
        public IActionResult ProvinceDeleteModal(int id)
        {
            var dto = _Context.Provinces.Where(a => a.Id == id).FirstOrDefault();
            return PartialView("~/Views/Province/_ProvinceDeleteModal.cshtml", dto);
        }
        public IActionResult MunicipalityEditModal(int id)
        {
            var dto = _Context.Municipalities.Where(a => a.Id == id).FirstOrDefault();
            return PartialView("~/Views/Province/_MunicipalityEditModal.cshtml", dto);
        }
        public IActionResult MunicipalityDeleteModal(int id)
        {
            var dto = _Context.Municipalities.Where(a => a.Id == id).FirstOrDefault();
            return PartialView("~/Views/Province/_MunicipalityDeleteModal.cshtml", dto);
        }
        public async Task<IActionResult> DeleteProvinces(int id)
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                try
                {
                    var prdPrvnc = await _Context.ProdProvinces.Where(a => a.ProvinceId == id).ToListAsync();
                    _Context.ProdProvinces.RemoveRange(prdPrvnc);
                    _Context.SaveChanges();

                    var mncLst = await _Context.Municipalities.Where(a => a.PrvId == id).ToListAsync();
                    foreach (var item in mncLst)
                    {
                        var prodMnc = await _Context.ProdMunicipalities.Where(a => a.MncId == item.Id).ToListAsync();
                        _Context.ProdMunicipalities.RemoveRange(prodMnc);
                        _Context.SaveChanges();

                        var users = await _Context.Users.Where(a => a.MunicipalityId == item.Id).ToListAsync();

                        foreach (var itm in users)
                        {
                            itm.Municipality = null;
                            _Context.Users.Update(itm);
                            _Context.SaveChanges();
                        }
                    }

                    var usr = await _Context.Users.Where(a => a.ProvinceId == id).ToListAsync();
                    foreach (var item in usr)
                    {
                        item.ProvinceId = null;
                        _Context.Users.Update(item);
                        _Context.SaveChanges();
                    }
                    var prvnce = _Context.Provinces.Where(a => a.Id == id).FirstOrDefault();
                    if (prvnce != null)
                    {
                        _Context.Provinces.Remove(prvnce);
                        _Context.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    throw;
                }

            }
            return RedirectToAction("Login", "Home");
        }
        public async Task<IActionResult> DeleteMunicipality(int id)
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                try
                {
                    var prodMnc = await _Context.ProdMunicipalities.Where(a => a.MncId == id).ToListAsync();
                    _Context.ProdMunicipalities.RemoveRange(prodMnc);
                    _Context.SaveChanges();

                    var users = await _Context.Users.Where(a => a.MunicipalityId == id).ToListAsync();
                    foreach (var user in users)
                    {
                        user.MunicipalityId = null;
                        _Context.Users.Update(user);
                        _Context.SaveChanges();
                    }

                    var mnc = _Context.Municipalities.Where(a => a.Id == id).FirstOrDefault();
                    _Context.Municipalities.Remove(mnc);
                    _Context.SaveChanges();

                    return RedirectToAction("Municiplity", new {id = mnc.PrvId });
                }
                catch (Exception)
                {

                    throw;
                }
            
            }
            return RedirectToAction("Login", "Home");
        }
    }
}
