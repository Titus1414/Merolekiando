namespace Merolekiando.Models.Dtos
{
    public class MessaveDto
    {
        public int Id { get; set; }
        public int? From { get; set; }
        public int? To { get; set; }
        public string ConnId { get; set; }
        public string LastMessage { get; set; }
        public string Name { get; set; }
        public bool? Read { get; set; }
        public int ProductIds { get; set; }
        public string FromImage { get; set; }
        public long LastMessgeTime { get; set; }
    }
}
