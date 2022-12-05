using Merolekando.Models;
using Merolekando.Models.Dtos;
using Merolekando.Services.Auth;
using Merolekando.Services.Token;
using Merolekiando.Controllers;
using Merolekiando.Models;
using Merolekiando.Models.Dtos;
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
    public class AuthController : ControllerBase
    {
        private readonly IAuth _auth;
        private readonly IJwtToken _token;
        private readonly MerolikandoDBContext _Context;
        public static object token;
        public AuthController(IAuth auth, IJwtToken token, MerolikandoDBContext Context)
        {
            _auth = auth;
            _token = token;
            _Context = Context;
        }
        //[AllowAnonymous]
        [HttpPost]
        [Route("SignUp")]
        public IActionResult SignUp(UserDto user)
        {
            if (user.LoginType != null)
            {
                var res = _auth.SignUp(user);
                if (res.Result.Item2 == "Success")
                {
                    int sdf = _Context.Users.Max(a => a.Id);
                    var usr = _Context.Users.Where(a => a.Id == sdf).FirstOrDefault();
                    var token = _token.CreateUserToken(usr);
                    return Ok(new { res.Result, token });
                }
                else
                {
                    if (res.Result.Item2 == "Ya existe el correo electrónico")
                    {
                        return BadRequest("Ya existe el correo electrónico");
                    }
                    else {
                        return BadRequest("Error");
                    }
                    
                }
                
            }
            return Unauthorized();
        }
        //[AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public IActionResult Login(User user)
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
                            return Unauthorized("Tu cuenta ha sido bloqueada");
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
                            return Unauthorized("Tu cuenta ha sido bloqueada");
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
                        return BadRequest("Error");
                    }
                    var token = _token.CreateUserToken(user: usr);
                    AuthController.token = token;
                    return Ok(token);
                }
                else if (res.Result == "Invalid credentials")
                {
                    return BadRequest("credenciales no válidas");
                }
                else {
                    return BadRequest(res.Result);
                }
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("UpdateUser")]
        public IActionResult UpdateUser(UserDto user)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    user.Id = Convert.ToInt32(name);
                    var result = _auth.UpdateUser(user);
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                    return BadRequest();
                }
                return Unauthorized("Sesión caducada. Por favor, inicie sesión de nuevo");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("ResetPassword")]
        public IActionResult ResetPassword(ResetPasswordDto dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    dto.Uid = Convert.ToInt32(name);
                    if (dto.NewPass == dto.SamePass)
                    {
                        var result = _auth.ResetPasssword(dto);
                        if (result.Result != null)
                        {
                            return Ok(new { result.Result });
                        }
                        return BadRequest();
                    }
                    return BadRequest("Password must be same!");
                }
                return Unauthorized("Sesión caducada. Por favor, inicie sesión de nuevo");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("DeleteUser")]
        public IActionResult DeleteUser(DeleteAccountDto dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    dto.Id = Convert.ToInt32(name);
                    var result = _auth.DeleteUser(dto);
                    if (result.Result == "Success")
                    {
                        return Ok(new { result.Result });
                    }
                    return BadRequest(result.Result);
                }
                return Unauthorized("Sesión caducada. Por favor, inicie sesión de nuevo");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("SetImage")]
        public IActionResult SetImage([FromForm] SetUserImageDto dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    dto.Id = Convert.ToInt32(name);
                    var result = _auth.SetImage(dto);
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                    return BadRequest();
                }
                return Unauthorized("Sesión caducada. Por favor, inicie sesión de nuevo");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("SetEmail")]
        public IActionResult SetEmail(SetUserEmailDto dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    dto.Id = Convert.ToInt32(name);
                    var result = _auth.SetEmail(dto);
                    if (result.Result.Item2 == "Success")
                    {
                        return Ok(new { result.Result });
                    }
                    return BadRequest(result.Result.Item2);
                }
                return Unauthorized("Sesión caducada. Por favor, inicie sesión de nuevo");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("SetFavProduct")]
        public IActionResult SetFavProduct(int id)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _auth.SetFavProduct(Convert.ToInt32(name), id);
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                    return BadRequest(result.Result);
                }
                return Unauthorized("Sesión caducada. Por favor, inicie sesión de nuevo");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("SentVerification")]
        public IActionResult SentVerification([FromForm] VerifyDto dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    dto.Userid = Convert.ToInt32(name);
                    var result = _auth.SendVerification(dto);
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                    return BadRequest(result.Result);
                }
                return Unauthorized("Sesión caducada. Por favor, inicie sesión de nuevo");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("ChangePassword")]
        public IActionResult SendEmail(ChangePassDto dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    dto.Uid = Convert.ToInt32(name);
                    var result = _auth.ChangePassword(dto);
                    if (result.Result == "Success")
                    {
                        return Ok(new { result.Result });
                    }
                    return BadRequest(result.Result);
                }
                return Unauthorized("Sesión caducada. Por favor, inicie sesión de nuevo");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("SendEmail")]
        public IActionResult ChangePassword(string to)
        {
            var result = _auth.SendEmail(to);
            if (result.Result == "Not Exist")
            {
                return BadRequest(result.Result);
            }
            return Ok();
        }
        [HttpGet]
        [Route("GetUsers")]
        public IActionResult GetUsers()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var usera = _Context.Users.Where(a => a.Id == Convert.ToInt32(name)).FirstOrDefault();
                    if (usera.IsBlock == true)
                    {
                        return Unauthorized("Tu cuenta ha sido bloqueada");
                    }
                    var result = _auth.GetUser();
                    return Ok(new { result.Result });
                }
                return Unauthorized("Sesión caducada. Por favor, inicie sesión de nuevo");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("GetUsersById")]
        public IActionResult GetUsersById(int id)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _auth.GetUserById(id);
                    return Ok(new { result.Result });
                }
                return Unauthorized("Sesión caducada. Por favor, inicie sesión de nuevo");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("GetUser")]
        public IActionResult GetUser()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;

                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var usera = _Context.Users.Where(a => a.Id == Convert.ToInt32(name)).FirstOrDefault();
                    if (usera.IsBlock == true)
                    {
                        return Unauthorized("Tu cuenta ha sido bloqueada");
                    }
                    var result = _auth.GetUserById(Convert.ToInt32(name));
                    return Ok(new { result.Result });
                }
                return Unauthorized("Sesión caducada. Por favor, inicie sesión de nuevo");
            }
            return Unauthorized();
        }
        //[HttpGet]
        //[Route("CheckUser")]
        //public IActionResult CheckUser()
        //{
        //    var identity = User.Identity as ClaimsIdentity;
        //    if (identity != null)
        //    {
        //        IEnumerable<Claim> claims = identity.Claims;
        //        var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
        //        if (name != null)
        //        {
        //            var result = _auth.CheckUser(Convert.ToInt32(name));
        //            return Ok(new { result.Result });
        //        }
        //        return Unauthorized("Sesión caducada. Por favor, inicie sesión de nuevo");
        //    }
        //    return Unauthorized();
        //}
        [HttpGet]
        [Route("token")]
        public async Task<IActionResult> GetTokenfo(string QrCode)
        {
            return Ok(AuthController.token);
        }
    }
}
