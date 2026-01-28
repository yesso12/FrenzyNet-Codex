using System.Net;
using FrenzyNet.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace FrenzyNet.Api.Services;

public class IpAllocator
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _configuration;

    public IpAllocator(AppDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task<string> AllocateAsync(CancellationToken cancellationToken = default)
    {
        var cidr = _configuration["WG_ADDRESS_POOL"] ?? "10.0.0.0/24";
        var availableIps = GetHostAddresses(cidr).ToList();
        if (availableIps.Count == 0)
        {
            throw new InvalidOperationException("No available IPs in pool.");
        }

        var allocated = await _db.Devices
            .Where(d => d.RevokedAt == null && d.IpAddress != null)
            .Select(d => d.IpAddress!)
            .ToListAsync(cancellationToken);

        var allocatedSet = new HashSet<string>(allocated);
        var free = availableIps.FirstOrDefault(ip => !allocatedSet.Contains(ip));
        if (free is null)
        {
            throw new InvalidOperationException("IP pool exhausted.");
        }

        return free;
    }

    private IEnumerable<string> GetHostAddresses(string cidr)
    {
        var parts = cidr.Split('/');
        if (parts.Length != 2)
        {
            throw new InvalidOperationException("Invalid WG_ADDRESS_POOL CIDR.");
        }

        var baseIp = IPAddress.Parse(parts[0]);
        var prefix = int.Parse(parts[1]);

        var baseBytes = baseIp.GetAddressBytes();
        if (baseBytes.Length != 4)
        {
            throw new InvalidOperationException("Only IPv4 pools are supported.");
        }

        if (prefix < 16 || prefix > 30)
        {
            throw new InvalidOperationException("WG_ADDRESS_POOL must be between /16 and /30.");
        }

        var hostBits = 32 - prefix;
        var totalHosts = (int)Math.Pow(2, hostBits);
        if (totalHosts > 1024)
        {
            throw new InvalidOperationException("WG_ADDRESS_POOL too large; limit to <= 1024 hosts.");
        }

        var baseInt = BitConverter.ToUInt32(baseBytes.Reverse().ToArray(), 0);
        for (var i = 2; i < totalHosts; i++)
        {
            var ipInt = baseInt + (uint)i;
            var bytes = BitConverter.GetBytes(ipInt).Reverse().ToArray();
            yield return new IPAddress(bytes).ToString();
        }
    }
}
