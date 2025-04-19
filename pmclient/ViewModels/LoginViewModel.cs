using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using pmclient.Contracts.Requests.Auth;
using pmclient.RefitClients;
using ReactiveUI;
using Splat;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reactive.Linq;
using System.Security.Claims;
using pmclient.Services;

namespace pmclient.ViewModels;

public class LoginViewModel :  ViewModelBase, IRoutableViewModel
{
    private readonly IIdentityWebApi? _identityWebApi;
    private readonly IUsersWebApi? _usersWebApi;
    private string _errorMessage = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private LoginRequest _loginRequest = new LoginRequest();

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

    public IScreen HostScreen { get; }
    public string UrlPathSegment => "/login";

    public ReactiveCommand<Unit, IRoutableViewModel> LoginCommand { get; set; }
    public ReactiveCommand<Unit, IRoutableViewModel> RegisterCommand { get; set; }

    public LoginViewModel()
    {
        Email = "admin@gmail.com";
        Password = "123456";
        LoginCommand = ReactiveCommand.CreateFromObservable(Login);
        RegisterCommand = ReactiveCommand.CreateFromObservable(Login);
    }

    public LoginViewModel(IIdentityWebApi? identityWebApi = null, IUsersWebApi? usersWebApi = null, IScreen? screen = null)
    {
        _identityWebApi = identityWebApi ?? Locator.Current.GetService<IIdentityWebApi>();
        _usersWebApi = usersWebApi ?? Locator.Current.GetService<IUsersWebApi>();
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!; 
            
        LoginCommand = ReactiveCommand.CreateFromObservable(Login, CanExecLogin());
        RegisterCommand = ReactiveCommand.CreateFromObservable( () => 
            HostScreen.Router.Navigate.Execute(new RegisterViewModel(HostScreen)));
    }

    private IObservable<IRoutableViewModel> Login() => LoginAsync()
        .Where(result => result)
        .ObserveOn(RxApp.TaskpoolScheduler)
        .SelectMany(_ => HostScreen.Router.Navigate.Execute(Locator.Current.GetService<HomeViewModel>()!));

    private IObservable<bool> LoginAsync() => Observable.FromAsync(async (cancellationToken) =>
    {
        _loginRequest.Email = _email;
        _loginRequest.Password = _password;
        
        var authResponse = await _identityWebApi.LoginAsync(_loginRequest, cancellationToken);
        if (!authResponse.IsSuccessStatusCode)
        {
            ErrorMessage = "Invalid username or password";
            return false;
        }

        string token = authResponse.Content!;
        var tokenHandler = new JwtSecurityTokenHandler().ReadJwtToken(token);

        string email = tokenHandler.Claims.First(claim => claim.Type is ClaimTypes.Email).Value;
        StaticStorage.JwtToken = authResponse.Content!;

        var userResponse = await _usersWebApi.GetUserByEmail(email, cancellationToken);
        if (!userResponse.IsSuccessStatusCode)
        {
            ErrorMessage = "Invalid username or password";
            return false;
        }

        StaticStorage.User = userResponse.Content;
        return true;
    });

    private IObservable<bool> CanExecLogin() =>
        this.WhenAnyValue(
            x => x.Email,
            x => x.Password,
            (username, password) =>
                !string.IsNullOrWhiteSpace(username) &&
                !string.IsNullOrWhiteSpace(password));
}