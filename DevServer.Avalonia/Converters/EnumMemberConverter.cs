using System.Globalization;
using System.Runtime.Serialization;

using Avalonia.Data.Converters;

namespace DevServer.Avalonia.Converters;

public class EnumMemberConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(value);

        var @enum = (Enum)value;
        var fieldInfo = value.GetType().GetField(@enum.ToString())!;
        var attributes = fieldInfo.GetCustomAttributes(inherit: false);

        if (attributes.Length == 0)
        {
            return Enum.GetName(@enum.GetType(), @enum);
        }

        string? enumValue = null;
        foreach (var attribute in attributes)
        {
            if (attribute is EnumMemberAttribute enumMemberAttribute)
            {
                enumValue = enumMemberAttribute.Value;
            }
        }

        return enumValue ?? Enum.GetName(@enum.GetType(), @enum);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return string.Empty;
    }
}
