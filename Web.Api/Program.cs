using Application;
using Infrastructure;
using Serilog;
using Web.Api;
using Web.Api.Extensions;
using Web.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddPresentation();

var app = builder.Build();

Log.Information("Application setup completed successfully.");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Apply the database migrations
    app.ApplyMigrations();
}

// Add the custom exception handler
app.UseExceptionHandler("/api/errors");

// Endpoint for health checks
app.UseHealthChecks("/api/health");

app.UseHttpsRedirection();

app.UseAuthorization();

// Add the custom exception handling middleware
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapControllers();

Log.Information("Starting the application...");
app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }
