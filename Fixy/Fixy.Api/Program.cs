using Fixy.Api.Extensions;
using Fixy.Application;
using Fixy.Infrastructure;
using Fixy.Infrastructure.Persistence;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersConfiguration();

builder.Services.AddOpenApi();

// 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Dependency Injection
builder.Services.AddInfrastructureDependencies().AddApplicationDependencies().AddServiceRegisteration(builder.Configuration);

// Localization
builder.Services.AddLocalization(opt =>
{
    opt.ResourcesPath = "";
});



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


// CORS
builder.Services.AddCorsPolicy();

// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

await app.UseApiPipeline();

app.Run();