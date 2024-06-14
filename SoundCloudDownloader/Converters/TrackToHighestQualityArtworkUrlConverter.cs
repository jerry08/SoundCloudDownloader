using System;
using System.Globalization;
using Avalonia.Data.Converters;
using SoundCloudExplode.Tracks;

namespace SoundCloudDownloader.Converters;

public class TrackToHighestQualityArtworkUrlConverter : IValueConverter
{
    public static TrackToHighestQualityArtworkUrlConverter Instance { get; } = new();

    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) =>
        value is Track track
            ? track.ArtworkUrl?.ToString().Replace("large", "t500x500").Replace("small", "t500x500")
            : null;

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) => throw new NotSupportedException();
}
