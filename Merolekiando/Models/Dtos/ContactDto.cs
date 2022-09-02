using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Merolekiando.Models.Dtos
{
    public class ContactDto
    {
        public int Id { get; set; }
        public IFormFile Image { get; set; }
        public string Email { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public string UserImage { get; set; }
        public List<string> BannerImage { get; set; }
        public bool? AllowAll { get; set; }
    }
}
