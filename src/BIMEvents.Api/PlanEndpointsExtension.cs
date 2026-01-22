using BIMEvents.Application;
using BIMEvents.Domain;
using BIMEvents.Infrastructure;
using Wolverine;

namespace BIMEvents.Api;

public static class PlanEndpointsExtension
{
    extension(IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder MapPlanEndpoints()
        {
            var planRoute = endpoints
                .MapGroup("/plan");

            planRoute
                .MapGet("/{id:guid}", async (Guid id, IMessageBus bus) =>
                {
                    var plan = await bus.InvokeAsync<Plan?>(new GetPlanById(id));
                    return plan is not null ? Results.Ok(plan) : Results.NotFound();
                })
                .WithName("PlanById");
            
            planRoute
                .MapGet("/activity/{id:guid}", async (Guid id, IMessageBus bus) =>
                {
                    var activity = await bus.InvokeAsync<IReadOnlyList<PlanActivity>?>(new GetPlanActivity(id));
                    return activity is not null ? Results.Ok(activity) : Results.NotFound();
                }).WithName("PlanActivityById");

            planRoute
                .MapGet("/all", async (IMessageBus bus) =>
                {
                    var plans = await bus.InvokeAsync<IReadOnlyList<Plan>>(new GetAllPlans());
                    return Results.Ok(plans);
                }).WithName("AllPlans");

            planRoute
                .MapPost("/create", async (CreatePlanDto body, IHttpContextAccessor contextAccessor, IMessageBus bus) =>
                {
                    var createPlan = body.CreatePlan(contextAccessor.GetAccount());
                    var plan = await bus.InvokeAsync<PlanCreated>(createPlan);
                    return Results.Created($"/plan/{plan.PlanId}", plan);
                })
                .WithName("CreatePlan")
                .RequireAuthorization();

            planRoute
                .MapPost("/updateroofspecification", async (UpdateRoofSpecificationDto body,
                    IHttpContextAccessor contextAccessor, IMessageBus bus) =>
                {
                    var updateRoofSpecification = body.CreateUpdateRoofSpecification(contextAccessor.GetAccount());
                    await bus.InvokeAsync(updateRoofSpecification);

                    var plan = await bus.InvokeAsync<Plan?>(new GetPlanById(body.PlanId));
                    return plan is not null ? Results.Accepted($"/plan/{plan.Id}", plan) : Results.NotFound();
                })
                .WithName("UpdateRoofSpecification")
                .RequireAuthorization();

            return planRoute;
        }
    }
}