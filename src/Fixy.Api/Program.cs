using DotNetEnv;
using Fixy.Api.Extensions;
using Fixy.Api.Filters;
using Fixy.Application;
using Fixy.Infrastructure;
using Fixy.Infrastructure.Persistence;
using Hangfire;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;
using System.Globalization;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllersConfiguration().AddHealthCheck().AddCustomRateLimiter();

builder.Services.AddOpenApi();

// 
builder.Services.AddDbContext<FixyDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["Azure:DatabaseConnection:ConnectionString"]).ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)); ;
});

// Dependency Injection
builder.Services.AddInfrastructureDependencies().AddApplicationDependencies().AddServiceRegisteration(builder.Configuration);

// Localization
builder.Services.AddLocalization(opt => opt.ResourcesPath = "");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    List<CultureInfo> supportedCultures = new List<CultureInfo>
    {
            new CultureInfo("en-US"),
            new CultureInfo("de-DE"),
            new CultureInfo("fr-FR"),
            new CultureInfo("ar-EG")
    };

    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Serilog
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Services.AddSerilog();
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponsePropertiesAndHeaders;
});

// CORS
builder.Services.AddCorsPolicy();

// Filters
builder.Services.AddScoped<TechnicianStatusFilter>();
builder.Services.AddScoped<CustomerStatusFilter>();
builder.Services.AddScoped<TechnicianFeedbackFilter>();
builder.Services.AddScoped<CustomerFeedbackFilter>();

// Hangfire Client
builder.Services.AddHangfire(config =>
{
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration["Azure:DatabaseConnection:ConnectionString"]);
});

// Hangfire Server
builder.Services.AddHangfireServer();

builder.Services.AddHttpClient();

builder.Services.AddVersioning();

var app = builder.Build();

await app.UseApiPipeline();

app.Run();