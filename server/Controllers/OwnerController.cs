using FrenzyNet.Api.Contracts;
using FrenzyNet.Api.Data;
using FrenzyNet.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrenzyNet.Api.Controllers;

[ApiController]
[Authorize(Roles = "owner")]
[Route("api/owner")]
public class OwnerController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly AuditLogger _audit;

    public OwnerController(AppDbContext db, AuditLogger audit)
    {
        _db = db;
        _audit = audit;
    }

    [HttpPatch("users/{userId:guid}/role")]
    public async Task<IActionResult> UpdateUserRole(Guid userId, UpdateUserRoleRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            return NotFound();
        }

        user.Role = request.Role.Trim().ToLowerInvariant();
        await _db.SaveChangesAsync();
        await _audit.LogAsync(GetActorId(), "owner_role_update", $"User {userId} role changed to {user.Role}.");

        return NoContent();
    }

    [HttpPatch("subscriptions/{subscriptionId:guid}")]
    public async Task<IActionResult> UpdateSubscription(Guid subscriptionId, UpdateSubscriptionRequest request)
    {
        var subscription = await _db.Subscriptions.FirstOrDefaultAsync(s => s.Id == subscriptionId);
        if (subscription is null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            subscription.Status = request.Status.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.PlanName))
        {
            subscription.PlanName = request.PlanName.Trim();
        }

        if (request.MaxDevices.HasValue)
        {
            subscription.MaxDevices = request.MaxDevices.Value;
        }

        if (request.PricePerDevice.HasValue)
        {
            subscription.PricePerDevice = request.PricePerDevice.Value;
        }

        await _db.SaveChangesAsync();
        await _audit.LogAsync(GetActorId(), "owner_subscription_update", $"Subscription {subscriptionId} updated.");

        return NoContent();
    }

    private Guid? GetActorId()
    {
        var claim = User.FindFirst("sub");
        return claim is null ? null : Guid.Parse(claim.Value);
    }
}
