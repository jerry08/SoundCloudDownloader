﻿using System;
using System.Collections.Generic;
using SoundCloudDownloader.Framework;
using SoundCloudDownloader.Services;
using SoundCloudDownloader.Utils;
using SoundCloudDownloader.Utils.Extensions;

namespace SoundCloudDownloader.ViewModels.Dialogs;

public class SettingsViewModel : DialogViewModelBase
{
    private readonly SettingsService _settingsService;

    private readonly DisposableCollector _eventRoot = new();

    public SettingsViewModel(SettingsService settingsService)
    {
        _settingsService = settingsService;

        _eventRoot.Add(_settingsService.WatchAllProperties(OnAllPropertiesChanged));
    }

    public IReadOnlyList<ThemeVariant> AvailableThemes { get; } = Enum.GetValues<ThemeVariant>();

    public ThemeVariant Theme
    {
        get => _settingsService.Theme;
        set => _settingsService.Theme = value;
    }

    public bool IsAutoUpdateEnabled
    {
        get => _settingsService.IsAutoUpdateEnabled;
        set => _settingsService.IsAutoUpdateEnabled = value;
    }

    public bool ShouldInjectTags
    {
        get => _settingsService.ShouldInjectTags;
        set => _settingsService.ShouldInjectTags = value;
    }

    public bool ShouldSkipExistingFiles
    {
        get => _settingsService.ShouldSkipExistingFiles;
        set => _settingsService.ShouldSkipExistingFiles = value;
    }

    public string FileNameTemplate
    {
        get => _settingsService.FileNameTemplate;
        set => _settingsService.FileNameTemplate = value;
    }

    public int ParallelLimit
    {
        get => _settingsService.ParallelLimit;
        set => _settingsService.ParallelLimit = Math.Clamp(value, 1, 10);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _eventRoot.Dispose();
        }

        base.Dispose(disposing);
    }
}
