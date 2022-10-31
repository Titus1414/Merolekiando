using Merolekando.Models;
using Merolekando.Models.Dtos;
using Merolekando.Services.Auth;
using Merolekando.Services.Extra;
using Merolekando.Services.Product;
using Merolekando.Services.Token;
using Merolekiando.Models;
using Merolekiando.Models.Dtos;
using Merolekiando.Models.WebDtos;
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
            var sd = from t1 in _Context.Municipalities
                     where id.Contains((int)t1.PrvId)

                     select t1;

            return PartialView("~/Views/Web/_DropdownMncs.cshtml", sd.ToList());
        }
        public IActionResult PrvncDropdownMain(List<int> id, List<int> mncVal, string search, string CatId, string SubcateId)
        {
            var cat = _Context.Categories.Where(a => a.Name == CatId).FirstOrDefault();
            var subcat = _Context.SubCategories.Where(a => a.Name == SubcateId).FirstOrDefault();

            var dt = from t1 in _Context.Products
                     join t2 in _Context.ProdImages on t1.Id equals t2.PId into f
                     from pi in f.DefaultIfEmpty()
                     join t3 in _Context.ProdProvinces on t1.Id equals t3.Pid
                     join t4 in _Context.ProdMunicipalities on t1.Id equals t4.Pid
                     join t5 in _Context.Categories on t1.CategoryId equals t5.Id into a
                     from a1 in a.DefaultIfEmpty()
                     join t6 in _Context.SubCategories on t1.SubCategoryId equals t6.Id into ab
                     from a2 in ab.DefaultIfEmpty()
                     where id.Contains((int)t3.Id) || mncVal.Contains(t4.Id) || search.Contains(t1.Title) || search.Contains(t1.Description) && t1.IsActive == true && t1.IsSold != true
                     select new { t1.Id, t1.Title, t1.Price, pi.Image, a1.Name, t1.IsPromoted, t1.SellerId };

            List<ProductCard> lst = new();
            foreach (var item in dt)
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
            var res = await _productService.GetProductId(pid);


            Productdto dto = new();
            if (res != null)
            {
                dto = res;
                //return Ok(new { result.Result });
            }
            if (userid != null)
            {
                ViewBag.UserIdSession = userid;
            }
            else
            {
                ViewBag.UserIdSession = null;
            }
            bool favCheck = false;
            var s = _Context.Favorites.Where(a => a.Pid == pid && a.UserId == userid).FirstOrDefault();
            if (s != null)
            {
                favCheck = true;
            }

            ViewBag.FavCheck = favCheck;
            if (userid != null)
            {
                var ress = _extra.GetChatsByProduct((int)dto.Id, (int)userid);

                ViewBag.InboxYep = ress.Result;
            }



            var results = _auth.GetUserById((int)dto.SellerId);

            ViewBag.UserbyId = results.Result;
            ViewBag.ToId = (int)dto.SellerId;
            ViewBag.ProdId = pid;

            ViewBag.UserIdBtnId = (int)dto.SellerId;


            return PartialView("~/Views/Home/_ProductDetails.cshtml", res);
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
            var res = await _productService.GetProductId(pid);

            Productdto dto = new();
            if (res != null)
            {
                dto = res;
                //return Ok(new { result.Result });
            }
            if (userid != null)
            {
                ViewBag.UserIdSession = userid;
            }
            else
            {
                ViewBag.UserIdSession = null;
            }
            bool favCheck = false;
            var s = _Context.Favorites.Where(a => a.Pid == pid && a.UserId == userid).FirstOrDefault();
            if (s != null)
            {
                favCheck = true;
            }

            ViewBag.FavCheck = favCheck;
            if (userid != null)
            {
                var ress = _extra.GetChatsByProduct((int)dto.Id, (int)userid);

                ViewBag.InboxYep = ress.Result;
            }

            var results = _auth.GetUserById((int)dto.SellerId);

            ViewBag.UserbyId = results.Result;
            ViewBag.ToId = (int)dto.SellerId;
            ViewBag.ProdId = pid;

            ViewBag.UserIdBtnId = (int)dto.SellerId;

            return PartialView("~/Views/Home/_ProductDetails.cshtml", res);
        }
        public async Task<IActionResult> Chats()
        {
            try
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
            }
            catch (Exception)
            {

                throw;
            }
            
            //return View();
        }

        public async Task<IActionResult> UserProfile(int id)
        {
            var userid = HttpContext.Session.GetInt32("WebUserId");

            var s = _Context.Folowers.Where(a => a.Folowers == id && a.Fuser == (int)userid).FirstOrDefault();
            bool isfolow;
            if (s == null)
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
