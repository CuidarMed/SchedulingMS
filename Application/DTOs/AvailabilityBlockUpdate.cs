using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class AvailabilityBlockUpdate
    {
        [Required]
        public DateTimeOffset StartTime { get; set; }

        [Required]
        public DateTimeOffset EndTime { get; set; }

        [Required]
        [StringLength(255)]
        public string? Reason { get; set; }

        public bool? IsActive { get; set; }
    }
}
