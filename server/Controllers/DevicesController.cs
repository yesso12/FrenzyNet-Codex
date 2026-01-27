using FrenzyNet.Api.Contracts;
using FrenzyNet.Api.Data;
using FrenzyNet.Api.Models;
using FrenzyNet.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FrenzyNet.Api.Controllers;

[ApiController]
[Route("api/devices")]
[Authorize]
public class DevicesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly WireGuardService _wg;
    private readonly AuditLogger _audit;

    public DevicesController(AppDbContext db, WireGuardService wg, AuditLogger audit)
    {
        _db = db;
        _wg = wg;
        _audit = audit;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var devices = await _db.Devices.Where(d => d.UserId == userId).ToListAsync();
        return Ok(devices);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDeviceRequest req)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var ip = $"10.0.0.{new Random().Next(2, 254)}";

        var device = new Device
        {
            UserId = userId,
            Name = req.Name,
            PublicKey = req.PublicKey,
            IpAddress = ip
        };

        _db.Devices.Add(device);
        await _db.SaveChangesAsync();

        _wg.AddPeer(req.PublicKey, ip);
        await _audit.LogAsync(userId, $"device:create:{req.Name}");

        return Ok(device);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var device = await _db.Devices.FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

        if (device == null)
            return NotFound();

        _wg.RemovePeer(device.PublicKey, device.IpAddress);
        _db.Devices.Remove(device);
        await _db.SaveChangesAsync();

        await _audit.LogAsync(userId, $"device:delete:{device.Name}");

        return Ok();
    }
}
