using System;
using System.Reactive.Linq;
using System.Windows.Input;
using pmclient.Contracts.Requests.Auth;
using pmclient.Helpers;
using pmclient.Services;
using pmclient.Settings;
using ReactiveUI;
using Splat;

namespace pmclient.ViewModels;

public class LoginViewModel : ViewModelBase, IRoutableViewModel
{
    private readonly AuthService? _authService;
    private readonly TwoFaService? _twoFaService;
    private readonly UserService? _userService;
    private string _errorMessage = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _code = string.Empty;
    private bool _isError;
    private bool _isValid;

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

    public string Code
    {
        get => _code;
        set => this.RaiseAndSetIfChanged(ref _code, value);
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

    public bool IsValid
    {
        get => _isValid;
        set => this.RaiseAndSetIfChanged(ref _isValid, value);
    }

    public IScreen HostScreen { get; }

    public string UrlPathSegment => "/login";

    public ICommand LoginCommand { get; }

    public ICommand RegisterCommand { get; }

    public ICommand GeneratePasswordCommand { get; }

    public ICommand ValidateCommand { get; }

    public LoginViewModel()
    {
        Email = "admin@gmail.com";
        Password = "123456";
        HostScreen = Locator.Current.GetService<IScreen>()!;
        LoginCommand = ReactiveCommand.CreateFromObservable(Login);
        RegisterCommand = ReactiveCommand.CreateFromObservable(Login);
    }

    public LoginViewModel(IScreen? screen = null, AuthService? authService = null, UserService? userService = null,
        TwoFaService? twoFaService = null)
    {
        _authService = authService ?? Locator.Current.GetService<AuthService>();
        _userService = userService ?? Locator.Current.GetService<UserService>();
        _twoFaService = twoFaService ?? Locator.Current.GetService<TwoFaService>();
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;

        LoginCommand = ReactiveCommand.CreateFromObservable(Login, CanExecLogin);
        RegisterCommand = ReactiveCommand.CreateFromObservable(() =>
            HostScreen.Router.Navigate.Execute(new RegisterViewModel(HostScreen, _authService, _userService)));
        GeneratePasswordCommand = ReactiveCommand.Create(GeneratePassword);
        ValidateCommand = ReactiveCommand.CreateFromObservable(Validate, CanExecValid);

        IsValid = true;
    }

    private void GeneratePassword()
    {
        Password = PasswordGenerator.GenerateSecurePassword();
    }

    private IObservable<IRoutableViewModel> Validate() => Observable.FromAsync(async cancellationToken =>
    {
        var request = new ValidateRequest { Code = Code };

        if (!await _twoFaService!.VerifyTwoFaAsync(request, cancellationToken))
        {
            ErrorMessage = "Invalid code";
            IsError = true;
            return null;
        }

        return HostScreen.Router.Navigate.Execute(new HomeViewModel(HostScreen));
    }).ObserveOn(RxApp.MainThreadScheduler).WhereNotNull().SelectMany(x => x);

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

        if (!string.IsNullOrEmpty(UserSettings.User!.Secret))
        {
            IsValid = false;
            return null;
        }

        return HostScreen.Router.Navigate.Execute(new HomeViewModel(HostScreen));
    }).ObserveOn(RxApp.MainThreadScheduler).WhereNotNull().SelectMany(x => x);

    private IObservable<bool> CanExecLogin => this
        .WhenAnyValue(
            x => x.Email,
            x => x.Password,
            (username, password) =>
                !string.IsNullOrWhiteSpace(username) &&
                !string.IsNullOrWhiteSpace(password));

    private IObservable<bool> CanExecValid => this
        .WhenAnyValue(x => x.Code,
            code => !string.IsNullOrWhiteSpace(code));
}