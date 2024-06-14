using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace SoundCloudDownloader.Converters;

internal class MillisecondsToHumanReadableFormatConverter : IValueConverter
{
    public static MillisecondsToHumanReadableFormatConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (long.TryParse(value?.ToString(), out var ms))
        {
            var duration = TimeSpan.FromMilliseconds(ms);

            if (duration.TotalSeconds < 3600)
            {
                //return string.Format(
                //    "{0}:{1:0#}.{2:000}",
                //    duration.Minutes,
                //    duration.Seconds,
                //    duration.Milliseconds
                //);
                return string.Format("{0}:{1:0#}", duration.Minutes, duration.Seconds);
            }
            else
            {
                //return string.Format(
                //    "{0}:{1:0#}:{2:0#}.{3:000}",
                //    (int)duration.TotalHours,
                //    duration.Minutes,
                //    duration.Seconds,
                //    duration.Milliseconds
                //);
                return string.Format(
                    "{0}:{1:0#}:{2:0#}",
                    (int)duration.TotalHours,
                    duration.Minutes,
                    duration.Seconds
                );
            }
        }

        return null!;
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) => throw new NotSupportedException();
}
