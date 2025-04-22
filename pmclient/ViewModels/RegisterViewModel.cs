using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using pmclient.Contracts.Requests.Auth;
using pmclient.RefitClients;
using pmclient.Services;
using ReactiveUI;
using Splat;

namespace pmclient.ViewModels;

public class RegisterViewModel : ViewModelBase, IRoutableViewModel
{
    private readonly IIdentityWebApi? _identityWebApi;
    private readonly IUsersWebApi? _usersWebApi;
    private RegisterRequest _request = null!;
    private string _email = string.Empty;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;
    private string _errorMessage = string.Empty;

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
    
    public IScreen HostScreen { get; }
    public string UrlPathSegment { get; } = "register";
    public ReactiveCommand<Unit, IRoutableViewModel> RegisterCommand { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> BackCommand { get; }

    public RegisterViewModel()
    {
        Email = "email@gmail.com";
        Username = "username";
        Password = "password";
        ConfirmPassword = "password";
        RegisterCommand = ReactiveCommand.CreateFromObservable(Register);
        BackCommand = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.NavigateBack.Execute());
    }
    
    public RegisterViewModel(IIdentityWebApi? identityWebApi = null, IUsersWebApi? usersWebApi = null, IScreen? screen = null)
    {
       HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
       _usersWebApi = usersWebApi ?? Locator.Current.GetService<IUsersWebApi>()!;
       _identityWebApi = identityWebApi ?? Locator.Current.GetService<IIdentityWebApi>()!;
       
       RegisterCommand = ReactiveCommand.CreateFromObservable(Register, CanExecRegister());
       BackCommand = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.NavigateBack.Execute());
    }

    private IObservable<IRoutableViewModel> Register() => RegisterAsync()
        .Where(result => result)
        .ObserveOn(RxApp.MainThreadScheduler)
        .SelectMany(_ => HostScreen.Router.Navigate.Execute(new HomeViewModel(null, HostScreen)));

    private IObservable<bool> RegisterAsync() => Observable.FromAsync(async cancellationToken =>
    {
        _request = new RegisterRequest
        {
            Email = Email,
            Username = Username,
            Password = Password,
        }; 
        
        var authResponse = await _identityWebApi!.RegisterAsync(_request, cancellationToken);
        if (!authResponse.IsSuccessStatusCode)
        {
            ErrorMessage = "Registration failed";
            return false;
        }

        var token = authResponse.Content!.Replace("\"", "");
        StaticStorage.JwtToken = token;
        
        var userResponse = await _usersWebApi!.GetCurrentUser(cancellationToken);
        if (!userResponse.IsSuccessStatusCode)
        {
            ErrorMessage = "Registration failed";
            return false;
        }

        StaticStorage.User = userResponse.Content;
        return true;
    });
    
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