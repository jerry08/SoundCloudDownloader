using System.Collections.Generic;
using SoundCloudExplode.Tracks;

namespace SoundCloudDownloader.Core.Resolving;

public record QueryResult(
    QueryResultKind Kind,
    string Title,
    IReadOnlyList<Track> Tracks
);