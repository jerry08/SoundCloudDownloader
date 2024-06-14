using Avalonia.Controls;
using Avalonia.Controls.Templates;
using SoundCloudDownloader.ViewModels;
using SoundCloudDownloader.ViewModels.Components;
using SoundCloudDownloader.ViewModels.Dialogs;
using SoundCloudDownloader.Views;
using SoundCloudDownloader.Views.Components;
using SoundCloudDownloader.Views.Dialogs;

namespace SoundCloudDownloader.Framework;

public partial class ViewManager
{
    private Control? TryCreateView(ViewModelBase viewModel) =>
        viewModel switch
        {
            MainViewModel => new MainView(),
            DashboardViewModel => new DashboardView(),
            DownloadMultipleSetupViewModel => new DownloadMultipleSetupView(),
            DownloadSingleSetupViewModel => new DownloadSingleSetupView(),
            MessageBoxViewModel => new MessageBoxView(),
            SettingsViewModel => new SettingsView(),
            _ => null
        };

    public Control? TryBindView(ViewModelBase viewModel)
    {
        var view = TryCreateView(viewModel);
        if (view is null)
            return null;

        view.DataContext ??= viewModel;

        return view;
    }
}

public partial class ViewManager : IDataTemplate
{
    bool IDataTemplate.Match(object? data) => data is ViewModelBase;

    Control? ITemplate<object?, Control?>.Build(object? data) =>
        data is ViewModelBase viewModel ? TryBindView(viewModel) : null;
}
