namespace FrenzyNet.Api.Contracts;

public record RegisterRequest(string Email, string Username, string Password);

public record LoginRequest(string Identifier, string Password);

public record AuthResponse(string Token, string Username, string Role);

public record MeResponse(Guid Id, string Email, string Username, string Role, int DeviceCount, int DeviceLimit);
