using BugStore.Api.ExtensionsMethods;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddDependecyInjection();

var app = builder.Build();

app.MapEndpoints(); 

app.Run();