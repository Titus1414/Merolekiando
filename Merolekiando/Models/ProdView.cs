using System;
using System.Collections.Generic;

#nullable disable

namespace Merolekiando.Models
{
    public partial class ProdView
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? PId { get; set; }

        public virtual Product PIdNavigation { get; set; }
        public virtual User User { get; set; }
    }
}
