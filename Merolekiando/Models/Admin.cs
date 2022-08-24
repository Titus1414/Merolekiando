using System;
using System.Collections.Generic;

#nullable disable

namespace Merolekiando.Models
{
    public partial class Admin
    {
        public int Id { get; set; }
        public int? CurrentColor { get; set; }
        public bool? AllowAll { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string SubscriptionImage { get; set; }
    }
}
