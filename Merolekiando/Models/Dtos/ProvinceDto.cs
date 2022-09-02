using Merolekiando.Models;
using Merolekiando.Models.Dtos;
using System.Collections.Generic;

namespace Merolekando.Models
{
    public class ProvinceDto
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public int? Time { get; set; }

        public List<MunicipalityDto> Municipalitiees { get; set; }
    }
}
