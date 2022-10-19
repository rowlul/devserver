namespace DevServer.Services;

public class HttpClientHandler : IHttpHandler
{
    private readonly HttpClient _client;

    public HttpClientHandler(HttpClient client)
    {
        _client = client;
    }

    public async Task<HttpResponseMessage> GetAsync(string url)
    {
        return await _client.GetAsync(url);
    }

    public async Task<Stream> GetStreamAsync(string url)
    {
        return await _client.GetStreamAsync(url);
    }

    public async Task<byte[]> GetByteArrayAsync(string url)
    {
        return await _client.GetByteArrayAsync(url);
    }

    public async Task<string> GetStringAsync(string url)
    {
        return await _client.GetStringAsync(url);
    }
}
