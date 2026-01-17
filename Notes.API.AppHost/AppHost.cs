var builder = DistributedApplication.CreateBuilder(args);

var kafka = builder.AddKafka("kafka");

var postgres = builder.AddPostgres("postgres")
    .WithHostPort(5432).WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var notesDb = postgres.AddDatabase("notesdb");
var tagsDb = postgres.AddDatabase("tagsdb");

var tagsApi = builder.AddProject<Projects.Tags_API>("tags-api")
    .WithReference(tagsDb)
    .WithReference(kafka)
    .WaitFor(tagsDb)
    .WaitFor(kafka);

builder.AddProject<Projects.Notes_API>("notes-api")
    .WithHttpEndpoint(5001, name: "public")
    .WithReference(notesDb)
    .WithReference(tagsApi)
    .WithReference(kafka)
    .WaitFor(notesDb)
    .WaitFor(tagsApi)
    .WaitFor(kafka);

builder.Build().Run();