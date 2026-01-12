namespace BIMEvents.Domain;

public class PlanCreated
{
    public DateTimeOffset CreatedAt { get;set; } = DateTime.UtcNow;
    
    public required Guid PlanId { get; set;  }
    public required string PlanName { get; set;  }
    public required MetaData MetaData { get; set; }
}