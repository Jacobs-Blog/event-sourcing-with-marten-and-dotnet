using BIMEvents.Domain;
using Marten.Events.Projections;

namespace BIMEvents.Application;

public class UserActivityProjectionBuilder : MultiStreamProjection<UserActivityProjection, Guid>
{
    public UserActivityProjectionBuilder()
    {
        Identity<PlanCreated>(e => e.MetaData.UserId);
        Identity<RoofSpecificationChanged>(e => e.MetaData.UserId);
    }

    public void Apply(UserActivityProjection projection, PlanCreated @event)
    {
        projection.Id = @event.MetaData.UserId;
        projection.UserId = @event.MetaData.UserId;
        projection.UserName = @event.MetaData.UserName;
        projection.Email = @event.MetaData.Email;
        projection.PlansCreated++;
        projection.LastActivity = @event.CreatedAt;

        var month = new MonthlyActivity
        {
            Year = @event.CreatedAt.Year,
            Month = @event.CreatedAt.Month,
            PlansCreated = 1,
            SpecificationsUpdated = 0
        };

        var existing = projection.ActivityByMonth.FirstOrDefault(m => 
            m.Year == month.Year && m.Month == month.Month);

        if (existing != null)
            existing.PlansCreated++;
        else
            projection.ActivityByMonth.Add(month);
    }

    public void Apply(UserActivityProjection projection, RoofSpecificationChanged @event)
    {
        projection.SpecificationsUpdated++;
        
        if (@event.CreatedAt > projection.LastActivity)
            projection.LastActivity = @event.CreatedAt;

        var month = new MonthlyActivity
        {
            Year = @event.CreatedAt.Year,
            Month = @event.CreatedAt.Month,
            PlansCreated = 0,
            SpecificationsUpdated = 1
        };

        var existing = projection.ActivityByMonth.FirstOrDefault(m => 
            m.Year == month.Year && m.Month == month.Month);

        if (existing != null)
            existing.SpecificationsUpdated++;
        else
            projection.ActivityByMonth.Add(month);
    }
}
