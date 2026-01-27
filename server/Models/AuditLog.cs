namespace FrenzyNet.Api.Models;

public class AuditLog
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string Action { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
