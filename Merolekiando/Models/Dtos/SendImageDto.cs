using Microsoft.AspNetCore.Http;

namespace Merolekiando.Models.Dtos
{
    public class SendImageDto
    {
        public int senderId { get; set; }
        public int receiverId { get; set; }
        public IFormFile link { get; set; }
        public string type { get; set; }
    }
}
