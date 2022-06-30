using System.Drawing;

namespace DevServer.Models;

public record Entry(string Name, string? Description, Bitmap? Logo, string ServerAddress);
