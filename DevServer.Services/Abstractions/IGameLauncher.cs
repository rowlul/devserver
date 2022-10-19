namespace DevServer.Services;

public interface IGameLauncher
{
    System.Diagnostics.Process Start(string executablePath, string serverAddress, WineStartInfo? wine = null);
}
