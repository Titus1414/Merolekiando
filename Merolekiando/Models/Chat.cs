using System;
using System.Collections.Generic;

#nullable disable

namespace Merolekiando.Models
{
    public partial class Chat
    {
        public int Id { get; set; }
        public int? SenderId { get; set; }
        public int? RecieverId { get; set; }
        public string Message { get; set; }
        public string ConnId { get; set; }
        public string ConnFrom { get; set; }
        public string ConnTo { get; set; }
        public string Type { get; set; }
        public string Link { get; set; }
        public bool? Status { get; set; }
        public int? Time { get; set; }
        public string Key { get; set; }
    }
}
