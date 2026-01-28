using System.Diagnostics;
using System.Text.Json;

namespace FrenzyNet.Api.Services;

public class WireGuardService
{
    private readonly ILogger<WireGuardService> _logger;
    private readonly IConfiguration _configuration;

    public WireGuardService(ILogger<WireGuardService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public Task<(string Config, string PublicKey)> ProvisionAsync(Guid userId, string deviceName, string ipAddress, Guid deviceId)
    {
        var scriptPath = GetScriptPath();
        return RunScriptAsync(scriptPath, "add", userId.ToString(), deviceName, ipAddress, deviceId.ToString());
    }

    public async Task<(string Config, string PublicKey)> RotateAsync(Guid userId, string deviceName, string ipAddress, string oldPublicKey, Guid deviceId)
    {
        var scriptPath = GetScriptPath();
        await RunScriptWithoutResponseAsync(scriptPath, "remove", userId.ToString(), oldPublicKey, ipAddress, deviceId.ToString());
        return await RunScriptAsync(scriptPath, "add", userId.ToString(), deviceName, ipAddress, deviceId.ToString());
    }

    public Task RemoveAsync(Guid userId, string publicKey, string ipAddress, Guid deviceId)
    {
        var scriptPath = GetScriptPath();
        return RunScriptWithoutResponseAsync(scriptPath, "remove", userId.ToString(), publicKey, ipAddress, deviceId.ToString());
    }

    private string GetScriptPath() => _configuration["WG_SCRIPT_PATH"] ?? "/usr/local/bin/wg-manage.sh";

    private async Task<(string Config, string PublicKey)> RunScriptAsync(string scriptPath, string command, string arg1, string arg2, string arg3, string arg4)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = scriptPath,
                ArgumentList = { command, arg1, arg2, arg3, arg4 },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };

        process.Start();

        var stdout = await process.StandardOutput.ReadToEndAsync();
        var stderr = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            _logger.LogError("WireGuard script failed: {Error}", stderr);
            throw new InvalidOperationException("WireGuard provisioning failed.");
        }

        if (string.IsNullOrWhiteSpace(stdout))
        {
            return (string.Empty, string.Empty);
        }

        var parsed = JsonSerializer.Deserialize<WireGuardResponse>(stdout.Trim());
        if (parsed is null)
        {
            return (string.Empty, string.Empty);
        }

        return (parsed.Config, parsed.PublicKey);
    }

    private async Task RunScriptWithoutResponseAsync(string scriptPath, string command, string arg1, string arg2, string arg3, string arg4)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = scriptPath,
                ArgumentList = { command, arg1, arg2, arg3, arg4 },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };

        process.Start();

        var stderr = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            _logger.LogError("WireGuard script failed: {Error}", stderr);
            throw new InvalidOperationException("WireGuard provisioning failed.");
        }
    }

    private record WireGuardResponse(string Config, string PublicKey);
}
