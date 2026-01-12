using BIMEvents.Domain;
using Marten;

namespace BIMEvents.Application;

public record CreatePlan(string PlanName, MetaData MetaData);

public class CreatePlanHandler(IDocumentStore store)
{
    public async Task<PlanCreated> Handle(CreatePlan command, CancellationToken cancellationToken)
    {
        var planCreated = new PlanCreated
        {
            PlanId = Guid.NewGuid(),
            PlanName = command.PlanName,
            MetaData = command.MetaData
        };

        await using var session = store.LightweightSession();
        session.Events.StartStream(planCreated.PlanId, planCreated);
        await session.SaveChangesAsync(cancellationToken);

        return planCreated;
    }
}