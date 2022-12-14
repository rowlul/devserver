using System.IO.Abstractions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using DevServer.Services.Converters;

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

    public string ConfigFilePath => Path.Combine(_platformService.GetAppDataPath(), "settings.json");

    public ConfigurationManager(IPlatformService platformService, IFileSystem fileSystem)
    {
        _platformService = platformService;
        _fileSystem = fileSystem;
    }

    public Settings Settings { get; private set; } = new();

    public void Load()
    {
        using var fileStream = _fileSystem.File.OpenRead(ConfigFilePath);
        Settings = JsonSerializer.Deserialize<Settings>(fileStream, JsonSerializerOptions) ??
                   throw new InvalidOperationException("Settings was null");
    }

    public async Task LoadAsync()
    {
        await using var fileStream = _fileSystem.FileStream.Create(ConfigFilePath,
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
        using var fileStream = _fileSystem.File.Open(ConfigFilePath,
                                                     FileMode.Create,
                                                     FileAccess.ReadWrite,
                                                     FileShare.ReadWrite);

        JsonSerializer.Serialize(fileStream, Settings, JsonSerializerOptions);

        var newline = Encoding.ASCII.GetBytes(Environment.NewLine);
        fileStream.WriteAsync(newline, 0, newline.Length);
    }

    public async Task SaveAsync()
    {
        await using var fileStream = _fileSystem.FileStream.Create(ConfigFilePath,
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
