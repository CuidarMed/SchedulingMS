using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class AvailabilityBlockCreate
    {
        [Required]
        public DateTimeOffset StartTime { get; set; }

        [Required]
        public DateTimeOffset EndTime { get; set; }

        [Required]
        [StringLength(255)]
        public string? Reason { get; set; }
    }
}
