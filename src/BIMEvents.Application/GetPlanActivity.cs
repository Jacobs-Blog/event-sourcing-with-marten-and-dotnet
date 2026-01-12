using BIMEvents.Domain;
using Marten;

namespace BIMEvents.Application;

public record GetPlanActivity(Guid PlanId);

public class GetPlanActivityHandler(IDocumentStore store)
{
    public async Task<IReadOnlyList<PlanActivity>> Handle(GetPlanActivity query, CancellationToken cancellationToken)
    {
        await using var session = store.LightweightSession();
        var plan = await session.LoadAsync<Plan>(query.PlanId, cancellationToken);
        var events = await session.Events.FetchStreamAsync(plan!.Id, token: cancellationToken);
        return events.Select(e => new PlanActivity 
        {
            Id = e.Id,
            Timestamp = e.Timestamp,
            Data = e.Data,
            EventType = e.EventType.ToString()
        }).ToList();
        
    }
}