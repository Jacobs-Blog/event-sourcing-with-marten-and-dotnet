using BIMEvents.Domain;
using Bogus;
using Marten;

namespace BIMEvents.Infrastructure;

public class HistoricDataSeeder(IDocumentStore store)
{
    private static readonly string[] BuildingTypes = 
    {
        "Tower", "Plaza", "Center", "Heights", "Square", "Garden", "Park",
        "Court", "Hall", "House", "Ridge", "Point", "Terrace", "View",
        "Pinnacle", "Summit", "Landmark", "Building", "Complex", "Quarter"
    };

    private string GenerateBuildingName(Faker faker)
    {
        var buildingType = faker.PickRandom(BuildingTypes);
        var name = faker.Company.CompanyName();
        var number = faker.Random.Int(100, 9999);
        
        return faker.Random.Int(0, 2) switch
        {
            0 => $"{name} {buildingType}",
            1 => $"{buildingType} {number}",
            _ => $"{faker.Address.City()} {buildingType}"
        };
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        Faker faker = new();
        var baseDate = DateTime.UtcNow.AddYears(-1);
        
        // Fetch real Keycloak users
        var keycloakService = new KeycloakUserService(
            "http://localhost:8080",
            "bimevents",
            "admin",
            "keycloak" // Get this from Keycloak client settings
        );
    
        var keycloakUsers = await keycloakService.GetUsersAsync();
        if (!keycloakUsers.Any())
            throw new InvalidOperationException("No Keycloak users found!");
        
        keycloakUsers.RemoveAll(x => x.Username == "admin");

        await using var session = store.LightweightSession();

        for (var month = 0; month < 12; month++)
        {
            var monthStart = baseDate.AddMonths(month);

            for (var planIndex = 0; planIndex < 2; planIndex++)
            {
                var user = faker.PickRandom(keycloakUsers);
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
                    PlanName = GenerateBuildingName(faker),
                    MetaData = new MetaData(
                        Guid.Parse(user.Id),
                        $"{user.FirstName} {user.LastName}",
                        user.Username!,
                        user.Email!
                    )
                };

                session.Events.StartStream(planCreated.PlanId, planCreated);
                
                var updateCount = faker.Random.Int(1, 6);
                var currentDate = planCreatedAt;

                for (var i = 0; i < updateCount; i++)
                {
                    var specUser = faker.PickRandom(keycloakUsers);
                    currentDate = currentDate.AddHours(faker.Random.Int(1, 720));

                    var roofSpecUpdated = new RoofSpecificationChanged(
                        planCreated.PlanId,
                        new RoofSpecification(
                            SnowLoadKiloNewtonPerM2: faker.Random.Double(0.5, 3.0),
                            WindUpliftKiloNewtonPerM2: faker.Random.Double(0.3, 2.5)
                        ),
                        new MetaData(
                            Guid.Parse(specUser.Id),
                            $"{specUser.FirstName} {specUser.LastName}",
                            specUser.Username!,
                            specUser.Email!
                        ),
                        currentDate);

                    session.Events.Append(planCreated.PlanId, roofSpecUpdated);
                }
            }
        }

        await session.SaveChangesAsync(cancellationToken);
    }
}