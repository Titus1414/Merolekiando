using Merolekiando.Models.Dtos;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Merolekando.Models.Dtos
{
    public class testDto
    {
        public List<IFormFile>? Images { get; set; }
        public string? Id { get; set; }
        public TestCopyDto? provices {get; set;}
    }
}
