using Merolekando.Models.Dtos;
using Merolekando.Services.Extra;
using Merolekiando.Models;
using Merolekiando.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Merolekando.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExtraController : ControllerBase
    {
        private readonly IExtra _extra;
        public ExtraController(IExtra extra)
        {
            _extra = extra;
        }
        [HttpPost]
        [Route("AddCategory")]
        public IActionResult AddCategory(Category dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.ManageCategory(dto);
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("EditCategory")]
        public IActionResult EditCategory(Category dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.ManageCategory(dto);
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("AddSubCategory")]
        public IActionResult AddSubCategory(SubCategory dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.ManageSubCategory(dto);
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("EditSubCategory")]
        public IActionResult EditSubCategory(SubCategory dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.ManageSubCategory(dto);
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("AddBanner")]
        public IActionResult AddBanner([FromForm] BannerDto dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.ManageBanner(dto);
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("EditBanner")]
        public IActionResult EditBanner([FromForm] BannerDto dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.ManageBanner(dto);
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("AddProvince")]
        public IActionResult AddProvince(Province dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.ManageProvince(dto);
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("EditProvince")]
        public IActionResult EditProvince(Province dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.ManageProvince(dto);
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("AddMunicipilty")]
        public IActionResult AddMunicipilty(Municipality dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.ManageMunicipality(dto);
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("EditMunicipilty")]
        public IActionResult EditMunicipilty(Municipality dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.ManageMunicipality(dto);
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("GiveRate")]
        public IActionResult GiveRate(Rating dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    dto.UserId = Convert.ToInt32(name);
                    var result = _extra.GiveRate(dto);
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                    
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("SendMessage")]
        public IActionResult SendMessage(ChatsDto dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    dto.senderId = Convert.ToInt32(name);
                    var result = _extra.SendMessage(dto);
                    if (result.Result == "Success")
                    {
                        return Ok(new { result.Result });
                    }
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("SendImageMessage")]
        public IActionResult SendImageMessage([FromForm] SendImageDto dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    dto.senderId = Convert.ToInt32(name);
                    var result = _extra.SendImage(dto);
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("Follower")]
        public IActionResult Follower(Folower dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    dto.Fuser = Convert.ToInt32(name);
                    var result = _extra.Follow(dto);
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("SetNotification")]
        public IActionResult SetNotification(int Pid)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.Notify(Convert.ToInt32(name), Pid);
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("SetMessageUserlist")]
        public IActionResult SetMessageUserlist(MessaveDto dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    dto.From = Convert.ToInt32(name);
                    var result = _extra.SetMessageUser(dto);
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("GetMunicipilty")]
        public IActionResult GetMunicipilty(int id)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.GetMunicipalityById(id);
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("GetMunicipilties")]
        public IActionResult GetMunicipilties()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.GetMunicipalities();
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("GetProvince")]
        public IActionResult GetProvince(int id)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.GetProvinceById(id);
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("GetProvinces")]
        public IActionResult GetProvinces()
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                    if (name != null)
                    {
                        var result = _extra.GetProvinces();
                        return Ok(new { result.Result });
                    }
                    return Unauthorized("Token Issue");
                }
                return Unauthorized("asdfasdf");
            }
            catch (Exception ex)
            {
                return Unauthorized();
                throw;
            }
            
        }
        [HttpGet]
        [Route("GetCategory")]
        public IActionResult GetCategory(int id)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.GetCategoryById(id);
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("GetCategories")]
        public IActionResult GetCategories()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.GetCategories();
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("GetSubCategory")]
        public IActionResult GetSubCategory(int id)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.GetSubCategoryById(id);
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("GetSubCategories")]
        public IActionResult GetSubCategories()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.GetSubCategories();
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("GetBanner")]
        public IActionResult GetBanner(int id)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.GetBannerById(id);
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("GetBanners")]
        public IActionResult GetBanners()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.GetBanners();
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("GetChatUsers")]
        public IActionResult GetChatUsers()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.GetChatUsers(Convert.ToInt32(name));
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("GetChatsByProduct")]
        public IActionResult GetChatsByProduct(int id)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.GetChatsByProduct(id);
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("GetChatById")]
        public async Task<IActionResult> GetChatById(int id)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.GetChatById(id, Convert.ToInt32(name));
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("GetNotification")]
        public async Task<IActionResult> GetNotification()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.GetNotify( Convert.ToInt32(name));
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("AllInfo")]
        public async Task<IActionResult> AllInfo()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _extra.AllIfon();
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
    }
}
