using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Gress;
using SoundCloudExplode;
using SoundCloudExplode.Track;
using SoundCloudExplode.Common;

namespace SoundCloudDownloader.Core.Resolving;

public class QueryResolver
{
    private readonly SoundCloudClient _soundcloud = new();

    public async Task<QueryResult> ResolveAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        // Playlist
        if (_soundcloud.Playlists.IsUrlValid(query))
        {
            var playlist = await _soundcloud.Playlists.GetAsync(query, true, cancellationToken);
            var tracks = await _soundcloud.Playlists.GetTracksAsync(query, cancellationToken: cancellationToken);
            return new QueryResult(QueryResultKind.Playlist, $"Playlist: {playlist!.Title}", tracks);
        }

        // Track
        if (_soundcloud.Tracks.IsUrlValid(query))
        {
            var track = await _soundcloud.Tracks.GetAsync(query, cancellationToken);
            return new QueryResult(QueryResultKind.Track, track!.Title!, new[] { track });
        }

        // Default
        {
            var tracks = await _soundcloud.Tracks.GetTracksAsync(query, cancellationToken);
            return new QueryResult(QueryResultKind.Track, "Tracks", tracks);
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