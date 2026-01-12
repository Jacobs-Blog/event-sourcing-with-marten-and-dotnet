using BIMEvents.Api;
using BIMEvents.Application;
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
            
            if (builder.Environment.IsDevelopment())
                options.RequireHttpsMetadata = false;
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
        options.DatabaseSchemaName = "cli";
        options.OpenTelemetry.TrackConnections = TrackLevel.Normal;
        options.OpenTelemetry.TrackEventCounters();
        options.AutoCreateSchemaObjects = AutoCreate.All; //.All will wipe out the schema each time this is run
        options.Projections.Add<PlanProjection>(ProjectionLifecycle.Inline); 
    }).AddAsyncDaemon(DaemonMode.Solo)
    .UseNpgsqlDataSource();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapOpenApi();
app.MapPlanEndpoints();

return await app.RunJasperFxCommands(args);