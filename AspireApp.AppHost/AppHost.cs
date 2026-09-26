var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("pgsql")
    .WithPgWeb()
    .WithDataVolume()
    .AddDatabase("mydb");

var server = builder.AddProject<Projects.AspireApp_Server>("server")
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints()
    .WithReference(db);

var webfrontend = builder.AddViteApp("webfrontend", "../frontend")
    .WithReference(server)
    .WaitFor(server);

server.PublishWithContainerFiles(webfrontend, "wwwroot");

builder.Build().Run();
