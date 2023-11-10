using System;
using System.Globalization;
using System.Windows.Data;
using SoundCloudExplode.Tracks;

namespace SoundCloudDownloader.Converters;

[ValueConversion(typeof(Track), typeof(string))]
public class TrackToHighestQualityArtworkUrlConverter : IValueConverter
{
    public static TrackToHighestQualityArtworkUrlConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
        value is Track track
            ? track.ArtworkUrl?.ToString().Replace("large", "t500x500").Replace("small", "t500x500")
            : null;

    public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}