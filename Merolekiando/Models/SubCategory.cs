using System;
using System.Collections.Generic;

#nullable disable

namespace Merolekiando.Models
{
    public partial class SubCategory
    {
        public int Id { get; set; }
        public int? CatId { get; set; }
        public string Name { get; set; }

        public virtual Category Cat { get; set; }
    }
}
