using Avalonia.Interactivity;
using SoundCloudDownloader.Framework;
using SoundCloudDownloader.ViewModels.Dialogs;

namespace SoundCloudDownloader.Views.Dialogs;

public partial class DownloadSingleSetupView : UserControl<DownloadSingleSetupViewModel>
{
    public DownloadSingleSetupView() => InitializeComponent();

    private void UserControl_OnLoaded(object? sender, RoutedEventArgs args) =>
        DataContext.InitializeCommand.Execute(null);
}
