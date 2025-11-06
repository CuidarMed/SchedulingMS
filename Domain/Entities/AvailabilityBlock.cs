using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class AvailabilityBlock
    {
        [Key]
        public long BlockId { get; set; }

        [Required]
        public long DoctorId { get; set; }  // Referencia al doctor (DirectoryMS)

        [Required]
        public DateTimeOffset StartTime { get; set; }

        [Required]
        public DateTimeOffset EndTime { get; set; }

        [Required]
        public bool IsBlock { get; set; } // true = bloqueo, false = disponibilidad

        [MaxLength(255)]
        public string? Reason { get; set; }

        public string? Note { get; set; }

        [Required]
        public bool AllDay { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        // Soft delete
        public bool Deleted { get; set; }
    }
}
