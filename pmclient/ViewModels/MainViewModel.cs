using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using pmclient.Helpers;
using pmclient.Services;
using pmclient.Settings;
using ReactiveUI;
using Splat;

namespace pmclient.ViewModels;

public class MainViewModel : ViewModelBase, IScreen
{
    private readonly AuthService? _authService;

    public RoutingState Router { get; } = new RoutingState();

    public ICommand MinimizeCommand { get; }

    public ICommand MaximizeCommand { get; }

    public ICommand CloseCommand { get; }

    public ICommand LoginCommand { get; }

    public MainViewModel(Window? window = null)
    {
        if (window != null)
        {
            MinimizeCommand = ReactiveCommand.Create(() => { window.WindowState = WindowState.Minimized; });
            CloseCommand = ReactiveCommand.Create(window.Close);
            MaximizeCommand = ReactiveCommand.Create(() =>
            {
                window.WindowState = window.WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
            });
        }

        _authService = Locator.Current.GetService<AuthService>();
        LoginCommand = ReactiveCommand.CreateFromObservable(Login);
        LoginCommand.Execute(null);
    }

    private IObservable<IRoutableViewModel> Login() => Observable
        .FromAsync<IRoutableViewModel>(async ct =>
        {
            if (!await _authService!.GetAccessTokenAsync(ct)) return new LoginViewModel(this);
            if (!App.IsMobileApp()) return new HomeViewModel(this);
            UserSettings.Pin = await FileStorage.LoadFileAsync("pin.dat");
            return new PinViewModel(this);
        }).ObserveOn(RxApp.MainThreadScheduler).SelectMany(x => Router.Navigate.Execute(x));
}