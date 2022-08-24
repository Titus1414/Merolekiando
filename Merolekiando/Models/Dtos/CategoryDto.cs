using Merolekiando.Models;
using System.Collections.Generic;

namespace Merolekando.Models.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public long? Time { get; set; }

        public List<SubCategory> SubCategories { get; set; }
    }
}
