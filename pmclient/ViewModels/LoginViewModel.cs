using System;
using System.Reactive.Linq;
using System.Windows.Input;
using pmclient.Contracts.Requests.Auth;
using pmclient.Services;
using pmclient.Settings;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace pmclient.ViewModels;

public class LoginViewModel : ViewModelBase, IRoutableViewModel
{
    private readonly AuthService? _authService;
    private readonly TwoFaService? _twoFaService;
    private readonly UserService? _userService;

    [Reactive] public string Email { get; set; }

    [Reactive] public string Password { get; set; }

    [Reactive] public string Code { get; set; }

    [Reactive] public bool IsError { get; set; }

    [Reactive] public bool IsValid { get; set; }

    public IScreen HostScreen { get; }

    public string UrlPathSegment => "/login";

    public ICommand LoginCommand { get; }

    public ICommand RegisterCommand { get; }

    public ICommand ValidateCommand { get; }

    public LoginViewModel()
    {
        Email = "admin@gmail.com";
        Password = "123456";
        HostScreen = Locator.Current.GetService<IScreen>()!;
        LoginCommand = ReactiveCommand.CreateFromObservable(Login);
        RegisterCommand = ReactiveCommand.CreateFromObservable(Login);
    }

    public LoginViewModel(IScreen? screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
        _authService = Locator.Current.GetService<AuthService>();
        _userService = Locator.Current.GetService<UserService>();
        _twoFaService = Locator.Current.GetService<TwoFaService>();

        LoginCommand = ReactiveCommand.CreateFromObservable(Login, CanExecLogin);
        RegisterCommand = ReactiveCommand.CreateFromObservable(() =>
            HostScreen.Router.Navigate.Execute(new RegisterViewModel(HostScreen)));
        ValidateCommand = ReactiveCommand.CreateFromObservable(Validate, CanExecValid);

        IsValid = true;
    }

    private IObservable<IRoutableViewModel> Validate() => Observable.FromAsync(async cancellationToken =>
    {
        var request = new ValidateRequest { Code = Code };

        if (!await _twoFaService!.VerifyTwoFaAsync(request, cancellationToken))
        {
            IsError = true;
            return null;
        }

        return HostScreen.Router.Navigate.Execute(new HomeViewModel(HostScreen));
    }).WhereNotNull().SelectMany(x => x);

    private IObservable<IRoutableViewModel> Login() => Observable.FromAsync(async cancellationToken =>
    {
        var request = new LoginRequest
        {
            Email = Email,
            Password = Password,
        };

        if (!await _authService!.LoginAsync(request, cancellationToken))
        {
            IsError = true;
            return null;
        }

        if (!await _userService!.GetUserAsync(cancellationToken))
        {
            IsError = true;
            return null;
        }

        if (!string.IsNullOrEmpty(UserSettings.User!.Secret))
        {
            IsValid = false;
            return null;
        }

        return HostScreen.Router.Navigate.Execute(new HomeViewModel(HostScreen));
    }).WhereNotNull().SelectMany(x => x);

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