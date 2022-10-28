using Merolekando.Models;
using Merolekando.Models.Dtos;
using Merolekando.Services.Auth;
using Merolekando.Services.Extra;
using Merolekando.Services.Product;
using Merolekando.Services.Token;
using Merolekiando.Models;
using Merolekiando.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Merolekiando.Controllers
{
    public class WebController : Controller
    {
        private readonly IProductService _productService;
        private readonly IExtra _extra;
        private readonly IAuth _auth;
        private readonly IJwtToken _token;
        private readonly MerolikandoDBContext _Context;
        public WebController(IProductService productService, IExtra extra, MerolikandoDBContext Context, IAuth auth, IJwtToken token)
        {
            _productService = productService;
            _extra = extra;
            _Context = Context;
            _auth = auth;
            _token = token;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> EditUser(int id)
        {
            var res = _auth.GetUserById(id);
            return PartialView("~/Views/Web/_MyUersProfile.cshtml", res.Result);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser([FromForm] IFormFile Imagess, UserDto dto)
        {
            SetUserImageDto d = new();
            d.Id = dto.Id;
            d.Image = Imagess;

            await _auth.SetImage(d);
            var res = _auth.UpdateUser(dto);
            return PartialView("~/Views/Web/_MyUersProfile.cshtml", res.Result);
        }
        public IActionResult PrvncDropdown(List<int> id)
        {
            var sd = from t1 in _Context.SubCategories
                     where id.Contains((int)t1.CatId)
                     select t1;

            return PartialView("~/Views/Web/_DropdownMncs.cshtml", sd.ToList());
        }
        [HttpPost]
        public async Task<IActionResult> ManageProduct([FromForm] Productdto user, List<int> prvnc, List<int> mnclst)
        {
            var userid = HttpContext.Session.GetInt32("WebUserId");
            user.SellerId = userid;
            List<MunicipalityDto> lsts = new();
            foreach (var item in mnclst)
            {
                MunicipalityDto dt = new();
                dt.Id = item;
                lsts.Add(dt);
            }
            List<ProvinceDto> lst = new();
            foreach (var item in prvnc)
            {
                ProvinceDto dto = new();
                dto.Municipalitiees = lsts;
                dto.Id = item;
                lst.Add(dto);
            }

            user.provinceDtos = lst;

            var result = _productService.ManageProduct(user);
            return RedirectToAction("WebIndex", "Home");
        }
        public async Task<IActionResult> GetProdDetails(int id)
        {
            var resa = _extra.GetCategories();

            ViewBag.GetCategories = resa.Result;
            ViewBag.SubCategories = _Context.SubCategories.ToList();
            var rs = _extra.GetProvinces();
            ViewBag.GetProvnces = rs.Result;

            var res = await _productService.GetProductId(id);
            return PartialView("~/Views/Web/_GetProductById.cshtml", res);
        }
        public async Task<IActionResult> SetFavProd(int pid)
        {
            var userid = HttpContext.Session.GetInt32("WebUserId");
            var result = await _auth.SetFavProduct(Convert.ToInt32(userid), pid);
            
            return RedirectToAction("WebIndex", "Home");
        }
        public async Task<IActionResult> SetReportProd(int pid)
        {
            var userid = HttpContext.Session.GetInt32("WebUserId");

            var sd = _Context.Products.Where(a => a.Id == pid).FirstOrDefault();
            if (sd.IsReported != true)
            {
                sd.IsReported = true;
                _Context.Products.Update(sd);
                _Context.SaveChanges();
            }

            return RedirectToAction("WebIndex", "Home");
        }
        public async Task<IActionResult> Chats()
        {
            var userid = HttpContext.Session.GetInt32("WebUserId");

            var res = await _extra.GetChatUsers(Convert.ToInt32(userid));
            ViewBag.Inbox = res;

            
            //ViewBag.alert = flag;

            var resa = _extra.GetCategories();

            ViewBag.GetCategories = resa.Result;
            ViewBag.SubCategories = _Context.SubCategories.ToList();
            var rs = _extra.GetProvinces();
            ViewBag.GetProvnces = rs.Result;
            ViewBag.SessionValue = HttpContext.Session.GetInt32("WebUserId");
            return PartialView("~/Views/Web/_Chatpage.cshtml");
            //return View();
        }

        public async Task<IActionResult> UserProfile(int id)
        {
            var userid = HttpContext.Session.GetInt32("WebUserId");

            var s = _Context.Folowers.Where(a => a.Fuser == id && a.Folowers == (int)userid).FirstOrDefault();
            bool isfolow;
            if (s != null)
            {
                isfolow = true;
            }else
            {
                isfolow = false;
            }
            ViewBag.IsFollow = isfolow;

            var res = await _auth.GetUserById(id);
            return PartialView("~/Views/Web/_UersProfile.cshtml", res);
        }
        public async Task<IActionResult> FollowUser(int id)
        {
            var userid = HttpContext.Session.GetInt32("WebUserId");
            Folower dto = new();
            dto.Fuser = (int)userid;
            dto.Folowers = id;

            var res = await _extra.Follow(dto);

            var s = _Context.Folowers.Where(a => a.Folowers == id && a.Fuser == (int)userid).FirstOrDefault();
            bool isfolow;
            if (s == null)
            {
                isfolow = true;
            }
            else
            {
                isfolow = false;
            }
            ViewBag.IsFollow = isfolow;
            return PartialView("~/Views/Web/_UersProfile.cshtml", res);
        }
        public async Task<IActionResult> GetChat(int fromid)
        {
            var userid = HttpContext.Session.GetInt32("WebUserId");
            ViewBag.Users = Convert.ToInt32(userid);
            var result = await _extra.GetChatById(fromid, Convert.ToInt32(userid));

            return PartialView("~/Views/Web/_Messages.cshtml", result);
        }

        public async Task<IActionResult> GetChatbyProdId(int from, int to, int pid)
        {
            MessaveDto dtos = new();
            dtos.From = from;
            dtos.To = to;
            dtos.ProductIds = pid;

            await _extra.SetMessageUser(dtos);

            var result = await _extra.GetChatById(to, from);

            var userid = HttpContext.Session.GetInt32("WebUserId");
            ViewBag.Users = Convert.ToInt32(userid);

            return PartialView("~/Views/Web/_MessagesMdl.cshtml", result);
        }
        public async Task<IActionResult> Gettoken()
        {
            var userid = HttpContext.Session.GetInt32("WebUserId");
            var user = _Context.Users.Where(a => a.Id == userid).FirstOrDefault();
            if (user != null)
            {
                return Ok(_token.CreateUserToken(user));
            }
            return BadRequest();
        }

        public async Task<IActionResult> SendMessage(ChatsDto dto)
        {
            var res = _extra.SendMessage(dto);

            return Ok("Success");

        }

    }
}
