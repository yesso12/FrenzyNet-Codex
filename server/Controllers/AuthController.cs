using FrenzyNet.Api.Contracts;
using FrenzyNet.Api.Data;
using FrenzyNet.Api.Models;
using FrenzyNet.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrenzyNet.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher _hasher;
    private readonly JwtService _jwt;
    private readonly AuditLogger _audit;

    public AuthController(AppDbContext db, PasswordHasher hasher, JwtService jwt, AuditLogger audit)
    {
        _db = db;
        _hasher = hasher;
        _jwt = jwt;
        _audit = audit;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        if (await _db.Users.AnyAsync(u => u.Username == req.Username || u.Email == req.Email))
            return BadRequest("User already exists");

        var user = new User
        {
            Username = req.Username,
            Email = req.Email,
            PasswordHash = _hasher.Hash(req.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        await _audit.LogAsync(user.Id, "register");

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u =>
            u.Username == req.UsernameOrEmail || u.Email == req.UsernameOrEmail);

        if (user == null || !_hasher.Verify(req.Password, user.PasswordHash))
            return Unauthorized();

        await _audit.LogAsync(user.Id, "login");

        return Ok(new { token = _jwt.Generate(user) });
    }
}
