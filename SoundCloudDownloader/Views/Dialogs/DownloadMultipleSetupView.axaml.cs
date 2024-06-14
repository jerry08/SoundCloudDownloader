using Avalonia.Interactivity;
using SoundCloudDownloader.Framework;
using SoundCloudDownloader.ViewModels.Dialogs;

namespace SoundCloudDownloader.Views.Dialogs;

public partial class DownloadMultipleSetupView : UserControl<DownloadMultipleSetupViewModel>
{
    public DownloadMultipleSetupView() => InitializeComponent();

    private void UserControl_OnLoaded(object? sender, RoutedEventArgs args) =>
        DataContext.InitializeCommand.Execute(null);
}
