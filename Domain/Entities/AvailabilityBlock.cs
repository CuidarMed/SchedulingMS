namespace Domain.Entities
{
    public class AvailabilityBlock
    {
        public long BlockId { get; set; }
        public long DoctorId { get; set; } // Relación con el médico (DirectoryMS)
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public string? Reason { get; set; } // motivo del bloqueo (vacaciones, licencia, etc.)
        public string? Note { get; set; } // observaciones adicionales
        public bool AllDay { get; set; } = false; // si bloquea todo el día
        public bool IsBlock { get; set; }
        // Audit
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        // Soft delete
        public bool IsDeleted { get; set; } = false;
    }
}
