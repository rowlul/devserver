using System.Diagnostics;

namespace DevServer.Services;

public class NativeRunner : INativeRunner
{
    private readonly IProcess _process;

    public NativeRunner(IProcess process)
    {
        _process = process;
    }

    public Process RunWithArgs(string exePath, string ipAddress)
    {
        var args = $"-devserver {ipAddress}";
        var process = _process.Start(exePath, args);

        if (process is null)
        {
            throw new InvalidOperationException($"Process {Path.GetFileName(exePath)} was not started");
        }

        return process;
    }
}
