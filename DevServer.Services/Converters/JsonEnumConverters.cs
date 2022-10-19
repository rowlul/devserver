using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DevServer.Services.Converters;

// https://stackoverflow.com/a/59061296

public class JsonPropertyNameStringEnumConverter : GeneralJsonStringEnumConverter
{
    public JsonPropertyNameStringEnumConverter()
    {
    }

    public JsonPropertyNameStringEnumConverter(JsonNamingPolicy? namingPolicy = default, bool allowIntegerValues = true)
        : base(namingPolicy, allowIntegerValues)
    {
    }

    protected override bool TryOverrideName(Type enumType, string name, out ReadOnlyMemory<char> overrideName)
    {
        if (JsonEnumExtensions.TryGetEnumAttribute<JsonPropertyNameAttribute>(enumType, name, out var attr) &&
            attr.Name != null)
        {
            overrideName = attr.Name.AsMemory();
            return true;
        }

        return base.TryOverrideName(enumType, name, out overrideName);
    }
}

public class JsonEnumMemberStringEnumConverter : GeneralJsonStringEnumConverter
{
    public JsonEnumMemberStringEnumConverter()
    {
    }

    public JsonEnumMemberStringEnumConverter(JsonNamingPolicy? namingPolicy = default, bool allowIntegerValues = true) :
        base(namingPolicy, allowIntegerValues)
    {
    }

    protected override bool TryOverrideName(Type enumType, string name, out ReadOnlyMemory<char> overrideName)
    {
        if (JsonEnumExtensions.TryGetEnumAttribute<EnumMemberAttribute>(
                enumType,
                name,
                out var attr) && attr.Value != null)
        {
            overrideName = attr.Value.AsMemory();
            return true;
        }

        return base.TryOverrideName(enumType, name, out overrideName);
    }
}

public delegate bool TryOverrideName(Type enumType, string name, out ReadOnlyMemory<char> overrideName);

public class GeneralJsonStringEnumConverter : JsonConverterFactory
{
    private readonly bool _allowIntegerValues;
    private readonly JsonNamingPolicy? _namingPolicy;

    public GeneralJsonStringEnumConverter() : this(null) { }

