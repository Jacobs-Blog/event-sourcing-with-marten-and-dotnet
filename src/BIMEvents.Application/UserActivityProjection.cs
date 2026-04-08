namespace BIMEvents.Application;

public class UserActivityProjection
{
    public Guid Id { get; set; } = default!;
    public Guid UserId { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public int PlansCreated { get; set; }
    public int SpecificationsUpdated { get; set; }
    public DateTimeOffset LastActivity { get; set; }
    public List<MonthlyActivity> ActivityByMonth { get; set; } = new();
}

public class MonthlyActivity
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int PlansCreated { get; set; }
    public int SpecificationsUpdated { get; set; }
}