using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username", "admin");
var password = builder.AddParameter("password", "keycloak");
var keycloak = builder
    .AddKeycloak("keycloak", 8080, username, password)
    .WithRealmImport("../../keycloak/bimevents-realm.json")
    .WithOtlpExporter();

var postgresdb = builder.AddPostgres("marten")
    .WithPgAdmin();

var api = builder
    .AddProject<Projects.BIMEvents_Api>("api")
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithReference(keycloak)
    .WaitFor(keycloak);

var scalar = builder.AddScalarApiReference()
    .WithApiReference(api)
    .WaitFor(api);
builder.Build().Run();