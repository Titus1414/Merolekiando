using Merolekando.Common;
using Merolekiando.Models;
using Merolekiando.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Merolekiando.Controllers
{
    public class UserController : Controller
    {
        private readonly MerolikandoDBContext _Context;
        public UserController(MerolikandoDBContext Context)
        {
            _Context = Context;
        }
        public IActionResult Index()
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                return View();
            }
            return RedirectToAction("Login");
        }
        public IActionResult Verification()
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                var usr = from t1 in _Context.Users
                          join t2 in _Context.UserVerifications on t1.Id equals t2.UserId
                          where t1.IsDeleted != true && t1.VerificationSent == true && t1.IsVerified != true
                          select new { t1.Name, t1.Id, t2.Fimage, t2.Bimage, t2.Message };

                List<VerifyDto> lst = new();
                foreach (var item in usr.ToList())
                {
                    VerifyDto dto = new();
                    dto.Userid = item.Id;
                    dto.SFImage = Methods.baseurl + item.Fimage;
                    dto.SBImage = Methods.baseurl + item.Bimage;
                    dto.Message = item.Message;
                    dto.UserName = item.Name;
                    lst.Add(dto);
                }

                ViewBag.VerificationSent = lst;

                var usr1 = from t1 in _Context.Users
                           join t2 in _Context.UserVerifications on t1.Id equals t2.UserId
                           where t1.IsDeleted != true && t1.VerificationSent == true && t1.IsVerified == true
                           select new { t1.Name, t1.Id, t2.Fimage, t2.Bimage, t2.Message };

                List<VerifyDto> lst1 = new();
                foreach (var item in usr1.ToList())
                {
                    VerifyDto dto = new();
                    dto.Userid = item.Id;
                    dto.SFImage = Methods.baseurl + item.Fimage;
                    dto.SBImage = Methods.baseurl + item.Bimage;
                    dto.Message = item.Message;
                    dto.UserName = item.Name;
                    lst1.Add(dto);
                }
                ViewBag.Verified = lst1;

                return View();
            }
            return RedirectToAction("Login");
        }
        public IActionResult VerifyAcceptModal(int id)
        {
            var dt = _Context.UserVerifications.Where(a => a.UserId == id).FirstOrDefault();
            return PartialView("~/Views/User/_AcceptModal.cshtml", dt);
        }
        public IActionResult VerifyRejectModal(int id)
        {
            var dt = _Context.UserVerifications.Where(a => a.UserId == id).FirstOrDefault();
            return PartialView("~/Views/User/_RejectModal.cshtml", dt);
        }
        public IActionResult VerifyCancelModal(int id)
        {
            var dt = _Context.UserVerifications.Where(a => a.UserId == id).FirstOrDefault();
            return PartialView("~/Views/User/_CancelModal.cshtml", dt);
        }
        public IActionResult AcceptAccount(int UserId, int check)
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                try
                {
                    var usr = _Context.Users.Where(a => a.Id == UserId).FirstOrDefault();
                    if (check == 1)
                    {
                        usr.IsVerified = true;
                    }
                    else
                    {
                        usr.IsVerified = false;
                        usr.VerificationSent = false;

                        var ver = _Context.UserVerifications.Where(a => a.UserId == UserId).ToList();
                        _Context.UserVerifications.RemoveRange(ver);
                        _Context.SaveChanges();
                    }
                    _Context.Users.Update(usr);
                    _Context.SaveChanges();


                }
                catch (System.Exception ex)
                {
                    throw;
                }

                return RedirectToAction("Verification");
            }
            return RedirectToAction("Login");
        }
        public IActionResult Subscription()
        {
            var UsId = HttpContext.Session.GetInt32("userId");
            if (UsId != null)
            {
                var user = _Context.Users.Where(a => a.IsDeleted != true && a.LoginType != "Admin").ToList();
                ViewBag.Users = user;
                var dt = _Context.Settings.Select(a => a.SubsAllAllow).FirstOrDefault();
                if (dt == null)
                {
                    ViewBag.AllAllow = false;
                }
                else
                {
                    ViewBag.AllAllow = dt;
                }

                return View();
            }
            return RedirectToAction("Login");
        }
        public IActionResult AllowSubsModal(int id)
        {
            var dt = _Context.Users.Where(a => a.Id == id).FirstOrDefault();
            return PartialView("~/Views/User/_AllowSubsModal.cshtml", dt);

        }
        public IActionResult DenySubsModal(int id)
        {
            var dt = _Context.Users.Where(a => a.Id == id).FirstOrDefault();
            return PartialView("~/Views/User/_DenySubsModal.cshtml", dt);
        }
        public IActionResult ManageSub(SubsDto dto)
        {
            var user = _Context.Users.Where(a => a.Id == dto.Id).FirstOrDefault();
            if (user != null)
            {
                DateTimeOffset dateTime = new();
                dateTime = dto.Date;
                user.Subscriptions = dateTime.ToUnixTimeMilliseconds();
                _Context.Users.Update(user);
                _Context.SaveChanges();
            }

            return RedirectToAction("Subscription");
        }
        public IActionResult DenySub(SubsDto dto)
        {
            var user = _Context.Users.Where(a => a.Id == dto.Id).FirstOrDefault();
            if (user != null)
            {
                DateTimeOffset dateTime = new();
                dateTime = DateTime.Now.AddDays(-5);
                user.Subscriptions = dateTime.ToUnixTimeMilliseconds();
                _Context.Users.Update(user);
                _Context.SaveChanges();
            }

            return RedirectToAction("Subscription");
        }
        public IActionResult ALlowAll()
        {
            var dt = _Context.Settings.FirstOrDefault();
            if (dt == null)
            {
                Setting setting = new();
                setting.SubsAllAllow = true;
                _Context.Settings.Add(setting);
                _Context.SaveChanges();
            }
            else
            {
                if (dt.SubsAllAllow == false)
                {
                    dt.SubsAllAllow = true;
                }
                else
                {
                    dt.SubsAllAllow = false;
                }
                _Context.Settings.Update(dt);
                _Context.SaveChanges();
            }

            return RedirectToAction("Subscription");
        }
    }
}
