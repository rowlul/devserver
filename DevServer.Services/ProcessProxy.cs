using System.Diagnostics;

namespace DevServer.Services;

public class ProcessProxy : IProcess
{
    public Process? Start(string fileName, string? arguments = null) => Process.Start(fileName, arguments ?? "");
    public Process? Start(ProcessStartInfo processStartInfo) => Process.Start(processStartInfo);
    public bool IsRunning(string processName) => Process.GetProcessesByName(processName).Length > 0;
}
