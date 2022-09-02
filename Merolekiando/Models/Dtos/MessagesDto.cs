using System.Collections.Generic;

namespace Merolekiando.Models.Dtos
{
    public class MessagesDto
    {
        public int Id { get; set; }
        public int? From { get; set; }
        public int? To { get; set; }
        public string ConnId { get; set; }
        public string LastMessage { get; set; }
        public string Name { get; set; }
        public bool? Read { get; set; }
        public List<int> ProductIds { get; set; }
        public string FromImage { get; set; }
        public string ProductImage { get; set; }
        public long LastMessgeTime { get; set; }
    }
}
