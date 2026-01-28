using System.Security.Claims;
using FrenzyNet.Api.Contracts;
using FrenzyNet.Api.Data;
using FrenzyNet.Api.Models;
using FrenzyNet.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using QRCoder;

namespace FrenzyNet.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/devices")]
public class DevicesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly WireGuardService _wireGuard;
    private readonly AuditLogger _audit;
    private readonly IpAllocator _ipAllocator;
    private readonly IMemoryCache _cache;
    private readonly BillingOptions _billingOptions;

    public DevicesController(
        AppDbContext db,
        WireGuardService wireGuard,
        AuditLogger audit,
        IpAllocator ipAllocator,
        IMemoryCache cache,
        IOptions<BillingOptions> billingOptions)
    {
        _db = db;
        _wireGuard = wireGuard;
        _audit = audit;
        _ipAllocator = ipAllocator;
        _cache = cache;
        _billingOptions = billingOptions.Value;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeviceResponse>>> GetDevices()
    {
        var userId = GetUserId();
        var devices = await _db.Devices
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => new DeviceResponse(d.Id, d.Name, d.IpAddress, d.PublicKey, d.CreatedAt, d.RevokedAt))
            .ToListAsync();

        return Ok(devices);
    }

    [HttpPost]
    public async Task<ActionResult<ProvisionResponse>> CreateDevice(CreateDeviceRequest request)
    {
        var userId = GetUserId();
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Device name is required.");
        }

        if (request.Name.Contains('\n') || request.Name.Contains('\r'))
        {
            return BadRequest("Device name contains invalid characters.");
        }

        if (await _db.Devices.AnyAsync(d => d.UserId == userId && d.Name == request.Name))
        {
            return Conflict("Device name already exists.");
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            return Unauthorized();
        }

        if (!string.Equals(user.Role, "admin", StringComparison.OrdinalIgnoreCase))
        {
            var limit = user.DeviceLimit ?? _billingOptions.DefaultDeviceLimit;
            if (user.DeviceCount >= limit)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Device limit reached.");
            }
        }

        var ipAddress = await _ipAllocator.AllocateAsync();
        var (config, publicKey) = await _wireGuard.ProvisionAsync(userId, request.Name.Trim(), ipAddress);

        var device = new Device
        {
            UserId = userId,
            Name = request.Name.Trim(),
            PublicKey = publicKey,
            IpAddress = ipAddress
        };

        _db.Devices.Add(device);
        user.DeviceCount += 1;
        await _db.SaveChangesAsync();

        CacheConfig(device.Id, config);
        await _audit.LogAsync(userId, "device_created", $"Device {device.Name} created.");

        return Ok(new ProvisionResponse(device.Id, config, publicKey));
    }

    [HttpGet("{deviceId:guid}/config")]
    public async Task<IActionResult> DownloadConfig(Guid deviceId)
    {
        var device = await GetActiveDevice(deviceId);
        if (device is null)
        {
            return NotFound();
        }

        var config = await GetOrRotateConfig(device);
        var fileName = $"frenzynet-{device.Name}.conf";
        return File(System.Text.Encoding.UTF8.GetBytes(config), "text/plain", fileName);
    }

    [HttpGet("{deviceId:guid}/qrcode")]
    public async Task<IActionResult> GetQrCode(Guid deviceId)
    {
        var device = await GetActiveDevice(deviceId);
        if (device is null)
        {
            return NotFound();
        }

        var config = await GetOrRotateConfig(device);
        using var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(config, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrData);
        var qrBytes = qrCode.GetGraphic(20);
        return File(qrBytes, "image/png");
    }

    [HttpDelete("{deviceId:guid}")]
    public async Task<IActionResult> RevokeDevice(Guid deviceId)
    {
        var userId = GetUserId();
        var device = await _db.Devices.FirstOrDefaultAsync(d => d.Id == deviceId && d.UserId == userId);
        if (device is null)
        {
            return NotFound();
        }

        if (device.RevokedAt is not null)
        {
            return BadRequest("Device already revoked.");
        }

        if (device.IpAddress is null)
        {
            return BadRequest("Device is missing an IP address.");
        }

        await _wireGuard.RemoveAsync(userId, device.PublicKey, device.IpAddress);
        device.RevokedAt = DateTimeOffset.UtcNow;
        device.IpAddress = null;
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is not null && user.DeviceCount > 0)
        {
            user.DeviceCount -= 1;
        }
        await _db.SaveChangesAsync();
        _cache.Remove(CacheKey(device.Id));

        await _audit.LogAsync(userId, "device_revoked", $"Device {device.Name} revoked.");

        return NoContent();
    }

    private async Task<Device?> GetActiveDevice(Guid deviceId)
    {
        var userId = GetUserId();
        return await _db.Devices.FirstOrDefaultAsync(d => d.Id == deviceId && d.UserId == userId && d.RevokedAt == null);
    }

    private async Task<string> GetOrRotateConfig(Device device)
    {
        if (_cache.TryGetValue(CacheKey(device.Id), out string? cached) && cached is not null)
        {
            return cached;
        }

        if (device.IpAddress is null)
        {
            throw new InvalidOperationException("Device is missing an IP address.");
        }

        var (config, publicKey) = await _wireGuard.RotateAsync(device.UserId, device.Name, device.IpAddress, device.PublicKey);
        device.PublicKey = publicKey;
        await _db.SaveChangesAsync();

        CacheConfig(device.Id, config);
        await _audit.LogAsync(device.UserId, "device_config_issued", $"Device {device.Name} config reissued.");

        return config;
    }

    private void CacheConfig(Guid deviceId, string config)
    {
        _cache.Set(CacheKey(deviceId), config, TimeSpan.FromMinutes(15));
    }

    private static string CacheKey(Guid deviceId) => $"device-config:{deviceId}";

    private Guid GetUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("sub");
        return Guid.Parse(idClaim ?? throw new InvalidOperationException("Missing user claim"));
    }
}
