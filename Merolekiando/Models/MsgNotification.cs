using System;
using System.Collections.Generic;

#nullable disable

namespace Merolekiando.Models
{
    public partial class MsgNotification
    {
        public int Id { get; set; }
        public int? Fid { get; set; }
        public int? Uid { get; set; }
        public string Fname { get; set; }
        public string Message { get; set; }
        public long? Date { get; set; }
        public bool? IsRead { get; set; }
    }
}
