using System.IO.Abstractions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using DevServer.Models;
using DevServer.Services.Helpers;

namespace DevServer.Services;

public class ConfigurationManager : IConfigurationManager
{
    private readonly IPlatformService _platformService;
    private readonly IFileSystem _fileSystem;

    private static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        Converters = { new JsonEnumMemberStringEnumConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public Settings Settings { get; private set; } = new();

    public ConfigurationManager(IPlatformService platformService, IFileSystem fileSystem)
    {
        _platformService = platformService;
        _fileSystem = fileSystem;
    }

    public void Load()
    {
        using var fileStream = _fileSystem.File.OpenRead(_platformService.GetConfigFile());
        Settings = JsonSerializer.Deserialize<Settings>(fileStream, JsonSerializerOptions) ??
                   throw new InvalidOperationException("Settings was null");
    }

    public async Task LoadAsync()
    {
        await using var fileStream = _fileSystem.FileStream.Create(_platformService.GetConfigFile(),
                                                                   FileMode.Open,
                                                                   FileAccess.Read,
                                                                   FileShare.Read,
                                                                   bufferSize: 4096,
                                                                   useAsync: true);

        Settings = await JsonSerializer.DeserializeAsync<Settings>(fileStream, JsonSerializerOptions) ??
                   throw new InvalidOperationException("Settings was null");
    }

    public void Save()
    {
        using var fileStream = _fileSystem.File.Open(_platformService.GetConfigFile(),
                                                     FileMode.Create,
                                                     FileAccess.ReadWrite,
                                                     FileShare.ReadWrite);

        JsonSerializer.Serialize(fileStream, Settings, JsonSerializerOptions);

        var newline = Encoding.ASCII.GetBytes(Environment.NewLine);
        fileStream.WriteAsync(newline, 0, newline.Length);
    }

    public async Task SaveAsync()
    {
        await using var fileStream = _fileSystem.FileStream.Create(_platformService.GetConfigFile(),
                                                                   FileMode.Create,
                                                                   FileAccess.ReadWrite,
                                                                   FileShare.ReadWrite,
                                                                   bufferSize: 4096,
                                                                   useAsync: true);

        await JsonSerializer.SerializeAsync(fileStream, Settings, JsonSerializerOptions);

        var newline = Encoding.ASCII.GetBytes(Environment.NewLine);
        await fileStream.WriteAsync(newline, 0, newline.Length);
    }
}
