using Microsoft.AspNetCore.Http;

namespace Merolekando.Models.Dtos
{
    public class BannerDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public IFormFile Image { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
