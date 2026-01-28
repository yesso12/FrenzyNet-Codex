using System.ComponentModel.DataAnnotations;

namespace FrenzyNet.Api.Models;

public class Device
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; set; }

    public User? User { get; set; }

    [Required]
    public Guid SubscriptionId { get; set; }

    public Subscription? Subscription { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string PublicKey { get; set; } = string.Empty;

    public string? IpAddress { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? RevokedAt { get; set; }
}
