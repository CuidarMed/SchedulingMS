using System.ComponentModel;
using System.Text.Json.Serialization;

public class AvailabilityBlockResponse
{
    public long BlockId { get; set; }
    public long DoctorId { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public string? Reason { get; set; }
    public string? Note { get; set; }
    public bool AllDay { get; set; }
    public bool IsBlock { get; set; }   
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}