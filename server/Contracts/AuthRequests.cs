namespace FrenzyNet.Api.Contracts;

public record RegisterRequest(string Email, string Username, string Password, bool AcceptTerms);

public record LoginRequest(string Identifier, string Password);

public record AuthResponse(string Token, string Username, string Role);

public record SubscriptionSummary(
    Guid Id,
    string PlanName,
    decimal PricePerDevice,
    int MaxDevices,
    int DeviceCount,
    string Status);

public record MeResponse(
    Guid Id,
    string Email,
    string Username,
    string Role,
    bool AcceptedTerms,
    SubscriptionSummary Subscription);
