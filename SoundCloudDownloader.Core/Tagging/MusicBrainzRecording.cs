﻿namespace SoundCloudDownloader.Core.Tagging;

internal record MusicBrainzRecording(
    string Artist,
    string? ArtistSort,
    string Title,
    string? Album
);
