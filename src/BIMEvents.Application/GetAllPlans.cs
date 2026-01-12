using BIMEvents.Domain;
using Marten;

namespace BIMEvents.Application;

public record GetAllPlans();

public class GetAllPlansHandler(IDocumentStore store)
{
    public async Task<IReadOnlyList<Plan>> Handle(GetAllPlans query, CancellationToken cancellationToken)
    {
        await using var session = store.QuerySession();
        var plans = await session.Query<Plan>()
            .ToListAsync(cancellationToken);
        return plans;
    }
}