using FrenzyNet.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrenzyNet.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("users")]
    public IActionResult Users()
    {
        return Ok(_db.Users.ToList());
    }

    [HttpGet("audit")]
    public IActionResult Audit()
    {
        return Ok(_db.AuditLogs.OrderByDescending(a => a.CreatedAt).Take(100).ToList());
    }
}
