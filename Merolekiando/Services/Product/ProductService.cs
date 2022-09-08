using Merolekando.Common;
using Merolekando.Models;
using Merolekando.Models.Dtos;
using Merolekiando.Hubs;
using Merolekiando.Models;
using Merolekiando.Models.Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Merolekando.Services.Product
{
    public class ProductService : IProductService
    {
        private readonly MerolikandoDBContext _Context;
        public static IWebHostEnvironment? _environment;
        private readonly IHubContext<ChatHub> _hubContext;
        public ProductService(MerolikandoDBContext Context, IWebHostEnvironment environment, IHubContext<ChatHub> hubContext)
        {
            _Context = Context;
            _environment = environment;
            _hubContext = hubContext;
        }
        public async Task<Productdto> ManageProduct(Productdto dto)
        {
            if (dto.Id > 0)
            {
                try
                {
                    var pro = await _Context.Products.Where(a => a.Id == dto.Id).FirstOrDefaultAsync();
                    pro.CategoryId = dto.CategoryId;
                    pro.Condition = dto.Condition;
                    pro.Description = dto.Description;
                    pro.FireOnPrice = dto.FireOnPrice;
                    pro.IsDelivering = dto.IsDelivering;
                    pro.IsPickup = dto.IsPickup;
                    pro.IsPromoted = dto.IsPromoted;
                    pro.IsReported = dto.IsReported;
                    pro.IsSold = dto.IsSold;
                    pro.Price = dto.Price;
                    pro.SellerId = dto.SellerId;
                    pro.SubCategoryId = dto.SubCategoryId;
                    pro.Title = dto.Title;

                    _Context.Products.Update(pro);
                    _Context.SaveChanges();
                    if (dto.removeImagesId != null)
                    {
                        foreach (var item in dto.removeImagesId)
                        {
                            var img = await _Context.ProdImages.Where(a => a.Id == item).FirstOrDefaultAsync();
                            _Context.ProdImages.Remove(img);
                            _Context.SaveChanges();
                        }
                    }
                    if (dto.images != null)
                    {
                        foreach (var item in dto.images)
                        {
                            ProdImage img = new();

                            var filename1 = "";
                            Random rnd = new();
                            var rn = rnd.Next(111, 999);

                            if (item != null)
                            {
                                var ImagePath1 = rn + Methods.RemoveWhitespace(item.FileName);
                                var pathh = "";
                                using (FileStream fileStream = File.Create(_environment.WebRootPath + "\\Resources\\Images\\Products\\" + ImagePath1))
                                {
                                    item.CopyTo(fileStream);
                                    pathh = Path.Combine(_environment.WebRootPath, "/Resources/Images/Products/" + ImagePath1);
                                    filename1 = ImagePath1;
                                    fileStream.Flush();
                                }
                                img.Image = "/Resources/Images/Products/" + filename1;
                            }
                            else
                            {
                                img.Image = "";
                            }
                            img.PId = dto.Id;
                            img.IsActive = true;
                            await _Context.ProdImages.AddAsync(img);
                            _Context.SaveChanges();

                            
                        }
                    }

                    var prvnc = await _Context.ProdProvinces.Where(a => a.Pid == dto.Id).ToListAsync();
                    _Context.RemoveRange(prvnc);
                    _Context.SaveChanges();

                    var mncplt = await _Context.ProdMunicipalities.Where(a => a.Pid == dto.Id).ToListAsync();
                    _Context.RemoveRange(mncplt);
                    _Context.SaveChanges();

                    foreach (var itm in dto.provinceDtos)
                    {
                        var data = _Context.ProdProvinces.Where(a => a.Pid == dto.Id && a.ProvinceId == itm.Id).FirstOrDefault();
                        if (data == null)
                        {
                            ProdProvince prod = new();

                            prod.Pid = dto.Id;
                            prod.ProvinceId = itm.Id;
                            await _Context.ProdProvinces.AddAsync(prod);
                            _Context.SaveChanges();
                        }

                        foreach (var iem in itm.Municipalitiees)
                        {
                            var adata = _Context.ProdMunicipalities.Where(a => a.MncId == iem.Id && a.Pid == dto.Id).FirstOrDefault();
                            if (adata == null)
                            {
                                ProdMunicipality proda = new();
                                proda.MncId = iem.Id;
                                proda.Pid = dto.Id;

                                await _Context.ProdMunicipalities.AddAsync(proda);
                                _Context.SaveChanges();
                            }
                        }
                    }

                    Productdto dataa = new();

                    var dt = await _Context.Products.Where(a => a.Id == dto.Id && a.IsActive == true).FirstOrDefaultAsync();
                    dataa.Id = dt.Id;
                    dataa.IsDelivering = dt.IsDelivering;
                    dataa.CategoryId = dt.CategoryId;
                    dataa.Condition = dt.Condition;
                    dataa.Description = dt.Description;
                    dataa.FireOnPrice = dt.FireOnPrice;
                    dataa.IsPickup = dt.IsPickup;
                    dataa.IsPromoted = dt.IsPromoted;
                    dataa.IsReported = dt.IsReported;
                    dataa.IsSold = dt.IsSold;
                    dataa.Price = dt.Price;
                    dataa.SellerId = dt.SellerId;
                    dataa.SubCategoryId = dt.SubCategoryId;
                    dataa.Title = dt.Title;
                    dataa.CreatedDate = dt.CreatedDate;

                    var pv = await _Context.ProdViews.Where(a => a.PId == dt.Id).ToListAsync();
                    List<ProdViewsDto> pvLst = new();
                    foreach (var itm1 in pv)
                    {
                        ProdViewsDto dt1 = new();
                        dt1.Id = itm1.Id;
                        dt1.UserId = itm1.UserId;
                        dt1.PId = itm1.PId;
                        pvLst.Add(dt1);
                    }
                    dataa.ProdViews = pvLst;

                    var imgs = await _Context.ProdImages.Where(a => a.PId == dt.Id).ToListAsync();

                    List<ProdImagesDto> lst = new();

                    foreach (var item1 in imgs)
                    {
                        ProdImagesDto prod = new();
                        prod.Id = item1.Id;
                        prod.Image = item1.Image;
                        prod.PId = item1.PId;
                        lst.Add(prod);
                    }

                    dataa.ProdImages = lst;

                    List<ProvinceDto> lstA = new();
                    var provnc = await _Context.ProdProvinces.Where(a => a.Pid == dt.Id).ToListAsync();
                    foreach (var itemA in provnc)
                    {
                        var d = await _Context.Provinces.Where(a => a.Id == itemA.ProvinceId).FirstOrDefaultAsync();

                        List<MunicipalityDto> mnclst = new();
                        ProvinceDto dtoA = new();
                        dtoA.Id = itemA.ProvinceId;
                        dtoA.Name = d.Name;
                        var prodMunc = await _Context.ProdMunicipalities.Where(a => a.Pid == dt.Id).ToListAsync();
                        foreach (var itm in prodMunc)
                        {
                            var sd = await _Context.Municipalities.Where(a => a.Id == itm.MncId).FirstOrDefaultAsync();
                            if (sd.PrvId == d.Id)
                            {
                                MunicipalityDto dto1 = new();
                                dto1.Id = sd.Id;
                                dto1.Name = sd.Name;
                                dto1.Time = sd.Time;
                                mnclst.Add(dto1);
                            }
                        }
                        dtoA.Municipalitiees = mnclst;
                        lstA.Add(dtoA);
                    }

                    dataa.provinceDtos = lstA;
                    return dataa;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            else
            {
                try
                {

                    var usr = _Context.Users.Where(a => a.Id == dto.SellerId).FirstOrDefault();
                    if (usr.Subscriptions >= DateTimeOffset.Now.ToUnixTimeMilliseconds())
                    {
                        Merolekiando.Models.Product pro = new();
                        pro.CategoryId = dto.CategoryId;
                        pro.Condition = dto.Condition;
                        pro.Description = dto.Description;
                        pro.FireOnPrice = dto.FireOnPrice;
                        pro.IsDelivering = dto.IsDelivering;
                        pro.IsPickup = dto.IsPickup;
                        pro.IsPromoted = dto.IsPromoted;
                        pro.IsReported = dto.IsReported;
                        pro.IsSold = dto.IsSold;
                        pro.Price = dto.Price;
                        pro.SellerId = dto.SellerId;
                        pro.SubCategoryId = dto.SubCategoryId;
                        pro.Title = dto.Title;
                        pro.CreatedDate = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        pro.IsActive = true;
                        await _Context.Products.AddAsync(pro);
                        _Context.SaveChanges();

                        int pid = _Context.Products.Max(a => a.Id);
                        if (dto.images != null)
                        {
                            foreach (var item in dto.images)
                            {
                                ProdImage img = new();

                                var filename1 = "";
                                Random rnd = new();
                                var rn = rnd.Next(111, 999);

                                if (item != null)
                                {
                                    var ImagePath1 = rn + Methods.RemoveWhitespace(item.FileName);
                                    var pathh = "";
                                    using (FileStream fileStream = File.Create(_environment.WebRootPath + "\\Resources\\Images\\Products\\" + ImagePath1))
                                    {
                                        item.CopyTo(fileStream);
                                        pathh = Path.Combine(_environment.WebRootPath, "/Resources/Images/Products/" + ImagePath1);
                                        filename1 = ImagePath1;
                                        fileStream.Flush();
                                    }
                                    img.Image = "/Resources/Images/Products/" + filename1;
                                }
                                else
                                {
                                    img.Image = "";
                                }
                                img.IsActive = true;
                                img.PId = pid;
                                await _Context.ProdImages.AddAsync(img);
                                _Context.SaveChanges();

                                foreach (var itm in dto.provinceDtos)
                                {
                                    var data = _Context.ProdProvinces.Where(a => a.Pid == pid && a.ProvinceId == itm.Id).FirstOrDefault();
                                    if (data == null)
                                    {
                                        ProdProvince prod = new();
                                        prod.Pid = pid;
                                        prod.ProvinceId = itm.Id;
                                        await _Context.ProdProvinces.AddAsync(prod);
                                        _Context.SaveChanges();
                                    }

                                    foreach (var iem in itm.Municipalitiees)
                                    {
                                        var adata = _Context.ProdMunicipalities.Where(a => a.MncId == iem.Id && a.Pid == pid).FirstOrDefault();
                                        if (adata == null)
                                        {
                                            ProdMunicipality proda = new();
                                            proda.MncId = iem.Id;
                                            proda.Pid = pid;
                                            await _Context.ProdMunicipalities.AddAsync(proda);
                                            _Context.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }

                        Productdto dataa = new();

                        var dt = await _Context.Products.Where(a => a.Id == pid && a.IsActive == true).FirstOrDefaultAsync();
                        dataa.Id = dt.Id;
                        dataa.IsDelivering = dt.IsDelivering;
                        dataa.CategoryId = dt.CategoryId;
                        dataa.Condition = dt.Condition;
                        dataa.Description = dt.Description;
                        dataa.FireOnPrice = dt.FireOnPrice;
                        dataa.IsPickup = dt.IsPickup;
                        dataa.IsPromoted = dt.IsPromoted;
                        dataa.IsReported = dt.IsReported;
                        dataa.IsSold = dt.IsSold;
                        dataa.Price = dt.Price;
                        dataa.SellerId = dt.SellerId;
                        dataa.SubCategoryId = dt.SubCategoryId;
                        dataa.Title = dt.Title;
                        dataa.CreatedDate = dt.CreatedDate;

                        var pv = await _Context.ProdViews.Where(a => a.PId == dt.Id).ToListAsync();
                        List<ProdViewsDto> pvLst = new();
                        foreach (var itm1 in pv)
                        {
                            ProdViewsDto dt1 = new();
                            dt1.Id = itm1.Id;
                            dt1.UserId = itm1.UserId;
                            dt1.PId = itm1.PId;
                            pvLst.Add(dt1);
                        }
                        dataa.ProdViews = pvLst;

                        var imgs = await _Context.ProdImages.Where(a => a.PId == dt.Id).ToListAsync();

                        List<ProdImagesDto> lst = new();

                        foreach (var item1 in imgs)
                        {
                            ProdImagesDto prod = new();
                            prod.Id = item1.Id;
                            prod.Image = item1.Image;
                            prod.PId = item1.PId;
                            lst.Add(prod);
                        }

                        dataa.ProdImages = lst;

                        List<ProvinceDto> lstA = new();
                        var provnc = await _Context.ProdProvinces.Where(a => a.Pid == dt.Id).ToListAsync();
                        foreach (var itemA in provnc)
                        {
                            var d = await _Context.Provinces.Where(a => a.Id == itemA.ProvinceId).FirstOrDefaultAsync();

                            List<MunicipalityDto> mnclst = new();
                            ProvinceDto dtoA = new();
                            dtoA.Id = itemA.ProvinceId;
                            dtoA.Name = d.Name;
                            var prodMunc = await _Context.ProdMunicipalities.Where(a => a.Pid == dt.Id).ToListAsync();
                            foreach (var itm in prodMunc)
                            {
                                var sd = await _Context.Municipalities.Where(a => a.Id == itm.MncId).FirstOrDefaultAsync();
                                if (sd.PrvId == d.Id)
                                {
                                    MunicipalityDto dto1 = new();
                                    dto1.Id = sd.Id;
                                    dto1.Name = sd.Name;
                                    dto1.Time = sd.Time;
                                    dto1.PrvId = sd.PrvId;
                                    mnclst.Add(dto1);
                                }
                            }
                            dtoA.Municipalitiees = mnclst;
                            lstA.Add(dtoA);
                        }

                        dataa.provinceDtos = lstA;

                        var folowers = _Context.Folowers.Where(a => a.Folowers == dt.SellerId).ToList();
                        foreach (var item in folowers)
                        {
                            var user = _Context.Messages.Where(a => a.From == item.Fuser).FirstOrDefault();
                            var SelerName = _Context.Users.Where(a => a.Id == dt.SellerId).Select(a => a.Name).FirstOrDefault();
                            await _hubContext.Clients.Clients(user.ConnId).SendAsync("ProductNotification", SelerName, dataa);

                        }

                        //await _hubContext.Clients.


                        return dataa;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public async Task<List<Productdto>> GetProductAsync(int id)
        {
            try
            {
                var usr = await _Context.Users.Where(a => a.Id == id).FirstOrDefaultAsync();

                var dta = from t1 in _Context.Products
                         join t3 in _Context.ProdProvinces on t1.Id equals t3.Pid
                         join t4 in _Context.ProdMunicipalities on t1.Id equals t4.Pid
                         where t1.IsActive == true && t3.ProvinceId == usr.ProvinceId && t4.MncId == usr.MunicipalityId && t1.IsSold != true
                         && t1.SellerId != usr.Id
                         select new { t1, t3, t4 };

                var dt = await _Context.Products.Where(a => a.IsActive == true && a.IsSold != true && a.SellerId != usr.Id).ToListAsync();

                List<Productdto> lst = new();

                foreach (var item in dta.Select(a => a.t1))
                {
                    Productdto dto = new();
                    dto.Id = item.Id;
                    dto.CategoryId = item.CategoryId;
                    dto.Condition = item.Condition;
                    dto.IsSold = item.IsSold;
                    dto.Description = item.Description;
                    dto.FireOnPrice = item.FireOnPrice;
                    dto.IsDelivering = item.IsDelivering;
                    dto.IsPickup = item.IsPickup;
                    dto.IsPromoted = item.IsPromoted;
                    dto.IsReported = item.IsReported;
                    dto.Price = item.Price;
                    dto.SellerId = item.SellerId;
                    dto.SubCategoryId = item.SubCategoryId;
                    dto.Title = item.Title;
                    dto.CreatedDate = item.CreatedDate;
                    List<ProdImagesDto> prodImages = new();
                    var pdimg = await _Context.ProdImages.Where(a => a.PId == item.Id).ToListAsync();
                    foreach (var itm in pdimg)
                    {
                        if (itm.PId == dto.Id)
                        {
                            ProdImagesDto prodImagesDto = new();
                            prodImagesDto.Id = itm.Id;
                            prodImagesDto.Image = itm.Image;
                            prodImagesDto.PId = itm.PId;
                            prodImages.Add(prodImagesDto);
                        }
                    }
                    dto.ProdImages = prodImages;


                    List<ProvinceDto> lstA = new();
                    var provnc = await _Context.ProdProvinces.Where(a => a.Pid == item.Id).ToListAsync();
                    foreach (var itemA in provnc)
                    {
                        var d = await _Context.Provinces.Where(a => a.Id == itemA.ProvinceId).FirstOrDefaultAsync();
                        if (d != null)
                        {
                            List<MunicipalityDto> mnclst = new();
                            ProvinceDto dtoA = new();
                            dtoA.Id = itemA.ProvinceId;
                            dtoA.Name = d.Name;
                            var prodMunc = await _Context.ProdMunicipalities.Where(a => a.Pid == item.Id).ToListAsync();
                            foreach (var itm in prodMunc)
                            {
                                var sd = await _Context.Municipalities.Where(a => a.Id == itm.MncId).FirstOrDefaultAsync();
                                if (sd.PrvId == d.Id)
                                {
                                    MunicipalityDto dto1 = new();
                                    dto1.Id = sd.Id;
                                    dto1.Name = sd.Name;
                                    dto1.Time = sd.Time;
                                    mnclst.Add(dto1);
                                }
                            }
                            dtoA.Municipalitiees = mnclst;
                            lstA.Add(dtoA);
                        }
                    }

                    dto.provinceDtos = lstA;

                    var pv = await _Context.ProdViews.Where(a => a.PId == item.Id).ToListAsync();
                    List<ProdViewsDto> pvLst = new();
                    foreach (var itm1 in pv)
                    {
                        ProdViewsDto dt1 = new();
                        dt1.Id = itm1.Id;
                        dt1.UserId = itm1.UserId;
                        dt1.PId = itm1.PId;
                        pvLst.Add(dt1);
                    }
                    dto.ProdViews = pvLst;
                    lst.Add(dto);
                }

                return lst;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
            
        }
        public async Task<List<Productdto>> GetUserProduct(int id)
        {
            try
            {
                var usr = await _Context.Users.Where(a => a.Id == id).FirstOrDefaultAsync();

                //var dt = from t1 in _Context.Products
                //         join t2 in _Context.ProdImages on t1.Id equals t2.PId
                //         where t1.SellerId == usr.Id && t1.IsActive == true

                //         select new { t1, t2 };
                var dt = await _Context.Products.Where(a => a.SellerId == usr.Id && a.IsActive == true).ToListAsync();
                List<Productdto> lst = new();

                foreach (var item in dt)
                {
                    Productdto dto = new();
                    dto.Id = item.Id;
                    dto.CategoryId = item.CategoryId;
                    dto.Condition = item.Condition;
                    dto.IsSold = item.IsSold;
                    dto.Description = item.Description;
                    dto.FireOnPrice = item.FireOnPrice;
                    dto.IsDelivering = item.IsDelivering;
                    dto.IsPickup = item.IsPickup;
                    dto.IsPromoted = item.IsPromoted;
                    dto.IsReported = item.IsReported;
                    dto.Price = item.Price;
                    dto.SellerId = item.SellerId;
                    dto.SubCategoryId = item.SubCategoryId;
                    dto.Title = item.Title;
                    dto.CreatedDate = item.CreatedDate;
                    List<ProdImagesDto> prodImages = new();
                    var imgs = await _Context.ProdImages.Where(a => a.PId == item.Id).ToListAsync();
                    foreach (var itm in imgs)
                    {
                        if (itm.PId == dto.Id)
                        {
                            ProdImagesDto prodImagesDto = new();
                            prodImagesDto.Id = itm.Id;
                            prodImagesDto.Image = itm.Image;
                            prodImagesDto.PId = itm.PId;
                            prodImages.Add(prodImagesDto);
                        }
                    }
                    dto.ProdImages = prodImages;

                    var pv = await _Context.ProdViews.Where(a => a.PId == item.Id).ToListAsync();
                    List<ProdViewsDto> pvLst = new();
                    foreach (var itm1 in pv)
                    {
                        ProdViewsDto dt1 = new();
                        dt1.Id = itm1.Id;
                        dt1.UserId = itm1.UserId;
                        dt1.PId = itm1.PId;
                        pvLst.Add(dt1);
                    }
                    dto.ProdViews = pvLst;

                    List<ProvinceDto> lstA = new();
                    var provnc = await _Context.ProdProvinces.Where(a => a.Pid == item.Id).ToListAsync();
                    foreach (var itemA in provnc)
                    {
                        var d = await _Context.Provinces.Where(a => a.Id == itemA.ProvinceId).FirstOrDefaultAsync();

                        List<MunicipalityDto> mnclst = new();
                        ProvinceDto dtoA = new();
                        dtoA.Id = itemA.ProvinceId;
                        dtoA.Name = d.Name;
                        var prodMunc = await _Context.ProdMunicipalities.Where(a => a.Pid == item.Id).ToListAsync();
                        foreach (var itm in prodMunc)
                        {
                            var sd = await _Context.Municipalities.Where(a => a.Id == itm.MncId).FirstOrDefaultAsync();
                            if (sd.PrvId == d.Id)
                            {
                                MunicipalityDto dto1 = new();
                                dto1.Id = sd.Id;
                                dto1.Name = sd.Name;
                                dto1.Time = sd.Time;
                                mnclst.Add(dto1);
                            }
                        }
                        dtoA.Municipalitiees = mnclst;
                        lstA.Add(dtoA);
                    }

                    dto.provinceDtos = lstA;

                    var msgsCnt = _Context.Messages.Where(a => a.ProductId == item.Id).ToList();
                    dto.ChatCount = msgsCnt.Count;

                    lst.Add(dto);
                }

                return lst;
            }
            catch (Exception)
            {
                return null;
                throw;
            }

        }
        public async Task<Productdto> GetProductId(int id)
        {
            try
            {
                Productdto data = new();

                var dt = await _Context.Products.Where(a => a.Id == id && a.IsActive == true).FirstOrDefaultAsync();
                data.Id = dt.Id;
                data.IsDelivering = dt.IsDelivering;
                data.CategoryId = dt.CategoryId;
                data.Condition = dt.Condition;
                data.Description = dt.Description;
                data.FireOnPrice = dt.FireOnPrice;
                data.IsPickup = dt.IsPickup;
                data.IsPromoted = dt.IsPromoted;
                data.IsReported = dt.IsReported;
                data.IsSold = dt.IsSold;
                data.Price = dt.Price;
                data.SellerId = dt.SellerId;
                data.SubCategoryId = dt.SubCategoryId;
                data.Title = dt.Title;
                data.CreatedDate = dt.CreatedDate;

                var pv = await _Context.ProdViews.Where(a => a.PId == dt.Id).ToListAsync();
                List<ProdViewsDto> pvLst = new();
                foreach (var itm1 in pv)
                {
                    ProdViewsDto dt1 = new();
                    dt1.Id = itm1.Id;
                    dt1.UserId = itm1.UserId;
                    dt1.PId = itm1.PId;
                    pvLst.Add(dt1);
                }
                data.ProdViews = pvLst;

                var imgs = await _Context.ProdImages.Where(a => a.PId == dt.Id).ToListAsync();

                List<ProdImagesDto> lst = new();

                foreach (var item1 in imgs)
                {
                    ProdImagesDto prod = new();
                    prod.Id = item1.Id;
                    prod.Image = item1.Image;
                    prod.PId = item1.PId;
                    lst.Add(prod);
                }

                data.ProdImages = lst;

                List<ProvinceDto> lstA = new();
                var provnc = await _Context.ProdProvinces.Where(a => a.Pid == dt.Id).ToListAsync();
                foreach (var itemA in provnc)
                {
                    var d = await _Context.Provinces.Where(a => a.Id == itemA.ProvinceId).FirstOrDefaultAsync();

                    List<MunicipalityDto> mnclst = new();
                    ProvinceDto dtoA = new();
                    dtoA.Id = itemA.ProvinceId;
                    dtoA.Name = d.Name;
                    var prodMunc = await _Context.ProdMunicipalities.Where(a => a.Pid == dt.Id).ToListAsync();
                    foreach (var itm in prodMunc)
                    {
                        var sd = await _Context.Municipalities.Where(a => a.Id == itm.MncId).FirstOrDefaultAsync();
                        if (sd.PrvId == d.Id)
                        {
                            MunicipalityDto dto1 = new();
                            dto1.Id = sd.Id;
                            dto1.Name = sd.Name;
                            dto1.Time = sd.Time;
                            mnclst.Add(dto1);
                        }
                    }
                    dtoA.Municipalitiees = mnclst;
                    lstA.Add(dtoA);
                }

                data.provinceDtos = lstA;

                return data;
            }
            catch (Exception)
            {
                return null;
                throw;
            }

            
        }
        public async Task<List<Productdto>> GetProductSellerId(int sellerId)
        {
            try
            {
                List<Productdto> lstp = new();

                var dt = await _Context.Products.Where(a => a.SellerId == sellerId && a.IsActive == true).ToListAsync();
                foreach (var item in dt)
                {
                    Productdto data = new();
                    data.Id = item.Id;
                    data.IsDelivering = item.IsDelivering;
                    data.CategoryId = item.CategoryId;
                    data.Condition = item.Condition;
                    data.Description = item.Description;
                    data.FireOnPrice = item.FireOnPrice;
                    data.IsPickup = item.IsPickup;
                    data.IsPromoted = item.IsPromoted;
                    data.IsReported = item.IsReported;
                    data.IsSold = item.IsSold;
                    data.Price = item.Price;
                    data.SellerId = item.SellerId;
                    data.SubCategoryId = item.SubCategoryId;
                    data.Title = item.Title;
                    data.CreatedDate = item.CreatedDate;
                    //data.Provice = await _Context.ProdProvinces.Where(a => a.Pid == id).ToListAsync();
                    //data.Municipalities = await _Context.ProdMunicipalities.Where(a => a.Pid == id).ToListAsync();
                    //data.ProdViews = await _Context.ProdViews.Where(a => a.PId == item.Id).ToListAsync();

                    var pv = await _Context.ProdViews.Where(a => a.PId == item.Id).ToListAsync();
                    List<ProdViewsDto> pvLst = new();
                    foreach (var itm1 in pv)
                    {
                        ProdViewsDto dt1 = new();
                        dt1.Id = itm1.Id;
                        dt1.UserId = itm1.UserId;
                        dt1.PId = itm1.PId;
                        pvLst.Add(dt1);
                    }
                    data.ProdViews = pvLst;

                    var imgs = await _Context.ProdImages.Where(a => a.PId == item.Id).ToListAsync();

                    List<ProdImagesDto> lst = new();

                    foreach (var item1 in imgs)
                    {
                        ProdImagesDto prod = new();
                        prod.Id = item1.Id;
                        prod.Image = item1.Image;
                        prod.PId = item1.PId;
                        lst.Add(prod);
                    }

                    data.ProdImages = lst;

                    List<ProvinceDto> lstA = new();
                    var provnc = await _Context.ProdProvinces.Where(a => a.Pid == item.Id).ToListAsync();
                    foreach (var itemA in provnc)
                    {
                        var d = await _Context.Provinces.Where(a => a.Id == itemA.ProvinceId).FirstOrDefaultAsync();

                        List<MunicipalityDto> mnclst = new();
                        ProvinceDto dtoA = new();
                        dtoA.Id = itemA.ProvinceId;
                        dtoA.Name = d.Name;
                        var prodMunc = await _Context.ProdMunicipalities.Where(a => a.Pid == item.Id).ToListAsync();
                        foreach (var itm in prodMunc)
                        {
                            var sd = await _Context.Municipalities.Where(a => a.Id == itm.MncId).FirstOrDefaultAsync();
                            if (sd.PrvId == d.Id)
                            {
                                MunicipalityDto dto1 = new();
                                dto1.Id = sd.Id;
                                dto1.Name = sd.Name;
                                dto1.Time = sd.Time;
                                mnclst.Add(dto1);
                            }
                        }
                        dtoA.Municipalitiees = mnclst;
                        lstA.Add(dtoA);
                    }

                    data.provinceDtos = lstA;

                    lstp.Add(data);

                }
                return lstp;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
            
        }
        public async Task<List<Productdto>> GetProductCategoryId(int id, int uid)
        {
            try
            {
                List<Productdto> lstp = new();
                var usr = _Context.Users.Where(a => a.Id == uid).FirstOrDefault();

                var dt = from t1 in _Context.Products
                           join t3 in _Context.ProdProvinces on t1.Id equals t3.Pid
                           join t4 in _Context.ProdMunicipalities on t1.Id equals t4.Pid
                           where t1.CategoryId == id && t1.IsActive == true && t3.ProvinceId == usr.ProvinceId && t4.MncId == usr.MunicipalityId && t1.IsSold != true
                           select new { t1 };

                //var dt = await _Context.Products.Where(a => a.CategoryId == id && a.IsActive == true && a.IsSold != true).ToListAsync();
                
                foreach (var item in dt.ToList())
                {
                    Productdto data = new();
                    data.Id = item.t1.Id;
                    data.IsDelivering = item.t1.IsDelivering;
                    data.CategoryId = item.t1.CategoryId;
                    data.Condition = item.t1.Condition;
                    data.Description = item.t1.Description;
                    data.FireOnPrice = item.t1.FireOnPrice;
                    data.IsPickup = item.t1.IsPickup;
                    data.IsPromoted = item.t1.IsPromoted;
                    data.IsReported = item.t1.IsReported;
                    data.IsSold = item.t1.IsSold;
                    data.Price = item.t1.Price;
                    data.SellerId = item.t1.SellerId;
                    data.SubCategoryId = item.t1.SubCategoryId;
                    data.Title = item.t1.Title;
                    data.CreatedDate = item.t1.CreatedDate;
                    //data.Provice = await _Context.ProdProvinces.Where(a => a.Pid == id).ToListAsync();
                    //data.Municipalities = await _Context.ProdMunicipalities.Where(a => a.Pid == id).ToListAsync();
                    //data.ProdViews = await _Context.ProdViews.Where(a => a.PId == item.Id).ToListAsync();
                    var pv = await _Context.ProdViews.Where(a => a.PId == item.t1.Id).ToListAsync();
                    List<ProdViewsDto> pvLst = new();
                    foreach (var itm1 in pv)
                    {
                        ProdViewsDto dt1 = new();
                        dt1.Id = itm1.Id;
                        dt1.UserId = itm1.UserId;
                        dt1.PId = itm1.PId;
                        pvLst.Add(dt1);
                    }
                    data.ProdViews = pvLst;
                    var imgs = await _Context.ProdImages.Where(a => a.PId == item.t1.Id).ToListAsync();

                    List<ProdImagesDto> lst = new();

                    foreach (var item1 in imgs)
                    {
                        ProdImagesDto prod = new();
                        prod.Id = item1.Id;
                        prod.Image = item1.Image;
                        prod.PId = item1.PId;
                        lst.Add(prod);
                    }

                    data.ProdImages = lst;

                    List<ProvinceDto> lstA = new();
                    var provnc = await _Context.ProdProvinces.Where(a => a.Pid == item.t1.Id).ToListAsync();
                    foreach (var itemA in provnc)
                    {
                        var d = await _Context.Provinces.Where(a => a.Id == itemA.ProvinceId).FirstOrDefaultAsync();

                        List<MunicipalityDto> mnclst = new();
                        ProvinceDto dtoA = new();
                        dtoA.Id = itemA.ProvinceId;
                        dtoA.Name = d.Name;
                        var prodMunc = await _Context.ProdMunicipalities.Where(a => a.Pid == item.t1.Id).ToListAsync();
                        foreach (var itm in prodMunc)
                        {
                            var sd = await _Context.Municipalities.Where(a => a.Id == itm.MncId).FirstOrDefaultAsync();
                            if (sd.PrvId == d.Id)
                            {
                                MunicipalityDto dto1 = new();
                                dto1.Id = sd.Id;
                                dto1.Name = sd.Name;
                                dto1.Time = sd.Time;
                                mnclst.Add(dto1);
                            }
                        }
                        dtoA.Municipalitiees = mnclst;
                        lstA.Add(dtoA);
                    }

                    data.provinceDtos = lstA;

                    lstp.Add(data);
                }
                
                return lstp;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public async Task<List<Productdto>> GetProductSubCategoryId(int id, int uid)
        {
            try
            {
                List<Productdto> lstp = new();

                var usr = _Context.Users.Where(a => a.Id == uid).FirstOrDefault();

                var dt = from t1 in _Context.Products
                         join t3 in _Context.ProdProvinces on t1.Id equals t3.Pid
                         join t4 in _Context.ProdMunicipalities on t1.Id equals t4.Pid
                         where t1.SubCategoryId == id && t1.IsActive == true && t3.ProvinceId == usr.ProvinceId && t4.MncId == usr.MunicipalityId && t1.IsSold != true
                         select new { t1 };

                //var dt = await _Context.Products.Where(a => a.SubCategoryId == id && a.IsActive == true && a.IsSold != true).ToListAsync();
                foreach (var item in dt.ToList())
                {
                    Productdto data = new();
                    data.Id = item.t1.Id;
                    data.IsDelivering = item.t1.IsDelivering;
                    data.CategoryId = item.t1.CategoryId;
                    data.Condition = item.t1.Condition;
                    data.Description = item.t1.Description;
                    data.FireOnPrice = item.t1.FireOnPrice;
                    data.IsPickup = item.t1.IsPickup;
                    data.IsPromoted = item.t1.IsPromoted;
                    data.IsReported = item.t1.IsReported;
                    data.IsSold = item.t1.IsSold;
                    data.Price = item.t1.Price;
                    data.SellerId = item.t1.SellerId;
                    data.SubCategoryId = item.t1.SubCategoryId;
                    data.Title = item.t1.Title;
                    data.CreatedDate = item.t1.CreatedDate;
                    //data.Provice = await _Context.ProdProvinces.Where(a => a.Pid == id).ToListAsync();
                    //data.Municipalities = await _Context.ProdMunicipalities.Where(a => a.Pid == id).ToListAsync();
                    //data.ProdViews = await _Context.ProdViews.Where(a => a.PId == item.Id).ToListAsync();

                    var pv = await _Context.ProdViews.Where(a => a.PId == item.t1.Id).ToListAsync();
                    List<ProdViewsDto> pvLst = new();
                    foreach (var itm1 in pv)
                    {
                        ProdViewsDto dt1 = new();
                        dt1.Id = itm1.Id;
                        dt1.UserId = itm1.UserId;
                        dt1.PId = itm1.PId;
                        pvLst.Add(dt1);
                    }
                    data.ProdViews = pvLst;

                    var imgs = await _Context.ProdImages.Where(a => a.PId == item.t1.Id).ToListAsync();

                    List<ProdImagesDto> lst = new();

                    foreach (var item1 in imgs)
                    {
                        ProdImagesDto prod = new();
                        prod.Id = item1.Id;
                        prod.Image = item1.Image;
                        prod.PId = item1.PId;
                        lst.Add(prod);
                    }

                    data.ProdImages = lst;

                    List<ProvinceDto> lstA = new();
                    var provnc = await _Context.ProdProvinces.Where(a => a.Pid == item.t1.Id).ToListAsync();
                    foreach (var itemA in provnc)
                    {
                        var d = await _Context.Provinces.Where(a => a.Id == itemA.ProvinceId).FirstOrDefaultAsync();

                        List<MunicipalityDto> mnclst = new();
                        ProvinceDto dtoA = new();
                        dtoA.Id = itemA.ProvinceId;
                        dtoA.Name = d.Name;
                        var prodMunc = await _Context.ProdMunicipalities.Where(a => a.Pid == item.t1.Id).ToListAsync();
                        foreach (var itm in prodMunc)
                        {
                            var sd = await _Context.Municipalities.Where(a => a.Id == itm.MncId).FirstOrDefaultAsync();
                            if (sd.PrvId == d.Id)
                            {
                                MunicipalityDto dto1 = new();
                                dto1.Id = sd.Id;
                                dto1.Name = sd.Name;
                                dto1.Time = sd.Time;
                                mnclst.Add(dto1);
                            }
                        }
                        dtoA.Municipalitiees = mnclst;
                        lstA.Add(dtoA);
                    }

                    data.provinceDtos = lstA;

                    lstp.Add(data);
                }
                
                return lstp;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public async Task<string> DeleteProduct(int id)
        {
            try
            {
                var dt = await _Context.Products.Where(a => a.Id == id).FirstOrDefaultAsync();
                dt.IsActive = false;
                _Context.Products.Update(dt);
                _Context.SaveChanges();
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
                throw;
            }
            
        }
        public async Task<Productdto> ProductSold(SoldDto dto)
        {
            try
            {
                var dt1 = await _Context.Products.Where(a => a.Id == dto.Id).FirstOrDefaultAsync();
                dt1.IsSold = dto.IsSold;
                _Context.Products.Update(dt1);
                _Context.SaveChanges();

                Productdto data = new();
                var dt = await _Context.Products.Where(a => a.Id == dto.Id && a.IsActive == true).FirstOrDefaultAsync();
                data.Id = dt.Id;
                data.IsDelivering = dt.IsDelivering;
                data.CategoryId = dt.CategoryId;
                data.Condition = dt.Condition;
                data.Description = dt.Description;
                data.FireOnPrice = dt.FireOnPrice;
                data.IsPickup = dt.IsPickup;
                data.IsPromoted = dt.IsPromoted;
                data.IsReported = dt.IsReported;
                data.IsSold = dt.IsSold;
                data.Price = dt.Price;
                data.SellerId = dt.SellerId;
                data.SubCategoryId = dt.SubCategoryId;
                data.Title = dt.Title;
                data.CreatedDate = dt.CreatedDate;
                //data.Provice = await _Context.ProdProvinces.Where(a => a.Pid == dto.Id).ToListAsync();
                //data.Municipalities = await _Context.ProdMunicipalities.Where(a => a.Pid == dto.Id).ToListAsync();
                //data.ProdViews = await _Context.ProdViews.Where(a => a.PId == dt.Id).ToListAsync();
                var pv = await _Context.ProdViews.Where(a => a.PId == dt.Id).ToListAsync();
                List<ProdViewsDto> pvLst = new();
                foreach (var itm1 in pv)
                {
                    ProdViewsDto dta = new();
                    dta.Id = itm1.Id;
                    dta.UserId = itm1.UserId;
                    dta.PId = itm1.PId;
                    pvLst.Add(dta);
                }
                data.ProdViews = pvLst;
                var imgs = await _Context.ProdImages.Where(a => a.PId == dt.Id).ToListAsync();

                List<ProdImagesDto> lst = new();

                foreach (var item1 in imgs)
                {
                    ProdImagesDto prod = new();
                    prod.Id = item1.Id;
                    prod.Image = item1.Image;
                    prod.PId = item1.PId;
                    lst.Add(prod);
                }

                data.ProdImages = lst;
                List<ProvinceDto> lstA = new();
                var provnc = await _Context.ProdProvinces.Where(a => a.Pid == dt.Id).ToListAsync();
                foreach (var itemA in provnc)
                {
                    var d = await _Context.Provinces.Where(a => a.Id == itemA.ProvinceId).FirstOrDefaultAsync();

                    List<MunicipalityDto> mnclst = new();
                    ProvinceDto dtoA = new();
                    dtoA.Id = itemA.ProvinceId;
                    dtoA.Name = d.Name;
                    var prodMunc = await _Context.ProdMunicipalities.Where(a => a.Pid == dt.Id).ToListAsync();
                    foreach (var itm in prodMunc)
                    {
                        var sd = await _Context.Municipalities.Where(a => a.Id == itm.MncId).FirstOrDefaultAsync();
                        if (sd.PrvId == d.Id)
                        {
                            MunicipalityDto dto1 = new();
                            dto1.Id = sd.Id;
                            dto1.Name = sd.Name;
                            dto1.Time = sd.Time;
                            mnclst.Add(dto1);
                        }
                    }
                    dtoA.Municipalitiees = mnclst;
                    lstA.Add(dtoA);
                }

                data.provinceDtos = lstA;
                return data;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }

        }
        public async Task<Productdto> ProductReport(ReportDto dto)
        {
            try
            {
                var dt1 = await _Context.Products.Where(a => a.Id == dto.Id).FirstOrDefaultAsync();
                dt1.IsReported = dto.IsReport;
                _Context.Products.Update(dt1);
                _Context.SaveChanges();

                Productdto data = new();
                var dt = await _Context.Products.Where(a => a.Id == dto.Id && a.IsActive == true).FirstOrDefaultAsync();
                data.Id = dt.Id;
                data.IsDelivering = dt.IsDelivering;
                data.CategoryId = dt.CategoryId;
                data.Condition = dt.Condition;
                data.Description = dt.Description;
                data.FireOnPrice = dt.FireOnPrice;
                data.IsPickup = dt.IsPickup;
                data.IsPromoted = dt.IsPromoted;
                data.IsReported = dt.IsReported;
                data.IsSold = dt.IsSold;
                data.Price = dt.Price;
                data.SellerId = dt.SellerId;
                data.SubCategoryId = dt.SubCategoryId;
                data.Title = dt.Title;
                data.CreatedDate = dt.CreatedDate;
                //data.Provice = await _Context.ProdProvinces.Where(a => a.Pid == dto.Id).ToListAsync();
                //data.Municipalities = await _Context.ProdMunicipalities.Where(a => a.Pid == dto.Id).ToListAsync();
                //data.ProdViews = await _Context.ProdViews.Where(a => a.PId == dt.Id).ToListAsync();

                var pv = await _Context.ProdViews.Where(a => a.PId == dt.Id).ToListAsync();
                List<ProdViewsDto> pvLst = new();
                foreach (var itm1 in pv)
                {
                    ProdViewsDto dta = new();
                    dta.Id = itm1.Id;
                    dta.UserId = itm1.UserId;
                    dta.PId = itm1.PId;
                    pvLst.Add(dta);
                }
                data.ProdViews = pvLst;

                var imgs = await _Context.ProdImages.Where(a => a.PId == dt.Id).ToListAsync();

                List<ProdImagesDto> lst = new();

                foreach (var item1 in imgs)
                {
                    ProdImagesDto prod = new();
                    prod.Id = item1.Id;
                    prod.Image = item1.Image;
                    prod.PId = item1.PId;
                    lst.Add(prod);
                }

                data.ProdImages = lst;

                List<ProvinceDto> lstA = new();
                var provnc = await _Context.ProdProvinces.Where(a => a.Pid == dt.Id).ToListAsync();
                foreach (var itemA in provnc)
                {
                    var d = await _Context.Provinces.Where(a => a.Id == itemA.ProvinceId).FirstOrDefaultAsync();

                    List<MunicipalityDto> mnclst = new();
                    ProvinceDto dtoA = new();
                    dtoA.Id = itemA.ProvinceId;
                    dtoA.Name = d.Name;
                    var prodMunc = await _Context.ProdMunicipalities.Where(a => a.Pid == dt.Id).ToListAsync();
                    foreach (var itm in prodMunc)
                    {
                        var sd = await _Context.Municipalities.Where(a => a.Id == itm.MncId).FirstOrDefaultAsync();
                        if (sd.PrvId == d.Id)
                        {
                            MunicipalityDto dto1 = new();
                            dto1.Id = sd.Id;
                            dto1.Name = sd.Name;
                            dto1.Time = sd.Time;
                            mnclst.Add(dto1);
                        }
                    }
                    dtoA.Municipalitiees = mnclst;
                    lstA.Add(dtoA);
                }

                data.provinceDtos = lstA;

                return data;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }

        }
        public async Task<Productdto> SetCountOfViewProduct(int id, int pid)
        {
            try
            {
                var dt11 = await _Context.ProdViews.Where(a => a.PId == pid && a.UserId == id).FirstOrDefaultAsync();

                if (dt11 == null)
                {
                    ProdView prod = new();
                    prod.PId = pid;
                    prod.UserId = id;
                    await _Context.ProdViews.AddAsync(prod);
                    _Context.SaveChanges();
                }

                Productdto data = new();
                var dt = await _Context.Products.Where(a => a.Id == pid && a.IsActive == true).FirstOrDefaultAsync();
                if (dt != null)
                {
                    data.Id = dt.Id;
                    data.IsDelivering = dt.IsDelivering;
                    data.CategoryId = dt.CategoryId;
                    data.Condition = dt.Condition;
                    data.Description = dt.Description;
                    data.FireOnPrice = dt.FireOnPrice;
                    data.IsPickup = dt.IsPickup;
                    data.IsPromoted = dt.IsPromoted;
                    data.IsReported = dt.IsReported;
                    data.IsSold = dt.IsSold;
                    data.Price = dt.Price;
                    data.SellerId = dt.SellerId;
                    data.SubCategoryId = dt.SubCategoryId;
                    data.Title = dt.Title;
                    data.CreatedDate = dt.CreatedDate;
                    //data.Provice = await _Context.ProdProvinces.Where(a => a.Pid == id).ToListAsync();
                    //data.Municipalities = await _Context.ProdMunicipalities.Where(a => a.Pid == id).ToListAsync();
                    //data.ProdViews = await _Context.ProdViews.Where(a => a.PId == dt.Id).ToListAsync();

                    var pv = await _Context.ProdViews.Where(a => a.PId == dt.Id).ToListAsync();
                    List<ProdViewsDto> pvLst = new();
                    foreach (var itm1 in pv)
                    {
                        ProdViewsDto dt1 = new();
                        dt1.Id = itm1.Id;
                        dt1.UserId = itm1.UserId;
                        dt1.PId = itm1.PId;
                        pvLst.Add(dt1);
                    }
                    data.ProdViews = pvLst;

                    var imgs = await _Context.ProdImages.Where(a => a.PId == dt.Id).ToListAsync();

                    List<ProdImagesDto> lst = new();

                    foreach (var item1 in imgs)
                    {
                        ProdImagesDto prod = new();
                        prod.Id = item1.Id;
                        prod.Image = item1.Image;
                        prod.PId = item1.PId;
                        lst.Add(prod);
                    }

                    data.ProdImages = lst;

                    List<ProvinceDto> lstA = new();
                    var provnc = await _Context.ProdProvinces.Where(a => a.Pid == dt.Id).ToListAsync();
                    foreach (var itemA in provnc)
                    {
                        var d = await _Context.Provinces.Where(a => a.Id == itemA.ProvinceId).FirstOrDefaultAsync();

                        List<MunicipalityDto> mnclst = new();
                        ProvinceDto dtoA = new();
                        dtoA.Id = itemA.ProvinceId;
                        dtoA.Name = d.Name;
                        var prodMunc = await _Context.ProdMunicipalities.Where(a => a.Pid == dt.Id).ToListAsync();
                        foreach (var itm in prodMunc)
                        {
                            var sd = await _Context.Municipalities.Where(a => a.Id == itm.MncId).FirstOrDefaultAsync();
                            if (sd.PrvId == d.Id)
                            {
                                MunicipalityDto dto1 = new();
                                dto1.Id = sd.Id;
                                dto1.Name = sd.Name;
                                dto1.Time = sd.Time;
                                mnclst.Add(dto1);
                            }
                        }
                        dtoA.Municipalitiees = mnclst;
                        lstA.Add(dtoA);
                    }

                    data.provinceDtos = lstA;

                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }

        public async Task<List<Productdto>> GetFavProducts(int id)
        {
            try
            {
                var data = from t1 in _Context.Products
                           join t2 in _Context.Favorites on t1.Id equals t2.Pid
                           where t2.UserId == id && t1.IsActive == true
                           select t1;

                List<Productdto> lstP = new();

                foreach (var item in data.ToList())
                {
                    
                    Productdto dataP = new();
                    var dt = await _Context.Products.Where(a => a.Id == item.Id && a.IsActive == true).FirstOrDefaultAsync();
                    dataP.Id = dt.Id;
                    dataP.IsDelivering = dt.IsDelivering;
                    dataP.CategoryId = dt.CategoryId;
                    dataP.Condition = dt.Condition;
                    dataP.Description = dt.Description;
                    dataP.FireOnPrice = dt.FireOnPrice;
                    dataP.IsPickup = dt.IsPickup;
                    dataP.IsPromoted = dt.IsPromoted;
                    dataP.IsReported = dt.IsReported;
                    dataP.IsSold = dt.IsSold;
                    dataP.Price = dt.Price;
                    dataP.SellerId = dt.SellerId;
                    dataP.SubCategoryId = dt.SubCategoryId;
                    dataP.Title = dt.Title;
                    dataP.CreatedDate = dt.CreatedDate;
                    //dataP.Provice = await _Context.ProdProvinces.Where(a => a.Pid == item.Id).ToListAsync();
                    //dataP.Municipalities = await _Context.ProdMunicipalities.Where(a => a.Pid == item.Id).ToListAsync();
                    //dataP.ProdViews = await _Context.ProdViews.Where(a => a.PId == dt.Id).ToListAsync();

                    var pv = await _Context.ProdViews.Where(a => a.PId == dt.Id).ToListAsync();
                    List<ProdViewsDto> pvLst = new();
                    foreach (var itm1 in pv)
                    {
                        ProdViewsDto dt1 = new();
                        dt1.Id = itm1.Id;
                        dt1.UserId = itm1.UserId;
                        dt1.PId = itm1.PId;
                        pvLst.Add(dt1);
                    }
                    dataP.ProdViews = pvLst;

                    var imgs = await _Context.ProdImages.Where(a => a.PId == dt.Id).ToListAsync();

                    List<ProdImagesDto> lst = new();

                    foreach (var item1 in imgs)
                    {
                        ProdImagesDto prod = new();
                        prod.Id = item1.Id;
                        prod.Image = item1.Image;
                        prod.PId = item1.PId;
                        lst.Add(prod);
                    }

                    dataP.ProdImages = lst;

                    List<ProvinceDto> lstA = new();
                    var provnc = await _Context.ProdProvinces.Where(a => a.Pid == item.Id).ToListAsync();
                    foreach (var itemA in provnc)
                    {
                        var d = await _Context.Provinces.Where(a => a.Id == itemA.ProvinceId).FirstOrDefaultAsync();

                        List<MunicipalityDto> mnclst = new();
                        ProvinceDto dtoA = new();
                        dtoA.Id = itemA.ProvinceId;
                        dtoA.Name = d.Name;
                        var prodMunc = await _Context.ProdMunicipalities.Where(a => a.Pid == item.Id).ToListAsync();
                        foreach (var itm in prodMunc)
                        {
                            var sd = await _Context.Municipalities.Where(a => a.Id == itm.MncId).FirstOrDefaultAsync();
                            if (sd.PrvId == d.Id)
                            {
                                MunicipalityDto dto1 = new();
                                dto1.Id = sd.Id;
                                dto1.Name = sd.Name;
                                dto1.Time = sd.Time;
                                mnclst.Add(dto1);
                            }
                        }
                        dtoA.Municipalitiees = mnclst;
                        lstA.Add(dtoA);
                    }

                    dataP.provinceDtos = lstA;

                    lstP.Add(dataP);
                }

                return lstP;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }

        public async Task<List<Productdto>> SearchProducts(string Search, int id)
        {
            try
            {
                var usr = _Context.Users.Where(a => a.Id == id).FirstOrDefault();

                var data = from t1 in _Context.Products
                          join t3 in _Context.ProdProvinces on t1.Id equals t3.Pid
                          join t4 in _Context.ProdMunicipalities on t1.Id equals t4.Pid
                          where t1.Title.Contains(Search) && t1.IsActive == true && t3.ProvinceId == usr.ProvinceId && t4.MncId == usr.MunicipalityId && t1.IsSold != true
                          select new { t1 };

                //var data = await _Context.Products.Where(a => a.Title.Contains(Search) && a.IsActive == true && a.IsSold != true).ToListAsync();

                List<Productdto> lstP = new();

                foreach (var item in data.ToList())
                {

                    Productdto dataP = new();
                    var dt = await _Context.Products.Where(a => a.Id == item.t1.Id && a.IsActive == true).FirstOrDefaultAsync();
                    if (dt != null)
                    {
                        dataP.Id = dt.Id;
                        dataP.IsDelivering = dt.IsDelivering;
                        dataP.CategoryId = dt.CategoryId;
                        dataP.Condition = dt.Condition;
                        dataP.Description = dt.Description;
                        dataP.FireOnPrice = dt.FireOnPrice;
                        dataP.IsPickup = dt.IsPickup;
                        dataP.IsPromoted = dt.IsPromoted;
                        dataP.IsReported = dt.IsReported;
                        dataP.IsSold = dt.IsSold;
                        dataP.Price = dt.Price;
                        dataP.SellerId = dt.SellerId;
                        dataP.SubCategoryId = dt.SubCategoryId;
                        dataP.Title = dt.Title;
                        dataP.CreatedDate = dt.CreatedDate;
                        //dataP.Provice = await _Context.ProdProvinces.Where(a => a.Pid == item.Id).ToListAsync();
                        //dataP.Municipalities = await _Context.ProdMunicipalities.Where(a => a.Pid == item.Id).ToListAsync();
                        //dataP.ProdViews = await _Context.ProdViews.Where(a => a.PId == dt.Id).ToListAsync();

                        var pv = await _Context.ProdViews.Where(a => a.PId == dt.Id).ToListAsync();
                        List<ProdViewsDto> pvLst = new();
                        foreach (var itm1 in pv)
                        {
                            ProdViewsDto dt1 = new();
                            dt1.Id = itm1.Id;
                            dt1.UserId = itm1.UserId;
                            dt1.PId = itm1.PId;
                            pvLst.Add(dt1);
                        }
                        dataP.ProdViews = pvLst;

                        var imgs = await _Context.ProdImages.Where(a => a.PId == dt.Id).ToListAsync();

                        List<ProdImagesDto> lst = new();

                        foreach (var item1 in imgs)
                        {
                            ProdImagesDto prod = new();
                            prod.Id = item1.Id;
                            prod.Image = item1.Image;
                            prod.PId = item1.PId;
                            lst.Add(prod);
                        }

                        dataP.ProdImages = lst;

                        List<ProvinceDto> lstA = new();
                        var provnc = await _Context.ProdProvinces.Where(a => a.Pid == item.t1.Id).ToListAsync();
                        foreach (var itemA in provnc)
                        {
                            var d = await _Context.Provinces.Where(a => a.Id == itemA.ProvinceId).FirstOrDefaultAsync();

                            List<MunicipalityDto> mnclst = new();
                            ProvinceDto dtoA = new();
                            dtoA.Id = itemA.ProvinceId;
                            dtoA.Name = d.Name;
                            var prodMunc = await _Context.ProdMunicipalities.Where(a => a.Pid == item.t1.Id).ToListAsync();
                            foreach (var itm in prodMunc)
                            {
                                var sd = await _Context.Municipalities.Where(a => a.Id == itm.MncId).FirstOrDefaultAsync();
                                if (sd.PrvId == d.Id)
                                {
                                    MunicipalityDto dto1 = new();
                                    dto1.Id = sd.Id;
                                    dto1.Name = sd.Name;
                                    dto1.Time = sd.Time;
                                    mnclst.Add(dto1);
                                }
                            }
                            dtoA.Municipalitiees = mnclst;
                            lstA.Add(dtoA);
                        }

                        dataP.provinceDtos = lstA;

                        lstP.Add(dataP);
                    }
                    
                }

                return lstP;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }

        public async Task<List<ImagesDto>> GetImagesByPId(int id)
        {
            var dt = await _Context.ProdImages.Where(a => a.PId == id).ToListAsync();
                List<ImagesDto> lst = new();
            foreach (var item in dt)
            {
                ImagesDto dto = new();
                dto.Id = item.Id;
                dto.Pid = item.PId;
                dto.Image = item.Image;
                lst.Add(dto);
            }
            return lst;
        }
    }
}
