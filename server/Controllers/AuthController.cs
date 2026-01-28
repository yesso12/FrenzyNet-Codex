using System.Security.Claims;
using FrenzyNet.Api.Contracts;
using FrenzyNet.Api.Data;
using FrenzyNet.Api.Models;
using FrenzyNet.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FrenzyNet.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher _hasher;
    private readonly JwtService _jwt;
    private readonly AuditLogger _audit;
    private readonly BillingOptions _billingOptions;

    public AuthController(AppDbContext db, PasswordHasher hasher, JwtService jwt, AuditLogger audit, IOptions<BillingOptions> billingOptions)
    {
        _db = db;
        _hasher = hasher;
        _jwt = jwt;
        _audit = audit;
        _billingOptions = billingOptions.Value;
    }

    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email || u.Username == request.Username))
        {
            return Conflict("Email or username already exists.");
        }

        var user = new User
        {
            Email = request.Email.Trim(),
            Username = request.Username.Trim(),
            PasswordHash = _hasher.Hash(request.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        await _audit.LogAsync(user.Id, "register", $"User {user.Username} registered.");

        var token = _jwt.CreateToken(user);
        return Ok(new AuthResponse(token, user.Username, user.Role));
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var identifier = request.Identifier.Trim();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == identifier || u.Username == identifier);
        if (user is null || !_hasher.Verify(request.Password, user.PasswordHash))
        {
            await _audit.LogAsync(null, "login_failed", $"Login failed for {identifier}.");
            return Unauthorized("Invalid credentials.");
        }

        await _audit.LogAsync(user.Id, "login", $"User {user.Username} logged in.");

        var token = _jwt.CreateToken(user);
        return Ok(new AuthResponse(token, user.Username, user.Role));
    }

    [HttpGet("/api/me")]
    [Authorize]
    public async Task<ActionResult<MeResponse>> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Unauthorized();
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));
        if (user is null)
        {
            return NotFound();
        }

        var limit = user.DeviceLimit ?? _billingOptions.DefaultDeviceLimit;
        return Ok(new MeResponse(user.Id, user.Email, user.Username, user.Role, user.DeviceCount, limit));
    }
}
