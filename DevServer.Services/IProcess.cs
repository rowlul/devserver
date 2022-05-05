using System.Diagnostics;

namespace DevServer.Services;

public interface IProcess
{
    Process? Start(string fileName, string? arguments = null);
    Process? Start(ProcessStartInfo processStartInfo);
    bool IsRunning(string processName);
}
