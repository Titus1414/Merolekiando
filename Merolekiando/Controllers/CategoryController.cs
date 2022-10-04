using Merolekiando.Models;
using Merolekiando.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Merolekiando.Controllers
{
    public class CategoryController : Controller
    {
        private readonly MerolikandoDBContext _Context;
        public CategoryController(MerolikandoDBContext Context)
        {
            _Context = Context;
        }
        public IActionResult Index()
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                ViewBag.Categories = _Context.Categories.ToList();
                return View();
            }
            return RedirectToAction("Login", "Home");
        }
        public IActionResult Product(int Id)
        {
            return View();
        }
        public IActionResult SubCategories(int id)
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                var sd = from t1 in _Context.Categories
                         join t2 in _Context.SubCategories on t1.Id equals t2.CatId
                         where t1.Id == id
                         select new { t1.Name, t2.Id, CatName = t2.Name };
                List<SubCategoriesDto> lst = new();
                foreach (var item in sd.ToList())
                {
                    SubCategoriesDto dto = new();
                    dto.Id = item.Id;
                    dto.Name = item.Name;
                    dto.CatName = item.CatName;
                    lst.Add(dto);
                }
                var sdf = _Context.Categories.Where(a => a.Id == id).FirstOrDefault();
                ViewBag.CategoryName = sdf.Name;
                ViewBag.CateId = id;
                ViewBag.SubCategories = lst;
                return View();
            }
            return RedirectToAction("Login", "Home");
        }
        public IActionResult CategoryEditModal(int id)
        {
            var dto = _Context.Categories.Where(a => a.Id == id).FirstOrDefault();
            return PartialView("~/Views/Category/_EditCategory.cshtml", dto);

        }
        public IActionResult SubCategoryEditModal(int id)
        {
            var dto = _Context.SubCategories.Where(a => a.Id == id).FirstOrDefault();
            return PartialView("~/Views/Category/_EditSubCategory.cshtml", dto);
        }
        [HttpPost]
        public IActionResult Index(Category dto)
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                try
                {
                    if (dto.Id > 0)
                    {
                        var dt = _Context.Categories.Where(a => a.Id == dto.Id).FirstOrDefault();
                        dt.Name = dto.Name;
                        _Context.Categories.Update(dt);
                        _Context.SaveChanges();
                    }
                    else
                    {
                        if (dto.Name != null)
                        {
                            Category category = new();
                            category.Name = dto.Name;
                            category.Time = (int)DateTimeOffset.Now.ToUnixTimeMilliseconds();
                            _Context.Categories.Add(category);
                            _Context.SaveChanges();
                        }
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
        public IActionResult ManageSubCategory(SubCategory dto)
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                try
                {
                    if (dto.Id > 0)
                    {
                        var sd = _Context.SubCategories.Where(a => a.Id == dto.Id).FirstOrDefault();
                        sd.Name = dto.Name;
                        _Context.SubCategories.Update(sd);
                        _Context.SaveChanges();

                        return RedirectToAction("SubCategories", new { id = dto.CatId });
                    }
                    else
                    {
                        SubCategory sub = new();
                        sub.Name = dto.Name;
                        sub.CatId = dto.CatId;
                        _Context.SubCategories.Add(sub);
                        _Context.SaveChanges();
                        int id = _Context.SubCategories.Max(a => a.Id);
                        return RedirectToAction("SubCategories", new { id = dto.CatId });
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            return RedirectToAction("Login", "Home");
        }
        public IActionResult CategoryDeleteModal(int id)
        {
            var dto = _Context.Categories.Where(a => a.Id == id).FirstOrDefault();
            return PartialView("~/Views/Category/_DeleteCategory.cshtml", dto);
        }
        public IActionResult CategoryDelete(int id)
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                var sbCat = _Context.SubCategories.Where(a => a.CatId == id).ToList();
                _Context.SubCategories.RemoveRange(sbCat);
                _Context.SaveChanges();

                var dt = _Context.Categories.Where(a => a.Id == id).FirstOrDefault();
                _Context.Categories.Remove(dt);
                _Context.SaveChanges();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Login", "Home");
        }
        public IActionResult SubCategoryDeleteModal(int id)
        {
            var dto = _Context.SubCategories.Where(a => a.Id == id).FirstOrDefault();
            return PartialView("~/Views/Category/_DeleteSubCategory.cshtml", dto);
        }
        public IActionResult DeleteSubCategory(int id)
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                var dt = _Context.SubCategories.Where(a => a.Id == id).FirstOrDefault();
                _Context.SubCategories.Remove(dt);
                _Context.SaveChanges();

                return RedirectToAction("SubCategories", new { id = dt.CatId });
            }
            return RedirectToAction("Login", "Home");
        }
    }
}
