using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DevServer.Services.Tests;

public class ProcessMock : IProcess
{
    private IList<System.Diagnostics.Process> Processes => new List<System.Diagnostics.Process>();

    public System.Diagnostics.Process? Start(string fileName, string? arguments = null)
    {
        var processStartInfo = new ProcessStartInfo { FileName = fileName, Arguments = arguments };

        var process = new System.Diagnostics.Process { StartInfo = processStartInfo };

        Processes.Add(process);

        return process;
    }

    public System.Diagnostics.Process? Start(ProcessStartInfo processStartInfo)
    {
        var process = new System.Diagnostics.Process { StartInfo = processStartInfo };

        Processes.Add(process);

        return process;
    }

    public IEnumerable<System.Diagnostics.Process> GetProcesses() => Processes;

    public IEnumerable<System.Diagnostics.Process> GetProcessesByName(string? processName = null) =>
        Processes.Where(x => x.ProcessName == processName);

    public bool IsRunning(string? processName = null) => GetProcessesByName(processName).Any();
}
