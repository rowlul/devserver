using System.Diagnostics;

namespace DevServer.Services;

public interface IGameLauncher
{
    Process Start(string serverAddress);
}
