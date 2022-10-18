namespace Merolekiando.Models.WebDtos
{
    public class ProductCard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Category { get; set; }
        public decimal? Price { get; set; }
        public bool? IsPromot { get; set; }
    }
}
