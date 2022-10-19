using System.Diagnostics;

namespace DevServer.Services;

public interface IProcess
{
    System.Diagnostics.Process? Start(string fileName, string? arguments = null);
    System.Diagnostics.Process? Start(ProcessStartInfo processStartInfo);
    bool IsRunning(string processName);
}
