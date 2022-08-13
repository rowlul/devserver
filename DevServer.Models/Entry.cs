using System.Drawing;

using SkiaSharp;

namespace DevServer.Models;

public record Entry(string Name, string? Description, SKImage? Logo, string ServerAddress);
