namespace Application.DTOs
{
    public class AvailabilityBlockResponse
    {
        public long BlockId { get; set; }

        public long DoctorId { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public string? Reason { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
