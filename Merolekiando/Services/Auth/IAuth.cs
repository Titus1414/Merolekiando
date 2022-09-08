using Merolekando.Models;
using Merolekando.Models.Dtos;
using Merolekiando.Models;
using Merolekiando.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Merolekando.Services.Auth
{
    public interface IAuth
    {
        public Task<string> Login(User user);
        public Task<Tuple<UserDto, string>> SignUp(UserDto user);
        public Task<List<UserDto>> GetUser();
        public Task<UserDto> GetUserById(int id);
        public Task<UserDto> ResetPasssword(ResetPasswordDto dto);
        public Task<string> DeleteUser(DeleteAccountDto dto);
        public Task<UserDto> SetImage(SetUserImageDto dto);
        public Task<Tuple<UserDto, string>> SetEmail(SetUserEmailDto dto);
        public Task<UserDto> SetFavProduct(int id, int pid);
        public Task<UserDto> SendVerification(VerifyDto dto);
        public Task<UserDto> UpdateUser(UserDto user);
        public Task<string> ChangePassword(ChangePassDto dto);
        public Task<string> SendEmail(string to);
        //public Task<string> CheckUser(int id);
    }
}
