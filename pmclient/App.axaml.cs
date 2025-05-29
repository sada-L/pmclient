using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using pmclient.Helpers;
using pmclient.Services;
using pmclient.ViewModels;
using pmclient.Views;
using Splat;

namespace pmclient;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Bootstrapper.ConfigureServices();

        var settings = Locator.Current.GetService<SettingsService>()!;
        settings.SetTheme(settings.CurrentSettings.Theme);
        settings.SetLanguage(settings.CurrentSettings.Language);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            desktop.MainWindow.DataContext = new MainViewModel(desktop.MainWindow);
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            singleView.MainView = new MainView();
            singleView.MainView.DataContext = new MainViewModel();
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static bool IsMobileApp()
    {
        return Current!.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime;
    }
}