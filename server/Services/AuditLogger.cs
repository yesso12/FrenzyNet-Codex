using FrenzyNet.Api.Data;
using FrenzyNet.Api.Models;

namespace FrenzyNet.Api.Services;

public class AuditLogger
{
    private readonly AppDbContext _db;

    public AuditLogger(AppDbContext db)
    {
        _db = db;
    }

    public async Task LogAsync(Guid? userId, string action, string detail)
    {
        _db.AuditLogs.Add(new AuditLog
        {
            UserId = userId,
            Action = action,
            Detail = detail,
            CreatedAt = DateTimeOffset.UtcNow
        });

        await _db.SaveChangesAsync();
    }
}
