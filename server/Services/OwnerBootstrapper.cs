using FrenzyNet.Api.Data;
using FrenzyNet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FrenzyNet.Api.Services;

public class OwnerBootstrapper : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OwnerBootstrapper> _logger;

    public OwnerBootstrapper(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<OwnerBootstrapper> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var bootstrap = _configuration["OWNER_BOOTSTRAP"];
        if (!string.Equals(bootstrap, "true", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Owner bootstrap skipped: OWNER_BOOTSTRAP not enabled.");
            return;
        }

        var email = _configuration["OWNER_EMAIL"];
        var password = _configuration["OWNER_PASSWORD"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Owner bootstrap skipped: OWNER_EMAIL or OWNER_PASSWORD missing.");
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<PasswordHasher>();
        var subscriptions = scope.ServiceProvider.GetRequiredService<SubscriptionService>();

        var existingOwner = await db.Users.FirstOrDefaultAsync(u => u.Role == "owner", cancellationToken);
        if (existingOwner is not null)
        {
            _logger.LogInformation("Owner bootstrap skipped: owner already exists.");
            return;
        }

        var user = new User
        {
            Email = email.Trim(),
            Username = "owner",
            PasswordHash = hasher.Hash(password),
            Role = "owner",
            AcceptedTerms = true
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        var subscription = await subscriptions.CreateDefaultAsync(user, cancellationToken);
        user.SubscriptionId = subscription.Id;
        await db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Owner bootstrap complete. Disable OWNER_BOOTSTRAP after first run.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
