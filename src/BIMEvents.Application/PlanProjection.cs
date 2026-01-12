using BIMEvents.Domain;
using Marten.Events.Aggregation;

namespace BIMEvents.Application;

public class PlanProjection : SingleStreamProjection<Plan, Guid>
{
    public Plan Create(PlanCreated created)
    {
        var plan = new Plan()
        {
            Id = created.PlanId,
            Name = created.PlanName,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.CreatedAt
        };

        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        Console.WriteLine($"Plan '{plan.Name}' created");
        return plan;
    }
    
    public void Apply(RoofSpecificationChanged @event, Plan plan)
    {
        plan.RoofSpecification = @event.RoofSpecification;
        plan.UpdatedAt = DateTimeOffset.UtcNow;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Roof specification changed for '{plan.Name}': {@event.RoofSpecification}");
    }
}