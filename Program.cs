using KTUN_Final_Year_Project;
using KTUN_Final_Year_Project.Entities;
using KTUN_Final_Year_Project.Options;  // Options sınıflarının namespace'ini ekle
using KTUN_Final_Year_Project.Services; // CertificateLoadingService'in namespace'ini ekle
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configuration object for easy access
var configuration = builder.Configuration;

// Remove or comment out Kestrel configuration related to specific certificates
/*
// Read Kestrel configuration from appsettings.json
builder.Services.Configure<KestrelServerOptions>(builder.Configuration.GetSection("Kestrel"));

// Configure Kestrel to use the specified certificate
builder.WebHost.ConfigureKestrel((context, options) =>
{
    var kestrelConfig = context.Configuration.GetSection("Kestrel:Endpoints:Https:Certificate"); // Check if this path is still valid
    var certPath = kestrelConfig.GetValue<string>("Path");
    var keyPath = kestrelConfig.GetValue<string>("KeyPath");

    Console.WriteLine($"Certificate Path: {certPath}");
    Console.WriteLine($"Key Path: {keyPath}");

    if (!string.IsNullOrEmpty(certPath) && !File.Exists(certPath)) // Add null/empty check
    {
        throw new FileNotFoundException($"Certificate file not found: {certPath}");
    }

    if (!string.IsNullOrEmpty(keyPath) && !File.Exists(keyPath)) // Add null/empty check
    {
        throw new FileNotFoundException($"Key file not found: {keyPath}");
    }
    
    // Default HTTPS port is often configured elsewhere (e.g., launchSettings.json)
    // or Kestrel uses defaults if no specific endpoint is configured here.
    // If you still need specific ports, configure them without specific certificate files.
    // Example:
    // options.ListenAnyIP(5001, listenOptions => listenOptions.UseHttps()); 
    // options.ListenAnyIP(5000); // For HTTP
    
    // Original code trying to load PEM files:
    // options.ListenAnyIP(5001, listenOptions =>
    // {
    //     listenOptions.UseHttps(certPath, keyPath);
    // });
});
*/

// Sertifika yükleme servisini ekle (Bu servis PEM dosyalarıyla ilgiliyse kaldırılabilir)
// Eğer CertificateLoadingService PEM dosyalarını yüklüyorsa, bu satırı kaldırın:
// builder.Services.AddHostedService<CertificateLoadingService>();

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

    // Add JWT Authentication support to Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] { }
    }
    });

});

// Configure Entity Framework Core with SQL Server
var connectionString = builder.Configuration.GetConnectionString("KTUN_DbContext");

builder.Services.AddDbContext<KTUN_DbContext>(options =>
    options.UseSqlServer(connectionString ?? throw new InvalidOperationException("Connection string 'KTUN_DbContext' not found in configuration.")));

// ---- Add Identity ----
builder.Services.AddIdentity<Users, IdentityRole<int>>(options =>
{
    // Configure Identity options (e.g., password requirements) here if needed
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true; // Ensure emails are unique
})
.AddEntityFrameworkStores<KTUN_DbContext>()
.AddDefaultTokenProviders(); // Adds token providers for password reset, email confirmation etc.

// ---- Add Authentication and JWT Bearer ----
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true; // Save token in AuthenticationProperties
    options.RequireHttpsMetadata = false; // Set to true in production
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT Secret not found in configuration.")))
    };
});

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

// Add Authentication middleware *before* Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Run the application
app.Run();
