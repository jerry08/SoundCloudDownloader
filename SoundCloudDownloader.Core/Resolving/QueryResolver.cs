using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Gress;
using SoundCloudExplode;
using SoundCloudExplode.Track;

namespace SoundCloudDownloader.Core.Resolving;

public class QueryResolver
{
    private readonly SoundCloudClient _soundcloud = new();

    //private readonly Regex PlaylistRegex = new (@"soundcloud\..+?/(.*?)\/sets\/.+");
    //private readonly Regex TrackRegex = new (@"soundcloud\..+?/(.*?)\/sets\/.+");
    private readonly Regex PlaylistRegex = new (@"soundcloud\..+?\/(.*?)\/sets\/[a-zA-Z]+");
    private readonly Regex TrackRegex = new (@"soundcloud\..+?\/(.*?)\/[a-zA-Z0-9~@#$^*()_+=[\]{}|\\,.?: -]+");

    public async Task<QueryResult> ResolveAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        // Only consider URLs when parsing IDs.
        // All other queries are treated as search queries.
        var isUrl = Uri.IsWellFormedUriString(query, UriKind.Absolute);

        // Playlist
        if (isUrl && PlaylistRegex.IsMatch(query))
        {
            var playlist = await _soundcloud.GetPlaylistAsync(query, cancellationToken);
            var tracks = await _soundcloud.GetTracksAsync(query, cancellationToken);
            return new QueryResult(QueryResultKind.Playlist, $"Playlist: {playlist.Title}", tracks);
        }

        // Track
        if (isUrl && TrackRegex.IsMatch(query))
        {
            var track = await _soundcloud.GetTrackAsync(query, cancellationToken);
            return new QueryResult(QueryResultKind.Track, track.Title!, new[] { track });
        }

        // Default
        {
            var tracks = await _soundcloud.GetTracksAsync(query, cancellationToken);
            return new QueryResult(QueryResultKind.Playlist, "Tracks", tracks);
        }
    }

    public async Task<QueryResult> ResolveAsync(
        IReadOnlyList<string> queries,
        IProgress<Percentage>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (queries.Count == 1)
            return await ResolveAsync(queries.Single(), cancellationToken);

        var tracks = new List<TrackInformation>();

        var completed = 0;

        foreach (var query in queries)
        {
            var result = await ResolveAsync(query, cancellationToken);

            foreach (var track in result.Tracks)
            {
                tracks.Add(track);
            }

            progress?.Report(
                Percentage.FromFraction(1.0 * ++completed / queries.Count)
            );
        }

        return new QueryResult(QueryResultKind.Aggregate, $"{queries.Count} queries", tracks);
    }
}