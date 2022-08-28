using System.Diagnostics;
using System.Runtime.InteropServices;

using DevServer.Models;

namespace DevServer.Services;

public class GameLauncher : IGameLauncher
{
    private readonly IProcess _process;
    private readonly IConfigurationManager _config;

    public GameLauncher(IProcess process,
                        IConfigurationManager config)
    {
        _process = process;
        _config = config;
    }

    public Process Start(string serverAddress)
    {
        var process = _config.Settings.WineSettings is not null
            ? StartWine(serverAddress)
            : StartNative(serverAddress);

        if (process is null)
        {
            throw new InvalidOperationException("Process was not started");
        }

        return process;
    }

    internal Process? StartNative(string serverAddress)
    {
        var args = $"-devserver {serverAddress}";
        var process = _process.Start(_config.Settings.OsuExePath, args);
        return process;
    }

    internal Process? StartWine(string serverAddress)
    {
        var wine = _config.Settings.WineSettings!;

        var processStartInfo = new ProcessStartInfo
        {
            FileName = wine.Path,
            Arguments = $"{_config.Settings.OsuExePath} -devserver {serverAddress}",
            Environment =
            {
                { "WINEPREFIX", wine.Prefix },
                { "WINEARCH", Enum.GetName(typeof(WineArch), wine.Arch)?.ToLower() },
            }
        };

        if (wine.Environment != null)
        {
            foreach (KeyValuePair<string, string> envVar in wine.Environment)
            {
                processStartInfo.EnvironmentVariables[envVar.Key] = envVar.Value;
            }
        }

        var process = _process.Start(processStartInfo);
        return process;
    }
}
