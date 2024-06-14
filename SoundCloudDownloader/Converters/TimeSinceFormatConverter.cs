using System;
using System.Globalization;
using Avalonia.Data.Converters;
using SoundCloudDownloader.Utils.Extensions;

namespace SoundCloudDownloader.Converters;

public class TimeSinceFormatConverter : IValueConverter
{
    public static TimeSinceFormatConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        DateTime.TryParse(value?.ToString(), out var dateTime)
            ? dateTime.PeriodOfTimeShortFormat()
            : null!;

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) => throw new NotSupportedException();
}
