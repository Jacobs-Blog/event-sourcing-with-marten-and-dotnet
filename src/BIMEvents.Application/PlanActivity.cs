namespace BIMEvents.Application;

public class PlanActivity
{
    public Guid Id { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public object Data { get; set; }
    public string EventType { get; set; } 
}