using Merolekando.Models;
using Merolekando.Models.Dtos;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Merolekiando.Models.Dtos
{
    public class NotificationDto
    {
        public int uid { get; set; }
        public string uName { get; set; }
        public int Id { get; set; }
        public int? SellerId { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
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
        public List<IFormFile> images { get; set; }
        public List<int> removeImagesId { get; set; }

        public List<string>? imagesGet { get; set; }
        public List<ProdProvince>? Provice { get; set; }
        public List<ProdMunicipality>? Municipalities { get; set; }
        public List<ProdImagesDto>? ProdImages { get; set; }
        public List<ProdProviceDto>? Provices { get; set; }
        public List<ProdMuniciplityDto>? Municipality { get; set; }
        public List<ProdView>? ProdViews { get; set; }
        public List<ProvinceDto> provinceDtos { get; set; }
    }
}
