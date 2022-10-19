using System.Diagnostics;

namespace DevServer.Services;

public class Process : IProcess
{
    public System.Diagnostics.Process? Start(string fileName, string? arguments = null) => System.Diagnostics.Process.Start(fileName, arguments ?? "");
    public System.Diagnostics.Process? Start(ProcessStartInfo processStartInfo) => System.Diagnostics.Process.Start(processStartInfo);
    public bool IsRunning(string processName) => System.Diagnostics.Process.GetProcessesByName(processName).Length > 0;
}
