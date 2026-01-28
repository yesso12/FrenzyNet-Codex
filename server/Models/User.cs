using System.ComponentModel.DataAnnotations;

namespace FrenzyNet.Api.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "user";

    public Guid? SubscriptionId { get; set; }

    public Subscription? Subscription { get; set; }

    public bool AcceptedTerms { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public List<Device> Devices { get; set; } = new();
}
