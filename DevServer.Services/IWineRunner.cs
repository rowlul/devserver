using System.Diagnostics;

using DevServer.Models;

namespace DevServer.Services;

public interface IWineRunner
{
    Process RunWithArgs(string osuPath, string ipAddress, WineStartInfo wine);
}
