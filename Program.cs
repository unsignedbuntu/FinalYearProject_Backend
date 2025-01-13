using KTUN_Final_Year_Project;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add AutoMapper service
builder.Services.AddAutoMapper(typeof(Program));

// Configure CORS to allow all origins, methods, and headers
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Configure JSON serialization options
builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Add API documentation with Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.ToString());
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "KTUN API", Version = "v1" });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.SupportNonNullableReferenceTypes();
    c.MapType<object>(() => new OpenApiSchema { Type = "object" });
});

// Configure Entity Framework Core with SQL Server
var connectionString = builder.Configuration.GetConnectionString("KTUN_DbContext");

// Modify the connection string to include TrustServerCertificate=True
builder.Services.AddDbContext<KTUN_DbContext>(options =>
    options.UseSqlServer(connectionString + ";TrustServerCertificate=True;" ?? throw new InvalidOperationException("Connection string 'KTUN_DbContext' not found in configuration.")));

// For development purposes, allow insecure SSL connections (dangerous for production)
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.HttpClientHandler.DangerousAcceptAnyServerCertificateValidator", builder.Environment.IsDevelopment());

var app = builder.Build();

// Use the configured CORS policy
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    // Enable Swagger in development environment
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "KTUN API");
    });

    app.UseHttpsRedirection(); // Comment this out for local testing
}


// Enable authorization
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Run the application
app.Run();
