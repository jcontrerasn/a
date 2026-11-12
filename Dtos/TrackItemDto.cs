using System;

namespace TrackIT_backend.Dtos
{
    public class TrackItemDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool Completed { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
