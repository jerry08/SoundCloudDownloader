using SoundCloudDownloader.ViewModels.Components;
using SoundCloudDownloader.ViewModels.Dialogs;

namespace SoundCloudDownloader.ViewModels.Framework
{
    // Used to instantiate new view models while making use of dependency injection
    public interface IViewModelFactory
    {
        DashboardViewModel CreateDashboardViewModel();

        DownloadViewModel CreateDownloadViewModel();

        DownloadSingleSetupViewModel CreateDownloadSingleSetupViewModel();

        DownloadMultipleSetupViewModel CreateDownloadMultipleSetupViewModel();

        MessageBoxViewModel CreateMessageBoxViewModel();

        SettingsViewModel CreateSettingsViewModel();
    }
}