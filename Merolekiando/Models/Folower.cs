using System;
using System.Collections.Generic;

#nullable disable

namespace Merolekiando.Models
{
    public partial class Folower
    {
        public int Id { get; set; }
        public int? Fuser { get; set; }
        public int? Folowers { get; set; }

        public virtual User FuserNavigation { get; set; }
    }
}
