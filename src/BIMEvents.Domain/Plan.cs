namespace BIMEvents.Domain;

public class Plan
{
   public Guid Id { get; set; } 
   public int Version { get; set; }
   public string Name { get; set; }
   public RoofSpecification RoofSpecification { get; set; }
   public DateTimeOffset CreatedAt { get; set; }
   public DateTimeOffset UpdatedAt { get; set; }
}