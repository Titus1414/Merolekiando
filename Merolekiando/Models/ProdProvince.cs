using System;
using System.Collections.Generic;

#nullable disable

namespace Merolekiando.Models
{
    public partial class ProdProvince
    {
        public int Id { get; set; }
        public int? Pid { get; set; }
        public int? ProvinceId { get; set; }

        public virtual Product PidNavigation { get; set; }
        public virtual Province Province { get; set; }
    }
}
