using BIMEvents.Domain;
using Marten;

namespace BIMEvents.Application;

public record GetPlanById(Guid PlanId);

public class GetPlanByIdHandler(IDocumentStore store)
{
    public async Task<Plan?> Handle(GetPlanById query, CancellationToken cancellationToken)
    {
        await using var session = store.QuerySession();
        return await session.LoadAsync<Plan>(query.PlanId, cancellationToken);
    }
}