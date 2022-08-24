﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Merolekiando.Models
{
    public partial class Message
    {
        public int Id { get; set; }
        public int? From { get; set; }
        public int? To { get; set; }
        public string ConnId { get; set; }
        public string LastMessage { get; set; }
        public string Name { get; set; }
        public bool? Read { get; set; }
        public string ProductIds { get; set; }

        public virtual User ToNavigation { get; set; }
    }
}
