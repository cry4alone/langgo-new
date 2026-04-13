using FluentValidation;
using Microsoft.EntityFrameworkCore;
using LanggoNew;
using LanggoNew.Endpoints;
using LanggoNew.Features.Games;
using LanggoNew.Middleware;
using LanggoNew.Shared.Infrastructure;
using LanggoNew.Shared.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSwagger();
builder.Services.AddCorsConfig();
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddAutoMapperConfig(builder.Configuration);
builder.Services.AddMediatRConfig(builder.Configuration);
builder.Services.AddHangfireAndHangfireServer(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddGameTimingOptions(builder.Configuration);
builder.Services.AddEmailSmtp(builder.Configuration);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddAuthorization();
builder.Services.AddJwt(builder.Configuration);

builder.Services.AddSingleton<IPasswordHashingService, PasswordHashingService>();
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IGameTimerService, GameTimerService>();
builder.Services.AddSingleton<IEmailVerificationLinkFactory, EmailVerificationLinkFactory>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IRedisCache, RedisCache>();
builder.Services.AddScoped<IWordService, WordService>();

builder.Services.AddEndpoints();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.UseCors("AllowAll");
app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<GameHub>("/game");
app.MapEndpoints();

app.Run();
