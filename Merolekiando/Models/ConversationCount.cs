using System;
using System.Collections.Generic;

#nullable disable

namespace Merolekiando.Models
{
    public partial class ConversationCount
    {
        public int Id { get; set; }
        public int? UserIdTo { get; set; }
        public int? UserIdFrom { get; set; }
        public int? Pid { get; set; }

        public virtual Product PidNavigation { get; set; }
    }
}
