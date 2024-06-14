using Avalonia.Interactivity;
using SoundCloudDownloader.Framework;
using SoundCloudDownloader.ViewModels;

namespace SoundCloudDownloader.Views;

public partial class MainView : Window<MainViewModel>
{
    public MainView() => InitializeComponent();

    private void DialogHost_OnLoaded(object? sender, RoutedEventArgs args) =>
        DataContext.InitializeCommand.Execute(null);
}
