using BIMEvents.Api;
using BIMEvents.Application;
using BIMEvents.Infrastructure;
using JasperFx;
using JasperFx.Events.Daemon;
using JasperFx.Events.Projections;
using Marten;
using Marten.Services;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Wolverine;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services
    .AddAuthentication()
    .AddKeycloakJwtBearer(
        serviceName: "keycloak",
        realm: "api",
        options =>
        {
            options.Authority = "http://localhost:8080/realms/bimevents";
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidIssuer = "http://localhost:8080/realms/bimevents"
            };

            options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        });
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddOpenApi(options => options.AddScalarTransformers());
builder.AddNpgsqlDataSource("marten");
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
});
builder.Host.UseWolverine(options => options.ApplicationAssembly = typeof(CreatePlan).Assembly);
builder.Services.AddMarten(options =>
    {
        options.OpenTelemetry.TrackConnections = TrackLevel.Normal;
        options.OpenTelemetry.TrackEventCounters();
        options.AutoCreateSchemaObjects = AutoCreate.All; 
        options.Projections.Add<PlanProjection>(ProjectionLifecycle.Inline); 
        options.Projections.Add<UserActivityProjectionBuilder>(ProjectionLifecycle.Inline);
    })
    .AddAsyncDaemon(DaemonMode.Solo)
    .UseNpgsqlDataSource();
builder.Services.AddScoped<HistoricDataSeeder>();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapOpenApi();
app.MapPlanEndpoints();
app.MapUserEndpoints();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<HistoricDataSeeder>();
    await seeder.SeedAsync(app.Lifetime.ApplicationStopping);
}

return await app.RunJasperFxCommands(args);