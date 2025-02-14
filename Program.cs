using KTUN_Final_Year_Project;
using KTUN_Final_Year_Project.Entities;
using KTUN_Final_Year_Project.Options;  // Options sýnýflarýnýn namespace'ini ekle
using KTUN_Final_Year_Project.Services; // CertificateLoadingService'in namespace'ini ekle
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using Microsoft.OpenApi.Models;
using System.Runtime.ConstrainedExecution;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Read Kestrel configuration from appsettings.json
builder.Services.Configure<KestrelServerOptions>(builder.Configuration.GetSection("Kestrel"));

// Configure Kestrel to use the specified certificate
builder.WebHost.ConfigureKestrel((context, options) =>
{
    var kestrelConfig = context.Configuration.GetSection("Kestrel:Endpoints:Https:Certificate");
    var certPath = kestrelConfig.GetValue<string>("Path");
    var keyPath = kestrelConfig.GetValue<string>("KeyPath");

    Console.WriteLine($"Certificate Path: {certPath}");
    Console.WriteLine($"Key Path: {keyPath}");

    if (!File.Exists(certPath))
    {
        throw new FileNotFoundException($"Certificate file not found: {certPath}");
    }

    if (!File.Exists(keyPath))
    {
        throw new FileNotFoundException($"Key file not found: {keyPath}");
    }

    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.UseHttps(certPath, keyPath);
    });
});

// Sertifika yükleme servisini ekle
builder.Services.AddHostedService<CertificateLoadingService>();

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

builder.Services.AddDbContext<KTUN_DbContext>(options =>
    options.UseSqlServer(connectionString ?? throw new InvalidOperationException("Connection string 'KTUN_DbContext' not found in configuration.")));

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
