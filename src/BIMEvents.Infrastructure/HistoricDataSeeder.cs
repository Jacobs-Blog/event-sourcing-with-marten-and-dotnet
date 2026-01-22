using BIMEvents.Domain;
using Bogus;
using Marten;

namespace BIMEvents.Infrastructure;

public class HistoricDataSeeder(IDocumentStore store)
{
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        Faker faker = new();
        var baseDate = DateTime.UtcNow.AddYears(-1);

        await using var session = store.LightweightSession();

        for (var month = 0; month < 12; month++)
        {
            var monthStart = baseDate.AddMonths(month);

            for (var planIndex = 0; planIndex < 2; planIndex++)
            {
                var user = faker.Person;
                var planCreatedAt = new DateTimeOffset(
                    monthStart.Year,
                    monthStart.Month,
                    faker.Random.Int(1, DateTime.DaysInMonth(monthStart.Year, monthStart.Month)),
                    faker.Random.Int(9, 18),
                    faker.Random.Int(0, 59),
                    faker.Random.Int(0, 59),
                    TimeSpan.Zero
                );
                
                var planCreated = new PlanCreated
                {
                    CreatedAt = planCreatedAt,
                    PlanId = Guid.NewGuid(),
                    PlanName = faker.Commerce.ProductName(),
                    MetaData = new MetaData(user.FullName, user.UserName, user.Email)
                };

                session.Events.StartStream(planCreated.PlanId, planCreated);
                
                var updateCount = faker.Random.Int(1, 6);
                var currentDate = planCreatedAt;

                for (var i = 0; i < updateCount; i++)
                {
                    var specUser = faker.Random.Bool() ? user : faker.Person;
                    currentDate = currentDate.AddHours(faker.Random.Int(1, 720));

                    var roofSpecUpdated = new RoofSpecificationChanged(
                        planCreated.PlanId,
                        new RoofSpecification(
                            SnowLoadKiloNewtonPerM2: faker.Random.Double(0.5, 3.0),
                            WindUpliftKiloNewtonPerM2: faker.Random.Double(0.3, 2.5)
                        ),
                        new MetaData(specUser.FullName, specUser.UserName, specUser.Email), 
                        currentDate);

                    session.Events.Append(planCreated.PlanId, roofSpecUpdated);
                }
            }
        }

        await session.SaveChangesAsync(cancellationToken);
    }
}