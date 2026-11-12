using System.ComponentModel.DataAnnotations;

namespace TrackIT_backend.Dtos
{
    public class CreateTrackItemDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }
    }
}
