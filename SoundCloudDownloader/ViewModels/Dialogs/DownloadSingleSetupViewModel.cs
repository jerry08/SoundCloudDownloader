using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using SoundCloudDownloader.Core.Downloading;
using SoundCloudDownloader.Services;
using SoundCloudDownloader.Utils;
using SoundCloudDownloader.ViewModels.Components;
using SoundCloudDownloader.ViewModels.Framework;
using SoundCloudExplode.Track;

namespace SoundCloudDownloader.ViewModels.Dialogs;

public class DownloadSingleSetupViewModel : DialogScreen<DownloadViewModel>
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly DialogManager _dialogManager;
    private readonly SettingsService _settingsService;

    public TrackInformation? Track { get; set; }

    public IReadOnlyList<string>? AvailableDownloadOptions { get; set; } = new[]
    {
        "mp3",
        //"m4a",
        //"ogg"
    };

    public string? SelectedDownloadOption { get; set; }

    public DownloadSingleSetupViewModel(
        IViewModelFactory viewModelFactory,
        DialogManager dialogManager,
        SettingsService settingsService)
    {
        _viewModelFactory = viewModelFactory;
        _dialogManager = dialogManager;
        _settingsService = settingsService;
    }

    public void OnViewLoaded()
    {
        SelectedDownloadOption = AvailableDownloadOptions?.FirstOrDefault(o =>
            o == _settingsService.LastContainer
        );
    }

    public void CopyTitle() => Clipboard.SetText(Track!.Title);

    public void Confirm()
    {
        var container = SelectedDownloadOption!;

        var filePath = _dialogManager.PromptSaveFilePath(
            $"{container} file|*.{container}",
            FileNameTemplate.Apply(
                _settingsService.FileNameTemplate,
                Track!,
                container
            )
        );

        if (string.IsNullOrWhiteSpace(filePath))
            return;

        // Download does not start immediately, so lock in the file path to avoid conflicts
        DirectoryEx.CreateDirectoryForFile(filePath);
        File.WriteAllBytes(filePath, Array.Empty<byte>());

        _settingsService.LastContainer = container;

        Close(
            _viewModelFactory.CreateDownloadViewModel(Track!, filePath)
        );
    }
}

public static class DownloadSingleSetupViewModelExtensions
{
    public static DownloadSingleSetupViewModel CreateDownloadSingleSetupViewModel(
        this IViewModelFactory factory,
        TrackInformation track)
    {
        var viewModel = factory.CreateDownloadSingleSetupViewModel();

        viewModel.Track = track;

        return viewModel;
    }
}