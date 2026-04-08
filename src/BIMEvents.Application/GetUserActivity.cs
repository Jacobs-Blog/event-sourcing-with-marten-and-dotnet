using Marten;

namespace BIMEvents.Application;

public record GetUserActivity(Guid UserId);

public class GetUserActivityHandler(IDocumentStore store)
{
    public async Task<UserActivityProjection> Handle(GetUserActivity query, CancellationToken cancellationToken)
    {
        await using var session = store.QuerySession();
        var activity = await session.LoadAsync<UserActivityProjection>(query.UserId, cancellationToken);
        return activity;
    }
}