using FirebaseAdmin;
using Fixy.Domain.Entities.Identity;
using Fixy.Infrastructure.Configurations;
using Fixy.Infrastructure.Persistence;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Stripe;
using System.Text;

namespace Fixy.Infrastructure;

public static class ServiceRegisteration
{
    public static IServiceCollection AddServiceRegisteration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>().AddEntityFrameworkStores<FixyDbContext>().AddDefaultTokenProviders();
        
        var jwtSettings = new JWTSettings();
        configuration.GetSection(nameof(jwtSettings)).Bind(jwtSettings);
        services.AddSingleton(jwtSettings);

        var emailSettings = new EmailSettings();
        configuration.GetSection(nameof(emailSettings)).Bind(emailSettings);
        services.AddSingleton(emailSettings);

        var flaskApiSettings = new FlaskApiSettings();
        configuration.GetSection(nameof(flaskApiSettings)).Bind(flaskApiSettings);
        services.AddSingleton(flaskApiSettings);

        var paymobSettings = new PaymobSettings();
        configuration.GetSection(nameof(paymobSettings)).Bind(paymobSettings);
        services.AddSingleton(paymobSettings);

        var stripeSettings = new StripeSettings();
        configuration.GetSection(nameof(stripeSettings)).Bind(stripeSettings);
        services.AddSingleton(stripeSettings);
        
        StripeConfiguration.ApiKey = stripeSettings.Secretkey;

        // SignalR
        services.AddSignalR().AddAzureSignalR(configuration["Azure:SignalR:ConnectionString"]);

        // Firebase
        var path = Path.Combine(AppContext.BaseDirectory, configuration["Firebase:ServiceAccountPath"]!);

        using var stream = System.IO.File.OpenRead(path);

        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromStream(stream),
            ProjectId = configuration["Firebase:ProjectId"]
        });
        // Redis
        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(configuration["Azure:Redis:ConnectionString"]!));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.RequireHttpsMetadata = false;
            o.SaveToken = false;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                ClockSkew = TimeSpan.Zero,
            };
            // read token from cookie
            o.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["token"];
                    return Task.CompletedTask;
                }
            };
        });

        //Swagger Gn
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Fixy",
                Description = "",
                TermsOfService = new Uri("https://www.linkedin.com/in/mohamedsaad14/"),
                Contact = new OpenApiContact
                {
                    Name = "Mohamed Saad",
                    Email = "mohamedsaad2756@gmail.com",
                    Url = new Uri("https://www.linkedin.com/in/mohamedsaad14/")
                },
                License = new OpenApiLicense
                {
                    Name = "My license",
                    Url = new Uri("https://www.linkedin.com/in/mohamedsaad14/")
                }
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your api key"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                }
            );
        });

        return services;
    }
}
