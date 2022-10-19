namespace DevServer.Services;

public interface ILogoService
{
    string ImageCachePath { get; }
    Task<Stream?> GetLogoStream(string? source, string? cacheFileName = null);
}
