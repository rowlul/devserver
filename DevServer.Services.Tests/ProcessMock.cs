using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DevServer.Services.Tests;

public class ProcessMock : IProcess
{
    private IList<Process> Processes => new List<Process>();

    public Process? Start(string fileName, string? arguments = null)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments
        };
        
        var process = new Process
        {
            StartInfo = processStartInfo
        };

        Processes.Add(process);

        return process;
    }

    public Process? Start(ProcessStartInfo processStartInfo)
    {
        var process = new Process
        {
            StartInfo = processStartInfo
        };
        
        Processes.Add(process);

        return process;
    }

    public IEnumerable<Process> GetProcesses() => Processes;
    public IEnumerable<Process> GetProcessesByName(string? processName = null) => Processes.Where(x => x.ProcessName == processName);
    
    public bool IsRunning(string? processName = null) => GetProcessesByName(processName).Any();
}
