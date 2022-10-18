using System.Diagnostics;

using DevServer.Models;

namespace DevServer.Services;

public class GameLauncher : IGameLauncher
{
    private readonly IProcess _process;

    public GameLauncher(IProcess process)
    {
        _process = process;
    }

    public Process Start(string executablePath, string serverAddress, WineStartInfo? wine = null)
    {
        var process = wine is not null
            ? StartWine(executablePath, serverAddress, wine)
            : StartNative(executablePath, serverAddress);

        if (process is null)
        {
            throw new InvalidOperationException("Process was not started");
        }

        return process;
    }

    internal Process? StartNative(string executablePath, string serverAddress)
    {
        var args = $"-devserver {serverAddress}";
        var process = _process.Start(executablePath, args);
        return process;
    }

    internal Process? StartWine(string executablePath, string serverAddress, WineStartInfo wine)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = wine.Path,
            Arguments = $"{executablePath} -devserver {serverAddress}",
            Environment =
            {
                { "WINEPREFIX", wine.Prefix },
                { "WINEARCH", Enum.GetName(typeof(WineArch), wine.Arch)?.ToLower() }
            }
        };

        if (wine.Environment != null)
        {
            foreach (var envVar in wine.Environment)
            {
                processStartInfo.EnvironmentVariables[envVar.Key] = envVar.Value;
            }
        }

        var process = _process.Start(processStartInfo);
        return process;
    }
}
