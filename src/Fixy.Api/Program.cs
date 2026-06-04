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
using Serilog.Events;
using System.Globalization;

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
    var arEG = new CultureInfo("ar-EG");
    arEG.NumberFormat.NumberDecimalSeparator = ".";
    arEG.NumberFormat.CurrencyDecimalSeparator = ".";

    List<CultureInfo> supportedCultures = new List<CultureInfo>
    {
            new CultureInfo("en-US"),
            arEG
    };

    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Serilog
//builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
//{
//    loggerConfiguration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services);
//});
builder.Host.UseSerilog((context, services, config) =>
{
    config
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.ApplicationInsights(
            context.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"],
            TelemetryConverter.Traces
        );
});
builder.Services.AddHttpLogging();
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