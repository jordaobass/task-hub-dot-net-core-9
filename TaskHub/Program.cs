using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskHub.Data;
using TaskHub.Endpoints;
using TaskHub.Utils;
using TaskHub.Models; // adiciona para evitar erro de namespace ao referenciar Models

var builder = WebApplication.CreateBuilder(args);

// Configurações (para demo usamos chave fixa; em produção use Secret Manager/KeyVault)
builder.Configuration["Jwt:Issuer"] ??= "taskhub";
builder.Configuration["Jwt:Audience"] ??= "taskhub_api";
builder.Configuration["Jwt:Key"] ??= "dev_super_secret_key_please_override";

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseInMemoryDatabase("taskhub-db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization(options => { options.AddPolicy("AdminOnly", p => p.RequireRole("admin")); });

builder.Services.AddSingleton<JwtTokenService>();

builder.Services.AddProblemDetails();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
}


DbSeeder.Seed(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

// Versionamento via rotas
var api = app.MapGroup("/api/v1");

// Endpoints
api.MapAuthEndpoints();
api.MapBoardsEndpoints();
api.MapUsersEndpoints();

app.Run();