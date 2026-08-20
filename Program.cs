using UsersManagementApi.Extensions.Services.Collections;
using UsersManagementApi.Extensions.Middleware;
using UsersManagementApi.Extensions.Endpoints;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();
app.UseMiddleware();
app.UseEndpoints();
await app.RunAsync();