    public GeneralJsonStringEnumConverter(JsonNamingPolicy? namingPolicy = default, bool allowIntegerValues = true)
    {
        (_namingPolicy, _allowIntegerValues) = (namingPolicy, allowIntegerValues);
    }

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsEnum || Nullable.GetUnderlyingType(typeToConvert)?.IsEnum == true;
    }

    public sealed override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var enumType = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;
        var flagged = enumType.IsDefined(typeof(FlagsAttribute), true);
        JsonConverter enumConverter;
        TryOverrideName tryOverrideName =
            (Type t, string n, out ReadOnlyMemory<char> o) => TryOverrideName(t, n, out o);
        var converterType =
            (flagged ? typeof(FlaggedJsonEnumConverter<>) : typeof(UnflaggedJsonEnumConverter<>))
            .MakeGenericType(enumType);
        enumConverter = (JsonConverter)Activator.CreateInstance(converterType,
                                                                BindingFlags.Instance | BindingFlags.Public |
                                                                BindingFlags.NonPublic,
                                                                null,
                                                                new object[]
                                                                {
                                                                    _namingPolicy!, _allowIntegerValues,
                                                                    tryOverrideName
                                                                },
                                                                null)!;
        if (enumType == typeToConvert)
        {
            return enumConverter;
        }

        var nullableConverter = (JsonConverter)Activator.CreateInstance(
            typeof(NullableConverterDecorator<>).MakeGenericType(enumType),
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            new object[] { enumConverter },
            null)!;
        return nullableConverter;
    }

    protected virtual bool TryOverrideName(Type enumType, string name, out ReadOnlyMemory<char> overrideName)
    {
        overrideName = default;
        return false;
    }

    private class FlaggedJsonEnumConverter<TEnum> : JsonEnumConverterBase<TEnum> where TEnum : struct, Enum
    {
        private const char FlagSeparatorChar = ',';
        private const string FlagSeparatorString = ", ";

        public FlaggedJsonEnumConverter(JsonNamingPolicy? namingPolicy, bool allowNumbers,
                                        TryOverrideName? tryOverrideName) : base(
            namingPolicy,
            allowNumbers,
            tryOverrideName)
        {
        }

        protected override bool TryFormatAsString(EnumData<TEnum>[] enumData, TEnum value,
                                                  out ReadOnlyMemory<char> name)
        {
            var uInt64Value = value.ToUInt64(EnumTypeCode);
            var index = enumData.BinarySearchFirst(uInt64Value, EntryComparer);
            if (index >= 0)
            {
                // A single flag
                name = enumData[index].Name;
                return true;
            }

            if (uInt64Value != 0)
            {
                StringBuilder? sb = null;
                for (var i = ~index - 1; i >= 0; i--)
                {
                    if ((uInt64Value & enumData[i].UInt64Value) == enumData[i].UInt64Value &&
                        enumData[i].UInt64Value != 0)
                    {
                        if (sb == null)
                        {
                            sb = new StringBuilder();
                            sb.Append(enumData[i].Name.Span);
                        }
                        else
                        {
                            sb.Insert(0, FlagSeparatorString);
                            sb.Insert(0, enumData[i].Name.Span);
                        }

                        uInt64Value -= enumData[i].UInt64Value;
                    }
                }

                if (uInt64Value == 0 && sb != null)
                {
                    name = sb.ToString().AsMemory();
                    return true;
                }
            }

            name = default;
            return false;
        }

        protected override bool TryReadAsString(EnumData<TEnum>[] enumData,
                                                ILookup<ReadOnlyMemory<char>, int> nameLookup,
                                                ReadOnlyMemory<char> name, out TEnum value)
        {
            ulong uInt64Value = 0;
            foreach (var slice in name.Split(FlagSeparatorChar, StringSplitOptions.TrimEntries))
            {
                if (JsonEnumExtensions.TryLookupBest(enumData, nameLookup, slice, out var thisValue))
                {
                    uInt64Value |= thisValue.ToUInt64(EnumTypeCode);
                }
                else
                {
                    value = default;
                    return false;
                }
            }

            value = uInt64Value.FromUInt64<TEnum>();
            return true;
        }
    }

    private class UnflaggedJsonEnumConverter<TEnum> : JsonEnumConverterBase<TEnum> where TEnum : struct, Enum
    {
        public UnflaggedJsonEnumConverter(JsonNamingPolicy? namingPolicy, bool allowNumbers,
                                          TryOverrideName? tryOverrideName) : base(
            namingPolicy,
            allowNumbers,
            tryOverrideName)
        {
        }

        protected override bool TryFormatAsString(EnumData<TEnum>[] enumData, TEnum value,
                                                  out ReadOnlyMemory<char> name)
        {
            var index = enumData.BinarySearchFirst(value.ToUInt64(EnumTypeCode), EntryComparer);
            if (index >= 0)
            {
                name = enumData[index].Name;
                return true;
            }

            name = default;
            return false;
        }

        protected override bool TryReadAsString(EnumData<TEnum>[] enumData,
                                                ILookup<ReadOnlyMemory<char>, int> nameLookup,
                                                ReadOnlyMemory<char> name, out TEnum value)
        {
            return JsonEnumExtensions.TryLookupBest(enumData, nameLookup, name, out value);
        }
    }

    private abstract class JsonEnumConverterBase<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
    {
        protected static TypeCode EnumTypeCode { get; } = Type.GetTypeCode(typeof(TEnum));

        protected static Func<EnumData<TEnum>, ulong, int> EntryComparer { get; } =
            (item, key) => item.UInt64Value.CompareTo(key);

        private bool AllowNumbers { get; }
        private EnumData<TEnum>[] EnumData { get; }
        private ILookup<ReadOnlyMemory<char>, int> NameLookup { get; }

        public JsonEnumConverterBase(JsonNamingPolicy? namingPolicy, bool allowNumbers,
                                     TryOverrideName? tryOverrideName)
        {
            AllowNumbers = allowNumbers;
            EnumData = JsonEnumExtensions.GetData<TEnum>(namingPolicy, tryOverrideName).ToArray();
            NameLookup = JsonEnumExtensions.GetLookupTable(EnumData);
        }

        public sealed override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            // Todo: consider caching a small number of JsonEncodedText values for the first N enums encountered, as is done in
            // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/Converters/Value/EnumConverter.cs
            if (TryFormatAsString(EnumData, value, out var name))
            {
                writer.WriteStringValue(name.Span);
            }
            else
            {
                if (!AllowNumbers)
                {
                    throw new JsonException();
                }

                WriteEnumAsNumber(writer, value);
            }
        }

        protected abstract bool TryFormatAsString(EnumData<TEnum>[] enumData, TEnum value,
                                                  out ReadOnlyMemory<char> name);

        protected abstract bool TryReadAsString(EnumData<TEnum>[] enumData,
                                                ILookup<ReadOnlyMemory<char>, int> nameLookup,
                                                ReadOnlyMemory<char> name, out TEnum value);

        public sealed override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert,
                                          JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.String => TryReadAsString(EnumData,
                                                        NameLookup,
                                                        reader.GetString().AsMemory(),
                                                        out var value)
                    ? value
                    : throw new JsonException(),
                JsonTokenType.Number => AllowNumbers ? ReadNumberAsEnum(ref reader) : throw new JsonException(),
                _ => throw new JsonException()
            };
        }

        private static void WriteEnumAsNumber(Utf8JsonWriter writer, TEnum value)
        {
            switch (EnumTypeCode)
            {
                case TypeCode.SByte:
                    writer.WriteNumberValue(Unsafe.As<TEnum, sbyte>(ref value));
                    break;
                case TypeCode.Int16:
                    writer.WriteNumberValue(Unsafe.As<TEnum, short>(ref value));
                    break;
                case TypeCode.Int32:
                    writer.WriteNumberValue(Unsafe.As<TEnum, int>(ref value));
                    break;
                case TypeCode.Int64:
                    writer.WriteNumberValue(Unsafe.As<TEnum, long>(ref value));
                    break;
                case TypeCode.Byte:
                    writer.WriteNumberValue(Unsafe.As<TEnum, byte>(ref value));
                    break;
                case TypeCode.UInt16:
                    writer.WriteNumberValue(Unsafe.As<TEnum, ushort>(ref value));
                    break;
                case TypeCode.UInt32:
                    writer.WriteNumberValue(Unsafe.As<TEnum, uint>(ref value));
                    break;
                case TypeCode.UInt64:
                    writer.WriteNumberValue(Unsafe.As<TEnum, ulong>(ref value));
                    break;
                default:
                    throw new JsonException();
            }
        }

        private static TEnum ReadNumberAsEnum(ref Utf8JsonReader reader)
        {
            switch (EnumTypeCode)
            {
                case TypeCode.SByte:
                    {
                        var i = reader.GetSByte();
                        return Unsafe.As<sbyte, TEnum>(ref i);
                    }
                    ;
                case TypeCode.Int16:
                    {
                        var i = reader.GetInt16();
                        return Unsafe.As<short, TEnum>(ref i);
                    }
                    ;
                case TypeCode.Int32:
                    {
                        var i = reader.GetInt32();
                        return Unsafe.As<int, TEnum>(ref i);
                    }
                    ;
                case TypeCode.Int64:
                    {
                        var i = reader.GetInt64();
                        return Unsafe.As<long, TEnum>(ref i);
                    }
                    ;
                case TypeCode.Byte:
                    {
                        var i = reader.GetByte();
                        return Unsafe.As<byte, TEnum>(ref i);
                    }
                    ;
                case TypeCode.UInt16:
                    {
                        var i = reader.GetUInt16();
                        return Unsafe.As<ushort, TEnum>(ref i);
                    }
                    ;
                case TypeCode.UInt32:
                    {
                        var i = reader.GetUInt32();
                        return Unsafe.As<uint, TEnum>(ref i);
                    }
                    ;
                case TypeCode.UInt64:
                    {
                        var i = reader.GetUInt64();
                        return Unsafe.As<ulong, TEnum>(ref i);
                    }
                    ;
                default:
                    throw new JsonException();
            }
        }
    }
}

