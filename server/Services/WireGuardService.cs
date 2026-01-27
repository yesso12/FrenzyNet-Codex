using System.Diagnostics;

namespace FrenzyNet.Api.Services;

public class WireGuardService
{
    private const string ScriptPath = "/usr/local/bin/wg-manage.sh";

    public void AddPeer(string publicKey, string ip)
    {
        Run("add", publicKey, ip);
    }

    public void RemovePeer(string publicKey, string ip)
    {
        Run("remove", publicKey, ip);
    }

    private void Run(string cmd, string pubKey, string ip)
    {
        var psi = new ProcessStartInfo
        {
            FileName = ScriptPath,
            ArgumentList = { cmd, pubKey, ip },
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using var p = Process.Start(psi);
        p!.WaitForExit();

        if (p.ExitCode != 0)
            throw new Exception(p.StandardError.ReadToEnd());
    }
}
