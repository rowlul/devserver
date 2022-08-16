using System.Drawing;
using System.Text.Json.Serialization;

using SkiaSharp;

namespace DevServer.Models;

public record Entry([property: JsonIgnore] string FilePath,
                    string Name, string? Description, SKImage? Logo, string ServerAddress);
