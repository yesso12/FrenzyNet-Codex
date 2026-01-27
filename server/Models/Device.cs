namespace FrenzyNet.Api.Models;

public class Device
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string PublicKey { get; set; } = null!;
    public string IpAddress { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
