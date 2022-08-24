using Microsoft.AspNetCore.Http;

namespace Merolekiando.Models.Dtos
{
    public class VerifyDto
    {
        public int Userid { get; set; }
        public IFormFile FImage { get; set; }
        public IFormFile BImage { get; set; }
        public string Message { get; set; }
        public string UserName { get; set; }
        public string SFImage { get; set; }
        public string SBImage { get; set; }
    }
}
