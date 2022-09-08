﻿using Merolekando.Models;
using Merolekando.Models.Dtos;
using Merolekando.Services.Product;
using Merolekiando.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Merolekando.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly MerolikandoDBContext _Context;
        public ProductController(IProductService productService, MerolikandoDBContext Context)
        {
            _productService = productService;
            _Context = Context;
        }

        [HttpPost]
        [Route("Test")]
        //[DisableFormValueModelBinding]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Test([FromForm] testDto user) 
        {
            return Ok();
        }
        [HttpPost]
        [Route("AddProduct")]
        [AllowAnonymous]
        public async Task<IActionResult> AddProduct([FromForm] Productdto user)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var usr = _Context.Users.Where(a => a.Id == Convert.ToInt32(name)).FirstOrDefault();
                    if (usr.IsBlock == true)
                    {
                        return Unauthorized("Tu cuenta ha sido bloqueada");
                    }
                    var set = _Context.Settings.FirstOrDefault();
                    if (set.SubsAllAllow != true)
                    {
                        if (usr.Subscriptions == null || usr.Subscriptions < DateTimeOffset.Now.ToUnixTimeMilliseconds())
                        {
                            return BadRequest("Por favor, suscríbete para publicar");
                        }
                    }
                    
                    user.SellerId = Convert.ToInt32(name);
                    var result = _productService.ManageProduct(user);
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                    return Unauthorized("Algo salió mal");
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("EditProduct")]
        public async Task<IActionResult> EditProduct([FromForm] Productdto user)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var usr = _Context.Users.Where(a => a.Id == Convert.ToInt32(name)).FirstOrDefault();
                    if (usr.IsBlock == true)
                    {
                        return Unauthorized("Tu cuenta ha sido bloqueada");
                    }

                    user.SellerId = Convert.ToInt32(name);
                    var result = _productService.ManageProduct(user);
                    return Ok(new { result.Result });
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("ProductSold")]
        public IActionResult ProductSold(SoldDto dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _productService.ProductSold(dto);
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                    return BadRequest(result.Result);
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("ProductReport")]
        public IActionResult ProductSold(ReportDto dto)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _productService.ProductReport(dto);
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                    return BadRequest(result.Result);
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("DeleteProduct")]
        public IActionResult DeleteProduct(int id)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _productService.DeleteProduct(id);
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                    return BadRequest(result.Result);
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpPost]
        [Route("SetProductCount")]
        public IActionResult SetProductCount(int id)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _productService.SetCountOfViewProduct(Convert.ToInt32(name),id);
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                    return BadRequest(result.Result);
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        
        [HttpGet]
        [Route("GetProducts")]
        public IActionResult GetProducts()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {

                    var result = _productService.GetProductAsync(Convert.ToInt32(name));
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                    return BadRequest();
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("GetUserProducts")]
        public IActionResult GetUserProducts()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _productService.GetUserProduct(Convert.ToInt32(name));
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                    return BadRequest();
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("GetProductsById")]
        public IActionResult GetProductsId(int id)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {

                    var result = _productService.GetProductId(id);
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                    return BadRequest();
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("GetProductsBySellerId")]
        public IActionResult GetProductsSellerId(int id)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {

                    var result = _productService.GetProductSellerId( id);
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                    return BadRequest();
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("GetProductsByCategoryId")]
        public IActionResult GetProductsByCategoryId(int id)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {

                    var result = _productService.GetProductCategoryId(id, Convert.ToInt32(name));
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                    return BadRequest();
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("GetProductsBySubCategoryId")]
        public IActionResult GetProductsBySubCategoryId(int id)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {

                    var result = _productService.GetProductSubCategoryId(id, Convert.ToInt32(name));
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                    return BadRequest();
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("GetFavProducts")]
        public IActionResult GetFavProducts()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {

                    var result = _productService.GetFavProducts(Convert.ToInt32(name));
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                    return BadRequest();
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("SearchProducts")]
        public IActionResult SearchProducts(string Search)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {

                    var result = _productService.SearchProducts(Search, Convert.ToInt32(name));
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                    return BadRequest();
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
        [HttpGet]
        [Route("GetImagesByPid")]
        public IActionResult GetImagesByPid(int id)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var name = claims.Where(p => p.Type == "ID").FirstOrDefault()?.Value;
                if (name != null)
                {
                    var result = _productService.GetImagesByPId(id);
                    if (result.Result != null)
                    {
                        return Ok(new { result.Result });
                    }
                    return BadRequest();
                }
                return Unauthorized("Token Issue");
            }
            return Unauthorized();
        }
    }
}
