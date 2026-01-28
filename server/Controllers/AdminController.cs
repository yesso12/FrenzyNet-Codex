using FrenzyNet.Api.Data;
using FrenzyNet.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrenzyNet.Api.Controllers;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _db.Users
            .OrderBy(u => u.CreatedAt)
            .ToListAsync();
        return Ok(users.Select(u => new
        {
            u.Id,
            u.Email,
            u.Username,
            u.Role,
            u.CreatedAt
        }));
    }

    [HttpGet("audit")]
    public async Task<ActionResult<IEnumerable<AuditLog>>> GetAuditLogs()
    {
        var logs = await _db.AuditLogs
            .OrderByDescending(l => l.CreatedAt)
            .Take(200)
            .ToListAsync();
        return Ok(logs);
    }
}
