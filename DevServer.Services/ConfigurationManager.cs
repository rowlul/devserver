using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;

using DevServer.Models;

namespace DevServer.Services;

public class ConfigurationManager : IConfigurationManager
{
    private readonly IPlatformService _platformService;
    private readonly IFileSystem _fileSystem;

    private static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public Settings? Settings { get; set; }

    public ConfigurationManager(IPlatformService platformService, IFileSystem fileSystem)
    {
        _platformService = platformService;
        _fileSystem = fileSystem;
    }

    public void Load()
    {
        using var fileStream = _fileSystem.File.OpenRead(_platformService.GetConfigFile());
        Settings = JsonSerializer.Deserialize<Settings>(fileStream, JsonSerializerOptions);
    }

    public async Task LoadAsync()
    {
        await using var fileStream = _fileSystem.File.OpenRead(_platformService.GetConfigFile());
        Settings = await JsonSerializer.DeserializeAsync<Settings>(fileStream, JsonSerializerOptions);
    }

    public void Save()
    {
        using var fileStream = _fileSystem.File.OpenWrite(_platformService.GetConfigFile());
        JsonSerializer.Serialize<Settings>(fileStream, Settings, JsonSerializerOptions);
    }

    public async Task SaveAsync()
    {
        await using var fileStream = _fileSystem.File.OpenWrite(_platformService.GetConfigFile());
        await JsonSerializer.SerializeAsync(fileStream, Settings, JsonSerializerOptions);
    }
}
