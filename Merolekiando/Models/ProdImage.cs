using System;
using System.Collections.Generic;

#nullable disable

namespace Merolekiando.Models
{
    public partial class ProdImage
    {
        public int Id { get; set; }
        public int? PId { get; set; }
        public string Image { get; set; }
        public bool? IsActive { get; set; }

        public virtual Product PIdNavigation { get; set; }
    }
}
