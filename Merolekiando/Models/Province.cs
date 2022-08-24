using System;
using System.Collections.Generic;

#nullable disable

namespace Merolekiando.Models
{
    public partial class Province
    {
        public Province()
        {
            Municipalities = new HashSet<Municipality>();
            ProdProvinces = new HashSet<ProdProvince>();
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? Time { get; set; }

        public virtual ICollection<Municipality> Municipalities { get; set; }
        public virtual ICollection<ProdProvince> ProdProvinces { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
