namespace FrenzyNet.Api.Services;

public class BillingOptions
{
    public string DefaultPlanName { get; set; } = "per_device";

    public decimal PricePerDevice { get; set; } = 4.00m;

    public int DefaultDeviceLimit { get; set; } = 3;

    public List<BillingTier> Tiers { get; set; } = new();
}

public class BillingTier
{
    public string Name { get; set; } = string.Empty;

    public int DeviceLimit { get; set; }
}
