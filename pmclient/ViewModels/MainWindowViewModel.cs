using System;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using pmclient.Services;
using ReactiveUI;
using Splat;

namespace pmclient.ViewModels;

public class MainWindowViewModel : ViewModelBase, IScreen
{
    private readonly AuthService? _authService;

    public RoutingState Router { get; } = new RoutingState();

    public ReactiveCommand<Unit, Unit> MinimizeCommand { get; }

    public ReactiveCommand<Unit, Unit> MaximizeCommand { get; }

    public ReactiveCommand<Unit, Unit> CloseCommand { get; }

    public ReactiveCommand<Unit, IRoutableViewModel> LoginCommand { get; }

    public MainWindowViewModel(Window window, AuthService? authService = null)
    {
        _authService = authService ?? Locator.Current.GetService<AuthService>();

        MinimizeCommand = ReactiveCommand.Create(() => { window.WindowState = WindowState.Minimized; });
        CloseCommand = ReactiveCommand.Create(window.Close);
        MaximizeCommand = ReactiveCommand.Create(() =>
        {
            window.WindowState = window.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        });

        LoginCommand = ReactiveCommand.CreateFromObservable(Login);
        LoginCommand.Execute().Subscribe();
    }

    private IObservable<IRoutableViewModel> Login() => Observable.FromAsync(async cancellationToken =>
    {
        if (await _authService!.GetAccessTokenAsync(cancellationToken))
            return Router.Navigate.Execute(new HomeViewModel(this));

        return Router.Navigate.Execute(new LoginViewModel(this, _authService));
    }).ObserveOn(RxApp.MainThreadScheduler).SelectMany(x => x);
}