using System.Diagnostics;

namespace DevServer.Services;

public interface INativeRunner
{
    Process? RunWithArgs(string exePath, string ipAddress);
}
