using KTUN_Final_Year_Project;
using KTUN_Final_Year_Project.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Logging konfigürasyonu
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Console logging options
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole()
        .AddDebug()
        .SetMinimumLevel(LogLevel.Debug);
});

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

// Redis servisini ekle
builder.Services.AddSingleton<IRedisService>(provider =>
{
    var redis = new RedisService("127.0.0.1:6379");
    return redis;
});

// For development purposes, allow insecure SSL connections
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.HttpClientHandler.DangerousAcceptAnyServerCertificateValidator", builder.Environment.IsDevelopment());

var app = builder.Build();

// Request/Response logging middleware
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

    // Request logging
    logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");
    var originalBodyStream = context.Response.Body;

    await next();

    // Response logging
    logger.LogInformation($"Response Status: {context.Response.StatusCode}");
});

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

    // Global exception handling
    app.Use(async (context, next) =>
    {
        try
        {
            await next();
        }
        catch (Exception ex)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Unhandled exception occurred");
            throw;
        }
    });
}

// Enable routing and endpoints
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Run the application
app.Run();