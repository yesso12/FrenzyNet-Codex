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

    public int DeviceCount { get; set; }

    public int? DeviceLimit { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public List<Device> Devices { get; set; } = new();
}