public sealed class NullableConverterDecorator<T> : JsonConverter<T?> where T : struct
{
    // Read() and Write() are never called with null unless HandleNull is overwridden -- which it is not.
    private readonly JsonConverter<T> _innerConverter;

    public NullableConverterDecorator(JsonConverter<T> innerConverter)
    {
        _innerConverter =
            innerConverter ?? throw new ArgumentNullException(nameof(innerConverter));
    }

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return _innerConverter.Read(ref reader, Nullable.GetUnderlyingType(typeToConvert)!, options);
    }

    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        _innerConverter.Write(writer, value!.Value, options);
    }

    public override bool CanConvert(Type type)
    {
        return base.CanConvert(type) && _innerConverter.CanConvert(Nullable.GetUnderlyingType(type)!);
    }
}

internal readonly record struct EnumData<TEnum>(ReadOnlyMemory<char> Name, TEnum Value, ulong UInt64Value)
    where TEnum : struct, Enum;

internal static class JsonEnumExtensions
{
    public static bool TryGetEnumAttribute<TAttribute>(Type type, string name,
                                                       [NotNullWhen(true)] out TAttribute? attribute)
        where TAttribute : Attribute
    {
        var member = type.GetMember(name).SingleOrDefault();
        attribute = member?.GetCustomAttribute<TAttribute>(false);
        return attribute != null;
    }

