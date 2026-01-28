using FrenzyNet.Api.Data;
using FrenzyNet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FrenzyNet.Api.Services;

public class AdminBootstrapper : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdminBootstrapper> _logger;

    public AdminBootstrapper(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<AdminBootstrapper> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var email = _configuration["ADMIN_EMAIL"];
        var password = _configuration["ADMIN_PASSWORD"];
        var username = _configuration["ADMIN_USERNAME"] ?? "admin";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogInformation("Admin bootstrap skipped: ADMIN_EMAIL or ADMIN_PASSWORD not set.");
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<PasswordHasher>();
        var subscriptions = scope.ServiceProvider.GetRequiredService<SubscriptionService>();

        var existing = await db.Users.Include(u => u.Subscription).FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (existing is not null)
        {
            if (!string.Equals(existing.Role, "admin", StringComparison.OrdinalIgnoreCase))
            {
                existing.Role = "admin";
                await db.SaveChangesAsync(cancellationToken);
            }

            if (existing.Subscription is null)
            {
                var subscription = await subscriptions.CreateDefaultAsync(existing, cancellationToken);
                existing.SubscriptionId = subscription.Id;
                await db.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation("Admin bootstrap: existing admin found.");
            return;
        }

        var user = new User
        {
            Email = email.Trim(),
            Username = username.Trim(),
            PasswordHash = hasher.Hash(password),
            Role = "admin",
            AcceptedTerms = true
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        var defaultSubscription = await subscriptions.CreateDefaultAsync(user, cancellationToken);
        user.SubscriptionId = defaultSubscription.Id;
        await db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Admin bootstrap: admin user created.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
