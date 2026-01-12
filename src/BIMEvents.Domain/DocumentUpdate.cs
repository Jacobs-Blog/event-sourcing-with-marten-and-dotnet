namespace BIMEvents.Domain;

public abstract class DocumentUpdate
{
    public DateTimeOffset Time { get; set; } = DateTimeOffset.UtcNow;

    public abstract void Apply(Plan plan);
}