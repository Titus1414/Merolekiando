using Merolekando.Models;
using Merolekiando.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Merolekando.Services.Token
{
    public class JwtToken : IJwtToken
    {
        public readonly IConfiguration _config;
        public JwtToken(IConfiguration iconfiguration)
        {
            _config = iconfiguration;
        }
        public string CreateUserToken(User user)
        {
            var claims = new[]
               {
                          new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                          new Claim(ClaimTypes.Name,user.Id.ToString()),
                          new Claim("Name", value: user.Name),
                          new Claim("ID",user.Id.ToString())
                   };

            var key = new SymmetricSecurityKey(Encoding.UTF8.
                GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                //Expires = CommonMethods.EstDateTime().AddDays(5),
                Expires = DateTime.Now.AddDays(5),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var Createtoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(Createtoken);

            return token;
        }
    }
}
