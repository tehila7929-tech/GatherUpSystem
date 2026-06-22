using GatherUp.API.Middleware;
using GatherUp.API.Services;
using GatherUp.BL.Services;
using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;
using GatherUp.Infrastructure;
using GatherUp.Infrastructure.Data;
using GatherUp.Infrastructure.XML;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

string xmlFolder = Path.Combine(builder.Environment.ContentRootPath, "DataXML");

// ── Repositories ────────────────────────────────────────────────────────────
builder.Services.AddSingleton<IRepository<Event>>(_ => new XmlRepository<Event>(xmlFolder));
builder.Services.AddSingleton<IRepository<Participant>>(_ => new XmlRepository<Participant>(xmlFolder));
builder.Services.AddSingleton<IRepository<EventManager>>(_ => new XmlRepository<EventManager>(xmlFolder));
builder.Services.AddSingleton<IRepository<EventHost>>(_ => new XmlRepository<EventHost>(xmlFolder));
builder.Services.AddSingleton<IRepository<VendorAllocation>>(_ => new XmlRepository<VendorAllocation>(xmlFolder));
builder.Services.AddSingleton<IRepository<Poll>>(_ => new XmlRepository<Poll>(xmlFolder));

// ReceiptRepository registered both as itself (for FinancialController) and as IRepository<VendorAllocation>
// is already registered above; ReceiptRepository handles the physical receipt files separately.
builder.Services.AddSingleton<ReceiptRepository>(_ => new ReceiptRepository(xmlFolder));

// ── Infrastructure services ──────────────────────────────────────────────────
builder.Services.AddSingleton<IEmailService>(_ => new EmailService(xmlFolder));
builder.Services.AddSingleton<IEventNotifier, EventNotifier>();

// ── BL services ──────────────────────────────────────────────────────────────
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<ParticipantService>();
builder.Services.AddScoped<FinanceService>();
builder.Services.AddScoped<PollService>();

// NotificationSubscription must be Singleton so it registers to the Singleton IEventNotifier
// exactly once at startup, and stays alive for the entire application lifetime.
builder.Services.AddSingleton<NotificationSubscription>();

// ── JWT ───────────────────────────────────────────────────────────────────────
builder.Services.AddSingleton<JwtService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "GatherUp API",
        Version = "v1",
        Description = "Backend for managing group events, polls, participants and finances."
    });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token. Example: Bearer eyJhbG..."
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ── Resolve NotificationSubscription once to activate its event subscriptions ─
// This is the correct pattern: force the Singleton to be created at startup
// so its constructor runs and registers all domain event handlers.
app.Services.GetRequiredService<NotificationSubscription>();

// ── Seed data on first run (if XML files are empty) ───────────────────────────
SeedIfEmpty(app.Services, xmlFolder);

// ── Pipeline ─────────────────────────────────────────────────────────────────
// 1. Static files first - short-circuit before any middleware touches the request
app.UseDefaultFiles(); // serves index.html at /
app.UseStaticFiles();

// 2. Global error handler
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GatherUp API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// 3. Auth must come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// ─────────────────────────────────────────────────────────────────────────────
static void SeedIfEmpty(IServiceProvider services, string xmlFolder)
{
    var eventRepo = services.GetRequiredService<IRepository<Event>>();
    if (eventRepo.GetAll().Any()) return; // Already seeded

    var participantRepo = services.GetRequiredService<IRepository<Participant>>();
    var managerRepo = services.GetRequiredService<IRepository<EventManager>>();
    var hostRepo = services.GetRequiredService<IRepository<EventHost>>();
    var vendorRepo = services.GetRequiredService<IRepository<VendorAllocation>>();
    var pollRepo = services.GetRequiredService<IRepository<Poll>>();

    Initialize.DataInit(eventRepo, participantRepo, managerRepo, hostRepo, vendorRepo, pollRepo);
}
