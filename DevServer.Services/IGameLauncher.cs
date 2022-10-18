using System.Diagnostics;

using DevServer.Models;

namespace DevServer.Services;

public interface IGameLauncher
{
    Process Start(string executablePath, string serverAddress, WineStartInfo? wine = null);
}
