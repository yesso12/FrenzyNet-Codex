using System.ComponentModel.DataAnnotations;

namespace FrenzyNet.Api.Models;

public class Subscription
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserId { get; set; }

    public User? User { get; set; }

    [Required]
    public string PlanName { get; set; } = "per_device";

    [Required]
    public decimal PricePerDevice { get; set; } = 4.00m;

    public int MaxDevices { get; set; } = 3;

    public int DeviceCount { get; set; }

    [Required]
    public string Status { get; set; } = "active";

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