    public static ulong ToUInt64<TEnum>(this TEnum value) where TEnum : struct, Enum
    {
        return value.ToUInt64(Type.GetTypeCode(typeof(TEnum)));
    }

    internal static ulong ToUInt64<TEnum>(this TEnum value, TypeCode enumTypeCode) where TEnum : struct, Enum
    {
        Debug.Assert(enumTypeCode == Type.GetTypeCode(typeof(TEnum)));
        return enumTypeCode switch
        {
            TypeCode.SByte => unchecked((ulong)Unsafe.As<TEnum, sbyte>(ref value)),
            TypeCode.Int16 => unchecked((ulong)Unsafe.As<TEnum, short>(ref value)),
            TypeCode.Int32 => unchecked((ulong)Unsafe.As<TEnum, int>(ref value)),
            TypeCode.Int64 => unchecked((ulong)Unsafe.As<TEnum, long>(ref value)),
            TypeCode.Byte => Unsafe.As<TEnum, byte>(ref value),
            TypeCode.UInt16 => Unsafe.As<TEnum, ushort>(ref value),
            TypeCode.UInt32 => Unsafe.As<TEnum, uint>(ref value),
            TypeCode.UInt64 => Unsafe.As<TEnum, ulong>(ref value),
            _ => throw new ArgumentException(enumTypeCode.ToString())
        };
    }

    public static TEnum FromUInt64<TEnum>(this ulong value) where TEnum : struct, Enum
    {
        return value.FromUInt64<TEnum>(Type.GetTypeCode(typeof(TEnum)));
    }

    internal static TEnum FromUInt64<TEnum>(this ulong value, TypeCode enumTypeCode) where TEnum : struct, Enum
    {
        Debug.Assert(enumTypeCode == Type.GetTypeCode(typeof(TEnum)));
        switch (enumTypeCode)
        {
            case TypeCode.SByte:
                {
                    var i = unchecked((sbyte)value);
                    return Unsafe.As<sbyte, TEnum>(ref i);
                }
                ;
            case TypeCode.Int16:
                {
                    var i = unchecked((short)value);
                    return Unsafe.As<short, TEnum>(ref i);
                }
                ;
            case TypeCode.Int32:
                {
                    var i = unchecked((int)value);
                    return Unsafe.As<int, TEnum>(ref i);
                }
                ;
            case TypeCode.Int64:
                {
                    var i = unchecked((long)value);
                    return Unsafe.As<long, TEnum>(ref i);
                }
                ;
            case TypeCode.Byte:
                {
                    var i = unchecked((byte)value);
                    return Unsafe.As<byte, TEnum>(ref i);
                }
                ;
            case TypeCode.UInt16:
                {
                    var i = unchecked((ushort)value);
                    return Unsafe.As<ushort, TEnum>(ref i);
                }
                ;
            case TypeCode.UInt32:
                {
                    var i = unchecked((uint)value);
                    return Unsafe.As<uint, TEnum>(ref i);
                }
                ;
            case TypeCode.UInt64:
                {
                    var i = value;
                    return Unsafe.As<ulong, TEnum>(ref i);
                }
                ;
            default:
                throw new ArgumentException(enumTypeCode.ToString());
        }
    }

    // Return data about the enum sorted by the binary values of the enumeration constants (that is, by their unsigned magnitude)
    internal static IEnumerable<EnumData<TEnum>> GetData<TEnum>(JsonNamingPolicy? namingPolicy,
                                                                TryOverrideName? tryOverrideName)
        where TEnum : struct, Enum
    {
        return GetData<TEnum>(namingPolicy, tryOverrideName, Type.GetTypeCode(typeof(TEnum)));
    }

    // Return data about the enum sorted by the binary values of the enumeration constants (that is, by their unsigned magnitude)
    internal static IEnumerable<EnumData<TEnum>> GetData<TEnum>(JsonNamingPolicy? namingPolicy,
                                                                TryOverrideName? tryOverrideName, TypeCode enumTypeCode)
        where TEnum : struct, Enum
    {
        Debug.Assert(enumTypeCode == Type.GetTypeCode(typeof(TEnum)));
        var names = Enum.GetNames<TEnum>();
        var values = Enum.GetValues<TEnum>();
        return names.Zip(values,
                         (n, v) =>
                         {
                             if (tryOverrideName == null || !tryOverrideName(typeof(TEnum), n, out var jsonName))
                             {
                                 jsonName = namingPolicy == null
                                     ? n.AsMemory()
                                     : namingPolicy.ConvertName(n).AsMemory();
                             }

                             return new EnumData<TEnum>(jsonName, v, v.ToUInt64(enumTypeCode));
                         });
    }

