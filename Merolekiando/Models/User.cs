using System;
using System.Collections.Generic;

#nullable disable

namespace Merolekiando.Models
{
    public partial class User
    {
        public User()
        {
            Favorites = new HashSet<Favorite>();
            Folowers = new HashSet<Folower>();
            Messages = new HashSet<Message>();
            Notifications = new HashSet<Notification>();
            ProdViews = new HashSet<ProdView>();
            RatingUidToNavigations = new HashSet<Rating>();
            RatingUsers = new HashSet<Rating>();
            UserVerifications = new HashSet<UserVerification>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public bool? IsVerified { get; set; }
        public bool? VerificationSent { get; set; }
        public bool? Status { get; set; }
        public string Image { get; set; }
        public string IdImage { get; set; }
        public int? ProvinceId { get; set; }
        public string Email { get; set; }
        public string Number { get; set; }
        public int? MunicipalityId { get; set; }
        public long? MemberSince { get; set; }
        public long? Subscriptions { get; set; }
        public long? Date { get; set; }
        public string Address { get; set; }
        public string UniqueId { get; set; }
        public string LoginType { get; set; }
        public decimal? Rate { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual Municipality Municipality { get; set; }
        public virtual Province Province { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }
        public virtual ICollection<Folower> Folowers { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<ProdView> ProdViews { get; set; }
        public virtual ICollection<Rating> RatingUidToNavigations { get; set; }
        public virtual ICollection<Rating> RatingUsers { get; set; }
        public virtual ICollection<UserVerification> UserVerifications { get; set; }
    }
}
