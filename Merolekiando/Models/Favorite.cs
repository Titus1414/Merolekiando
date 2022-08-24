using System;
using System.Collections.Generic;

#nullable disable

namespace Merolekiando.Models
{
    public partial class Favorite
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? Pid { get; set; }

        public virtual Product PidNavigation { get; set; }
        public virtual User User { get; set; }
    }
}
