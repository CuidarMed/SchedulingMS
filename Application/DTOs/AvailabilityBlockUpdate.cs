using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.DTOs
{
    public class AvailabilityBlockUpdate
    {
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public string? Reason { get; set; }
        public string? Note { get; set; }
        public bool? AllDay { get; set; }
    }
}
