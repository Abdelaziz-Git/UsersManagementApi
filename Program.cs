using TailorSoftAPI.Extensions.Services.Collections;
using TailorSoftAPI.Extensions.Middleware;
using TailorSoftAPI.Extensions.Endpoints;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();
app.UseMiddleware();
app.UseEndpoints();
await app.RunAsync();
