namespace DevServer.Services;

public interface IHttpHandler
{
    Task<HttpResponseMessage> GetAsync(string url);
    Task<Stream> GetStreamAsync(string url);
    Task<byte[]> GetByteArrayAsync(string url);
    Task<string> GetStringAsync(string url);
}
