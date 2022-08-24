using System;
using System.Collections.Generic;

#nullable disable

namespace Merolekiando.Models
{
    public partial class Notification
    {
        public int Id { get; set; }
        public int? Uid { get; set; }
        public int? Pid { get; set; }
        public bool? IsRead { get; set; }

        public virtual Product PidNavigation { get; set; }
        public virtual User UidNavigation { get; set; }
    }
}
