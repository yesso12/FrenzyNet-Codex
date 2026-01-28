using FrenzyNet.Api.Data;
using FrenzyNet.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FrenzyNet.Api.Services;

public class SubscriptionService
{
    private readonly AppDbContext _db;
    private readonly BillingOptions _billingOptions;

    public SubscriptionService(AppDbContext db, IOptions<BillingOptions> billingOptions)
    {
        _db = db;
        _billingOptions = billingOptions.Value;
    }

    public async Task<Subscription> CreateDefaultAsync(User user, CancellationToken cancellationToken = default)
    {
        var subscription = new Subscription
        {
            UserId = user.Id,
            PlanName = _billingOptions.DefaultPlanName,
            PricePerDevice = _billingOptions.PricePerDevice,
            MaxDevices = _billingOptions.DefaultDeviceLimit,
            Status = "active"
        };

        _db.Subscriptions.Add(subscription);
        await _db.SaveChangesAsync(cancellationToken);
        return subscription;
    }

    public async Task<Subscription?> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _db.Subscriptions.FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
    }
}
