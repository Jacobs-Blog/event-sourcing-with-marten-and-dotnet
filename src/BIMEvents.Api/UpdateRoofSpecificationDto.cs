using BIMEvents.Application;
using BIMEvents.Domain;

namespace BIMEvents.Api;

public class UpdateRoofSpecificationDto(Guid planId, RoofSpecification roofSpecification)
{
   public Guid PlanId { get; } = planId;
   public RoofSpecification RoofSpecification { get; } = roofSpecification;
   
   public UpdateRoofSpecification CreateUpdateRoofSpecification(MetaData metaData) =>
       new UpdateRoofSpecification(PlanId, RoofSpecification, metaData);
    
}