namespace FrenzyNet.Api.Models;

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? UserId { get; set; }

    public string Action { get; set; } = string.Empty;

    public string Detail { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
