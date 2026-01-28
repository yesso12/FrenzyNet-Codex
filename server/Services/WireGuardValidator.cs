using System.Diagnostics;

namespace FrenzyNet.Api.Services;

public class WireGuardValidator : IHostedService
{
    private readonly ILogger<WireGuardValidator> _logger;

    public WireGuardValidator(ILogger<WireGuardValidator> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        ValidateCommand("wg");
        ValidateCommand("wg-quick");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private void ValidateCommand(string command)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = command,
                ArgumentList = { "--version" },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };

        try
        {
            process.Start();
            process.WaitForExit(5000);
            if (process.ExitCode != 0)
            {
                _logger.LogError("WireGuard dependency {Command} returned non-zero exit code.", command);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WireGuard dependency {Command} is missing.", command);
        }
    }
}
