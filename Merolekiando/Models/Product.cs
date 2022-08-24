using System;
using System.Collections.Generic;

#nullable disable

namespace Merolekiando.Models
{
    public partial class Product
    {
        public Product()
        {
            ConversationCounts = new HashSet<ConversationCount>();
            Favorites = new HashSet<Favorite>();
            Notifications = new HashSet<Notification>();
            ProdImages = new HashSet<ProdImage>();
            ProdMunicipalities = new HashSet<ProdMunicipality>();
            ProdProvinces = new HashSet<ProdProvince>();
            ProdViews = new HashSet<ProdView>();
        }

        public int Id { get; set; }
        public int? SellerId { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int? ChildCategory { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public string Title { get; set; }
        public bool? FireOnPrice { get; set; }
        public bool? IsSold { get; set; }
        public bool? IsPromoted { get; set; }
        public bool? IsPickup { get; set; }
        public bool? IsDelivering { get; set; }
        public string Condition { get; set; }
        public long? CreatedDate { get; set; }
        public bool? IsReported { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<ConversationCount> ConversationCounts { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<ProdImage> ProdImages { get; set; }
        public virtual ICollection<ProdMunicipality> ProdMunicipalities { get; set; }
        public virtual ICollection<ProdProvince> ProdProvinces { get; set; }
        public virtual ICollection<ProdView> ProdViews { get; set; }
    }
}
