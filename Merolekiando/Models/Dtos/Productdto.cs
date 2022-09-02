using Merolekiando.Models;
using Merolekiando.Models.Dtos;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Merolekando.Models.Dtos
{
    public class Productdto
    {
        public int? Id { get; set; }
        public int? SellerId { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int? ChatCount { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? Title { get; set; }
        public bool? FireOnPrice { get; set; }
        public bool? IsSold { get; set; }
        public bool? IsPromoted { get; set; }
        public bool? IsPickup { get; set; }
        public bool? IsDelivering { get; set; }
        public string? Condition { get; set; }
        public bool? IsReported { get; set; }
        public long? CreatedDate { get; set; }
        public List<IFormFile>? images { get; set; }
        public List<int>? removeImagesId { get; set; }
        public List<string>? imagesGet { get; set; }
        public List<ProdImagesDto>? ProdImages { get; set; }
        public List<ProdViewsDto>? ProdViews { get; set; }
        public List<ProvinceDto>? provinceDtos { get; set; }

    }
}
