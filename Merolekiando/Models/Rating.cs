using System;
using System.Collections.Generic;

#nullable disable

namespace Merolekiando.Models
{
    public partial class Rating
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? UidTo { get; set; }
        public string Compliment { get; set; }
        public decimal? Rating1 { get; set; }

        public virtual User UidToNavigation { get; set; }
        public virtual User User { get; set; }
    }
}
