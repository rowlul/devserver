using System.Diagnostics;

namespace DevServer.Services;

public interface IWineRunner
{
    Process? RunWithArgsViaWine(string osuPath, string ipAddress, WineStartInfo wine);
}
