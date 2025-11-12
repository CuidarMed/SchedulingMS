using System.ComponentModel;
using System.Text.Json.Serialization;

public class AvailabilityBlockCreate
{
    [JsonConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset? StartTime { get; set; }

    [JsonConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset? EndTime { get; set; }

    public string? Reason { get; set; }
    public string? Note { get; set; }
    public bool AllDay { get; set; } = false;
}