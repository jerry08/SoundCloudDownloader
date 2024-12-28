using System;
using System.IO;
using System.Text.Json.Serialization;
using Cogwheel;
using CommunityToolkit.Mvvm.ComponentModel;
using SoundCloudDownloader.Framework;

namespace SoundCloudDownloader.Services;

[INotifyPropertyChanged]
public partial class SettingsService : SettingsBase
{
    [ObservableProperty]
    public partial ThemeVariant Theme { get; set; }

    [ObservableProperty]
    public partial bool IsAutoUpdateEnabled { get; set; }

    [ObservableProperty]
    public partial bool ShouldInjectTags { get; set; }

    [ObservableProperty]
    public partial bool ShouldSkipExistingFiles { get; set; }

    [ObservableProperty]
    public partial string FileNameTemplate { get; set; }

    [ObservableProperty]
    public partial int ParallelLimit { get; set; }

    [ObservableProperty]
    public partial string LastContainer { get; set; }

    public SettingsService()
        : base(Path.Combine(AppContext.BaseDirectory, "Settings.dat"), SerializerContext.Default)
    {
        // Initialize properties here to prevent linux build error CS8050
        IsAutoUpdateEnabled = true;
        ShouldInjectTags = true;
        FileNameTemplate = "$title";
        ParallelLimit = 5;
        LastContainer = "Mp3";
    }
}

public partial class SettingsService
{
    [JsonSerializable(typeof(SettingsService))]
    private partial class SerializerContext : JsonSerializerContext;
}
