using System.Diagnostics;

namespace DevServer.Services;

public interface IWineRunner
{
    Process? RunWithArgs(string osuPath, string ipAddress, WineStartInfo wine);
}
