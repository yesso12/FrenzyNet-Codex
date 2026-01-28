using System.Text;
using FrenzyNet.Api.Data;
using FrenzyNet.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? builder.Configuration["DATABASE_URL"]
    ?? "Host=localhost;Port=5432;Database=frenzynet;Username=postgres;Password=postgres";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddMemoryCache();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/var/data-protection"));

builder.Services.Configure<BillingOptions>(builder.Configuration.GetSection("Billing"));

builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<AuditLogger>();
builder.Services.AddScoped<WireGuardService>();
builder.Services.AddScoped<IpAllocator>();

builder.Services.AddHostedService<AdminBootstrapper>();
builder.Services.AddHostedService<WireGuardValidator>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSecret = builder.Configuration["JWT_SECRET"] ?? "dev-secret-change";
        var issuer = builder.Configuration["JWT_ISSUER"] ?? "frenzynet";
        var audience = builder.Configuration["JWT_AUDIENCE"] ?? "frenzynet-web";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("auth", context =>
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        });
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins(builder.Configuration["WEB_ORIGIN"] ?? "http://localhost:5173");
    });
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
