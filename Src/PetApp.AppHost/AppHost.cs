var builder = DistributedApplication.CreateBuilder(args);

var minio = builder.AddMinioContainer("minio");

var rabbitmq = builder.AddRabbitMQ("messaging");

var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("postgresdb");

var cache = builder.AddRedis("cache");
var jtiBan = builder.AddRedis("jti-ban");


var authService = builder.AddProject<Projects.PetApp_AuthService>("authservice").WithReference(postgresdb)
    .WithReference(postgresdb)
    .WithReference(jtiBan)
    .WaitFor(postgresdb);

builder.AddProject<Projects.PetApp_ArtworkService>("artworkservice");

builder.AddProject<Projects.PetApp_ReactionService>("reactionservice");

builder.AddProject<Projects.PetApp_Gateway>("gateway")
    .WithReference(authService)
    .WithReference(jtiBan)
    .WaitFor(authService)
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.PetApp_UserService>("petapp-userinfoservice");

builder.Build().Run();