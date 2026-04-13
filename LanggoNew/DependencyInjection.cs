using System.Text;
using Hangfire;
using Hangfire.Redis.StackExchange;
using LanggoNew.Features.Dictionaries;
using LanggoNew.Shared.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace LanggoNew;

public static class DependencyInjection
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.CustomSchemaIds(t => t.FullName?.Replace('+', '.'));
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Введите 'Bearer' [пробел] и ваш JWT-токен.\n\nПример: \"Bearer eyJhbGciOi...\""
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                }
            });
        });
        
        return services;
    }

    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var conf = configuration.GetConnectionString("Redis");
            return ConnectionMultiplexer.Connect(conf);
        });
        
        return services;
    }

    public static IServiceCollection AddCorsConfig(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.WithOrigins(
                        "http://localhost:5500",
                        "http://127.0.0.1:5500"
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
        
        return services;
    }
    
    public static IServiceCollection AddAutoMapperConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.LicenseKey = configuration["LuckyPenny:LicenseKey"];
        }, typeof(DictionaryProfile));
        
        return services;
    }
    
    public static IServiceCollection AddMediatRConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(Program).Assembly);
            config.LicenseKey = configuration["LuckyPenny:LicenseKey"];;
        });
        
        return services;
    }

    public static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]))
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/game"))
                            context.Token = accessToken;

                        return Task.CompletedTask;
                    }
                };
            });
        
        return services;
    }
    
    public static IServiceCollection AddGameTimingOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GameTimingOptions>(configuration.GetSection("GameTiming"));
        return services;
    }
    
    public static IServiceCollection AddHangfireAndHangfireServer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config => config
            .UseRedisStorage(configuration.GetConnectionString("Redis"), new RedisStorageOptions
            {
                Prefix = "hangfire:", 
            })
        );
        
        services.AddHangfireServer(options =>
        {
            options.SchedulePollingInterval = TimeSpan.FromSeconds(1);
        });
        
        return services;
    }

    public static IServiceCollection AddEmailSmtp(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFluentEmail(configuration["EmailSettings:EmailSender"], configuration["EmailSettings:EmailSenderName"])
            .AddSmtpSender(configuration["EmailSettings:Host"], configuration.GetValue<int>("EmailSettings:Port"));
        
        return services;
    }
}