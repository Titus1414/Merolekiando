using Merolekiando.Models;
using System.Collections.Generic;

namespace Merolekando.Models.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public bool? IsVerified { get; set; }
        public bool? VerificationSent { get; set; }
        public bool? Status { get; set; }
        public string? Image { get; set; }
        public string? IdImage { get; set; }
        public int? ProvinceId { get; set; }
        public string? Email { get; set; }
        public string? Number { get; set; }
        public int? MunicipalityId { get; set; }
        public long? MemberSince { get; set; }
        public long? Subscriptions { get; set; }
        public long? Date { get; set; }
        public string? Address { get; set; }
        public string? UniqueId { get; set; }
        public string? LoginType { get; set; }
        public decimal? Rate { get; set; }
        public bool? IsDeleted { get; set; }

        public List<Rating>? Ratintg { get; set; }
        public List<Favorite>? Favorites { get; set; }
        public List<Folower>? Followers { get; set; }
    }
}
