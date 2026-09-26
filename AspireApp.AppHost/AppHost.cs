var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("pgsql")
    .WithPgWeb()
    .WithDataVolume("my_volume")
    .AddDatabase("mydb");

var server = builder.AddProject<Projects.AspireApp_Server>("server")
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints()
    .WaitFor(db)
    .WithReference(db);

var webfrontend = builder.AddViteApp("webfrontend", "../frontend")
    .WithReference(server)
    .WaitFor(server);


server.PublishWithContainerFiles(webfrontend, "wwwroot");

builder.Build().Run();
