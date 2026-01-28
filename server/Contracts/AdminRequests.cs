namespace FrenzyNet.Api.Contracts;

public record UpdateSubscriptionRequest(string? Status, int? MaxDevices, decimal? PricePerDevice, string? PlanName);

public record UpdateUserRoleRequest(string Role);
