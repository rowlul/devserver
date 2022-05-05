using System.Diagnostics;

namespace DevServer.Services;

public class WineRunner : IWineRunner
{
    private readonly IProcess _process;

    public WineRunner(IProcess process)
    {
        _process = process;
    }

    public Process? RunWithArgs(string osuPath, string ipAddress, WineStartInfo wine)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = wine.Path,
            Arguments = $"{osuPath} -devserver {ipAddress}", 
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
