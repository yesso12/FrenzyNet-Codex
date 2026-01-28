namespace FrenzyNet.Api.Contracts;

public record CreateDeviceRequest(string Name);

public record DeviceResponse(Guid Id, string Name, string? IpAddress, string PublicKey, DateTimeOffset CreatedAt, DateTimeOffset? RevokedAt);

public record ProvisionResponse(Guid Id, string Config, string PublicKey);
