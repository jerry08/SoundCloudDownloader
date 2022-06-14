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

public class DownloadMultipleSetupViewModel : DialogScreen<IReadOnlyList<DownloadViewModel>>
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly DialogManager _dialogManager;
    private readonly SettingsService _settingsService;

    public string? Title { get; set; }

    public IReadOnlyList<TrackInformation>? AvailableTracks { get; set; }

    public IReadOnlyList<TrackInformation>? SelectedTracks { get; set; }

    public IReadOnlyList<string> AvailableContainers { get; } = new[]
    {
        "mp3",
        //"m4a",
        //"ogg"
    };

    public string SelectedContainer { get; set; } = "mp3";

    public DownloadMultipleSetupViewModel(
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
        SelectedContainer = _settingsService.LastContainer;
    }

    public void CopyTitle() => Clipboard.SetText(Title!);

    public bool CanConfirm => SelectedTracks!.Any();

    public void Confirm()
    {
        var dirPath = _dialogManager.PromptDirectoryPath();
        if (string.IsNullOrWhiteSpace(dirPath))
            return;

        var downloads = new List<DownloadViewModel>();
        for (var i = 0; i < SelectedTracks!.Count; i++)
        {
            var track = SelectedTracks[i];

            var baseFilePath = Path.Combine(
                dirPath,
                FileNameTemplate.Apply(
                    _settingsService.FileNameTemplate,
                    track,
                    SelectedContainer,
                    (i + 1).ToString().PadLeft(SelectedTracks.Count.ToString().Length, '0')
                )
            );

            if (_settingsService.ShouldSkipExistingFiles && File.Exists(baseFilePath))
                continue;

            var filePath = PathEx.GenerateUniquePath(baseFilePath);

            // Download does not start immediately, so lock in the file path to avoid conflicts
            DirectoryEx.CreateDirectoryForFile(filePath);
            File.WriteAllBytes(filePath, Array.Empty<byte>());

            downloads.Add(
                _viewModelFactory.CreateDownloadViewModel(
                    track,
                    filePath
                )
            );
        }

        _settingsService.LastContainer = SelectedContainer;

        Close(downloads);
    }
}

public static class DownloadMultipleSetupViewModelExtensions
{
    public static DownloadMultipleSetupViewModel CreateDownloadMultipleSetupViewModel(
        this IViewModelFactory factory,
        string title,
        IReadOnlyList<TrackInformation> availableTracks,
        bool preselectTracks = true)
    {
        var viewModel = factory.CreateDownloadMultipleSetupViewModel();

        viewModel.Title = title;
        viewModel.AvailableTracks = availableTracks;
        viewModel.SelectedTracks = preselectTracks
            ? availableTracks
            : Array.Empty<TrackInformation>();

        return viewModel;
    }
}