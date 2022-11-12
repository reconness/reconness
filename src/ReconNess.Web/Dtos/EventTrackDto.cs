using ReconNess.Entities.Enum;
using System;

namespace ReconNess.Web.Dtos
{
    public class EventTrackDto
    {
        public Guid Id { get; set; }
        
        public string Description { get; set; }

        public string Username { get; set; }

        public string Status { get; set; }

        public bool Read { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
