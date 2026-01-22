using Marten;

namespace BIMEvents.Application;

public record GetAllUserActivity();

public class GetAllUserActivityHandler(IDocumentStore store)
{
    public async Task<IReadOnlyList<UserActivityProjection>> Handle(GetAllUserActivity query, CancellationToken cancellationToken)
    {
        await using var session = store.QuerySession();
        var activities = await session.Query<UserActivityProjection>()
            .OrderByDescending(x => x.LastActivity)
            .ToListAsync(token: cancellationToken);
        return activities;
        
        // await using var session = store.QuerySession();
        // var activity = await session.QueryAsync<IReadOnlyList<UserActivityProjection>>();
        // return activity;
        // is not null ? Results.Ok(activity) : Results.NotFound();
        // await using var session = store.LightweightSession();
        // var plan = await session.LoadAsync<Plan>(query.PlanId, cancellationToken);
        // var events = await session.Events.FetchStreamAsync(plan!.Id, token: cancellationToken);
        // return events.Select(e => new PlanActivity
        // {
        //     Id = e.Id,
        //     Timestamp = e.Timestamp,
        //     Data = e.Data,
        //     EventType = e.EventType.ToString()
        // }).ToList();
    }
}