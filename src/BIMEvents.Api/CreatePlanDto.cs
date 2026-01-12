using BIMEvents.Application;
using BIMEvents.Domain;

namespace BIMEvents.Api;

public class CreatePlanDto(string planName)
{
    public string PlanName { get; set; } = planName;
    
    public CreatePlan CreatePlan(MetaData metaData) =>
        new CreatePlan(PlanName, metaData);
}