using System;
using System.Collections.Generic;

#nullable disable

namespace Merolekiando.Models
{
    public partial class Municipality
    {
        public Municipality()
        {
            ProdMunicipalities = new HashSet<ProdMunicipality>();
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public int? PrvId { get; set; }
        public string Name { get; set; }
        public int? Time { get; set; }

        public virtual Province Prv { get; set; }
        public virtual ICollection<ProdMunicipality> ProdMunicipalities { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
