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

    public async Task LogAsync(int? userId, string action)
    {
        _db.AuditLogs.Add(new AuditLog
        {
            UserId = userId,
            Action = action
        });

        await _db.SaveChangesAsync();
    }
}
