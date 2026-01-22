namespace BIMEvents.Domain;

public record RoofSpecificationChanged(Guid PlanId, RoofSpecification RoofSpecification, MetaData MetaData, DateTimeOffset CreatedAt);