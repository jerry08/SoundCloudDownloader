using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using SoundCloudDownloader.Core.Utils.Extensions;
using SoundCloudDownloader.ViewModels;
using SoundCloudDownloader.ViewModels.Components;
using SoundCloudDownloader.ViewModels.Dialogs;
using SoundCloudExplode.Tracks;

namespace SoundCloudDownloader.Framework;

public class ViewModelManager(IServiceProvider services)
{
    public MainViewModel CreateMainViewModel() => services.GetRequiredService<MainViewModel>();

    public DashboardViewModel CreateDashboardViewModel() =>
        services.GetRequiredService<DashboardViewModel>();

    public DownloadViewModel CreateDownloadViewModel(Track track, string filePath)
    {
        var viewModel = services.GetRequiredService<DownloadViewModel>();

        viewModel.Track = track;
        viewModel.FilePath = filePath;

        return viewModel;
    }

    public DownloadMultipleSetupViewModel CreateDownloadMultipleSetupViewModel(
        string title,
        IReadOnlyList<Track> availableTracks,
        bool preselectTracks = true
    )
    {
        var viewModel = services.GetRequiredService<DownloadMultipleSetupViewModel>();

        viewModel.Title = title;
        viewModel.AvailableTracks = availableTracks;

        if (preselectTracks)
            viewModel.SelectedTracks.AddRange(availableTracks);

        return viewModel;
    }

    public DownloadSingleSetupViewModel CreateDownloadSingleSetupViewModel(Track track)
    {
        var viewModel = services.GetRequiredService<DownloadSingleSetupViewModel>();

        viewModel.Track = track;

        return viewModel;
    }

    public MessageBoxViewModel CreateMessageBoxViewModel(
        string title,
        string message,
        string? okButtonText,
        string? cancelButtonText
    )
    {
        var viewModel = services.GetRequiredService<MessageBoxViewModel>();

        viewModel.Title = title;
        viewModel.Message = message;
        viewModel.DefaultButtonText = okButtonText;
        viewModel.CancelButtonText = cancelButtonText;

        return viewModel;
    }

    public MessageBoxViewModel CreateMessageBoxViewModel(string title, string message) =>
        CreateMessageBoxViewModel(title, message, "CLOSE", null);

    public SettingsViewModel CreateSettingsViewModel() =>
        services.GetRequiredService<SettingsViewModel>();
}
