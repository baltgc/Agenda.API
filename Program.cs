using System.Text;
using Agenda.API.API;
using Agenda.API.Application;
using Agenda.API.Infrastructure;
using Agenda.API.Middleware;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Agregar DbContext
builder.Services.AddDbContext<AgendaDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder
    .Services.AddHttpClient("externalApi")
    .AddTransientHttpErrorPolicy(policyBuilder =>
        policyBuilder.WaitAndRetryAsync(
            3,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
        )
    );

builder.Services.AddValidatorsFromAssemblyContaining<ReminderRequestValidator>();

builder.Services.AddSingleton<ReminderPublisher>();

// JWT y servicios
builder.Services.AddScoped<ReminderService>();

// Autenticación JWT
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            ),
        };
    });

builder.Services.AddAuthorization();

// Swagger
builder.Services.AddSwaggerGen();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Habilitar Swagger
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapReminderEndpoints();

app.Run();
