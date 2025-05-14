using System;
using System.Reactive.Linq;
using System.Windows.Input;
using pmclient.Contracts.Requests.Auth;
using pmclient.Helpers;
using pmclient.Services;
using ReactiveUI;
using Splat;

namespace pmclient.ViewModels;

public class RegisterViewModel : ViewModelBase, IRoutableViewModel
{
    private readonly AuthService? _authService;
    private readonly UserService? _userService;
    private string _email = string.Empty;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isError;

    public string Email
    {
        get => _email;
        set => this.RaiseAndSetIfChanged(ref _email, value);
    }

    public string Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => this.RaiseAndSetIfChanged(ref _confirmPassword, value);
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

    public string UrlPathSegment => "/register";

    public ICommand RegisterCommand { get; }

    public ICommand BackCommand { get; }

    public ICommand GeneratePasswordCommand { get; }

    public RegisterViewModel()
    {
        Email = "email@gmail.com";
        Username = "username";
        Password = "password";
        ConfirmPassword = "password";
        RegisterCommand = ReactiveCommand.CreateFromObservable(Register);
    }

    public RegisterViewModel(IScreen? screen = null, AuthService? authService = null, UserService? userService = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
        _userService = userService ?? Locator.Current.GetService<UserService>()!;
        _authService = authService ?? Locator.Current.GetService<AuthService>()!;

        RegisterCommand = ReactiveCommand.CreateFromObservable(Register, CanExecRegister());
        BackCommand = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.NavigateBack.Execute());
        GeneratePasswordCommand = ReactiveCommand.Create(GeneratePassword);
    }

    private void GeneratePassword()
    {
        var password = PasswordGenerator.GenerateSecurePassword();
        Password = password;
        ConfirmPassword = password;
    }

    private IObservable<IRoutableViewModel> Register() => Observable.FromAsync(async cancellationToken =>
    {
        var request = new RegisterRequest
        {
            Email = Email,
            Username = Username,
            Password = Password,
        };

        if (!await _authService!.RegisterAsync(request, cancellationToken))
        {
            ErrorMessage = "Registration failed";
            IsError = true;
            return null;
        }

        if (!await _userService!.GetUserAsync(cancellationToken))
        {
            ErrorMessage = "Registration failed";
            IsError = true;
            return null;
        }

        return HostScreen.Router.Navigate.Execute(new HomeViewModel(HostScreen));
    }).ObserveOn(RxApp.MainThreadScheduler).WhereNotNull().SelectMany(x => x!);

    private IObservable<bool> CanExecRegister() =>
        this.WhenAnyValue(
            x => x.Email,
            x => x.Username,
            x => x.Password,
            x => x.ConfirmPassword,
            (email, username, password, confirmPassword) =>
                !string.IsNullOrWhiteSpace(email) &&
                !string.IsNullOrWhiteSpace(username) &&
                !string.IsNullOrWhiteSpace(password) &&
                !string.IsNullOrWhiteSpace(confirmPassword) &&
                password == confirmPassword);
}