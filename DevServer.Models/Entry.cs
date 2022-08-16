using System.Text.Json.Serialization;

namespace DevServer.Models;

public record Entry([property: JsonIgnore] string FilePath,
                    string Name, string? Description, string? Logo, string ServerAddress);
