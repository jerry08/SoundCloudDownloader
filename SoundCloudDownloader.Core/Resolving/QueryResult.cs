using System.Collections.Generic;
using SoundCloudExplode.Track;

namespace SoundCloudDownloader.Core.Resolving;

public record QueryResult(
    QueryResultKind Kind,
    string Title,
    IReadOnlyList<TrackInformation> Tracks
);