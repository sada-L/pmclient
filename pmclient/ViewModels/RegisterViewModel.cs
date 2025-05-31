using System;
using System.Reactive.Linq;
using System.Windows.Input;
using pmclient.Contracts.Requests.Auth;
using pmclient.Helpers;
using pmclient.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace pmclient.ViewModels;

public class RegisterViewModel : ViewModelBase, IRoutableViewModel
{
    private readonly AuthService? _authService;
    private readonly UserService? _userService;

    [Reactive] public string Email { get; set; }

    [Reactive] public string Username { get; set; }

    [Reactive] public string Password { get; set; }

    [Reactive] public string ConfirmPassword { get; set; }

    [Reactive] public bool IsError { get; set; }

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

    public RegisterViewModel(IScreen? screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
        _userService = Locator.Current.GetService<UserService>();
        _authService = Locator.Current.GetService<AuthService>();

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
            IsError = true;
            return null;
        }

        if (!await _userService!.GetUserAsync(cancellationToken))
        {
            IsError = true;
            return null;
        }

        return HostScreen.Router.Navigate.Execute(new HomeViewModel(HostScreen));
    }).WhereNotNull().SelectMany(x => x);

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