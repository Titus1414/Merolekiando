using Microsoft.AspNetCore.Http;

namespace Merolekando.Models.Dtos
{
    public class SetUserImageDto
    {
        public int Id { get; set; }
        public IFormFile Image { get; set; }
    }
}
