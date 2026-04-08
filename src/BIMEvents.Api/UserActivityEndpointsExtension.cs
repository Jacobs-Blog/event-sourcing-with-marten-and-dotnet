using BIMEvents.Application;
using Wolverine;

namespace BIMEvents.Api;

public static class UserActivityEndpointsExtension
{
    extension(IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder MapUserEndpoints()
        {
            var useractivityRoute = endpoints
                .MapGroup("/useractivity");
            
            useractivityRoute
                .MapGet("/", async (IMessageBus bus, CancellationToken cancellationToken) =>
                {
                    var activity = await bus.InvokeAsync<IReadOnlyList<UserActivityProjection>?>(new GetAllUserActivity(), cancellationToken);
                    return activity is not null ? Results.Ok(activity) : Results.NotFound();
                })
                .WithName("AllUserActivity")
                .WithDescription("Get all user activity");
            
            useractivityRoute
                .MapGet("/{userId}", async (Guid userId, IMessageBus bus, CancellationToken cancellationToken) =>
                {
                    var activity = await bus.InvokeAsync<UserActivityProjection?>(new GetUserActivity(userId), cancellationToken);
                    return activity is not null ? Results.Ok(activity) : Results.NotFound();
                })
                .WithName("UserActivityByUser")
                .WithDescription("Get user activity by user id");
            
            return useractivityRoute;
        }
    }
}