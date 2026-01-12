using BIMEvents.Domain;
using Marten;

namespace BIMEvents.Application;

public record UpdateRoofSpecification(Guid PlanId, RoofSpecification RoofSpecification, MetaData MetaData);

public class UpdateRoofSpecificationHandler(IDocumentStore store)
{
    public async Task Handle(UpdateRoofSpecification command, CancellationToken cancellationToken)
    {
        await using var session = store.LightweightSession();
        var plan = await session.LoadAsync<Plan>(command.PlanId, cancellationToken);
        var roofSpecificationAdded = new RoofSpecificationChanged(command.PlanId, command.RoofSpecification, command.MetaData);
        session.Events.Append(command.PlanId, roofSpecificationAdded);
        await session.SaveChangesAsync(cancellationToken);
    }
}