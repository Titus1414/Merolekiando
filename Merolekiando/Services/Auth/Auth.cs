using MailKit.Net.Smtp;
using Merolekando.Common;
using Merolekando.Models.Dtos;
using Merolekiando.Models;
using Merolekiando.Models.Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Merolekando.Services.Auth
{
    public class Auth : IAuth
    {
        private readonly MerolikandoDBContext _Context;
        public static IWebHostEnvironment? _environment;
        public Auth(MerolikandoDBContext dbContext, IWebHostEnvironment? environment)
        {
            _Context = dbContext;
            _environment = environment;
        }

        public async Task<string> DeleteUser(DeleteAccountDto dto)
        {
            try
            {
                if (dto.LoginType == "Custom")
                {
                    var user = await _Context.Users.Where(a => a.Id == dto.Id && a.IsDeleted != true).FirstOrDefaultAsync();
                    string encpass = Methods.Encrypt(dto.Pass);
                    if (user.Password != encpass)
                    {
                        return "Contraseña invalida";
                    }

                    user.IsDeleted = true;
                    _Context.Users.Update(user);
                    _Context.SaveChanges();

                    var chat = _Context.Chats.Where(a => a.SenderId == dto.Id || a.RecieverId == dto.Id).ToList();
                    _Context.Chats.RemoveRange(chat);

                    var prd = _Context.Products.Where(a => a.SellerId == dto.Id).ToList();
                    foreach (var item in prd)
                    {
                        item.IsActive = false;
                        _Context.Products.Update(item);
                    }
                    _Context.SaveChanges();

                    return "Success";
                }
                else
                {
                    var user = await _Context.Users.Where(a => a.Id == dto.Id && a.IsDeleted != true).FirstOrDefaultAsync();
                    if (user.UniqueId != dto.Pass)
                    {
                        return "Contraseña invalida";
                    }

                    user.IsDeleted = true;
                    _Context.Users.Update(user);
                    _Context.SaveChanges();

                    var chat = _Context.Chats.Where(a => a.SenderId == dto.Id || a.RecieverId == dto.Id).ToList();
                    _Context.Chats.RemoveRange(chat);

                    var prd = _Context.Products.Where(a => a.SellerId == dto.Id).ToList();
                    foreach (var item in prd)
                    {
                        item.IsActive = false;
                        _Context.Products.Update(item);
                    }
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

        public async Task<List<UserDto>> GetUser()
        {
            var dt = await _Context.Users.Where(a => a.IsDeleted != true).ToListAsync();
            List<UserDto> lst = new();
            foreach (var item in dt)
            {
                UserDto dto = new();
                dto.Address = item.Address;
                dto.Name = item.Name;
                dto.Date = item.Date;
                dto.Email = item.Email;
                dto.Id = item.Id;
                dto.IdImage = item.IdImage;
                dto.Image = item.Image;
                dto.IsDeleted = item.IsDeleted;
                dto.IsVerified = item.IsVerified;
                dto.LoginType = item.LoginType;
                dto.MemberSince = item.MemberSince;
                dto.MunicipalityId = item.MunicipalityId;
                dto.Number = item.Number;
                dto.Password = item.Password;
                dto.Rate = item.Rate;
                dto.VerificationSent = item.VerificationSent;
                dto.ProvinceId = item.ProvinceId;
                dto.Status = item.Status;
                dto.UniqueId = item.UniqueId;
                dto.Subscriptions = item.Subscriptions;
                dto.Ratintg = await _Context.Ratings.Where(a => a.UidTo == item.Id).ToListAsync();
                dto.Favorites = _Context.Favorites.Where(a => a.UserId == item.Id).ToList(); // _Context.Favorites.Where(a => a.UserId == item.Id).ToList();
                dto.Followers = _Context.Folowers.Where(a => a.Folowers == item.Id).ToList();
                lst.Add(dto);

            }


            return lst;
        }
        public async Task<UserDto> GetUserById(int id)
        {
            var dt = await _Context.Users.Where(a => a.IsDeleted != true && a.Id == id).FirstOrDefaultAsync();
            UserDto dto = new();
            if (dt != null)
            {
                dto.Address = dt.Address;
                dto.Name = dt.Name;
                dto.Date = dt.Date;
                dto.Email = dt.Email;
                dto.Id = dt.Id;
                dto.IdImage = dt.IdImage;
                dto.Image = dt.Image;
                dto.IsDeleted = dt.IsDeleted;
                dto.IsVerified = dt.IsVerified;
                dto.LoginType = dt.LoginType;
                dto.MemberSince = dt.MemberSince;
                dto.MunicipalityId = dt.MunicipalityId;
                dto.Number = dt.Number;
                dto.Password = dt.Password;
                dto.Rate = dt.Rate;
                dto.VerificationSent = dt.VerificationSent;
                dto.ProvinceId = dt.ProvinceId;
                dto.Status = dt.Status;
                dto.UniqueId = dt.UniqueId;
                dto.Subscriptions = dt.Subscriptions;
                dto.Ratintg = await _Context.Ratings.Where(a => a.UidTo == dt.Id).ToListAsync();
                dto.Favorites = _Context.Favorites.Where(a => a.UserId == dt.Id).ToList();
                dto.Followers = _Context.Folowers.Where(a => a.Folowers == dt.Id).ToList();
            }
            return dto;
        }
        public async Task<string> Login(User user)
        {
            if (user.LoginType == "Custom")
            {
                string encpass = Methods.Encrypt(user.Password);

                var usr = await _Context.Users.Where(a => a.Email == user.Email && a.Password == encpass && a.IsDeleted == false).FirstOrDefaultAsync();
                
                if (usr != null)
                {
                    if (usr.IsBlock == true)
                    {
                        return "Cuenta bloqueada";
                    }
                    if (usr.Date == null)
                    {
                        usr.Date = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        _Context.Users.Update(usr);
                        _Context.SaveChanges();
                    }
                    if (usr.MemberSince == null)
                    {
                        usr.MemberSince = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        _Context.Users.Update(usr);
                        _Context.SaveChanges();
                    }
                    return "Success";
                }
                return "Invalid credentials";
            }
            else
            {
                var Check = await _Context.Users.Where(a => a.UniqueId == user.UniqueId && a.IsDeleted != true && a.LoginType == user.LoginType).FirstOrDefaultAsync();
                if (Check != null)
                {
                    if (Check.IsBlock == true)
                    {
                        return "Cuenta bloqueada";
                    }
                    if (Check.Date == null)
                    {
                        Check.Date = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        _Context.Users.Update(Check);
                        _Context.SaveChanges();
                    }
                    if (Check.MemberSince == null)
                    {
                        Check.MemberSince = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        _Context.Users.Update(Check);
                        _Context.SaveChanges();
                    }
                    return "Success";
                }
                else
                {
                    User dt = new();
                    dt.UniqueId = user.UniqueId;
                    dt.IsDeleted = false;
                    dt.Name = user.Name;
                    dt.Email = user.Email;  
                    dt.LoginType = user.LoginType;
                    dt.Subscriptions = DateTimeOffset.Now.AddDays(-5).ToUnixTimeMilliseconds();
                    dt.Date = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    dt.MemberSince = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    await _Context.Users.AddAsync(dt);
                    _Context.SaveChanges();
                    return "Success";
                }
            }

        }
        public async Task<UserDto> ResetPasssword(ResetPasswordDto dto)
        {
            try
            {
                var user = await _Context.Users.Where(a => a.Id == dto.Uid && a.IsDeleted != true).FirstOrDefaultAsync();
                if (user != null)
                {
                    string encpass = Methods.Encrypt(dto.OldPass);
                    if (user.Password == encpass)
                    {
                        user.Password = Methods.Encrypt(dto.NewPass);
                        _Context.Users.Update(user);
                        _Context.SaveChanges();

                        UserDto dto1 = new();
                        dto1.Address = user.Address;
                        dto1.Name = user.Name;
                        dto1.Date = user.Date;
                        dto1.Email = user.Email;
                        dto1.Id = user.Id;
                        dto1.IdImage = user.IdImage;
                        dto1.Image = user.Image;
                        dto1.IsDeleted = user.IsDeleted;
                        dto1.IsVerified = user.IsVerified;
                        dto1.LoginType = user.LoginType;
                        dto1.MemberSince = user.MemberSince;
                        dto1.MunicipalityId = user.MunicipalityId;
                        dto1.Number = user.Number;
                        dto1.Password = user.Password;
                        dto1.Rate = user.Rate;
                        dto1.VerificationSent = user.VerificationSent;
                        dto1.ProvinceId = user.ProvinceId;
                        dto1.Status = user.Status;
                        dto1.UniqueId = user.UniqueId;
                        dto1.Subscriptions = user.Subscriptions;
                        dto1.Ratintg = await _Context.Ratings.Where(a => a.UidTo == user.Id).ToListAsync();
                        dto1.Favorites = _Context.Favorites.Where(a => a.UserId == user.Id).ToList();
                        dto1.Followers = _Context.Folowers.Where(a => a.Folowers == user.Id).ToList();
                        return dto1;
                    }
                    return null;
                }
                return null;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<UserDto> SetImage(SetUserImageDto dto)
        {
            var user = await _Context.Users.Where(a => a.Id == dto.Id && a.IsDeleted != true).FirstOrDefaultAsync();
            if (user != null)
            {
                var filename1 = "";
                Random rnd = new();
                var rn = rnd.Next(111, 999);

                if (dto.Image != null)
                {
                    var ImagePath1 = rn + Methods.RemoveWhitespace(dto.Image.FileName);
                    var pathh = "";
                    using (FileStream fileStream = File.Create(_environment.WebRootPath + "\\Resources\\Images\\Users\\" + ImagePath1))
                    {
                        dto.Image.CopyTo(fileStream);
                        pathh = Path.Combine(_environment.WebRootPath, "/Resources/Images/Users/" + ImagePath1);
                        filename1 = ImagePath1;
                        fileStream.Flush();
                    }
                    user.Image = "/Resources/Images/Users/" + filename1;
                }
                else
                {
                    user.Image = "";
                }
                _Context.Users.Update(user);
                _Context.SaveChanges();

                UserDto dto1 = new();
                dto1.Address = user.Address;
                dto1.Name = user.Name;
                dto1.Date = user.Date;
                dto1.Email = user.Email;
                dto1.Id = user.Id;
                dto1.IdImage = user.IdImage;
                dto1.Image = user.Image;
                dto1.IsDeleted = user.IsDeleted;
                dto1.IsVerified = user.IsVerified;
                dto1.LoginType = user.LoginType;
                dto1.MemberSince = user.MemberSince;
                dto1.MunicipalityId = user.MunicipalityId;
                dto1.Number = user.Number;
                dto1.Password = user.Password;
                dto1.Rate = user.Rate;
                dto1.VerificationSent = user.VerificationSent;
                dto1.ProvinceId = user.ProvinceId;
                dto1.Status = user.Status;
                dto1.UniqueId = user.UniqueId;
                dto1.Subscriptions = user.Subscriptions;
                dto1.Ratintg = await _Context.Ratings.Where(a => a.UidTo == user.Id).ToListAsync();
                dto1.Favorites = _Context.Favorites.Where(a => a.UserId == user.Id).ToList();
                dto1.Followers = _Context.Folowers.Where(a => a.Folowers == user.Id).ToList();
                return dto1;
            }
            return null;
        }
        public async Task<UserDto> UpdateUser(UserDto user)
        {
            var dt = await _Context.Users.Where(a => a.Id == user.Id && a.IsDeleted != true).FirstOrDefaultAsync();
            if (dt != null)
            {
                dt.Name = user.Name;
                dt.MunicipalityId = user.MunicipalityId;
                dt.ProvinceId = user.ProvinceId;
                dt.Number = user.Number;
                _Context.Users.Update(dt);
                _Context.SaveChanges();
                var msg = _Context.Messages.Where(a => a.From == dt.Id).ToList();
                foreach (var item in msg)
                {
                    item.Name = dt.Name;
                    _Context.Messages.Update(item);
                    _Context.SaveChanges();
                }
            }

            var data = await _Context.Users.Where(a => a.Id == user.Id && a.IsDeleted != true).FirstOrDefaultAsync();
            UserDto dto = new();

            dto.Status = data.Status;
            dto.Email = data.Email;
            dto.Address = data.Address;
            dto.Date = data.Date;
            dto.Subscriptions = data.Subscriptions;
            dto.Id = data.Id;
            dto.IdImage = data.IdImage;
            dto.Rate = data.Rate;
            dto.Image = data.Image;
            dto.IsDeleted = data.IsDeleted;
            dto.IsVerified = data.IsVerified;
            dto.LoginType = data.LoginType;
            dto.MemberSince = data.MemberSince;
            dto.MunicipalityId = data.MunicipalityId;
            dto.Name = data.Name;
            dto.Number = data.Number;
            dto.Password = data.Password;
            dto.ProvinceId = data.ProvinceId;
            dto.UniqueId = data.UniqueId;
            dto.VerificationSent = data.VerificationSent;
            dto.Favorites = _Context.Favorites.Where(a => a.UserId == data.Id).ToList();
            dto.Ratintg = await _Context.Ratings.Where(a => a.UidTo == data.Id).ToListAsync();
            dto.Followers = _Context.Folowers.Where(a => a.Folowers == data.Id).ToList();
            return dto;
        }
        public async Task<Tuple<UserDto, string>> SignUp(UserDto user)
        {
            try
            {
                if (user.LoginType == "Custom")
                {
                    string encpass = Methods.Encrypt(user.Password);
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        var emailCheck = await _Context.Users.Where(a => a.Email == user.Email && a.IsDeleted != true).FirstOrDefaultAsync();
                        if (emailCheck != null)
                        {
                            return Tuple.Create(user, "Ya existe el correo electrónico");
                        }

                        User dt = new();
                        dt.Name = user.Name;
                        dt.Email = user.Email;
                        dt.Password = encpass;
                        dt.Number = user.Number;
                        dt.Subscriptions = DateTimeOffset.Now.AddDays(-5).ToUnixTimeMilliseconds();
                        dt.Date = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        dt.MemberSince = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        dt.IsDeleted = false;
                        dt.LoginType = "Custom";
                        await _Context.Users.AddAsync(dt);
                        _Context.SaveChanges();

                        int uid = await _Context.Users.MaxAsync(a => a.Id);

                        var data = await _Context.Users.Where(a => a.Id == uid && a.IsDeleted != true).FirstOrDefaultAsync();
                        UserDto dto = new();

                        dto.Status = data.Status;
                        dto.Email = data.Email;
                        dto.Address = data.Address;
                        dto.Date = data.Date;
                        dto.Subscriptions = data.Subscriptions;
                        dto.Id = data.Id;
                        dto.IdImage = data.IdImage;
                        dto.Rate = data.Rate;
                        dto.Image = data.Image;
                        dto.IsDeleted = data.IsDeleted;
                        dto.IsVerified = data.IsVerified;
                        dto.LoginType = data.LoginType;
                        dto.MemberSince = data.MemberSince;
                        dto.MunicipalityId = data.MunicipalityId;
                        dto.Name = data.Name;
                        dto.Number = data.Number;
                        dto.Password = data.Password;
                        dto.ProvinceId = data.ProvinceId;
                        dto.UniqueId = data.UniqueId;
                        dto.VerificationSent = data.VerificationSent;
                        dto.Favorites = _Context.Favorites.Where(a => a.UserId == data.Id).ToList();
                        dto.Ratintg = await _Context.Ratings.Where(a => a.UidTo == data.Id).ToListAsync();
                        dto.Followers = _Context.Folowers.Where(a => a.Folowers == data.Id).ToList();
                        return Tuple.Create(dto, "Success");

                    }
                    //return "Error";
                }
                else
                {
                    var Check = await _Context.Users.Where(a => a.UniqueId == user.UniqueId && a.IsDeleted != true).FirstOrDefaultAsync();
                    if (Check == null)
                    {
                        User dt = new();
                        dt.UniqueId = user.UniqueId;
                        dt.IsDeleted = false;
                        dt.Name = user.Name;
                        dt.Email = user.Email;
                        dt.Number = user.Number;
                        dt.Subscriptions = DateTimeOffset.Now.AddDays(-5).ToUnixTimeMilliseconds();
                        dt.Date = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        dt.MemberSince = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                        dt.LoginType = user.LoginType;
                        await _Context.Users.AddAsync(dt);
                        _Context.SaveChanges();
                    }

                    int uid = await _Context.Users.MaxAsync(a => a.Id);

                    var data = await _Context.Users.Where(a => a.Id == uid && a.IsDeleted != true).FirstOrDefaultAsync();
                    UserDto dto = new();

                    dto.Status = data.Status;
                    dto.Email = data.Email;
                    dto.Address = data.Address;
                    dto.Date = data.Date;
                    dto.Subscriptions = data.Subscriptions;
                    dto.Id = data.Id;
                    dto.IdImage = data.IdImage;
                    dto.Rate = data.Rate;
                    dto.Image = data.Image;
                    dto.IsDeleted = data.IsDeleted;
                    dto.IsVerified = data.IsVerified;
                    dto.LoginType = data.LoginType;
                    dto.MemberSince = data.MemberSince;
                    dto.MunicipalityId = data.MunicipalityId;
                    dto.Name = data.Name;
                    dto.Number = data.Number;
                    dto.Password = data.Password;
                    dto.ProvinceId = data.ProvinceId;
                    dto.UniqueId = data.UniqueId;
                    dto.VerificationSent = data.VerificationSent;
                    dto.Favorites = _Context.Favorites.Where(a => a.UserId == data.Id).ToList();
                    dto.Ratintg = await _Context.Ratings.Where(a => a.UidTo == data.Id).ToListAsync();
                    dto.Followers = _Context.Folowers.Where(a => a.Folowers == data.Id).ToList();
                    return Tuple.Create(dto, "Success");
                }
                return Tuple.Create(user, "Error");
            }
            catch (Exception ex)
            {
                //return ex.Message;
                throw;
            }
        }

        public async Task<Tuple<UserDto, string>> SetEmail(SetUserEmailDto dto)
        {
            UserDto dto1 = new();
            var Check = await _Context.Users.Where(a => a.Id == dto.Id && a.IsDeleted != true).FirstOrDefaultAsync();
            if (Check != null)
            {
                string encpass = Methods.Encrypt(dto.Password);
                if (Check.Password == encpass)
                {
                    var email = await _Context.Users.Where(a => a.Email == dto.Email && a.IsDeleted != true).FirstOrDefaultAsync();
                    if (email == null)
                    {
                        Check.Email = dto.Email;
                        _Context.Users.Update(Check);
                        _Context.SaveChanges();


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
                        dto1.Favorites = _Context.Favorites.Where(a => a.UserId == Check.Id).ToList();
                        dto1.Followers = _Context.Folowers.Where(a => a.Folowers == Check.Id).ToList();
                        return Tuple.Create(dto1, "Success");
                    }
                    return Tuple.Create(dto1, "Ya existe el correo electrónico");
                }
                return Tuple.Create(dto1, "Contraseña antigua inválida");
            }
            return null;
        }
        public async Task<UserDto> SetFavProduct(int id, int pid)
        {
            try
            {
                var dt11 = await _Context.Favorites.Where(a => a.Pid == pid && a.UserId == id).FirstOrDefaultAsync();
                var Check = await _Context.Users.Where(a => a.Id == id && a.IsDeleted == false).FirstOrDefaultAsync();
                if (dt11 == null)
                {
                    Favorite prod = new();
                    prod.Pid = pid;
                    prod.UserId = id;
                    await _Context.Favorites.AddAsync(prod);
                    _Context.SaveChanges();
                }
                else
                {
                    _Context.Favorites.Remove(dt11);
                    _Context.SaveChanges();
                }

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
                dto1.Favorites = _Context.Favorites.Where(a => a.UserId == id).ToList();
                dto1.Followers = _Context.Folowers.Where(a => a.Folowers == id).ToList();
                return dto1;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }

        public async Task<UserDto> SendVerification(VerifyDto dto)
        {
            UserVerification user = new();
            user.UserId = dto.Userid;

            if (dto.FImage != null)
            {
                var filename1 = "";
                Random rnd = new();
                var rn = rnd.Next(111, 999);

                var ImagePath1 = rn + Methods.RemoveWhitespace(dto.FImage.FileName);
                var pathh = "";
                using (FileStream fileStream = File.Create(_environment.WebRootPath + "\\Resources\\Images\\Users\\" + ImagePath1))
                {
                    dto.FImage.CopyTo(fileStream);
                    pathh = Path.Combine(_environment.WebRootPath, "/Resources/Images/Users/" + ImagePath1);
                    filename1 = ImagePath1;
                    fileStream.Flush();
                }
                user.Fimage = "/Resources/Images/Users/" + filename1;
            }
            else
            {
                user.Fimage = "";
            }

            if (dto.BImage != null)
            {
                var filename1 = "";
                Random rnd = new();
                var rn = rnd.Next(111, 999);

                var ImagePath1 = rn + Methods.RemoveWhitespace(dto.BImage.FileName);
                var pathh = "";
                using (FileStream fileStream = File.Create(_environment.WebRootPath + "\\Resources\\Images\\Users\\" + ImagePath1))
                {
                    dto.BImage.CopyTo(fileStream);
                    pathh = Path.Combine(_environment.WebRootPath, "/Resources/Images/Users/" + ImagePath1);
                    filename1 = ImagePath1;
                    fileStream.Flush();
                }
                user.Bimage = "/Resources/Images/Users/" + filename1;
            }
            else
            {
                user.Bimage = "";
            }

            user.Message = dto.Message;
            _Context.UserVerifications.Add(user);

            var dt = _Context.Users.Where(a => a.Id == dto.Userid).FirstOrDefault();
            dt.VerificationSent = true;
            _Context.Users.Update(dt);
            _Context.SaveChanges();

            var Check = await _Context.Users.Where(a => a.Id == dto.Userid && a.IsDeleted == false).FirstOrDefaultAsync();

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
            dto1.Favorites = _Context.Favorites.Where(a => a.UserId == dto.Userid).ToList();
            dto1.Followers = _Context.Folowers.Where(a => a.Folowers == dto.Userid).ToList();
            return dto1;
        }

        public async Task<string> ChangePassword(ChangePassDto dto)
        {
            var dt = await _Context.Users.Where(a => a.Id == dto.Uid).FirstOrDefaultAsync();
            string encpass = Methods.Encrypt(dto.OldPass);
            if (dt.Password == encpass)
            {
                string enpass = Methods.Encrypt(dto.NewPass);
                dt.Password = enpass;
                _Context.Users.Update(dt);
                await _Context.SaveChangesAsync();
            }
            else
            {
                return "Contraseña antigua inválida";
            }
            return "Success";
        }
        public async Task<string> SendEmail(string to)
        {
            try
            {
                var dt = _Context.Users.Where(x => x.Email == to).FirstOrDefault();

                if (dt != null)
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Test Project", "titus.zaman@gmail.com"));
                    message.To.Add(new MailboxAddress("Titus", to));
                    message.Subject = "This Email by Merolikiando App ";
                    message.Body = new TextPart("plain")
                    {
                        Text = Methods.baseurl + "Home/PasswordRest?id= " + dt.Id + "&email=" + dt.Email
                    };
                    using (var client = new SmtpClient())
                    {
                        client.Connect("smtp.gmail.com", 587, false);
                        client.Authenticate("titus.zaman@gmail.com", "adjcozkqqodunwvs");
                        client.Send(message);
                        client.Disconnect(true);
                    }
                    return to;
                }
                return "Not Exist";
            }
            catch (Exception)
            {

                throw;
            }

            
        }

        //public async Task<string> CheckUser(int id)
        //{

        //    var user = _Context.Users.Where(a => a.Id == id ).FirstOrDefault();
        //    if (user.IsBlock == true)
        //    {
        //        return "Block";
        //    }
        //    return "Unblock";
        //}
    }
}
