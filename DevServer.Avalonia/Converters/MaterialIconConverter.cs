using System.Globalization;

using Avalonia.Data.Converters;

using DevServer.ViewModels;

using Material.Icons;

namespace DevServer.Avalonia.Converters;

/// <summary>
/// Converts MessageBoxIcon into MaterialIcon and vice versa.
/// </summary>
public class MaterialIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);

        var msgBoxIcon = (MessageBoxIcon)value;
        return msgBoxIcon switch
        {
            MessageBoxIcon.Warning => MaterialIconKind.Warning,
            MessageBoxIcon.Error => MaterialIconKind.Error,
            MessageBoxIcon.Question => MaterialIconKind.QuestionMarkCircle,
            _ => throw new ArgumentOutOfRangeException(nameof(msgBoxIcon), msgBoxIcon, $"No match for {value}.")
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);

        var materialIcon = (MaterialIconKind)value;
        return materialIcon switch
        {
            MaterialIconKind.Warning => MessageBoxIcon.Warning,
            MaterialIconKind.Error => MessageBoxIcon.Error,
            MaterialIconKind.QuestionMarkCircle => MessageBoxIcon.Question,
            _ => throw new ArgumentOutOfRangeException(nameof(materialIcon), materialIcon, $"No match for {value}.")
        };
    }
}
