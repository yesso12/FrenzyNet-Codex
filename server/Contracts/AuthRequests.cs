namespace FrenzyNet.Api.Contracts;

public record RegisterRequest(string Username, string Email, string Password);
public record LoginRequest(string UsernameOrEmail, string Password);
