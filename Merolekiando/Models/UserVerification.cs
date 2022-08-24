using System;
using System.Collections.Generic;

#nullable disable

namespace Merolekiando.Models
{
    public partial class UserVerification
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Message { get; set; }
        public string Fimage { get; set; }
        public string Bimage { get; set; }

        public virtual User User { get; set; }
    }
}
