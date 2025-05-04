using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using pmclient.Helpers;
using pmclient.ViewModels;
using pmclient.Views;
using ReactiveUI;
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

        Current!.Resources.MergedDictionaries.Add(
            (ResourceDictionary)Current.Resources["Default"]!);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            var vm = new MainWindowViewModel(desktop.MainWindow);
            desktop.MainWindow.DataContext = vm;
        }

        base.OnFrameworkInitializationCompleted();
    }
}