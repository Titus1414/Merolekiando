namespace Merolekiando.Models.Dtos
{
    public class PromotionDto
    {
        public int Id { get; set; }
        public int Uid { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }
        public string Decription { get; set; }
        public string Image { get; set; }
        public bool? IsPromote { get; set; }

    }
}
