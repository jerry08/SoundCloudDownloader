using System;
using System.Globalization;
using System.Windows.Data;

namespace SoundCloudDownloader.Converters;

[ValueConversion(typeof(long?), typeof(string))]
public class TrackDurationToHumanReadableFormatConverter : IValueConverter
{
    public static TrackDurationToHumanReadableFormatConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is long trackDuration)
        {
            var duration = TimeSpan.FromMilliseconds(trackDuration);

            if (duration.TotalSeconds < 3600)
            {
                return string.Format("{0}:{1:0#}.{2:000}", duration.Minutes, duration.Seconds, duration.Milliseconds);
            }
            else
            {
                return string.Format("{0}:{1:0#}:{2:0#}.{3:000}", (int)duration.TotalHours, duration.Minutes, duration.Seconds, duration.Milliseconds);
            }
        }

        return null!;
    }

    public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}