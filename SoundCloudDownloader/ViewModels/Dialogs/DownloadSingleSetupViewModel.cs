using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Platform.Storage;
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

public partial class DownloadSingleSetupViewModel(
    ViewModelManager viewModelManager,
    DialogManager dialogManager,
    SettingsService settingsService
) : DialogViewModelBase<DownloadViewModel>
{
    [ObservableProperty]
    private Track? _track;

    [ObservableProperty]
    IReadOnlyList<string>? _availableDownloadOptions = ["Mp3"];

    [ObservableProperty]
    string? _selectedDownloadOption;

    [RelayCommand]
    private void Initialize()
    {
        SelectedDownloadOption = AvailableDownloadOptions?.FirstOrDefault(o =>
            o == settingsService.LastContainer
        );
    }

    [RelayCommand]
    private async Task CopyTitleAsync()
    {
        if (Application.Current?.ApplicationLifetime?.TryGetTopLevel()?.Clipboard is { } clipboard)
            await clipboard.SetTextAsync(Track?.Title);
    }

    [RelayCommand]
    private async Task ConfirmAsync()
    {
        if (Track is null || SelectedDownloadOption is null)
            return;

        var container = SelectedDownloadOption;

        var filePath = await dialogManager.PromptSaveFilePathAsync(
            [new FilePickerFileType($"{container} file") { Patterns = [$"*.{container}"] }],
            FileNameTemplate.Apply(settingsService.FileNameTemplate, Track, container)
        );

        if (string.IsNullOrWhiteSpace(filePath))
            return;

        // Download does not start immediately, so lock in the file path to avoid conflicts
        DirectoryEx.CreateDirectoryForFile(filePath);
        await File.WriteAllBytesAsync(filePath, []);

        settingsService.LastContainer = container;

        Close(viewModelManager.CreateDownloadViewModel(Track, filePath));
    }
}
