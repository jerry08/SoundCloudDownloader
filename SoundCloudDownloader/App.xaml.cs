using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using SoundCloudDownloader.Utils;

namespace SoundCloudDownloader;

public partial class App
{
    private static Assembly Assembly { get; } = typeof(App).Assembly;

    public static string Name { get; } = Assembly.GetName().Name!;

    public static Version Version { get; } = Assembly.GetName().Version!;

    public static string VersionString { get; } = Version.ToString(3);

    public static string GitHubProjectUrl { get; } = "https://github.com/jerry08/SoundCloudDownloader";
}

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
    }

    private static Theme LightTheme { get; } = Theme.Create(
        new MaterialDesignLightTheme(),
        MediaColor.FromHex("#343838"),
        MediaColor.FromHex("#F9A825")
    );

    private static Theme DarkTheme { get; } = Theme.Create(
        new MaterialDesignDarkTheme(),
        MediaColor.FromHex("#E8E8E8"),
        MediaColor.FromHex("#F9A825")
    );

    public static void SetLightTheme()
    {
        var paletteHelper = new PaletteHelper();
        paletteHelper.SetTheme(LightTheme);

        Current.Resources["SuccessBrush"] = new SolidColorBrush(Colors.DarkGreen);
        Current.Resources["CanceledBrush"] = new SolidColorBrush(Colors.DarkOrange);
        Current.Resources["FailedBrush"] = new SolidColorBrush(Colors.DarkRed);
    }

    public static void SetDarkTheme()
    {
        var paletteHelper = new PaletteHelper();
        paletteHelper.SetTheme(DarkTheme);

        Current.Resources["SuccessBrush"] = new SolidColorBrush(Colors.LightGreen);
        Current.Resources["CanceledBrush"] = new SolidColorBrush(Colors.Orange);
        Current.Resources["FailedBrush"] = new SolidColorBrush(Colors.OrangeRed);
    }

    void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        e.Handled = true;

        MessageBox.Show(e.Exception.GetBaseException().Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}