    internal static ILookup<ReadOnlyMemory<char>, int> GetLookupTable<TEnum>(EnumData<TEnum>[] namesAndValues)
        where TEnum : struct, Enum
    {
        return Enumerable.Range(0, namesAndValues.Length)
                         .ToLookup(i => namesAndValues[i].Name, CharMemoryComparer.OrdinalIgnoreCase);
    }

    internal static bool TryLookupBest<TEnum>(EnumData<TEnum>[] namesAndValues,
                                              ILookup<ReadOnlyMemory<char>, int> lookupTable, ReadOnlyMemory<char> name,
                                              out TEnum value) where TEnum : struct, Enum
    {
        var i = 0;
        var firstMatch = -1;
        foreach (var index in lookupTable[name])
        {
            if (firstMatch == -1)
            {
                firstMatch = index;
            }
            else
            {
                if (i == 1 && namesAndValues[firstMatch].Name.Span.Equals(name.Span,
                                                                          StringComparison.Ordinal))
                {
                    value = namesAndValues[firstMatch].Value;
                    return true;
                }

                if (namesAndValues[index].Name.Span.Equals(name.Span, StringComparison.Ordinal))
                {
                    value = namesAndValues[index].Value;
                    return true;
                }
            }

            i++;
        }

        value = firstMatch == -1 ? default : namesAndValues[firstMatch].Value;
        return firstMatch != -1;
    }
}

public static class StringExtensions
{
    public static IEnumerable<ReadOnlyMemory<char>> Split(this ReadOnlyMemory<char> chars, char separator,
                                                          StringSplitOptions options = StringSplitOptions.None)
    {
        int index;
        while ((index = chars.Span.IndexOf(separator)) >= 0)
        {
            var slice = chars.Slice(0, index);
            if ((options & StringSplitOptions.TrimEntries) == StringSplitOptions.TrimEntries)
            {
                slice = slice.Trim();
            }

            if ((options & StringSplitOptions.RemoveEmptyEntries) == 0 || slice.Length > 0)
            {
                yield return slice;
            }

            chars = chars.Slice(index + 1);
        }

        if ((options & StringSplitOptions.TrimEntries) == StringSplitOptions.TrimEntries)
        {
            chars = chars.Trim();
        }

        if ((options & StringSplitOptions.RemoveEmptyEntries) == 0 || chars.Length > 0)
        {
            yield return chars;
        }
    }
}

public static class ListExtensions
{
    public static int BinarySearch<TValue, TKey>(this TValue[] list, TKey key, Func<TValue, TKey, int> comparer)
    {
        if (list == null || comparer == null)
        {
            throw new ArgumentNullException();
        }

        var low = 0;
        var high = list.Length - 1;
        while (low <= high)
        {
            var mid = low + ((high - low) >> 1);
            var order = comparer(list[mid], key);
            if (order == 0)
            {
                return mid;
            }

            if (order > 0)
            {
                high = mid - 1;
            }
            else
            {
                low = mid + 1;
            }
        }

        return ~low;
    }

    public static int BinarySearchFirst<TValue, TKey>(this TValue[] list, TKey key, Func<TValue, TKey, int> comparer)
    {
        var index = list.BinarySearch(key, comparer);
        for (; index > 0 && comparer(list[index - 1], key) == 0; index--)
        {
            ;
        }

        return index;
    }
}

public class CharMemoryComparer : IEqualityComparer<ReadOnlyMemory<char>>
{
    private readonly StringComparison _comparison;

    public static CharMemoryComparer OrdinalIgnoreCase { get; } = new(StringComparison.OrdinalIgnoreCase);

    public static CharMemoryComparer Ordinal { get; } = new(StringComparison.Ordinal);

    private CharMemoryComparer(StringComparison comparison)
    {
        _comparison = comparison;
    }

    public bool Equals(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y)
    {
        return x.Span.Equals(y.Span, _comparison);
    }

    public int GetHashCode(ReadOnlyMemory<char> obj)
    {
        return string.GetHashCode(obj.Span, _comparison);
    }
}
