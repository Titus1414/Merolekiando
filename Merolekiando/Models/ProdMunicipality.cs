using System;
using System.Collections.Generic;

#nullable disable

namespace Merolekiando.Models
{
    public partial class ProdMunicipality
    {
        public int Id { get; set; }
        public int? Pid { get; set; }
        public int? MncId { get; set; }

        public virtual Municipality Mnc { get; set; }
        public virtual Product PidNavigation { get; set; }
    }
}
