using FrenzyNet.Api.Data;
using FrenzyNet.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        $"Host=db;Database=frenzynet;Username=frenzynet;Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")}"
    );
});

// Services
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<AuditLogger>();
builder.Services.AddScoped<WireGuardService>();

builder.Services.AddControllers();

// JWT
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET")!;
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
