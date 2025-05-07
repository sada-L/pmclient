using System;
using System.Reactive.Linq;
using System.Windows.Input;
using pmclient.Contracts.Requests.Auth;
using pmclient.Helpers;
using pmclient.Services;
using ReactiveUI;
using Splat;

namespace pmclient.ViewModels;

public class LoginViewModel : ViewModelBase, IRoutableViewModel
{
    private readonly AuthService? _authService;
    private readonly UserService? _userService;
    private string _errorMessage = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private bool _isError;

    public string Email
    {
        get => _email;
        set => this.RaiseAndSetIfChanged(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    public bool IsError
    {
        get => _isError;
        set => this.RaiseAndSetIfChanged(ref _isError, value);
    }

    public IScreen HostScreen { get; }

    public string UrlPathSegment => "/login";

    public ICommand LoginCommand { get; }

    public ICommand RegisterCommand { get; }

    public ICommand GeneratePasswordCommand { get; }

    public LoginViewModel()
    {
        Email = "admin@gmail.com";
        Password = "123456";
        HostScreen = Locator.Current.GetService<IScreen>()!;
        LoginCommand = ReactiveCommand.CreateFromObservable(Login);
        RegisterCommand = ReactiveCommand.CreateFromObservable(Login);
    }

    public LoginViewModel(IScreen? screen = null, AuthService? authService = null, UserService? userService = null)
    {
        _authService = authService ?? Locator.Current.GetService<AuthService>();
        _userService = userService ?? Locator.Current.GetService<UserService>();
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;

        LoginCommand = ReactiveCommand.CreateFromObservable(Login, CanExecLogin());
        RegisterCommand = ReactiveCommand.CreateFromObservable(() =>
            HostScreen.Router.Navigate.Execute(new RegisterViewModel(HostScreen, _authService, _userService)));
        GeneratePasswordCommand = ReactiveCommand.Create(GeneratePassword);
    }

    private void GeneratePassword()
    {
        Password = PasswordGenerator.GenerateSecurePassword();
    }

    private IObservable<IRoutableViewModel> Login() => Observable.FromAsync(async cancellationToken =>
    {
        var request = new LoginRequest
        {
            Email = Email,
            Password = Password,
        };

        if (!await _authService!.LoginAsync(request, cancellationToken))
        {
            ErrorMessage = "Invalid username or password";
            IsError = true;
            return null;
        }

        if (!await _userService!.GetUserAsync(cancellationToken))
        {
            ErrorMessage = "Invalid username or password";
            IsError = true;
            return null;
        }

        return HostScreen.Router.Navigate.Execute(new HomeViewModel(HostScreen));
    }).ObserveOn(RxApp.MainThreadScheduler).SelectMany(x => x!);

    private IObservable<bool> CanExecLogin() => this
        .WhenAnyValue(
            x => x.Email,
            x => x.Password,
            (username, password) =>
                !string.IsNullOrWhiteSpace(username) &&
                !string.IsNullOrWhiteSpace(password));
}