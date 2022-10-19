using System.Text.Json.Serialization;

namespace DevServer.Services;

public class Entry
{
    [JsonIgnore]
    public string FilePath { get; set; }

    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Logo { get; set; }
    public string ServerAddress { get; set; }
}
