using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoundCloudDownloader.Core.Downloading;
using SoundCloudDownloader.Framework;
using SoundCloudDownloader.Services;
using SoundCloudDownloader.Utils;
using SoundCloudDownloader.Utils.Extensions;
using SoundCloudDownloader.ViewModels.Components;
using SoundCloudExplode.Tracks;

namespace SoundCloudDownloader.ViewModels.Dialogs;

public partial class DownloadMultipleSetupViewModel(
    ViewModelManager viewModelManager,
    DialogManager dialogManager,
    SettingsService settingsService
) : DialogViewModelBase<IReadOnlyList<DownloadViewModel>>
{
    [ObservableProperty]
    public partial string? Title { get; set; }

    [ObservableProperty]
    public partial IReadOnlyList<Track>? AvailableTracks { get; set; }

    [ObservableProperty]
    public partial string SelectedContainer { get; set; } = "Mp3";
    public ObservableCollection<Track> SelectedTracks { get; } = [];

    public IReadOnlyList<string> AvailableContainers { get; } = ["Mp3"];

    [RelayCommand]
    private void Initialize()
    {
        SelectedContainer = settingsService.LastContainer;
        SelectedTracks.CollectionChanged += (_, _) => ConfirmCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private async Task CopyTitleAsync()
    {
        if (Application.Current?.ApplicationLifetime?.TryGetTopLevel()?.Clipboard is { } clipboard)
            await clipboard.SetTextAsync(Title);
    }

    private bool CanConfirm() => SelectedTracks.Any();

    [RelayCommand(CanExecute = nameof(CanConfirm))]
    private async Task ConfirmAsync()
    {
        var dirPath = await dialogManager.PromptDirectoryPathAsync();
        if (string.IsNullOrWhiteSpace(dirPath))
            return;

        var downloads = new List<DownloadViewModel>();
        for (var i = 0; i < SelectedTracks.Count; i++)
        {
            var track = SelectedTracks[i];

            var baseFilePath = Path.Combine(
                dirPath,
                FileNameTemplate.Apply(
                    settingsService.FileNameTemplate,
                    track,
                    SelectedContainer,
                    (i + 1).ToString().PadLeft(SelectedTracks.Count.ToString().Length, '0')
                )
            );

            if (settingsService.ShouldSkipExistingFiles && File.Exists(baseFilePath))
                continue;

            var filePath = PathEx.EnsureUniquePath(baseFilePath);

            // Download does not start immediately, so lock in the file path to avoid conflicts
            DirectoryEx.CreateDirectoryForFile(filePath);
            await File.WriteAllBytesAsync(filePath, []);

            downloads.Add(viewModelManager.CreateDownloadViewModel(track, filePath));
        }

        settingsService.LastContainer = SelectedContainer;

        Close(downloads);
    }
}
