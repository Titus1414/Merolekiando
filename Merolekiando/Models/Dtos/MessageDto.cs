namespace Merolekiando.Models.Dtos
{
    public class MessageDto
    {
        public int From { get; set; }
        public int To { get; set; }
        public string Link { get; set; }
        public long Time { get; set; }
        public string Type { get; set; }
    }
}
