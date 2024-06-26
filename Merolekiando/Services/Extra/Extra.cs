﻿using Merolekando.Common;
using Merolekando.Models;
using Merolekando.Models.Dtos;
using Merolekiando.Hubs;
using Merolekiando.Models;
using Merolekiando.Models.Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Merolekando.Services.Extra
{
    public class Extra : IExtra
    {
        private readonly MerolikandoDBContext _Context;
        public static IWebHostEnvironment _environment;
        //private readonly ChatHub _chatHub;
        public Extra(MerolikandoDBContext Context, IWebHostEnvironment environment/*, ChatHub chatHub*/)
        {
            _Context = Context;
            _environment = environment;
            //_chatHub = chatHub;
        }
        public async Task<string> ManageBanner(BannerDto banner)
        {
            try
            {
                var filename1 = "";
                Random rnd = new();
                var rn = rnd.Next(111, 999);

                if (banner.Id > 0)
                {
                    var bner = await _Context.Banners.Where(a => a.Id == banner.Id).FirstOrDefaultAsync();
                    bner.Description = banner.Description;
                    bner.IsActive = true;
                    bner.Name = banner.Name;

                    if (banner.Image != null)
                    {
                        var ImagePath1 = rn + Methods.RemoveWhitespace(banner.Image.FileName);
                        var pathh = "";
                        using (FileStream fileStream = File.Create(_environment.ContentRootPath + "\\Resources\\Images\\Banner\\" + ImagePath1))
                        {
                            banner.Image.CopyTo(fileStream);
                            pathh = Path.Combine(_environment.ContentRootPath, "/Resources/Images/Banner/" + ImagePath1);
                            filename1 = ImagePath1;
                            fileStream.Flush();
                        }
                        bner.Image = "/Resources/Images/Banner/" + filename1;
                    }
                    else
                    {
                        bner.Image = "";
                    }
                    _Context.Banners.Update(bner);
                    _Context.SaveChanges();
                    return "Success";
                }
                else
                {

                    Banner bner = new();

                    bner.Description = banner.Description;
                    bner.IsActive = true;
                    bner.Name = banner.Name;

                    if (banner.Image != null)
                    {
                        var ImagePath1 = rn + Methods.RemoveWhitespace(banner.Image.FileName);
                        var pathh = "";
                        using (FileStream fileStream = File.Create(_environment.ContentRootPath + "\\Resources\\Images\\Products\\" + ImagePath1))
                        {
                            banner.Image.CopyTo(fileStream);
                            pathh = Path.Combine(_environment.ContentRootPath, "/Resources/Images/Products/" + ImagePath1);
                            filename1 = ImagePath1;
                            fileStream.Flush();
                        }
                        bner.Image = "/Resources/Images/Products/" + filename1;
                    }
                    else
                    {
                        bner.Image = "";
                    }

                    await _Context.Banners.AddAsync(bner);
                    _Context.SaveChanges();
                    return "Success";
                }


            }
            catch (Exception ex)
            {
                return ex.Message;
                throw;
            }
        }

        public async Task<string> ManageCategory(Category category)
        {
            try
            {
                if (category.Id > 0)
                {
                    var dt = await _Context.Categories.Where(a => a.Id == category.Id).FirstOrDefaultAsync();
                    dt.Name = category.Name;
                    _Context.Categories.Update(dt);
                    _Context.SaveChanges();
                    return "Success";
                }
                else
                {
                    category.Time = DateTime.Now.Millisecond;
                    await _Context.Categories.AddAsync(category);
                    _Context.SaveChanges();
                    return "Success";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
                throw;
            }
        }

        public async Task<string> ManageMunicipality(Municipality municipality)
        {
            try
            {
                if (municipality.Id > 0)
                {
                    var dt = await _Context.Municipalities.Where(a => a.Id == municipality.Id).FirstOrDefaultAsync();
                    dt.PrvId = municipality.PrvId;
                    dt.Name = municipality.Name;
                    _Context.Municipalities.Update(dt);
                    _Context.SaveChanges();
                    return "Success";
                }
                else
                {
                    municipality.Time = DateTime.Now.Millisecond;
                    await _Context.Municipalities.AddAsync(municipality);
                    _Context.SaveChanges();
                    return "Success";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
                throw;
            }
        }

        public async Task<string> ManageProvince(Province province)
        {
            try
            {
                if (province.Id > 0)
                {
                    var dt = await _Context.Provinces.Where(a => a.Id == province.Id).FirstOrDefaultAsync();
                    dt.Name = province.Name;
                    _Context.Provinces.Update(dt);
                    _Context.SaveChanges();
                    return "Success";
                }
                else
                {
                    province.Time = DateTime.Now.Millisecond;
                    await _Context.Provinces.AddAsync(province);
                    _Context.SaveChanges();
                    return "Success";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
                throw;
            }
        }

        public async Task<string> ManageSubCategory(SubCategory category)
        {
            try
            {
                if (category.Id > 0)
                {
                    var dt = await _Context.SubCategories.Where(a => a.Id == category.Id).FirstOrDefaultAsync();
                    dt.CatId = category.CatId;
                    dt.Name = category.Name;
                    _Context.SubCategories.Update(dt);
                    _Context.SaveChanges();
                    return "Success";
                }
                else
                {
                    await _Context.SubCategories.AddAsync(category);
                    _Context.SaveChanges();
                    return "Success";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
                throw;
            }
        }

        public async Task<List<ProvinceDto>> GetProvinces()
        {

            var dt = _Context.Provinces.ToList();
            List<ProvinceDto> lst = new();

            foreach (var item in dt)
            {
                List<MunicipalityDto> mnclst = new();
                ProvinceDto dto = new();
                dto.Id = item.Id;
                dto.Name = item.Name;
                dto.Time = item.Time;
                var dd = _Context.Municipalities.Where(a => a.PrvId == item.Id).ToList();
                foreach (var itm in dd)
                {
                    if (itm.PrvId == dto.Id)
                    {
                        MunicipalityDto dto1 = new();
                        dto1.Id = itm.Id;
                        dto1.Name = itm.Name;
                        dto1.Time = itm.Time;
                        mnclst.Add(dto1);
                    }
                }
                dto.Municipalitiees = mnclst;
                if (dd.Count > 0)
                {
                    lst.Add(dto);
                }
            }

            return lst;
        }

        public async Task<Province> GetProvinceById(int id)
        {
            var dt = await _Context.Provinces.Where(a => a.Id == id).FirstOrDefaultAsync();
            return dt;
        }

        public async Task<List<Municipality>> GetMunicipalities()
        {
            var lst = await _Context.Municipalities.ToListAsync();
            return lst;
        }

        public async Task<Municipality> GetMunicipalityById(int id)
        {
            var dt = await _Context.Municipalities.Where(a => a.Id == id).FirstOrDefaultAsync();
            return dt;
        }

        public async Task<List<CategoryDto>> GetCategories()
        {

            //var dt = from t1 in _Context.Categories
            //         join t2 in _Context.SubCategories on t1.Id equals t2.CatId
            //         select new { t1, t2 };

            var dt = await _Context.Categories.ToListAsync();

            List<CategoryDto> lst = new();

            foreach (var item in dt)
            {
                List<SubCategory> sclst = new();
                CategoryDto dto = new();
                dto.Name = item.Name;
                dto.Id = item.Id;
                dto.Time = item.Time;
                var sdt = _Context.SubCategories.Where(a => a.CatId == item.Id).ToList();
                foreach (var itm in sdt)
                {
                    if (itm.CatId == dto.Id)
                    {
                        sclst.Add(itm);
                    }
                }
                dto.SubCategories = sclst;
                lst.Add(dto);
            }
            return lst;
        }

        public async Task<Category> GetCategoryById(int id)
        {
            var dt = await _Context.Categories.Where(a => a.Id == id).FirstOrDefaultAsync();
            return dt;
        }

        public async Task<List<SubCategory>> GetSubCategories()
        {
            var lst = await _Context.SubCategories.ToListAsync();
            return lst;
        }

        public async Task<List<Banner>> GetBanners()
        {
            var lst = await _Context.Banners.ToListAsync();
            return lst;
        }

        public async Task<Banner> GetBannerById(int id)
        {
            var dt = await _Context.Banners.Where(a => a.Id == id).FirstOrDefaultAsync();
            return dt;
        }

        public async Task<SubCategory> GetSubCategoryById(int id)
        {
            var dt = await _Context.SubCategories.Where(a => a.Id == id).FirstOrDefaultAsync();
            return dt;
        }

        public async Task<UserDto> GiveRate(Rating dto)
        {

            try
            {
                await _Context.Ratings.AddAsync(dto);
                _Context.SaveChanges();


                var cnt = _Context.Ratings.Where(a => a.UidTo == dto.UidTo).ToList();
                var sumRate = _Context.Ratings.Where(a => a.UidTo == dto.UidTo).Select(a => a.Rating1).Sum(a => a.Value);

                double c = (double)sumRate / cnt.Count;

                var user = await _Context.Users.Where(a => a.Id == dto.UidTo).FirstOrDefaultAsync();

                user.Rate = (decimal)c;
                _Context.Users.Update(user);
                _Context.SaveChanges();


                var Check = await _Context.Users.Where(a => a.Id == dto.UidTo && a.IsDeleted == false).FirstOrDefaultAsync();

                UserDto dto1 = new();
                dto1.Address = Check.Address;
                dto1.Name = Check.Name;
                dto1.Date = Check.Date;
                dto1.Email = Check.Email;
                dto1.Id = Check.Id;
                dto1.IdImage = Check.IdImage;
                dto1.Image = Check.Image;
                dto1.IsDeleted = Check.IsDeleted;
                dto1.IsVerified = Check.IsVerified;
                dto1.LoginType = Check.LoginType;
                dto1.MemberSince = Check.MemberSince;
                dto1.MunicipalityId = Check.MunicipalityId;
                dto1.Number = Check.Number;
                dto1.Password = Check.Password;
                dto1.Rate = Check.Rate;
                dto1.VerificationSent = Check.VerificationSent;
                dto1.ProvinceId = Check.ProvinceId;
                dto1.Status = Check.Status;
                dto1.UniqueId = Check.UniqueId;
                dto1.Subscriptions = Check.Subscriptions;
                dto1.Ratintg = await _Context.Ratings.Where(a => a.UidTo == Check.Id).ToListAsync();
                dto1.Favorites = _Context.Favorites.Where(a => a.UserId == dto.UidTo).ToList();
                dto1.Followers = _Context.Folowers.Where(a => a.Folowers == dto.UidTo).ToList();
                return dto1;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<MessagesDto>> GetChatUsers(int id)
        {
            List<MessagesDto> lst = new();
            try
            {
                var data = await _Context.Messages.Where(a => a.From != id && a.ProductId != null && a.To == id).ToListAsync();
                
                if (data.Count == 0)
                {
                    var chk = _Context.Messages.Where(a => a.From == id).Distinct().Select(a => a.To).Distinct().ToList();
                    foreach (var item in chk)
                    {
                        var usr = _Context.Users.Where(a => a.Id == item).FirstOrDefault();
                        var chatId = _Context.Chats.Where(a => a.SenderId == usr.Id || a.RecieverId == usr.Id).Max(a => a.Id);
                        var chat = _Context.Chats.Where(a => a.Id == chatId).FirstOrDefault();
                        MessagesDto dto = new();
                        var dx = _Context.Messages.Where(a => a.To == item).ToList();
                        List<int> pidLst = new();
                        //var rsd = lst.Where(a => a.From == item.From && a.To == item.To).FirstOrDefault();
                        //if (rsd != null)
                        //{
                        foreach (var itm in dx)
                        {
                            if (itm.ProductId != null)
                            {
                                pidLst.Add((int)itm.ProductId);
                            }
                        }
                        if (chat.Type == "Link")
                        {

                            dto.ProductIds = pidLst;
                            dto.LastMessgeTime = (long)chat.Time;
                            dto.Name = usr.Name;
                            dto.From = item;
                            dto.To = id;
                        }
                        else
                        {
                            dto.LastMessage = chat.Message;
                            dto.ProductIds = pidLst;
                            dto.LastMessgeTime = (long)chat.Time;
                            dto.From = item;
                            dto.To = id;
                            if (usr.Name != null)
                            {
                                dto.Name = usr.Name;
                            }
                            else
                            {
                                var usra = _Context.Users.Where(a => a.Id == item).FirstOrDefault();
                                dto.Name = usra.Name;
                            }

                            dto.FromImage = usr.Image;
                        }
                        lst.Add(dto);
                    }

                }
                else
                {
                    foreach (var item in data)
                    {
                        var usr = _Context.Users.Where(a => a.Id == item.From && a.IsDeleted != true).FirstOrDefault();
                        if (usr != null)
                        {
                            var chts = _Context.Chats.Where(a => a.SenderId == usr.Id || a.RecieverId == usr.Id).FirstOrDefault();
                            if (chts != null)
                            {
                                var chatId = _Context.Chats.Where(a => a.SenderId == usr.Id || a.RecieverId == usr.Id).Max(a => a.Id);
                                var chat = _Context.Chats.Where(a => a.Id == chatId).FirstOrDefault();
                                MessagesDto dto = new();
                                var dx = _Context.Messages.Where(a => a.From == item.From && a.To == item.To).ToList();
                                List<int> pidLst = new();
                                foreach (var itm in dx)
                                {
                                    if (itm.ProductId != null)
                                    {
                                        pidLst.Add((int)itm.ProductId);
                                    }
                                }
                                if (chat.Type == "Link")
                                {
                                    dto.Id = item.Id;
                                    dto.LastMessage = "Image";
                                    dto.ConnId = item.ConnId;
                                    dto.From = item.From;
                                    dto.ProductIds = pidLst;
                                    dto.LastMessgeTime = (long)chat.Time;
                                    dto.Name = item.Name;
                                    dto.To = item.To;
                                    dto.Read = item.Read;
                                    dto.FromImage = usr.Image;
                                    dto.ProductImage = item.Image;
                                }
                                else
                                {
                                    dto.Id = item.Id;
                                    dto.LastMessage = item.LastMessage;
                                    dto.ConnId = item.ConnId;
                                    dto.From = item.From;
                                    dto.ProductIds = pidLst;
                                    dto.LastMessgeTime = (long)item.Time;
                                    if (item.Name != null)
                                    {
                                        dto.Name = item.Name;
                                    }
                                    else
                                    {
                                        var usra = _Context.Users.Where(a => a.Id == item.From).FirstOrDefault();
                                        if (usra != null)
                                        {
                                            dto.Name = usra.Name;
                                        }
                                    }

                                    dto.To = item.To;
                                    dto.Read = item.Read;
                                    dto.FromImage = usr.Image;

                                    if (item.ProductId != null)
                                    {
                                        dto.ProductImage = item.Image;
                                    }
                                }
                                lst.Add(dto);
                            }

                        }

                    }
                }


                return lst;
            }
            catch (Exception ex)
            {
                return lst;
            }
            //var data = await _Context.Messages.Where(a => a.From != null && a.To != null && ( a.To == id || a.From == id) && a.Name != null).ToListAsync();
            
        }



        public async Task<string> SendMessage(ChatsDto dto)
        {
            try
            {
                Chat chat = new();
                chat.SenderId = dto.senderId;
                chat.RecieverId = dto.recieverId;
                chat.Message = dto.message;
                chat.Time = DateTime.Now.Millisecond;
                await _Context.Chats.AddAsync(chat);
                _Context.SaveChanges();
                //await _chatHub.OnConnectedAsync();
                //await _chatHub.SendMessage(dto.message, dto.senderId.ToString());

                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
                throw;
            }
        }

        public async Task<List<Chat>> GetChatById(int id, int fromId)
        {
            //var dt = await _Context.Messages.Where(a => a.Id == id).FirstOrDefaultAsync();
            var data = await _Context.Chats.Where(a => (a.SenderId == fromId || a.SenderId == id) && (a.RecieverId == fromId || a.RecieverId == id)).ToListAsync();
            foreach (var item in data)
            {
                if (item.Type == "Link")
                {
                    item.Link = item.Link.Replace("/wwwroot", "");
                }
            }
            return data;

        }

        public async Task<MessageDto> SendImage(SendImageDto dto)
        {
            var sendr = _Context.Messages.Where(a => a.From == dto.senderId).FirstOrDefault();
            var recever = _Context.Messages.Where(a => a.From == dto.receiverId).FirstOrDefault();

            Chat chat = new();

            chat.SenderId = dto.senderId;
            chat.RecieverId = dto.receiverId;
            chat.Time = (int)DateTimeOffset.Now.ToUnixTimeMilliseconds();
            chat.ConnTo = recever.ConnId;
            chat.ConnFrom = sendr.ConnId;
            chat.Type = "Link";

            var filename1 = "";
            Random rnd = new();
            var rn = rnd.Next(111, 999);
            var sdPath = "";
            if (dto.link != null)
            {
                var ImagePath1 = rn + Methods.RemoveWhitespace(dto.link.FileName);
                var pathh = "";
                using (FileStream fileStream = File.Create(_environment.ContentRootPath + "\\wwwroot\\Resources\\Images\\Messages\\" + ImagePath1))
                {
                    dto.link.CopyTo(fileStream);
                    pathh = Path.Combine(_environment.ContentRootPath, "/wwwroot/Resources/Images/Messages/" + ImagePath1);
                    filename1 = ImagePath1;
                    fileStream.Flush();
                }
                chat.Link = "/wwwroot/Resources/Images/Messages/" + filename1;
                sdPath = "/Resources/Images/Messages/" + filename1;
            }
            else
            {
                chat.Link = "";
            }

            _Context.Chats.Add(chat);
            _Context.SaveChanges();

            MessageDto dt = new();
            dt.From = dto.senderId;
            dt.To = dto.receiverId;
            dt.Time = (int)DateTimeOffset.Now.ToUnixTimeMilliseconds();
            dt.Link = sdPath;
            dt.Type = "Link";

            return dt;

        }

        public async Task<UserDto> Follow(Folower dto)
        {
            var fr = _Context.Folowers.Where(a => a.Fuser == dto.Fuser && a.Folowers == dto.Folowers).FirstOrDefault();
            if (fr != null)
            {
                _Context.Folowers.Remove(fr);
                _Context.SaveChanges();
            }
            else
            {
                _Context.Folowers.Add(dto);
                _Context.SaveChanges();
            }

            var Check = await _Context.Users.Where(a => a.Id == dto.Folowers && a.IsDeleted != true).FirstOrDefaultAsync();

            UserDto dto1 = new();
            dto1.Address = Check.Address;
            dto1.Name = Check.Name;
            dto1.Date = Check.Date;
            dto1.Email = Check.Email;
            dto1.Id = Check.Id;
            dto1.IdImage = Check.IdImage;
            dto1.Image = Check.Image;
            dto1.IsDeleted = Check.IsDeleted;
            dto1.IsVerified = Check.IsVerified;
            dto1.LoginType = Check.LoginType;
            dto1.MemberSince = Check.MemberSince;
            dto1.MunicipalityId = Check.MunicipalityId;
            dto1.Number = Check.Number;
            dto1.Password = Check.Password;
            dto1.Rate = Check.Rate;
            dto1.VerificationSent = Check.VerificationSent;
            dto1.ProvinceId = Check.ProvinceId;
            dto1.Status = Check.Status;
            dto1.UniqueId = Check.UniqueId;
            dto1.Subscriptions = Check.Subscriptions;
            dto1.Ratintg = await _Context.Ratings.Where(a => a.UidTo == Check.Id).ToListAsync();
            dto1.Favorites = _Context.Favorites.Where(a => a.UserId == dto.Folowers).ToList();
            dto1.Followers = _Context.Folowers.Where(a => a.Folowers == dto.Folowers).ToList();
            return dto1;
        }

        public async Task<string> Notify(int Id, int Pid)
        {
            Notification notification = new();
            notification.Pid = Pid;
            notification.Uid = Id;
            notification.IsRead = false;
            _Context.Notifications.Add(notification);
            _Context.SaveChanges();

            return "Success";
        }

        public async Task<List<NotificationDto>> GetNotify(int id)
        {
            var data = from t1 in _Context.Notifications
                       join t2 in _Context.Products on t1.Pid equals t2.Id
                       join t3 in _Context.Users on t1.Uid equals t3.Id
                       join t4 in _Context.Municipalities on t3.MunicipalityId equals t4.Id
                       join t5 in _Context.Folowers on t3.Id equals t5.Folowers
                       where t5.Fuser == id && t1.IsRead != true
                       select new { t3.Name, Uid = t3.Id, t2, t3.MunicipalityId, t3.ProvinceId, t1.Id };

            List<NotificationDto> lst = new();
            foreach (var item in data.ToList())
            {
                NotificationDto dto = new();
                dto.uid = item.Uid;
                dto.uName = item.Name;

                dto.Id = item.t2.Id;
                dto.CategoryId = item.t2.CategoryId;
                dto.Condition = item.t2.Condition;
                dto.IsSold = item.t2.IsSold;
                dto.Description = item.t2.Description;
                dto.FireOnPrice = item.t2.FireOnPrice;
                dto.IsDelivering = item.t2.IsDelivering;
                dto.IsPickup = item.t2.IsPickup;
                dto.IsPromoted = item.t2.IsPromoted;
                dto.IsReported = item.t2.IsReported;
                dto.Price = item.t2.Price;
                dto.SellerId = item.t2.SellerId;
                dto.SubCategoryId = item.t2.SubCategoryId;
                dto.Title = item.t2.Title;
                dto.CreatedDate = item.t2.CreatedDate;
                List<ProdImagesDto> prodImages = new();
                var pdimg = await _Context.ProdImages.Where(a => a.PId == item.t2.Id).ToListAsync();
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
                var provnc = await _Context.ProdProvinces.Where(a => a.Pid == item.t2.Id).ToListAsync();
                foreach (var itemA in provnc)
                {
                    var d = await _Context.Provinces.Where(a => a.Id == itemA.ProvinceId).FirstOrDefaultAsync();

                    List<MunicipalityDto> mnclst = new();
                    ProvinceDto dtoA = new();
                    dtoA.Id = itemA.Id;
                    dtoA.Name = d.Name;
                    var prodMunc = await _Context.ProdMunicipalities.Where(a => a.Pid == item.t2.Id).ToListAsync();
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
                dto.ProdViews = await _Context.ProdViews.Where(a => a.PId == item.t2.Id).ToListAsync();
                lst.Add(dto);

                var noti = await _Context.Notifications.Where(a => a.Id == item.Id).FirstOrDefaultAsync();
                noti.IsRead = true;
                _Context.Notifications.Update(noti);
                _Context.SaveChanges();
            }
            return lst;
        }
        
        public async Task<List<MessagesDto>> GetChatsByProduct(int id, int uid)
        {
            //var data = await _Context.Messages.Where(a => a.ProductId == id && a.From != null && a.To != null && a.From != uid).ToListAsync();
            var data = await _Context.Chats.Where(a => a.ProductId == id).Select(a => new { a.SenderId, a.RecieverId }).Distinct().ToListAsync();
            List<MessagesDto> lst = new();
            foreach (var item in data)
            {
                var usr = _Context.Users.Where(a => a.Id == item.RecieverId).FirstOrDefault();
                if (usr != null)
                {
                    var chatIdCheck = _Context.Chats.Where(a => a.SenderId == usr.Id).FirstOrDefault();
                    int CheckChatId = 0;
                    if (chatIdCheck != null)
                    {
                        var chatId = _Context.Chats.Where(a => a.SenderId == usr.Id).Max(a => a.Id);
                        CheckChatId = chatId;
                    }
                    else
                    {
                        var chatId = _Context.Chats.Where(a => a.RecieverId == usr.Id).Max(a => a.Id);
                        CheckChatId = chatId;
                    }

                    
                    var chat = _Context.Chats.Where(a => a.Id == CheckChatId).FirstOrDefault();
                    if (chat != null)
                    {
                        var dx = _Context.Messages.Where(a => a.From == item.SenderId && a.To == item.RecieverId).ToList();
                        MessagesDto dto = new();
                        List<int> pidLst = new();

                        foreach (var itm in dx)
                        {
                            if (itm.ProductId != null)
                            {
                                pidLst.Add((int)itm.ProductId);
                            }
                        }
                        var msgs = _Context.Messages.Where(a => a.From == item.SenderId && a.To == item.RecieverId || a.From == item.RecieverId && a.To == item.SenderId && a.ProductId == id).FirstOrDefault();

                        if (chat.Type == "Link")
                        {
                            dto.Id = msgs.Id;
                            dto.LastMessage = "Image";
                            dto.ConnId = msgs.ConnId;
                            dto.From = item.SenderId;
                            dto.ProductIds = pidLst;
                            dto.LastMessgeTime = (long)chat.Time;
                            dto.Name = msgs.Name;
                            dto.To = item.RecieverId;
                            dto.Read = msgs.Read;
                            dto.FromImage = usr.Image;
                        }
                        else
                        {
                            dto.Id = msgs.Id;
                            dto.LastMessage = msgs.LastMessage;
                            dto.ConnId = msgs.ConnId;
                            dto.From = item.SenderId;
                            dto.ProductIds = pidLst;
                            dto.LastMessgeTime = (long)chat.Time;
                            dto.Name = msgs.Name;
                            dto.To = item.RecieverId;
                            dto.Read = msgs.Read;
                            dto.FromImage = usr.Image;
                        }
                        lst.Add(dto);
                    }
                    
                }
            }
            return lst;
        }

        public async Task<ContactDto> AllIfon()
        {
            ContactDto dto = new();
            var dt = _Context.Users.Where(a => a.LoginType == "Admin" && a.IsDeleted != true).FirstOrDefault();
            dto.Description = dt.Description;
            dto.Number = dt.Number;
            dto.Email = dt.Email;
            dto.UserImage = dt.Image;
            dto.BannerImage = _Context.Banners.Select(a => a.Image).ToList();
            dto.AllowAll = _Context.Settings.Select(a => a.SubsAllAllow).FirstOrDefault();
            return dto;
        }

        public async Task<string> SetMessageUser(MessaveDto dto)
        {
            var dt = _Context.Messages.Where(a => a.From == dto.From && a.To == dto.To && a.ProductId == dto.ProductIds).FirstOrDefault();
            var pdt = _Context.ProdImages.Where(a => a.PId == dto.ProductIds).FirstOrDefault();
            if (dt == null)
            {

                Message message = new();
                message.From = dto.From;
                message.To = dto.To;
                message.ProductId = dto.ProductIds;
                message.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                message.Image = pdt.Image;
                var user = _Context.Users.Where(a => a.Id == dto.From).FirstOrDefault();
                message.Name = user.Name;
                var msgs = _Context.Messages.Where(a => a.From == dto.From && a.ConnId != null).FirstOrDefault();
                message.ConnId = msgs.ConnId;
                _Context.Messages.Add(message);
                _Context.SaveChanges();

                return "Success";
            }
            else
            {
                if (dt.Name == null)
                {
                    var user = _Context.Users.Where(a => a.Id == dto.From).FirstOrDefault();
                    dt.Name = user.Name;
                }
                dt.Image = pdt.Image;

                _Context.Messages.Update(dt);
                _Context.SaveChanges();

                var dts = await _Context.Messages.Where(a => a.From == dto.From && a.To == dto.To || (a.From == dto.To && a.To == dto.From)).ToListAsync();
                if (dts.Count > 0)
                {
                    foreach (var item in dts)
                    {
                        item.Image = pdt.Image;
                        _Context.Messages.Update(item);
                        await _Context.SaveChangesAsync();
                    }
                }
            }
            return "Already Exist";
        }

        public async Task<long?> CheckSubs(int id)
        {
            var check = _Context.Settings.FirstOrDefault();
            if (check.SubsAllAllow != true)
            {
                var user = _Context.Users.Where(a => a.Id == id).FirstOrDefault();
                return user.Subscriptions;
            }
            return DateTimeOffset.Now.AddDays(1).ToUnixTimeMilliseconds();
        }

        public async Task<List<MsgNotification>> GetMSgNotify(int id)
        {
            var lst = await _Context.MsgNotifications.Where(a => a.Uid == id && a.IsRead != true).ToListAsync();

            foreach (var item in lst)
            {
                item.IsRead = true;
                _Context.MsgNotifications.Update(item);
                _Context.SaveChanges();
            }
            return lst;
        }

        public async Task<string> Chat(string message, string userId, string Pid, string from, string conn)
        {
            var tkn = conn;
            var sType = "";
            var connectIdUser = "";
            int usrid = 0;
            int stakrid = 0;
            var UserNameRMsg = "";
            string json = "";
            string ConnectionId = _Context.Messages.Where(a => a.From == Convert.ToInt32(userId)).Select(a => a.ConnId).FirstOrDefault();

            var chngeFrom = _Context.Messages.Where(a => a.To == Convert.ToInt32(from)).ToList();
            foreach (var item in chngeFrom)
            {
                item.ConnId = conn;
                _Context.Messages.Update(item);
                _Context.SaveChanges();
            }
            if (!string.IsNullOrEmpty(Pid))
            {
                var prod = _Context.ProdImages.Where(a => a.PId == Convert.ToInt32(Pid)).FirstOrDefault();
                if (prod != null)
                {
                    var rsMsg = _Context.Messages.Where(a => a.From == Convert.ToInt32(from) && a.To == Convert.ToInt32(userId) || (a.From == Convert.ToInt32(userId) && a.To == Convert.ToInt32(from)) && a.ProductId == Convert.ToInt32(Pid)).ToList();
                    foreach (var item in rsMsg)
                    {
                        item.Image = prod.Image;
                        _Context.Messages.Update(item);
                        _Context.SaveChanges();
                    }
                }
                var rsMsga = _Context.Messages.Where(a => a.From == Convert.ToInt32(from) && a.To == Convert.ToInt32(userId)).FirstOrDefault();
                if (rsMsga != null)
                {
                    rsMsga.ProductId = Convert.ToInt32(Pid);
                    _Context.Messages.Update(rsMsga);
                    _Context.SaveChanges();
                }
                var rsMsg1 = _Context.Messages.Where(a => a.From == Convert.ToInt32(userId) && a.To == Convert.ToInt32(from)).FirstOrDefault();
                if (rsMsg1 != null)
                {
                    rsMsg1.ProductId = Convert.ToInt32(Pid);
                    _Context.Messages.Update(rsMsg1);
                    _Context.SaveChanges();
                }
            }

            Chat dto = new();
            if (message.Contains("/Resources/Images/Messages"))
            {
                //message = "/wwwroot" + message;
                var img = _Context.Chats.Where(a => a.Link == "/wwwroot" + message).FirstOrDefault();
                if (img != null)
                {
                    img.Link = img.Link.Replace("/wwwroot", "");
                    //connectIdUser = Context.User.Claims.FirstOrDefault().Value;
                    usrid = Convert.ToInt32(from);
                    dto.SenderId = usrid;
                    UserNameRMsg = _Context.Users.AsQueryable().Where(a => a.Id == usrid).Select(a => a.Name).FirstOrDefault();
                    var ToId = _Context.Messages.AsQueryable().Where(a => a.From == Convert.ToInt32(userId)).Select(a => a.ConnId).FirstOrDefault();
                    dto.ConnTo = ToId;

                    json = "{" + @"""text""" + ":" + @"""" + "" + @"""" + "," + @"""connId""" + ":" + @"""" + conn + @"""" + "," + @"""from""" + ":" + usrid + "," + @"""when""" + ":" + DateTimeOffset.Now.ToUnixTimeMilliseconds() + "," + @"""to""" + ":" + userId + "," + @"""type""" + ":" + @"""" + img.Type.ToString() + @"""" + "," + @"""Link""" + ":" + @"""" + img.Link + @"""" + "}";
                }
            }
            else
            {
                //connectIdUser = Context.User.Claims.FirstOrDefault().Value;
                usrid = Convert.ToInt32(from);
                dto.SenderId = usrid;
                UserNameRMsg = _Context.Users.AsQueryable().Where(a => a.Id == usrid).Select(a => a.Name).FirstOrDefault();
                var ToId = _Context.Messages.AsQueryable().Where(a => a.From == Convert.ToInt32(userId)).Select(a => a.ConnId).FirstOrDefault();

                dto.SenderId = usrid;
                dto.RecieverId = Convert.ToInt32(userId);
                dto.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                dto.Message = message;
                dto.ConnTo = ToId;
                dto.ConnFrom = conn;
                dto.ConnId = userId;
                dto.Type = "Text";
                if (Pid != "")
                {
                    dto.ProductId = Convert.ToInt32(Pid);
                }
                _Context.Chats.Add(dto);
                _Context.SaveChanges();

                if (dto.Type == "Link")
                {
                    dto.Message = dto.Message.Replace("/wwwroot", "");
                }
                json = "{" + @"""text""" + ":" + @"""" + dto.Message + @"""" + "," + @"""connId""" + ":" + @"""" + ToId + @"""" + "," + @"""from""" + ":" + usrid + "," + @"""when""" + ":" + DateTimeOffset.Now.ToUnixTimeMilliseconds() + "," + @"""to""" + ":" + userId + "," + @"""type""" + ":" + @"""" + dto.Type.ToString() + @"""" + "," + @"""Link""" + ":" + @"""" + "" + @"""" + "}";
            }

            var msg = _Context.Messages.Where(a => a.From == dto.SenderId && a.To == dto.RecieverId || (a.From == dto.RecieverId && a.To == dto.SenderId)).ToList();
            foreach (var msgs in msg)
            {
                if (Pid != "")
                {
                    msg = _Context.Messages.Where(a => a.From == dto.SenderId && a.To == dto.RecieverId && a.ProductId == Convert.ToInt32(Pid)).ToList();
                }

                if (message.Contains("/Resources/Images/Messages"))
                {
                    msgs.LastMessage = "Image";
                    msgs.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    var msgN = _Context.Messages.Where(a => a.From == dto.SenderId && a.To == null).FirstOrDefault();
                    if (msgN != null)
                    {
                        msgN.To = dto.RecieverId;
                        if (Pid != "")
                        {
                            msgN.ProductId = Convert.ToInt32(Pid);
                        }
                        msgN.LastMessage = message;
                        _Context.Messages.Update(msgN);
                        _Context.SaveChanges();
                    }
                }
                else
                {
                    msgs.LastMessage = message;
                    msgs.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    var msgN = _Context.Messages.Where(a => a.From == dto.SenderId && a.To == null).FirstOrDefault();
                    if (msgN != null)
                    {
                        msgN.To = dto.RecieverId;
                        if (Pid != "")
                        {
                            msgN.ProductId = Convert.ToInt32(Pid);
                        }
                        msgN.LastMessage = message;
                        _Context.Messages.Update(msgN);
                        _Context.SaveChanges();
                    }
                }
                _Context.Messages.Update(msgs);
                _Context.SaveChanges();
            }



            var Messagess = _Context.Messages.Where(a => a.From == dto.SenderId && a.To == dto.RecieverId).ToList();
            if (Messagess.Count > 1)
            {
                var Messagessa = _Context.Messages.Where(a => a.From == dto.SenderId && a.To == dto.RecieverId && a.Image == null).FirstOrDefault();
                if (Messagessa != null)
                {
                    _Context.Messages.Remove(Messagessa);
                    _Context.SaveChanges();
                }
            }


            var WhoUser = _Context.Messages.Where(a => a.From == Convert.ToInt32(from)).FirstOrDefault();
            if (WhoUser != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(Pid))
                    {
                        var check = _Context.Messages.Where(a => a.From == WhoUser.From && a.To == Convert.ToInt32(userId)).FirstOrDefault();

                        if (check == null)
                        {
                            var nextCheck = _Context.Messages.Where(a => a.From == Convert.ToInt32(from) && a.To == null).FirstOrDefault();
                            if (nextCheck != null)
                            {
                                nextCheck.LastMessage = message;
                                nextCheck.ProductId = Convert.ToInt32(Pid);
                                var ausr = _Context.Users.Where(a => a.Id == Convert.ToInt32(userId)).FirstOrDefault();
                                nextCheck.Name = ausr.Name;
                                nextCheck.From = WhoUser.From;
                                nextCheck.To = Convert.ToInt32(userId);
                                nextCheck.ConnId = ConnectionId;
                                nextCheck.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                _Context.Messages.Update(nextCheck);
                                _Context.SaveChanges();
                            }
                            else
                            {
                                Message msgsfrom = new()
                                {
                                    LastMessage = message,
                                    ProductId = Convert.ToInt32(Pid)
                                };
                                var ausr = _Context.Users.Where(a => a.Id == Convert.ToInt32(userId)).FirstOrDefault();
                                msgsfrom.Name = ausr.Name;
                                msgsfrom.From = WhoUser.From;
                                msgsfrom.To = Convert.ToInt32(userId);
                                msgsfrom.ConnId = ConnectionId;
                                msgsfrom.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                _Context.Messages.Add(msgsfrom);
                                _Context.SaveChanges();
                            }
                        }
                    }

                }
                catch (Exception ex)
                {

                    throw;
                }



            }

            var WhoUser1 = _Context.Messages.Where(a => a.From == Convert.ToInt32(userId)).FirstOrDefault();
            if (WhoUser1 != null)
            {
                if (!string.IsNullOrEmpty(Pid))
                {
                    var check = _Context.Messages.Where(a => a.From == Convert.ToInt32(userId) && a.To == WhoUser1.From).FirstOrDefault();

                    if (check == null)
                    {
                        var nextCheck = _Context.Messages.Where(a => a.From == Convert.ToInt32(userId) && a.To == null).FirstOrDefault();
                        if (nextCheck != null)
                        {
                            var conId = _Context.Messages.Where(a => a.From == Convert.ToInt32(userId)).FirstOrDefault();
                            nextCheck.LastMessage = message;
                            nextCheck.ProductId = Convert.ToInt32(Pid);
                            var ausr = _Context.Users.Where(a => a.Id == Convert.ToInt32(userId)).FirstOrDefault();
                            nextCheck.Name = ausr.Name;
                            nextCheck.To = WhoUser.From;
                            nextCheck.From = Convert.ToInt32(userId);
                            nextCheck.ConnId = conId.ConnId;
                            nextCheck.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                            _Context.Messages.Update(nextCheck);
                            _Context.SaveChanges();
                        }
                        else
                        {
                            Message msgsfrom = new()
                            {
                                LastMessage = message,
                                ProductId = Convert.ToInt32(Pid)
                            };
                            var ausr = _Context.Users.Where(a => a.Id == Convert.ToInt32(userId)).FirstOrDefault();
                            msgsfrom.Name = ausr.Name;
                            msgsfrom.From = WhoUser.From;
                            msgsfrom.To = Convert.ToInt32(userId);
                            msgsfrom.ConnId = ConnectionId;
                            msgsfrom.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                            _Context.Messages.Add(msgsfrom);
                            _Context.SaveChanges();
                        }
                    }
                }

            }


            var delList = _Context.Messages.Where(a => a.From == Convert.ToInt32(userId) && a.To == Convert.ToInt32(from)).ToList();
            if (delList.Count > 1)
            {
                var dlList = _Context.Messages.Where(a => a.From == Convert.ToInt32(userId) && a.To == Convert.ToInt32(from)).FirstOrDefault();
                var dleList = _Context.Messages.Where(a => a.From == Convert.ToInt32(userId) && a.To == Convert.ToInt32(from) && a.Id != dlList.Id).ToList();
                _Context.Messages.RemoveRange(dleList);
                _Context.SaveChanges();
            }

            var remList = _Context.Messages.Where(a => a.From == Convert.ToInt32(from) && a.To == Convert.ToInt32(userId)).ToList();
            if (remList.Count > 1)
            {
                var rmList = _Context.Messages.Where(a => a.From == Convert.ToInt32(from) && a.To == Convert.ToInt32(userId)).FirstOrDefault();
                var rmeeList = _Context.Messages.Where(a => a.From == Convert.ToInt32(from) && a.To == Convert.ToInt32(userId) && a.Id != rmList.Id).ToList();
                _Context.Messages.RemoveRange(rmeeList);
                _Context.SaveChanges();
            }

            var ReverseChat = _Context.Messages.Where(a => a.From == Convert.ToInt32(userId) && a.To == Convert.ToInt32(from)).FirstOrDefault();
            if (ReverseChat == null)
            {
                Message msgs = new();
                msgs.From = Convert.ToInt32(userId);
                msgs.To = Convert.ToInt32(from);
                msgs.LastMessage = message;
                var pidset = _Context.Messages.Where(a => a.From == Convert.ToInt32(from) && a.To == Convert.ToInt32(userId)).FirstOrDefault();
                if (!string.IsNullOrEmpty(Pid))
                {
                    if (pidset != null)
                    {
                        msgs.ProductId = pidset.ProductId;
                    }
                }
                if (pidset != null)
                {
                    msgs.Image = pidset.Image;
                }
                
                var ausr = _Context.Users.Where(a => a.Id == Convert.ToInt32(userId)).FirstOrDefault();
                if (ausr != null)
                {
                    msgs.Name = ausr.Name;
                }
                
                string ConnectionIdas = _Context.Messages.Where(a => a.From == Convert.ToInt32(userId)).Select(a => a.ConnId).FirstOrDefault();
                if (ConnectionIdas != null)
                {
                    msgs.ConnId = ConnectionIdas;
                    msgs.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                }
                
                _Context.Messages.Add(msgs);
                _Context.SaveChanges();
            }

            MsgNotification msgNotification = new();
            msgNotification.Date = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            msgNotification.Message = message;
            msgNotification.Fid = Convert.ToInt32(from);
            msgNotification.Uid = Convert.ToInt32(userId);
            msgNotification.IsRead = false;
            var ausra = _Context.Users.Where(a => a.Id == Convert.ToInt32(from)).FirstOrDefault();
            if (ausra != null)
            {
                msgNotification.Fname = ausra.Name;
            }
            _Context.MsgNotifications.Add(msgNotification);
            _Context.SaveChanges();

            return json;
        }

        public async Task<string> ChangeConnId(string conn, int userId, int sellerId, int pId)
        {
            if (conn != null)
            {
                var dt = await _Context.Messages.AsQueryable().Where(a => a.From == userId).FirstOrDefaultAsync();

                if (dt != null)
                {
                    var sd = _Context.Chats.AsQueryable().Where(a => a.ConnId == userId.ToString()).ToList();
                    if (sd.Count > 0)
                    {
                        foreach (var item in sd)
                        {
                            item.ConnId = userId.ToString();
                            _Context.Chats.Update(item);
                            _Context.SaveChanges();
                        }
                    }
                }

                if (dt != null)
                {
                    var sd = await _Context.Chats.AsQueryable().Where(a => a.ConnFrom == dt.ConnId).ToListAsync();
                    if (sd.Count > 0)
                    {
                        foreach (var item in sd)
                        {
                            item.ConnFrom = conn;
                            _Context.Chats.Update(item);
                            _Context.SaveChanges();
                        }
                    }
                }

                if (dt != null)
                {
                    var msgs = _Context.Messages.Where(a => a.From == dt.From).ToList();
                    if (msgs.Count > 0)
                    {
                        foreach (var item in msgs)
                        {
                            item.ConnId = conn;
                            _Context.Messages.Update(item);
                            _Context.SaveChanges();
                        }
                    }
                }
                else
                {
                    Message lstd = new();
                    lstd.ConnId = conn;
                    lstd.From = userId;
                    lstd.To = sellerId;
                    lstd.ProductId = pId;
                    _Context.Messages.Add(lstd);
                    _Context.SaveChanges();
                }
            }
            
            return "Success";
        }

        public async Task<List<JsonApiDatum>> GetJsonApiData()
        {
            var data = _Context.JsonApiData.ToList();
            return data;
        }
    }
}