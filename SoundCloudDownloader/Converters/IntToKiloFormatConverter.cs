using System;
using System.Globalization;
using Avalonia.Data.Converters;
using SoundCloudDownloader.Utils.Extensions;

namespace SoundCloudDownloader.Converters;

public class IntToKiloFormatConverter : IValueConverter
{
    public static IntToKiloFormatConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        int.TryParse(value?.ToString(), out var count) ? count.ToKiloFormat(false) : null!;

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) => throw new NotSupportedException();
}
