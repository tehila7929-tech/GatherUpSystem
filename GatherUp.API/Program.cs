using GatherUp.API.Middleware;
using GatherUp.API.Services;
using GatherUp.BL.Services;
using GatherUp.Core.DO;
using GatherUp.Core.Interfaces;
using GatherUp.Infrastructure;
using GatherUp.Infrastructure.XML;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

string xmlFolder = Path.Combine(builder.Environment.ContentRootPath, "DataXML");

// Repositories
builder.Services.AddSingleton<IRepository<Event>>(_ => new XmlRepository<Event>(xmlFolder));
builder.Services.AddSingleton<IRepository<Participant>>(_ => new XmlRepository<Participant>(xmlFolder));
builder.Services.AddSingleton<IRepository<EventManager>>(_ => new XmlRepository<EventManager>(xmlFolder));
builder.Services.AddSingleton<IRepository<EventHost>>(_ => new XmlRepository<EventHost>(xmlFolder));
builder.Services.AddSingleton<IRepository<VendorAllocation>>(_ => new XmlRepository<VendorAllocation>(xmlFolder));
builder.Services.AddSingleton<IRepository<Poll>>(_ => new XmlRepository<Poll>(xmlFolder));

// Infrastructure services
builder.Services.AddSingleton<IEmailService>(_ => new EmailService(xmlFolder));
builder.Services.AddSingleton<IEventNotifier, EventNotifier>();

// BL services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<ParticipantService>();
builder.Services.AddScoped<FinanceService>();
builder.Services.AddScoped<PollService>();
builder.Services.AddScoped<NotificationSubscription>();

// JWT
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
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header
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

// Pipeline
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
