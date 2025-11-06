namespace Application.DTOs
{
    public class AvailabilitySlot
    {
        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public bool IsBlocked { get; set; }
    }
}
